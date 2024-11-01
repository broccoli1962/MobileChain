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
    //characterStat���� ���� ���� ǥ���Ű��

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
            Debug.LogError("ĳ���� ���ȿ� �´� ��ȣ ����" + CharacterNumber);
        }
    }

    public int GetHealth() => characterStat.Health;
    public int GetDamage() => characterStat.Damage;
    public int GetHealPower() => characterStat.Heal;
    public int GetArmor() => characterStat.Armor;
    public CharacterStat.Ability1 GetAbility1() => characterStat.ability1;
    public CharacterStat.Ability2 GetAbility2() => characterStat.ability2;
}
