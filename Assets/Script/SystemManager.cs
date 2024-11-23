using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SystemManager : MonoBehaviour
{
    public List<CharacterStat> clist = new List<CharacterStat>(); //캐릭터 정보 DB

    private Dictionary<int, CharacterStat> characterStats = new Dictionary<int, CharacterStat>();
    private Dictionary<int, MonsterStat> monsterStats = new Dictionary<int, MonsterStat>();
    public static SystemManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacterStats();
            LoadMonsterStats();
        }
    }

    //캐릭터 스탯
    private void LoadCharacterStats()
    {
        CharacterStat[] stats = Resources.LoadAll<CharacterStat>("CharacterStats");

        foreach(CharacterStat stat in stats)
        {
            characterStats.Add(stat.Number, stat);
        }
    }

    public CharacterStat GetCharacterStat(int characterNumber)
    {
        if(characterStats.TryGetValue(characterNumber, out CharacterStat stat))
        {
            return stat;
        }
        return null;
    }

    //몬스터 스탯
    private void LoadMonsterStats()
    {
        MonsterStat[] stats = Resources.LoadAll<MonsterStat>("MonsterStats");
        foreach(MonsterStat stat in stats)
        {
            monsterStats.Add(stat.Number, stat);
        }
    }
    public MonsterStat GetMonsterStat(int MonsterNumber)
    {
        if (monsterStats.TryGetValue(MonsterNumber, out MonsterStat stat))
        {
            return stat;
        }
        return null;
    }
}