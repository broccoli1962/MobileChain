using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        start, playerTurn, enemyTurn, win
    }

    public State state;
    public bool isLive;

    private void Awake()
    {
        state = State.start;
        BattleStart();
    }

    void BattleStart()
    {
        state = State.playerTurn;
    }

    public void PlayerAttackTurn()
    {
        if (state != State.playerTurn)
        {
            return;
        }
        StartCoroutine(PlayerAttack());
    }

    public IEnumerator PlayerAttack()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("�÷��̾� ����");
        if (!isLive)
        {
            state = State.win;
            EndBattle();
        }
        else
        {
            state = State.enemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    void EndBattle()
    {
        Debug.Log("���� ����");
    }

    public IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        state = State.playerTurn;
    }
}
