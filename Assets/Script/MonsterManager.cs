using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public List<Monster> Monsters = new List<Monster>();
    public Transform monsterSlotLine;
    [SerializeField] private int monsterIndex = 0;
    private Monster selectedMonster;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        Monsters.Clear();
        if (monsterSlotLine != null)
        {
            for (int i = 0; i < monsterSlotLine.childCount; i++)
            {
                Monster monster = monsterSlotLine.GetChild(i).GetComponent<Monster>();
                if (monster != null)
                {
                    Monsters.Add(monster);
                }
                else
                {
                    Debug.Log("몬스터 발견 불가");
                }
            }
        }
    }

    public Monster NextTarget()
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            if (Monsters[i].NowHp>0)
            {
                return Monsters[i];
            }
        }
        return null;
    }

    public Monster GetMonster()
    {
        if (monsterIndex >= 0 && monsterIndex < Monsters.Count)
        {
            return Monsters[monsterIndex];
        }
        return null;
    }

    public void SelectMonster(int index)
    {
        selectedMonster = GetMonster();
        if (selectedMonster != Monsters[index])
        {
            selectedMonster.DeSelected();
        }
        monsterIndex = index;
        Debug.Log(monsterIndex + "선택됨");
    }
}
