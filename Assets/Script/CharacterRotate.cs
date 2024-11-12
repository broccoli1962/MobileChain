using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRotate : MonoBehaviour
{
    //slot의 회전 기능 구현
    public HorizontalLayoutGroup layout;
    public CharacterSlot[] CharacterSlots;
    private int Characterindex = 0;
    public float duringTime = 0;

    private void Start()
    {
        layout = GetComponent<HorizontalLayoutGroup>();
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
}