using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SystemManager : MonoBehaviour
{
    public bool turn = false;
    public int canTapCount;
    public int maxHealth;
    public int currentHealth;
    public int totalBreak;

    public List<CharacterStat> clist = new List<CharacterStat>(); //ĳ���� ���� DB

    [SerializeField] private CharacterRotate crotate;
    [SerializeField] private TapCount tcount;

    public HealthBar healthBar;
    public TextMeshProUGUI healthText;

    private Dictionary<int, CharacterStat> characterStats = new Dictionary<int, CharacterStat>();
    private Dictionary<int, MonsterStat> monsterStats = new Dictionary<int, MonsterStat>();
    public static SystemManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacterStats();
            LoadMonsterStats();
        }
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
        Test();
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

    //ĳ���� ����
    private void LoadCharacterStats()
    {
        CharacterStat[] stats = Resources.LoadAll<CharacterStat>("CharacterStats");

        foreach(CharacterStat stat in stats)
        {
            characterStats.Add(stat.Number, stat);
        }
    }

    public CharacterStat GetCharacterStat(int characterNumber)
    {
        if(characterStats.TryGetValue(characterNumber, out CharacterStat stat))
        {
            return stat;
        }
        return null;
    }

    //���� ����
    private void LoadMonsterStats()
    {
        MonsterStat[] stats = Resources.LoadAll<MonsterStat>("MonsterStats");
        foreach(MonsterStat stat in stats)
        {
            monsterStats.Add(stat.Number, stat);
        }
    }
    public MonsterStat GetMonsterStat(int MonsterNumber)
    {
        if (monsterStats.TryGetValue(MonsterNumber, out MonsterStat stat))
        {
            return stat;
        }
        return null;
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
        //���� ��
        StartCoroutine(MonsterAttack());
        //�� �̵�
        tcount.EnableTapCount();
        crotate.NextTurn();
        totalBreak = 0;
        tcount.tapCount = canTapCount;
        //���� �ν��Ͻ� �߰� -> refresh();
        if (monsterManager.NextTarget() == null)
        {
            Debug.Log("���� Ŭ����");
            //monsterManager.Monsters.Add();
            //���� �߰�
        }
        
        turn = false;
    }

    IEnumerator MonsterAttack()
    {
        yield return new WaitForSeconds(2f);
    }

    public void Test() 
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            TakeD(5);
        }
    }

    public void TakeD(int d)
    {
        currentHealth -= d;
        healthBar.SetHealth(currentHealth);
    }
}