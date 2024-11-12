using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelInteract : MonoBehaviour
{
    List<GameObject> filter = new List<GameObject>();
    List<GameObject> copyList = new List<GameObject>(); //패널 후처리 제거용
    List<List<GameObject>> next = new List<List<GameObject>>();
    [SerializeField] private ParticleSystem explosedPanel;
    CreatePanel createPanel;
    AudioManager audioManage;
    TapCount tcount;
    SystemManager system;
    public static float distance;
    bool clicked;

    private void Start()
    {
        distance = 12f; //* (Screen.width / 720f);
        system = SystemManager.Instance;
        GameObject obj = GameObject.Find("PanelManager");
        createPanel = obj.GetComponent<CreatePanel>();
        GameObject obj2 = GameObject.Find("AudioManager");
        audioManage = obj2.GetComponent<AudioManager>();
        tcount = GameObject.Find("TouchCountBase").GetComponent<TapCount>();
    }

    public void click(GameObject clickedPanel)
    {
        if (clicked)
        {
            Debug.Log("중복 방지");
            return;
        }

        clicked = true;

        if (!system.turn)
        {
            tcount.TapDown(1);
        }
        
        //색깔 분류 : 서로같은 색 리스트 생성
        SpriteRenderer prefabSprite = clickedPanel.GetComponent<SpriteRenderer>();
        List<GameObject> colorList = new List<GameObject>();
        for (int i = 0; i < createPanel.panels.Count; i++)
        {
            if (createPanel.panels[i].GetComponent<SpriteRenderer>().sprite == prefabSprite.sprite)
            {
                colorList.Add(createPanel.panels[i]);
            }
        }
        //필터
        filter = insertList(clickedPanel, colorList, new List<GameObject>()); //클릭한 패널과 가까운 같은 색패널 리스트 반환
        filterSort();
        filterRemove();
        createPanel.PanelTime(true);
    }

    private void filterSort()
    {
        //부순 개수
        system.totalBreak += filter.Count;
        //한 턴에 부순 개수 계산
        //Debug.Log(system.Totalbreak);

        GameObject deletedPanel = filter[0]; //처음 클릭한거
        filter.RemoveAt(0);
        next.Add(new List<GameObject> { deletedPanel }); //처음 클릭한거 이중리스트 삽입
        while (filter.Count > 0) //필터가 빌때까지 돌린다.
        {
            List<GameObject> addList = new List<GameObject>(); //이중리스트에 추가할 리스트
            foreach (GameObject obj in next[next.Count - 1]) //이중리스트 가장 마지막에 있는 것
            {
                foreach(GameObject near in nearby(obj)) //필터에 있는 것중 6.3 거리내 오브젝트
                {
                    ConnectLine(obj, near);
                    addList.Add(near);
                    filter.Remove(near);
                }
            }
            if (addList.Count > 0) next.Add(addList);
        }
    }

    private void filterRemove()
    {
        if (next.Count > 0)
        {
            List<GameObject> list = next[0];
            next.RemoveAt(0);
            foreach (GameObject obj in list)
            {
                SpriteRenderer spriteColor = obj.GetComponent<SpriteRenderer>();

                if (spriteColor != null)
                {
                    Color color = spriteColor.color;
                    color.a = 0.5f;
                    spriteColor.color = color;
                }
                    
                Vector3 centerPosition = obj.GetComponent<SpriteRenderer>().bounds.center;
                ParticleColor(spriteColor);
                ParticleSystem particleInstance = Instantiate(explosedPanel, centerPosition, Quaternion.identity);
                particleInstance.Play();
                Destroy(particleInstance.gameObject, particleInstance.main.duration);

                copyList.Add(obj);
            }
            audioManage.PopSound();
            Invoke("filterRemove", 0.2f);
        } else
        {
            deletePanel();
            deleteLine();
            createPanel.PanelTime(false);
            clicked = false;
        }
    }
    
    void ParticleColor(SpriteRenderer sprite)
    {
        ParticleSystem.MainModule main = explosedPanel.main;
        if (sprite.sprite == createPanel.fireSprite)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.red);//인자값 = 2번째색, 1번째색
        }
        else if (sprite.sprite == createPanel.waterSprite)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.blue);
        }
        else if (sprite.sprite == createPanel.lightSprite)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.yellow);
        }
        else if (sprite.sprite == createPanel.grassSprite)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.magenta);
        }
        else if (sprite.sprite == createPanel.heartSprite)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.magenta);
        }
    }

    private void deletePanel()
    {
        foreach (GameObject obj in copyList)
        {
            createPanel.panels.Remove(obj);
            Destroy(obj);
        }
        createPanel.StartCoroutine(createPanel.create);
        copyList.Clear();
    }

    private void deleteLine()
    {
        foreach (GameObject deletedLine in GameObject.FindGameObjectsWithTag("Line"))
        {
            Destroy(deletedLine);
        }
    }

    private List<GameObject> nearby(GameObject obj)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject obj2 in filter)
        {
            if (nearObject(obj, obj2)) list.Add(obj2);
        }
        return list;
    }

    private Boolean nearObject(GameObject obj, GameObject obj2)
    {
        SpriteRenderer render1 = obj.GetComponent<SpriteRenderer>();
        SpriteRenderer render2 = obj2.GetComponent<SpriteRenderer>();

        Vector3 center1 = render1.bounds.center;
        Vector3 center2 = render2.bounds.center;

        return Vector3.Distance(center1, center2) < distance;
    }

    private List<GameObject> insertList(GameObject g, List<GameObject> gList, List<GameObject> finalList)
    {
        finalList.Add(g);
        foreach (GameObject obj in gList)
        {
            if (!finalList.Contains(obj) && nearObject(g,obj))
            {
                insertList(obj, gList, finalList);
            }
        }
        return finalList;
    }

    private void ConnectLine(GameObject obj, GameObject obj2)
    {
        SpriteRenderer render1 = obj.GetComponent<SpriteRenderer>();
        SpriteRenderer render2 = obj2.GetComponent<SpriteRenderer>();

        LineRenderer line = Instantiate(createPanel.linePrefab, gameObject.transform).GetComponent<LineRenderer>();

        Color color = LineColor(render1.sprite);
        line.startColor = color;
        line.endColor = color;

        line.SetPosition(0, render1.bounds.center);
        line.SetPosition(1, render2.bounds.center);
    }

    private Color LineColor(Sprite s1)
    {
        if (s1 == createPanel.fireSprite)
        {
            return Color.red;
        }
        else if (s1 == createPanel.waterSprite)
        {
            return Color.blue;
        }
        else if (s1 == createPanel.grassSprite)
        {
            return Color.green;
        }
        else if (s1 == createPanel.heartSprite)
        {
            return Color.magenta;
        }
        return Color.yellow;
    }
}
