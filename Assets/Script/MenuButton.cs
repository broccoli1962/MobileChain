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
    public GameObject alterBox;
    public TextMeshProUGUI alterText;

    public void Start()
    {
        stageManager = StageManager.instance;
    }
    public void Select(TextMeshProUGUI text)
    {
        stageManager.StageNumber = int.Parse(text.text);

        ScriptableStage stageData = stageManager.LoadStage(stageManager.StageNumber);

        if(stageManager.characters.Count < 2)
        {
            alterText.text = "캐릭터가 없거나 한명임";
            alterBox.SetActive(true);
            Debug.Log("캐릭터가 없거나 한명임");
            return;
        }

        if (stageData != null)
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            alterText.text = stageManager.StageNumber.ToString() + " 스테이지 정보 없음";
            alterBox.SetActive(true);
            Debug.LogError(stageManager.StageNumber + "스테이지 정보 없음");
        }
        Debug.Log(stageManager.StageNumber);
    }

    public void CloseAlter()
    {
        alterBox.SetActive(false);
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
        CharacterSlot slot = clickObject.GetComponent<CharacterSlot>();

        if (clickObject.transform.IsChildOf(characterSlots))
        {
            clickObject.transform.SetParent(characterList.transform, false);
            stageManager.characters.Remove(slot.CharacterNumber);
        }
        else if (stageManager.characters.Count < 4)
        {
            clickObject.transform.SetParent(characterSlots, false);
            stageManager.characters.Add(slot.CharacterNumber);
        }
    }
}