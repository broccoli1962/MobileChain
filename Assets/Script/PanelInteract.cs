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
    CreatePanel cp;
    AudioManager aumg;
    TapCount ta;
    public static float distance;

    private void Start()
    {
        distance = 9.5f; //* (Screen.width / 720f);
        Debug.Log(distance);
        GameObject obj = GameObject.Find("PanelManager");
        cp = obj.GetComponent<CreatePanel>();
        GameObject obj2 = GameObject.Find("AudioManager");
        aumg = obj2.GetComponent<AudioManager>();
        ta = GameObject.Find("TouchCountBase").GetComponent<TapCount>();
    }

    public void click(GameObject clickedPanel)
    {
        //�ߺ� Ŭ�� ����
        if (filter.Count > 0 || cp.panels.Count < cp.maxPanelCount || next.Count > 0) return;

        //�� ����
        ta.TapDown(1);
        //���� �з� : ���ΰ��� �� ����Ʈ ����
        SpriteRenderer prefabSprite = clickedPanel.GetComponent<SpriteRenderer>();
        List<GameObject> colorList = new List<GameObject>();
        for (int i = 0; i < cp.panels.Count; i++)
        {
            if (cp.panels[i].GetComponent<SpriteRenderer>().sprite == prefabSprite.sprite)
            {
                colorList.Add(cp.panels[i]);
            }
        }
        //����
        filter = insertList(clickedPanel, colorList, new List<GameObject>()); //Ŭ���� �гΰ� ����� ���� ���г� ����Ʈ ��ȯ
        filterSort();
        filterRemove();
        cp.PanelTime(true);
    }

    private void filterSort()
    {
        int panelAmount = filter.Count; //�μ� ����
        //�� �Ͽ� �μ� ���� ���

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
            foreach(GameObject obj in list)
            {
                SpriteRenderer spriteColor = obj.GetComponent<SpriteRenderer>();

                if(spriteColor != null)
                {
                    Color color = spriteColor.color;
                    color.a = 0.5f;
                    spriteColor.color = color;
                }
                copyList.Add(obj);
            }
            aumg.PopSound();
            Invoke("filterRemove", 0.2f);
        } else
        {
            deletePanel();
            deleteLine();
            cp.PanelTime(false);
        }
    }
    
    private void deletePanel()
    {
        foreach (GameObject obj in copyList)
        {
            cp.panels.Remove(obj);
            Destroy(obj);
        }
        cp.StartCoroutine(cp.create);
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
        else if (s1 == cp.heartSprite)
        {
            return Color.magenta;
        }
        return Color.yellow;
    }
}
