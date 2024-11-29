using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInteract : MonoBehaviour
{
    List<GameObject> filter = new();
    List<GameObject> copyList = new(); //패널 후처리 제거용
    List<List<GameObject>> next = new();
    List<RectTransform> clPanelTransform = new();

    [SerializeField] private ParticleSystem explosedPanel;
    
    Transform lastObjPanel;
    public int currentCount = 0;
    
    CreatePanel createPanel;
    CharacterRotate characterRotate;
    AudioManager audioManage;
    TapCount tcount;
    SystemManager system;
    PlayerSystem playerSystem;

    public float boomRadius = 15f;
    public static float distance;
    bool clicked;

    private void Start()
    {
        distance = 10f; //* (Screen.width / 720f);
        system = SystemManager.Instance;
        playerSystem = FindAnyObjectByType<PlayerSystem>();
        characterRotate = FindAnyObjectByType<CharacterRotate>();
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

        if (!playerSystem.turn)
        {
            tcount.TapDown(1);
        }

        //색깔 분류 : 서로같은 색 리스트 생성
        List<GameObject> colorList = new();
        foreach(var panel in createPanel.panels)
        {
            string kvalue = panel.Value;
            if(createPanel.panels.TryGetValue(clickedPanel, out string value) && kvalue == value)
            {
                colorList.Add(panel.Key);
            }
        }
        //필터
        filter = insertList(clickedPanel, colorList, new List<GameObject>()); //클릭한 패널과 가까운 같은 색패널 리스트 반환
        currentCount = filter.Count;
        filterSort(createPanel.panels[clickedPanel]);
        filterRemove();
        createPanel.PanelTime(true);
    }

    private void filterSort(string value)
    {
        //총 부순 개수 개수
        if (value != "heart")
        {
            playerSystem.totalBreak += filter.Count;
        }
        else
        {
            playerSystem.totalheal += filter.Count;
        }

        Transform lastObj = null;

        GameObject deletedPanel = filter[0]; //처음 클릭한거
        filter.RemoveAt(0);
        next.Add(new List<GameObject> { deletedPanel }); //처음 클릭한거 이중리스트 삽입
        while (filter.Count > 0) //필터가 빌때까지 돌린다.
        {
            if (filter.Count <= 2) //가장 마지막에 부서진 패널 위치 정보 가져오기
            {
                lastObj = filter[0].transform;
            }
            List<GameObject> addList = new List<GameObject>(); //이중리스트에 추가할 리스트
            foreach (GameObject obj in next[next.Count - 1]) //이중리스트 가장 마지막에 있는 것
            {
                foreach(GameObject near in nearby(obj)) //필터에 있는 것중 거리내 오브젝트
                {
                    ConnectLine(obj, near);
                    addList.Add(near);
                    filter.Remove(near);
                }
            }
            if (addList.Count > 0) next.Add(addList);
        }

        lastObjPanel = lastObj;
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
                
                //파티클 재생
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
            createPlayerPanel();//6 12 panel 생성
            createPanel.PanelTime(false);
            if (clPanelTransform.Count > 0)
            {
                StartCoroutine(boomExplode());
                return;
            }
            AfterRun();
        }
    }
    
    private void AfterRun()
    {
        clicked = false;
        createPanel.StartCoroutine(createPanel.create);
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
    
    private GameObject CreateBoom(Transform t)
    {
        GameObject boom = Instantiate(createPanel.Test_boom, t.localPosition, Quaternion.identity);
        boom.transform.rotation = t.rotation;
        boom.transform.SetParent(GameObject.Find("Panels").transform, false);
        return boom;
    }

    private IEnumerator boomExplode()
    {
        List<GameObject> booms = new();
        foreach(RectTransform t in clPanelTransform)
        {
            booms.Add(CreateBoom(t));
        }
        while (booms.Count > 0)
        {
            yield return new WaitForSeconds(1.5f);

            GameObject obj = booms[0];
            booms.RemoveAt(0);

            SpriteRenderer center = obj.GetComponent<SpriteRenderer>();

            Collider2D[] Colliders = Physics2D.OverlapCircleAll(center.bounds.center, boomRadius);

            Debug.Log(Colliders.Length + "개 범위 파악 완료");

            foreach (Collider2D searchCollider in Colliders)
            {
                if (searchCollider.gameObject.CompareTag("Panel"))
                {
                    if (createPanel.panels[searchCollider.gameObject] == "heart")
                    {
                        playerSystem.totalheal++;
                    }
                    else
                    {
                        playerSystem.totalBreak++;
                    }
                    createPanel.panels.Remove(searchCollider.gameObject);
                    Destroy(searchCollider.gameObject);
                }else if (searchCollider.gameObject.CompareTag("CLPanel"))
                {
                    createPanel.panels.Remove(searchCollider.gameObject);
                    Destroy(searchCollider.gameObject);
                    playerSystem.totalBreak++;
                    booms.Add(CreateBoom(searchCollider.gameObject.transform)); //새롭게 터칠 목록에 추가
                }
            }
            Destroy(obj);
        }
        clPanelTransform.Clear();
        AfterRun();
    }

    private void createPlayerPanel()
    {
        CharacterSlot charac = characterRotate.GetFirstSlot();
        if (currentCount > 6)
        {
            LargeCreateElements(charac.GetElement().ToString());
            GameObject item = Instantiate(createPanel.large_emptyPrefab, lastObjPanel.transform.localPosition, Quaternion.identity);
            item.transform.rotation = lastObjPanel.rotation;
            RawImage image = item.GetComponentInChildren<RawImage>();
            image.texture = charac.GetImage();
            item.transform.SetParent(GameObject.Find("Panels").transform, false);
            createPanel.panels.Add(item, charac.GetElement().ToString());
        }
        else if (currentCount > 5)
        {
            createElements(charac.GetElement().ToString());
            GameObject item = Instantiate(createPanel.emptyPrefab, lastObjPanel.transform.localPosition, Quaternion.identity);
            item.transform.rotation = lastObjPanel.rotation;
            RawImage image = item.GetComponentInChildren<RawImage>();
            image.texture = charac.GetImage();
            item.transform.SetParent(GameObject.Find("Panels").transform, false);
            createPanel.panels.Add(item , charac.GetElement().ToString());
        }
    }

    private void LargeCreateElements(string type)
    {
        SpriteRenderer sprite = createPanel.large_emptyPrefab.GetComponent<SpriteRenderer>();
        switch (type)
        {
            case "fire":
                sprite.sprite = createPanel.Large_fireSprite_empty;
                break;
            case "water":
                sprite.sprite = createPanel.Large_waterSprite_empty;
                break;
            case "light":
                sprite.sprite = createPanel.Large_lightSprite_empty;
                break;
            case "grass":
                sprite.sprite = createPanel.Large_grassSprite_empty;
                break;
        }
    }

    private void createElements(string type)
    {
        SpriteRenderer sprite = createPanel.emptyPrefab.GetComponent<SpriteRenderer>();
        switch (type) {
            case "fire":
                sprite.sprite = createPanel.fireSprite_empty;
                break;
            case "water":
                sprite.sprite = createPanel.waterSprite_empty;
                break;
            case "light":
                sprite.sprite = createPanel.lightSprite_empty;
                break;
            case "grass":
                sprite.sprite = createPanel.grassSprite_empty;
                break;
        }
    }

    private void deletePanel()
    {
        foreach (GameObject obj in copyList)
        {
            createPanel.panels.Remove(obj);
            if (obj.CompareTag("CLPanel"))
            {
                clPanelTransform.Add(obj.transform.GetComponent<RectTransform>());
            }
            Destroy(obj);
        }
        
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
        
        CircleCollider2D co1 = obj.GetComponent<CircleCollider2D>();
        CircleCollider2D co2 = obj2.GetComponent<CircleCollider2D>();

        float a = co1.radius / 0.525f; //1
        float b = co2.radius / 0.525f; //2

        Vector3 center1 = render1.bounds.center;
        Vector3 center2 = render2.bounds.center;

        return Vector3.Distance(center1, center2) < distance * ((a+b)/2f);
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
