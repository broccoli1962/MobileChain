using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePanel : MonoBehaviour
{
    public GameObject PanelPrefab;
    public SpriteRenderer PanelImg;
    public Sprite FireSprite;
    public Sprite WaterSprite;
    public Sprite grassSprite;
    public Sprite lightSprite;

    private void Start()
    {
        PanelImg = PanelPrefab.GetComponent<SpriteRenderer>();

        for(int i = 0; i<100; i++)
        {
            PanelDrop();
        }
    }

    public void PanelDrop()
    {
        int rand = Random.Range(0, 4);

        switch(rand){
            case 0:
                PanelImg.sprite = FireSprite;
                break;
            case 1:
                PanelImg.sprite = WaterSprite;
                break;
            case 2:
                PanelImg.sprite = grassSprite;
                break;
            case 3:
                PanelImg.sprite = lightSprite;
                break;
        }

        GameObject item = Instantiate(PanelPrefab, gameObject.transform);
    }
}
