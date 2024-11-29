using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [SerializeField] private List<ScriptableStage> stageDatas;
    public List<int> characters = new();
    public int StageNumber;
    public MonsterManager monsterManager;
    public CharacterRotate characterRotate;
    public GameObject monsterPrefab;
    public GameObject characterPrefab;
    public GameObject slotLine;

    public static StageManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GetSlotLine()
    {
        Transform[] childTransforms = slotLine.GetComponentsInChildren<Transform>();
        //첫 시작때 초기화 시켜줌
        foreach (Transform child in childTransforms)
        {
            CharacterSlot charac = child.GetComponent<CharacterSlot>();
            if (child.CompareTag("character"))
            {
                characters.Add(charac.CharacterNumber);
            }
        }
    }

    public ScriptableStage LoadStage(int stageNumber) //스테이지 정보 가져오기
    {
        foreach (ScriptableStage stage in stageDatas)
        {
            if(stage.stageNumber == stageNumber)
            {
                return stage;
            }
        }
        return null;
    }

    public void LoadStageMonster(int stageNumber, int floorNumber) //정보 내 몬스터 호출
    {
        ScriptableStage stageData = LoadStage(stageNumber);

        if (stageData != null) { 
            monsterManager.Monsters.Clear();
            foreach(Transform child in monsterManager.monsterSlotLine)
            {
                Destroy(child.gameObject);
            }

            foreach(FloorData floor in stageData.floors)
            {
                if(floor.floorNumber == floorNumber)
                {
                    foreach (MonsterSpawnData monster in floor.monsters)
                    {
                        SpawnMonster(monster.monsterNumber);
                    }
                }
                else
                {
                    Debug.Log(floorNumber+"층 몬스터 출력 끝");
                    break;
                }
            }

            monsterManager.Refresh();
        }
    }

    public void SpawnCharacter(int characterNumber)
    {
        CharacterStat characterStat = SystemManager.Instance.GetCharacterStat(characterNumber);

        if (characterStat != null)
        {
            GameObject characterInstance = Instantiate(characterPrefab);

            CharacterSlot character = characterInstance.GetComponent<CharacterSlot>();
            character.CharacterNumber = characterNumber;
            

            characterInstance.transform.SetParent(characterRotate.transform, false);
        }
        else
        {
            Debug.Log("해당 번호 캐릭터 없음 번호 = " + characterNumber);
        }
    }

    public void SpawnMonster(int monsterNumber)
    {
        MonsterStat monsterStat = SystemManager.Instance.GetMonsterStat(monsterNumber);

        if (monsterStat != null)
        {
            GameObject monsterInstance = Instantiate(monsterPrefab);

            Monster monster = monsterInstance.GetComponent<Monster>();
            monster.MonsterNumber = monsterNumber;
            monsterManager.Monsters.Add(monster);

            monsterInstance.transform.SetParent(monsterManager.monsterSlotLine, false);
        }
        else
        {
            Debug.Log("해당 번호 몬스터 없음 번호 = "+monsterNumber);
        }
    }
}
