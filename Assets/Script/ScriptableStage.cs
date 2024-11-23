using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableStage", menuName = "Scriptable Objects/ScriptableStage")]
public class ScriptableStage : ScriptableObject
{
    public int stageNumber;
    public List<FloorData> floors = new List<FloorData>();
}

[System.Serializable]
public class FloorData
{
    public int floorNumber;
    public List<MonsterSpawnData> monsters = new List<MonsterSpawnData>();
}

[System.Serializable]
public class MonsterSpawnData
{
    public int monsterNumber;
}