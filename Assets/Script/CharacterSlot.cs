using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CharacterSlot : MonoBehaviour
{
    [SerializeField] private int CharacterNumber;
    [SerializeField] private RawImage image;

    private CharacterStat characterStat;
    //characterStat에서 받은 정보 표출시키기

    private void Start()
    {
        SystemManager system = SystemManager.Instance;
        characterStat = system.GetCharacterStat(CharacterNumber);
        SystemManager.Instance.AddHealth(characterStat.Health);
        SetCharacter(CharacterNumber);
    }

    public void SetCharacter(int characterNumber)
    {
        if (characterStat != null)
        {
            image.texture = characterStat.image;
        }
        else
        {
            Debug.LogError("캐릭터 스탯에 맞는 번호 없음" + CharacterNumber);
        }
    }

    public int GetHealth() => characterStat.Health;
    public int GetDamage() => characterStat.Damage;
    public int GetHealPower() => characterStat.Heal;
    public int GetArmor() => characterStat.Armor;
    public CharacterStat.Ability1 GetAbility1() => characterStat.ability1;
    public CharacterStat.Ability2 GetAbility2() => characterStat.ability2;
}
