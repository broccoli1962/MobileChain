using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MonsterStat", menuName = "Scriptable Objects/MonsterStat")]
public class MonsterStat : ScriptableObject
{
    [SerializeField]
    public int Number;
    public int Health;
    public int Damage;
    public int Count;
    public Texture image;

    [System.Serializable]
    public enum Element
    {
        fire, water, light, grass
    }
    public Element type;
}