using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public List<ScriptableStage> stageDatas;
    public int StageNumber;
    public string monsterManagerSceneName = "MainScene";
    public MonsterManager monsterManager;
    public GameObject monsterPrefab;

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

    public ScriptableStage LoadStage(int stageNumber) //�������� ���� ��������
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

    public void LoadStageMonster(int stageNumber, int floorNumber) //���� �� ���� ȣ��
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
                    Debug.Log(floorNumber+"�� ���� ��� ��");
                    break;
                }
            }

            monsterManager.Refresh();
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
            Debug.Log("�ش� ��ȣ ���� ���� ��ȣ = "+monsterNumber);
        }
    }
}
