using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatePanel : MonoBehaviour
{
    [Header ("PanelPrefab, LinePrefab")]
    public GameObject panelPrefab;
    public GameObject linePrefab;
    SpriteRenderer panelImg;
    [Header ("4Panel")]
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite grassSprite;
    public Sprite lightSprite;
    public Sprite heartSprite;
    [Header ("MaxPanel")]
    public int maxPanelCount = 40;
    //Boolean runPanelCreate = true;

    public List<GameObject> panels;
    public IEnumerator create;
    Boolean createRun = true;

    Vector3 leftSpawn;
    Vector3 rightSpawn;

    private void Start()
    {
        Vector3 left = GameObject.Find("Arm(left)").GetComponent<RectTransform>().localPosition;
        leftSpawn = new Vector3(left.x, left.y-1000, 0);
        Vector3 right = GameObject.Find("Arm(right)").GetComponent<RectTransform>().localPosition;
        rightSpawn = new Vector3(right.x, right.y-1000, 0);

        create = Create();
        panelImg = panelPrefab.GetComponent<SpriteRenderer>();
        StartCoroutine(create);
    }

    IEnumerator Create()
    {
        while (createRun)
        {
            PanelDrop();
            if (panels.Count == maxPanelCount)
            {
                StopCoroutine(create);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void PanelDrop()
    {
        int rand = Random.Range(0, 5);
        float rand2 = Random.Range(-10f*PanelInteract.distance, 10f*PanelInteract.distance);

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
            case 4:
                panelImg.sprite = heartSprite;
                break;
        }

        GameObject item = Instantiate(panelPrefab, Random.value > 0.5 ? leftSpawn:rightSpawn, Quaternion.identity);
        item.transform.SetParent(GameObject.Find("Panels").transform, false);
        panels.Add(item);
    }
}


//private void Update()
//{
//    if (panels.Count > maxPanelCount)
//    {
//        runPanelCreate = false;
//    }else if(panels.Count <= maxPanelCount && !runPanelCreate) 
//    {
//        runPanelCreate = true;   
//    }

//    if (runPanelCreate)
//    {
//        PanelDrop();
//    }
//}