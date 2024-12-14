using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public HealthBar HealthBar;
    private CharacterRotate character;
    public ParticleSystem AttackParticle;
    public TextMeshProUGUI monsterTurnCount;
    public GameObject bullet;
    public int NowHp;
    public int MaxHealth;
    public int count;
    public List<string> patterns;
    [SerializeField] public int MonsterNumber;
    [SerializeField] private RawImage image;
    [SerializeField] private UnityEngine.UI.Image targeting;

    private MonsterStat MonsterStats;
    private void Start()
    {
        SystemManager system = SystemManager.Instance;
        character = FindAnyObjectByType<CharacterRotate>();
        //몬스터 종류 파악
        MonsterStats = system.GetMonsterStat(MonsterNumber);
        SetMonster(MonsterNumber);
        //채력 설정
        MaxHealth = MonsterStats.Health;
        NowHp = MaxHealth;
        HealthBar.SetMaxHealth(MaxHealth);
        //몬스터 턴 설정
        monsterTurnCount.text = MonsterStats.Count.ToString();
        count = GetCount();
        //몬스터 패턴 설정
        SetPattern();

        if (targeting != null)
        {
            targeting.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(patterns.Count == 0)
        {
            SetPattern();
        }
    }

    public void SetPattern()
    {
        if (MonsterStats.pattern != null)
        {
            foreach (string p in MonsterStats.pattern)
            {
                patterns.Add(p);
            }
        }
    }


    public void SetMonster(int MonsterNumber)
    {
        if (MonsterStats != null)
        {
            image.texture = MonsterStats.image;
        }
        else
        {
            Debug.LogError("몬스터 스탯에 맞는 번호 없음" + MonsterStats);
        }
    }

    public void Selected()
    {
        CancelInvoke("DeSelected");
        targeting.gameObject.SetActive(true);
        Invoke("DeSelected", 5f);
        ConvertIndex();
    }

    public void ConvertIndex()
    {
        MonsterManager monsterManager = FindAnyObjectByType<MonsterManager>();
        if (monsterManager != null)
        {
            for (int i = 0; i < monsterManager.Monsters.Count; i++)
            {
                if (monsterManager.Monsters[i] == this)
                {
                    monsterManager.SelectMonster(i);
                    break;
                }
            }
        }
    }

    public void DeSelected()
    {
        targeting.gameObject.SetActive(false);
    }


    public IEnumerator TakeDamage(int damage)
    {
        //데미지 계산
        CharacterSlot firstSlot = character.GetFirstSlot();

        CharacterStat.Element playerElement = firstSlot.GetElement();
        MonsterStat.Element monsterElement = this.GetElement();

        int power = firstSlot.GetDamage();
        //이팩트
        ParticleSystem particleInstance = Instantiate(AttackParticle, firstSlot.transform.position, Quaternion.identity);

        int Totaldamage = ElementsLogic(damage, power, playerElement, monsterElement, true);
        NowHp -= Totaldamage;
        particleInstance.Play();
        yield return StartCoroutine(HealthBar.SetHealth(NowHp));

        if(NowHp <= 0)
        {
            MonsterManager manager = FindAnyObjectByType<MonsterManager>();
            Monster nextMonster = manager.NextTarget();
            if (nextMonster != null) { 
                yield return StartCoroutine(nextMonster.TakeDamage(-NowHp/power));
            }
        }

        particleInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(particleInstance.gameObject, particleInstance.main.duration);

        if (NowHp <= 0)
        {
            Debug.Log("죽음");
            Destroy(gameObject);
        }
        else
        {
            ConvertIndex();
            //인덱스 전환
        }
    }

    public int ElementsLogic(int damage, int power, CharacterStat.Element c1, MonsterStat.Element m1 ,bool morc)
    {
        int Totaldamage = 0;
        //상성 유리 시 공격
        if (c1.ToString() == "water" && m1.ToString() == "fire" || c1.ToString() == "fire" && m1.ToString() == "grass" || c1.ToString() == "grass" && m1.ToString() == "light" || c1.ToString() == "light" && m1.ToString() == "water")
        {
            if (morc) {
                Totaldamage = damage * (power * 2);
            }
            else
            {
                Totaldamage = damage * (power / 2);
            }
        }
        //상성 불리 시 공격
        else if (c1.ToString() == "fire" && m1.ToString() == "water" || c1.ToString() == "water" && m1.ToString() == "light" || c1.ToString() == "light" && m1.ToString() == "grass" || c1.ToString() == "grass" && m1.ToString() == "fire")
        {
            if (morc)
            {
                Totaldamage = damage * (power / 2);
            }
            else
            {
                Totaldamage = damage * (power * 2);
            }
        }
        else
        {
            Totaldamage = damage * power;
        }
        return Totaldamage;
    }
    public IEnumerator MonsterTurn()
    {
        count--;
        if(count == 0 && this != null)
        {
            monsterTurnCount.text = count.ToString();
            int damage = this.GetDamage();
            switch (patterns[0])
            {
                case "전체공격":
                    yield return StartCoroutine(GiveDamageAll(damage));
                    Debug.Log(patterns[0]);
                    patterns.RemoveAt(0);
                    break;
                case "단일공격":
                    yield return StartCoroutine(GiveDamage(damage));
                    Debug.Log(patterns[0]);
                    patterns.RemoveAt(0);
                    break;
                case "즉사공격":
                    yield return StartCoroutine(GiveDamage(damage*100));
                    patterns.RemoveAt(0);
                    break;
                default:
                    Debug.Log("리스트에 비었음");
                    yield return StartCoroutine(GiveDamage(damage));
                    break;
            }
        }
        monsterTurnCount.text = count.ToString();
        yield return null;
    }

    public IEnumerator GiveDamageAll(int damage)
    {
        PlayerSystem manager = FindAnyObjectByType<PlayerSystem>();
        int totalArmor = 0;
        foreach(CharacterSlot a in character.CharacterSlots)
        {
            totalArmor += a.GetArmor();
        }
        CharacterSlot firstSlot = character.GetFirstSlot();
        int dm = ElementsLogic(1, damage, firstSlot.GetElement(), this.GetElement(), false);
        dm *= character.CharacterSlots.Count();
        dm -= totalArmor;

        if (dm <= 0)
        {
            dm = 0;
        }

        manager.currentHealth -= dm;
        count = GetCount();
        yield return StartCoroutine(manager.healthBar.SetHealth(manager.currentHealth));
    }

    public IEnumerator GiveDamage(int damage) //전체 공격, 단일 대상 공격, 특정 속성 공격 제작예정, 공격 방식은 json string으로 받는다.
    {
        //공격하는 기능
        PlayerSystem manager = FindAnyObjectByType<PlayerSystem>();
        int rand = Random.Range(0, 4);
        CharacterSlot slot1 = character.GetFirstSlot();
        CharacterSlot slot2 = character.GetSecondSlot();
        CharacterSlot slot3 = character.GetThirdSlot();
        CharacterSlot slot4 = character.GetFourSlot();

        CharacterSlot temp = null;
        int dm = 0;

        switch (rand)
        {
            case 0:
                dm = ElementsLogic(1, damage, slot1.GetElement(), this.GetElement(), false);
                dm -= slot1.GetArmor();
                temp = slot1;
                break;
            case 1:
                dm = ElementsLogic(1, damage, slot1.GetElement(), this.GetElement(), false);
                dm -= slot2.GetArmor();
                temp = slot2;
                break;
            case 2:
                dm = ElementsLogic(1, damage, slot1.GetElement(), this.GetElement(), false);
                dm -= slot3.GetArmor();
                temp = slot3;
                break;
            case 3:
                dm = ElementsLogic(1, damage, slot1.GetElement(), this.GetElement(), false);
                dm -= slot4.GetArmor();
                temp = slot4;
                break;
        }

        //방어력이 공격력보다 높은 경우
        if (dm <= 0)
        {
            dm = 0;
        }

        //단일 공격
        yield return StartCoroutine(AttackAnimation(temp));
        manager.currentHealth -= dm;
        count = GetCount();
        yield return StartCoroutine(manager.healthBar.SetHealth(manager.currentHealth));
    }

    IEnumerator AttackAnimation(CharacterSlot charac)
    {
        float speed = 1500f; // 이동 속도 조절

        GameObject item = Instantiate(bullet, Vector3.zero, Quaternion.identity);
        item.transform.SetParent(this.transform, false);

        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        Canvas canvas = item.GetComponentInParent<Canvas>();

        Vector3 targetWorldPosition = charac.transform.position;
        Vector2 localPoint;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetWorldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)this.transform, screenPoint, canvas.worldCamera, out localPoint);

        while (Vector2.Distance(itemRectTransform.anchoredPosition, localPoint) > 0.1f) // 목표 지점에 도착했는지 확인
        {
            itemRectTransform.anchoredPosition = Vector2.MoveTowards(itemRectTransform.anchoredPosition, localPoint, speed * Time.deltaTime);
            yield return null;
        }
        Destroy(item);
    }

    public int GetHealth() => MonsterStats.Health;
    public int GetDamage() => MonsterStats.Damage;
    public int GetCount() => MonsterStats.Count;
    public MonsterStat.Element GetElement() => MonsterStats.type;
}
