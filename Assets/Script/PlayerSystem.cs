using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public bool turn = false;
    public int canTapCount;
    public int maxHealth;
    public int currentHealth;
    public int totalBreak;
    public int totalheal;
    public int floorNumber = 0;

    [SerializeField] private CharacterRotate crotate;
    [SerializeField] private TapCount tcount;

    public HealthBar healthBar;
    public TextMeshProUGUI healthText;
    StageManager stageManager;

    private void Start()
    {
        stageManager = StageManager.instance;
        stageManager.monsterManager = GameObject.FindAnyObjectByType<MonsterManager>();
        FirstMonster();
    }

    public void FirstMonster()
    {
        stageManager.LoadStageMonster(stageManager.StageNumber, floorNumber);
    }

    public void AddHealth(int health)
    {
        maxHealth += health;
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        HpTextSet();
    }

    public void HpTextSet()
    {
        if (currentHealth > 0)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }
        else
        {
            healthText.text = "0" + " / " + maxHealth;
        }
    }

    public IEnumerator AttackLogic()
    {
        turn = true; //�� ������ bool
        yield return new WaitForSeconds(2f);

        //�÷��̾� ��
        MonsterManager monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        Monster selectMonster = monsterManager.GetMonster();

        if (selectMonster != null)
        {
            yield return StartCoroutine(selectMonster.TakeDamage(totalBreak));
        }
        StartCoroutine(PlayerHeal());

        //���� ��
        yield return new WaitForSeconds(2f);
        StartCoroutine(MonsterAttack());

        //�� �̵�
        tcount.EnableTapCount();
        crotate.NextTurn();
        totalBreak = 0;
        totalheal = 0;
        tcount.tapCount = canTapCount;
        //���� �ν��Ͻ� �߰� -> refresh();
        if (monsterManager.NextTarget() == null)
        {
            //���� �߰� �� �ִϸ��̼�

            //���� �߰�
            stageManager.LoadStageMonster(stageManager.StageNumber, floorNumber);
            Debug.Log(floorNumber + " �� ���� Ŭ����");
            ++floorNumber;
        }
        turn = false;
    }

    IEnumerator PlayerHeal()
    {
        CharacterSlot character = crotate.GetFirstSlot();
        int healPower = character.GetHealPower();
        currentHealth += healPower * totalheal;
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
        yield return healthBar.IncreaseHealth(currentHealth);
    }

    IEnumerator MonsterAttack()
    {
        MonsterManager monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        //��� ���� ����Ʈ
        for (int i = 0; i < monsterManager.Monsters.Count; i++)
        {
            if (monsterManager.Monsters != null)
            {
                yield return StartCoroutine(monsterManager.Monsters[i].MonsterTurn());
            }
        }
    }
}
