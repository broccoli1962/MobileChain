using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class CreatePanel : MonoBehaviour
{
    [Header ("PanelPrefab, LinePrefab")]
    public GameObject panelPrefab;
    public GameObject linePrefab;
    public GameObject emptyPrefab;
    public GameObject large_emptyPrefab;
    SpriteRenderer panelImg;
    [Header ("4Panel")]
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite grassSprite;
    public Sprite lightSprite;
    public Sprite heartSprite;
    [Header ("4Panel_Empty")]
    public Sprite fireSprite_empty;
    public Sprite waterSprite_empty;
    public Sprite grassSprite_empty;
    public Sprite lightSprite_empty;
    [Header ("MaxPanel")]
    public int maxPanelCount = 40;
    //Boolean runPanelCreate = true;

    private List<AccData> accList = new List<AccData>();
    public IEnumerator create;
    Boolean createRun = true;

    Vector3 leftSpawn;
    Vector3 rightSpawn;

    private String panelType = "";
    public Dictionary<GameObject, string> panels = new();

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
            if (panels.Count == maxPanelCount)
            {
                StopCoroutine(create);
            }
            else
            {
                PanelDrop();
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void PanelTime(bool enable)
    {
        foreach(GameObject panel in panels.Keys)
        {
            Rigidbody2D rb = panel.GetComponent<Rigidbody2D>();
            if (enable)
            {
                accList.Add(new AccData(rb));
            }
            rb.bodyType = enable ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
        }

        if (!enable)
        {
            foreach (AccData acc in accList)
            {
                acc.accept();
            }
            accList.Clear();
        }
    }

    class AccData
    {
        float drag, mass, angulerVel;
        Vector2 vel;
        Rigidbody2D rb;
        public AccData(Rigidbody2D body)
        {
            this.rb = body;
            this.drag = body.linearDamping;
            this.mass = body.mass;
            this.vel = body.linearVelocity;
            this.angulerVel = body.angularVelocity;
        }

        public void accept()
        {
            rb.bodyType = RigidbodyType2D.Dynamic; //? 왜 써야하는지 모르겠음 경고 나옴
            rb.linearDamping = drag;
            rb.mass = mass;
            rb.linearVelocity = vel;
            rb.angularVelocity = angulerVel;
        }
    }

    public void PanelDrop()
    {
        int rand = Random.Range(0, 5);
        float rand2 = Random.Range(-10f*PanelInteract.distance, 10f*PanelInteract.distance);

        switch(rand){
            case 0:
                panelImg.sprite = fireSprite;
                panelType = "fire";
                break;
            case 1:
                panelImg.sprite = waterSprite;
                panelType = "water";
                break;
            case 2:
                panelImg.sprite = grassSprite;
                panelType = "grass";
                break;
            case 3:
                panelImg.sprite = lightSprite;
                panelType = "light";
                break;
            case 4:
                panelImg.sprite = heartSprite;
                panelType = "heart";
                break;
        }

        GameObject item = Instantiate(panelPrefab, Random.value > 0.5 ? leftSpawn:rightSpawn, Quaternion.identity);
        item.transform.SetParent(GameObject.Find("Panels").transform, false);
        panels.Add(item, panelType);
    }
}