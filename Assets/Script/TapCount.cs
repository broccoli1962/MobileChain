using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapCount : MonoBehaviour
{
    //탭 할때마다 한칸 씩 줄기
    //탭을 다하였을 경우 다음 턴으로 넘어가기 (턴을 관리하는 스크립트 필요)
    [SerializeField]
    public GameObject[] tap;
    CharacterRotate rotate;
    SystemManager system;
    private int tapCount = 0;
    
    private void Start()
    {
        tapCount = tap.Length;
        system = GameObject.Find("GameSystemManager").GetComponent<SystemManager>();
        rotate = GameObject.Find("CharacterSlotLine").GetComponent<CharacterRotate>();
    }

    public void TapDown(int t)
    {
        if(tapCount >= 1)
        {
            tapCount -= t;

            if(tapCount>=0 && tapCount < tap.Length && tap[tapCount] != null)
            {
                tap[tapCount].gameObject.SetActive(false);
            }
            if (tapCount <= 0)
            {
                system.turn = false;
                NextTurn();
                Debug.Log(rotate.GetFirstSlot());
            }
        }
    }

    public void NextTurn()
    {
        system.turn = true;
        tapCount = system.TotaltapCount;
        for(int i = 0; i<tap.Length; i++)
        {
            if (tap[i] != null)
            {
                tap[i].gameObject.SetActive(true);
            }
        }
        rotate.NextTurn();
    }
}