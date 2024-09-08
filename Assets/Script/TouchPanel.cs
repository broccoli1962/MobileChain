using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour
{
    PanelInteract pi;

    private void Start()
    {
        pi = GameObject.Find("PanelManager").GetComponent<PanelInteract>();
    }

    //private void Update()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        if(touch.phase == TouchPhase.Began)
    //        {
    //            Vector2 point = Camera.main.ScreenToWorldPoint(touch.position);
    //            Ray2D ray = new Ray2D(point, Vector2.zero);
    //            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

    //            if(hit.collider != null && hit.collider.tag == "Panel")
    //            {
    //                pi.click(hit.collider.gameObject);
    //            }
    //        }
    //    }
    //}
    void OnMouseDown()
    {
        pi.click(this.gameObject);
    }
}