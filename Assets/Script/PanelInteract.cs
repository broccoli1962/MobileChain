using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelInteract : MonoBehaviour
{
    List<GameObject> filter = new List<GameObject>();
    List<List<GameObject>> next = new List<List<GameObject>>();
    CreatePanel cp;
    AudioManager aumg;

    private void Start()
    {
        GameObject obj = GameObject.Find("PanelManager");
        cp = obj.GetComponent<CreatePanel>();
        GameObject obj2 = GameObject.Find("AudioManager");
        aumg = obj2.GetComponent<AudioManager>();
    }

    public void click(GameObject clickedPanel)
    {
        //중복 클릭 방지
        if (filter.Count > 0) return;

        //색깔 분류 : 서로같은 색 리스트 생성
        SpriteRenderer prefabSprite = clickedPanel.GetComponent<SpriteRenderer>();
        List<GameObject> colorList = new List<GameObject>();
        for (int i = 0; i < cp.panels.Count; i++)
        {
            if (cp.panels[i].GetComponent<SpriteRenderer>().sprite == prefabSprite.sprite)
            {
                colorList.Add(cp.panels[i]);
            }
        }
        //필터
        filter = insertList(clickedPanel, colorList, new List<GameObject>()); //클릭한 패널과 가까운 같은 색패널 리스트 반환
        filterSort();
        filterRemove();
    }

    private void filterSort()
    {
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
            foreach(GameObject obj in list)
            {
                cp.panels.Remove(obj);
                Destroy(obj);
            }
            aumg.PopSound();
            Invoke("filterRemove", 0.2f);
        } else
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

    private List<GameObject> nearby(GameObject obj)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject obj2 in filter)
        {
            if (nearObject(obj, obj2, 6.3f)) list.Add(obj2);
        }
        return list;
    }

    private Boolean nearObject(GameObject obj, GameObject obj2, float distance)
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
            if (!finalList.Contains(obj) && nearObject(g,obj,6.3f))
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

        LineRenderer line = Instantiate(cp.linePrefab, gameObject.transform).GetComponent<LineRenderer>();

        Color color = LineColor(render1.sprite);
        line.startColor = color;
        line.endColor = color;

        line.SetPosition(0, render1.bounds.center);
        line.SetPosition(1, render2.bounds.center);
    }

    private Color LineColor(Sprite s1)
    {
        if (s1 == cp.fireSprite)
        {
            return Color.red;
        }
        else if (s1 == cp.waterSprite)
        {
            return Color.blue;
        }
        else if (s1 == cp.grassSprite)
        {
            return Color.green;
        }
        return Color.yellow;
    }
}
