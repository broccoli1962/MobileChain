using UnityEngine;
using UnityEngine.UI;

public class CharacterRotate : MonoBehaviour
{
    //slot의 회전 기능 구현
    public GridLayoutGroup layout;
    public CharacterSlot[] CharacterSlots;
    [SerializeField] private int Characterindex = 0;
    public float duringTime = 0;

    private void Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        CharacterSlots = GetComponentsInChildren<CharacterSlot>();
    }

    public void NextTurn()
    {
        Characterindex = (Characterindex + 1) % CharacterSlots.Length;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        for (int i = 0; i < CharacterSlots.Length; i++)
        {
            int targetIndex = (i + Characterindex) % CharacterSlots.Length;
            CharacterSlots[targetIndex].transform.SetSiblingIndex(i);
        }
        layout.CalculateLayoutInputHorizontal();
        layout.SetLayoutHorizontal();
    }

    public CharacterSlot GetFirstSlot()
    {
        return CharacterSlots[Characterindex];
    }
    public CharacterSlot GetSecondSlot()
    {
        return CharacterSlots[(Characterindex + 1) % CharacterSlots.Length];
    }
    public CharacterSlot GetThirdSlot()
    {
        return CharacterSlots[(Characterindex + 2) % CharacterSlots.Length];
    }
    public CharacterSlot GetFourSlot()
    {
        return CharacterSlots[(Characterindex + 3) % CharacterSlots.Length];
    }
}