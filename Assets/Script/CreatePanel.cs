using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatePanel : MonoBehaviour
{
    public GameObject panelPrefab;
    public GameObject linePrefab;
    public SpriteRenderer panelImg;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite grassSprite;
    public Sprite lightSprite;

    public List<GameObject> panels;

    public static CreatePanel cpanel;

    List<GameObject> filter;

    private void Start()
    {
        cpanel = this;
        panelImg = panelPrefab.GetComponent<SpriteRenderer>();

        for (int i = 0; i<100; i++)
        {
            PanelDrop();
        }
    }

    public void click(GameObject clickedPanel)
    {
        SpriteRenderer prefabSprite = clickedPanel.GetComponent<SpriteRenderer>();
        List<GameObject> colorList = new List<GameObject>();
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i].GetComponent<SpriteRenderer>().sprite == prefabSprite.sprite)
            {
                colorList.Add(panels[i]);
            }
        }

        filter = insertList(clickedPanel,colorList,new List<GameObject>());
        DeletePanel();

        //필터에 차 있을경우 click 불가능, filter 전역변수, 하나씩 invoke로 제거

        //1차 가장 가까이 있는 패널들을 탐색
        //2차 자신과 같은 색인지 탐색
        //연결

    }

    private void DeletePanel()
    {
        if (filter.Count > 0)
        {
            GameObject deletedPanel = filter[0];
            filter.RemoveAt(0);

            Invoke("DeletePanel",0.1f);

            panels.Remove(deletedPanel);
            Destroy(deletedPanel);
            AudioManager.caudio.PopSound();
        }
        else
        {
            DeleteLine();
        }
    }

    private void DeleteLine()
    {
        foreach (GameObject deletedLine in GameObject.FindGameObjectsWithTag("Line"))
        {
            Destroy(deletedLine);
        }
    }

    private List<GameObject> insertList(GameObject g, List<GameObject> gList, List<GameObject> finalList)
    {
        finalList.Add(g);
        foreach (GameObject obj in gList)
        {
            if (!finalList.Contains(obj))
            {
                SpriteRenderer render1 = g.GetComponent<SpriteRenderer>();
                SpriteRenderer render2 = obj.GetComponent<SpriteRenderer>();

                Bounds bounds1 = render1.bounds;
                Bounds bounds2 = render2.bounds;

                Vector3 center1 = bounds1.center;
                Vector3 center2 = bounds2.center;

                float distance = Vector3.Distance(center1, center2);
                if (distance < 6.3)
                {
                    insertList(obj, gList, finalList);
                    LineRenderer line = Instantiate(linePrefab, gameObject.transform).GetComponent<LineRenderer>();

                    Color color = LineColor(render1.sprite);

                    line.startColor = color;
                    line.endColor = color;

                    line.SetPosition(0, center1);
                    line.SetPosition(1, center2);
                }
            }
        }
        return finalList;
    }

    private Color LineColor(Sprite s1)
    {
        if (s1 == fireSprite)
        {
            return Color.red;
        }
        else if (s1 == waterSprite)
        {
            return Color.blue;
        }
        else if (s1 == grassSprite)
        {
            return Color.green;
        }
        return Color.yellow;
    }

    public void PanelDrop()
    {
        int rand = Random.Range(0, 4);

        switch(rand){
            case 0:
                panelImg.sprite = fireSprite;
                break;
            case 1:
                panelImg.sprite = waterSprite;
                break;
            case 2:
                panelImg.sprite = grassSprite;
                break;
            case 3:
                panelImg.sprite = lightSprite;
                break;
        }

        GameObject item = Instantiate(panelPrefab, gameObject.transform);
        panels.Add(item);
    }
}
