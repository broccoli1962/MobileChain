using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour
{
    PanelInteract pi;
    Vector3 MousePosition;
    Camera Camera;

    private void Start()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        pi = GameObject.Find("PanelManager").GetComponent<PanelInteract>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] rayhit = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition));
            foreach (RaycastHit2D ray in rayhit) {
                if (ray.transform.CompareTag("UI"))
                {
                    return;
                }
            }
            foreach(RaycastHit2D ray in rayhit)
            {
                if (ray.transform.CompareTag("Panel"))
                {
                    GameObject hitObj = ray.transform.gameObject;
                    pi.click(hitObj);
                    return;
                }
            }
            //panel들 클릭 하는 쏘스
            //RaycastHit2D rayhit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            //if (rayhit.transform!=null&&rayhit.transform.CompareTag("Panel"))
            //{
            //    GameObject hitObj = rayhit.transform.gameObject;
            //    Debug.Log(hitObj.name);
            //    pi.click(hitObj);
            //}
        }
    }
}