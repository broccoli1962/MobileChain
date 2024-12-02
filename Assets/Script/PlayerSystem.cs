using System.Collections;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private RectTransform floor;

    public HealthBar healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI floorText;
    StageManager stageManager;

    private void Start()
    {
        stageManager = StageManager.instance;
        StartCoroutine(FloorAnimaition());
        stageManager.characterRotate = GameObject.FindAnyObjectByType<CharacterRotate>();
        stageManager.monsterManager = GameObject.FindAnyObjectByType<MonsterManager>();
        
        FirstSetting();
    }

    public IEnumerator AttackLogic()
    {
        turn = true; //탭 금지용 bool
        yield return new WaitForSeconds(2f);

        //플레이어 턴
        MonsterManager monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        Monster selectMonster = monsterManager.GetMonster();

        if (selectMonster != null)
        {
            yield return StartCoroutine(selectMonster.TakeDamage(totalBreak));
        }
        StartCoroutine(PlayerHeal());

        //몬스터 턴
        yield return new WaitForSeconds(2f);
        StartCoroutine(MonsterAttack());
        if (currentHealth < 1)
        {
            Debug.Log("게임 오버 로직 제작");
        }

        //턴 이동
        tcount.EnableTapCount();
        crotate.NextTurn();
        totalBreak = 0;
        totalheal = 0;
        tcount.tapCount = canTapCount;
        //몬스터 인스턴스 추가 -> refresh();
        if (monsterManager.NextTarget() == null)
        {
            floorNumber++;
            //몬스터 추가 전 애니메이션
            yield return StartCoroutine(FloorAnimaition());
            //몬스터 추가
            stageManager.LoadStageMonster(stageManager.StageNumber, floorNumber);
            Debug.Log(floorNumber + " 층 몬스터 클리어");
        }
        turn = false;
    }

    IEnumerator FloorAnimaition()
    {
        float animationTime = 0.3f;
        float nowTime = 0f;

        floorText.text = (floorNumber+1).ToString() + " FLOOR";

        yield return new WaitForSeconds(1f);

        while (nowTime < animationTime)
        {
            floor.localPosition = Vector3.Lerp(floor.localPosition, new Vector3(0, floor.localPosition.y, floor.localPosition.z), nowTime / animationTime);
            nowTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    
        nowTime = 0f;
        while (nowTime < animationTime) {
            floor.localPosition = Vector3.Lerp(floor.localPosition, new Vector3(-1080, floor.localPosition.y, floor.localPosition.z), nowTime / animationTime);
            nowTime += Time.deltaTime;
            yield return null;
        }

        floor.localPosition = new Vector3(1080, floor.localPosition.y, floor.localPosition.z);
    }

    public void FirstSetting()
    {
        for(int i = 0; i<stageManager.characters.Count; i++)
        {
            stageManager.SpawnCharacter(stageManager.characters[i]);
            //crotate의 characterslots에 캐릭터 넣어줘야 버그 안생김
           
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
        //모든 몬스터 리스트
        for (int i = 0; i < monsterManager.Monsters.Count; i++)
        {
            if (monsterManager.Monsters != null)
            {
                yield return StartCoroutine(monsterManager.Monsters[i].MonsterTurn());
            }
        }
    }
}
