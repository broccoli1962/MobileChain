using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class CharacterStat : ScriptableObject
{
    [SerializeField]
    public int Number;
    public int Health;
    public int Damage;
    public int Heal;
    public int Armor;
    public Texture image;

    [System.Serializable]
    public enum Ability1
    {
        None, CanProtectBreak, PowerUp
    }
    public Ability1 ability1;
    [System.Serializable]
    public enum Ability2
    {
        None, CanProtectBreak, PowerUp
    }
    public Ability2 ability2;
}