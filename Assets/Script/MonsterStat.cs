using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStat", menuName = "Scriptable Objects/MonsterStat")]
public class MonsterStat : ScriptableObject
{
    [SerializeField]
    public int Number;
    public int Health;
    public int Damage;
    public int Count;
    public Texture image;
    public List<string> pattern = new();

    [System.Serializable]
    public enum Element
    {
        fire, water, light, grass
    }
    public Element type;
}