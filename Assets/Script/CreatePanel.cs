using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatePanel : MonoBehaviour
{
    [Header ("PanelPrefab, LinePrefab")]
    public GameObject panelPrefab;
    public GameObject linePrefab;
    public SpriteRenderer panelImg;
    [Header ("4Panel")]
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite grassSprite;
    public Sprite lightSprite;

    public List<GameObject> panels;

    private void Start()
    {
        panelImg = panelPrefab.GetComponent<SpriteRenderer>();

        for (int i = 0; i<100; i++)
        {
            PanelDrop();
        }
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
