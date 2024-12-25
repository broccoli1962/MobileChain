using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainButton : MonoBehaviour
{
    public GameObject alterDialog;
    public GameObject Log;
    bool logOpen;
    AudioManager audioManage;

    private void Start()
    {
        audioManage = AudioManager.Instance;
    }

    public void ReturnButton()
    {
        audioManage.ClickSound();
        alterDialog.SetActive(true);
    }
    public void LogButton()
    {
        audioManage.ClickSound();
        if (logOpen)
        {
            Log.SetActive(false);
            logOpen = false;
        }
        else
        {
            Log.SetActive(true);
            logOpen = true;
        }
        
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
