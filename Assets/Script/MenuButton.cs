using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    StageManager stageManager;
    [SerializeField] private Transform characterSlots;
    [SerializeField] private GameObject characterList;
    private bool openClose;
    public TextMeshProUGUI stageText;

    public void Start()
    {
        stageManager = StageManager.instance;
    }
    public void Select(TextMeshProUGUI text)
    {
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

    public void OpenCloseCharacterList()
    {
        if (openClose)
        {
            characterList.SetActive(false);
            openClose = false;
        }
        else
        {
            characterList.SetActive(true);
            openClose = true;
        }
    }

    public void InsertCharacter()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;

        if (clickObject.transform.IsChildOf(characterSlots))
        {
            clickObject.transform.SetParent(characterList.transform);
            stageManager.characters.Remove(clickObject);
            Debug.Log(characterSlots + "자식임");
        }
        else
        {
            clickObject.transform.SetParent(characterSlots);
            stageManager.characters.Add(clickObject);
            Debug.Log(characterSlots + "자식 아님");
        }
    }
}