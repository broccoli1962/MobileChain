using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPanel : MonoBehaviour
{
    private void OnMouseDown()
    {
        CreatePanel.cpanel.click(gameObject);
    }
}