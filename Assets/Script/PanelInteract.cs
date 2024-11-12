using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelInteract : MonoBehaviour
{
    List<GameObject> filter = new List<GameObject>();
    List<GameObject> copyList = new List<GameObject>(); //�г� ��ó�� ���ſ�
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
            Debug.Log("�ߺ� ����");
            return;
        }

        clicked = true;

        if (!system.turn)
        {
            tcount.TapDown(1);
        }
        
        //���� �з� : ���ΰ��� �� ����Ʈ ����
        SpriteRenderer prefabSprite = clickedPanel.GetComponent<SpriteRenderer>();
        List<GameObject> colorList = new List<GameObject>();
        for (int i = 0; i < createPanel.panels.Count; i++)
        {
            if (createPanel.panels[i].GetComponent<SpriteRenderer>().sprite == prefabSprite.sprite)
            {
                colorList.Add(createPanel.panels[i]);
            }
        }
        //����
        filter = insertList(clickedPanel, colorList, new List<GameObject>()); //Ŭ���� �гΰ� ����� ���� ���г� ����Ʈ ��ȯ
        filterSort();
        filterRemove();
        createPanel.PanelTime(true);
    }

    private void filterSort()
    {
        //�μ� ����
        system.totalBreak += filter.Count;
        //�� �Ͽ� �μ� ���� ���
        //Debug.Log(system.Totalbreak);

        GameObject deletedPanel = filter[0]; //ó�� Ŭ���Ѱ�
        filter.RemoveAt(0);
        next.Add(new List<GameObject> { deletedPanel }); //ó�� Ŭ���Ѱ� ���߸���Ʈ ����
        while (filter.Count > 0) //���Ͱ� �������� ������.
        {
            List<GameObject> addList = new List<GameObject>(); //���߸���Ʈ�� �߰��� ����Ʈ
            foreach (GameObject obj in next[next.Count - 1]) //���߸���Ʈ ���� �������� �ִ� ��
            {
                foreach(GameObject near in nearby(obj)) //���Ϳ� �ִ� ���� 6.3 �Ÿ��� ������Ʈ
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
            main.startColor = new ParticleSystem.MinMaxGradient(Color.white, Color.red);//���ڰ� = 2��°��, 1��°��
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
