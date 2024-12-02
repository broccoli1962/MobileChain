using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapCount : MonoBehaviour
{
    //탭 할때마다 한칸 씩 줄기
    //탭을 다하였을 경우 다음 턴으로 넘어가기 (턴을 관리하는 스크립트 필요)
    [SerializeField]
    public GameObject[] tap;
    PlayerSystem playerSystem;
    public int tapCount = 0;
    
    private void Start()
    {
        tapCount = tap.Length;
        playerSystem = FindAnyObjectByType<PlayerSystem>();
    }

    public void TapDown(int t)
    {
        if(tapCount >= 1)
        {
            tapCount -= t;

            if (tapCount <= 0)
            {
                StartCoroutine(playerSystem.AttackLogic());
            }
        }
    }

    public void TapDownImage()
    {
        int taps = tapCount-1;

        if (taps >= 0 && taps < tap.Length && tap[taps] != null)
        {
            tap[taps].gameObject.SetActive(false);
        }
    }

    public void EnableTapCount()
    {
        for (int i = 0; i < tap.Length; i++)
        {
            if (tap[i] != null)
            {
                tap[i].gameObject.SetActive(true);
            }
        }
    }
}