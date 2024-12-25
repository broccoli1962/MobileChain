using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    AudioManager audioManage;
    StageManager stageManager;
    [SerializeField] private Transform characterSlots;
    [SerializeField] private GameObject characterList;
    private bool openClose;
    public TextMeshProUGUI stageText;
    public GameObject alterBox;
    public TextMeshProUGUI alterText;

    public GameObject CharacterInfo;
    public float pressedTime = 2.5f;


    public void Start()
    {
        audioManage = AudioManager.Instance;
        stageManager = StageManager.instance;
    }
    public void Select(TextMeshProUGUI text)
    {
        stageManager.StageNumber = int.Parse(text.text);
        audioManage.ClickSound();

        ScriptableStage stageData = stageManager.LoadStage(stageManager.StageNumber);

        if(stageManager.characters.Count < 2)
        {
            alterText.text = "ĳ���Ͱ� ���ų� �Ѹ���";
            alterBox.SetActive(true);
            Debug.Log("ĳ���Ͱ� ���ų� �Ѹ���");
            return;
        }

        if (stageData != null)
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            alterText.text = stageManager.StageNumber.ToString() + " �������� ���� ����";
            alterBox.SetActive(true);
            Debug.LogError(stageManager.StageNumber + "�������� ���� ����");
        }
        Debug.Log(stageManager.StageNumber);
    }

    public void CloseAlter()
    {
        alterBox.SetActive(false);
    }

    public void OpenCloseCharacterList()
    {
        audioManage.ClickSound();
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

        audioManage.ClickSound();

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