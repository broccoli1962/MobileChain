using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapCount : MonoBehaviour
{
    //�� �Ҷ����� ��ĭ �� �ٱ�
    //���� ���Ͽ��� ��� ���� ������ �Ѿ�� (���� �����ϴ� ��ũ��Ʈ �ʿ�)
    [SerializeField]
    public GameObject[] tap;
    SystemManager system;
    public int tapCount = 0;
    
    private void Start()
    {
        tapCount = tap.Length;
        system = GameObject.Find("GameSystemManager").GetComponent<SystemManager>();
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
                StartCoroutine(system.AttackLogic());
            }
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