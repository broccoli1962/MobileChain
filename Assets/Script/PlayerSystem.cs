using System.Collections;
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
    public GameObject floorAnimation;
    public TextMeshProUGUI floorText;
    StageManager stageManager;

    private void Start()
    {
        stageManager = StageManager.instance;
        stageManager.characterRotate = GameObject.FindAnyObjectByType<CharacterRotate>();
        stageManager.monsterManager = GameObject.FindAnyObjectByType<MonsterManager>();
        FirstSetting();
    }

    public void FirstSetting()
    {
        for(int i = 0; i<stageManager.characters.Count; i++)
        {
            stageManager.SpawnCharacter(stageManager.characters[i]);
            //crotate�� characterslots�� ĳ���� �־���� ���� �Ȼ���
           
        }
        crotate.CharacterSlots = crotate.GetComponentsInChildren<CharacterSlot>();
        stageManager.LoadStageMonster(stageManager.StageNumber, floorNumber);
        stageManager.characters.Clear();
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
        if(currentHealth < 1)
        {
            Debug.Log("���� ���� ���� ����");
        }

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
            yield return StartCoroutine(FloorAnimaition());
            //���� �߰�
            stageManager.LoadStageMonster(stageManager.StageNumber, floorNumber);
            Debug.Log(floorNumber + " �� ���� Ŭ����");
            ++floorNumber;
        }
        turn = false;
    }

    IEnumerator FloorAnimaition()
    {
        yield return null;
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
