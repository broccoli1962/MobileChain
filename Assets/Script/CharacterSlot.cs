using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CharacterSlot : MonoBehaviour
{
    public int CharacterNumber;
    [SerializeField] private RawImage image;
    [SerializeField] private RawImage slotImage;

    private CharacterStat characterStat;
    //characterStat에서 받은 정보 표출시키기

    private void Start()
    {
        SystemManager system = SystemManager.Instance;
        characterStat = system.GetCharacterStat(CharacterNumber);
        PlayerSystem p = FindAnyObjectByType<PlayerSystem>();
        if (p != null)
        {
            p.AddHealth(characterStat.Health);
        }
        SetCharacter(CharacterNumber);
    }

    public void SetCharacter(int characterNumber)
    {
        if (characterStat != null)
        {
            image.texture = characterStat.image;
            getElementsColor(characterStat.type.ToString());
        }
        else
        {
            Debug.LogError("캐릭터 스탯에 맞는 번호 없음" + CharacterNumber);
        }
    }

    private void getElementsColor(string type)
    {
        switch (type)
        {
            case "fire":
                slotImage.color = Color.red;
                break;
            case "water":
                slotImage.color = Color.blue;
                break;
            case "light":
                slotImage.color = Color.yellow;
                break;
            case "grass":
                slotImage.color = Color.green;
                break;
        }
    }

    public int GetHealth() => characterStat.Health;
    public int GetDamage() => characterStat.Damage;
    public int GetHealPower() => characterStat.Heal;
    public int GetArmor() => characterStat.Armor;
    public Texture GetImage() => characterStat.image;
    public CharacterStat.Element GetElement() => characterStat.type;
    public CharacterStat.Ability1 GetAbility1() => characterStat.ability1;
    public CharacterStat.Ability2 GetAbility2() => characterStat.ability2;
}
