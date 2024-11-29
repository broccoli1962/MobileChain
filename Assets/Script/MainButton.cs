using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButton : MonoBehaviour
{
    public GameObject alterDialog;

    public void ReturnButton()
    {
        alterDialog.SetActive(true);
    }
    public void LogButton()
    {
        //³ªÁß¿¡
    }

    public void YesButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void NoButton()
    {
        alterDialog.SetActive(false);
    }
}
