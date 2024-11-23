using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSystem : MonoBehaviour
{
    StageManager stageManager;
    public TextMeshProUGUI stageText;

    public void Select(TextMeshProUGUI text)
    {
        stageManager = StageManager.instance;
        stageManager.StageNumber = int.Parse(text.text);

        ScriptableStage stageData = stageManager.LoadStage(stageManager.StageNumber);

        if (stageData != null)
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.LogError(stageManager.StageNumber + "스테이지 정보 없음");
        }
        Debug.Log(stageManager.StageNumber);
    }
}