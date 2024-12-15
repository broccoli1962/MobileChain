using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    public enum attackType
    {
        single, full, death
    }
    public attackType attackTypes;
    public float multifular;
    public int count;
    //단일공격용
    public bool spread;
}

[CreateAssetMenu(fileName = "MonsterStat", menuName = "Scriptable Objects/MonsterStat")]
public class MonsterStat : ScriptableObject
{
    [SerializeField]
    public int Number;
    public int Health;
    public int Damage;
    public int Count;
    public Texture image;
    public Pattern[] pattern;

    [System.Serializable]
    public enum Element
    {
        fire, water, light, grass
    }
    public Element type;
}