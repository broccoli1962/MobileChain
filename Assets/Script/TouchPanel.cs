using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPanel : MonoBehaviour
{
    PanelInteract pi;

    private void Start()
    {
        GameObject obj = GameObject.Find("PanelManager");
        pi = obj.GetComponent<PanelInteract>();
    }
    private void OnMouseDown()
    {
        pi.click(gameObject);
    }
}