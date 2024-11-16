using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public HealthBar HealthBar;
    private CharacterRotate character;
    public ParticleSystem AttackParticle;
    public TextMeshProUGUI monsterTurnCount;
    public int NowHp;
    public int MaxHealth;
    public int count;
    [SerializeField] private int MonsterNumber;
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

        if(targeting != null)
        {
            targeting.gameObject.SetActive(false);
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
        int power = firstSlot.GetDamage();
        //이팩트
        ParticleSystem particleInstance = Instantiate(AttackParticle, firstSlot.transform.position, Quaternion.identity);

        NowHp = NowHp - damage*power;
        Debug.Log(NowHp);
        particleInstance.Play();
        yield return StartCoroutine(HealthBar.SetHealth(NowHp));

        if(NowHp < 0)
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

    public IEnumerator MonsterTurn()
    {
        count--;
        if(count == 0)
        {
            monsterTurnCount.text = count.ToString();
            int damage = this.GetDamage();
            yield return StartCoroutine(GiveDamage(damage));
        }
        monsterTurnCount.text = count.ToString();
        yield return null;
    }

    public IEnumerator GiveDamage(int damage)
    {
        //공격하는 기능
        SystemManager manager = FindAnyObjectByType<SystemManager>();
        int rand = Random.Range(0, 4);
        CharacterSlot slot1 = character.GetFirstSlot();
        CharacterSlot slot2 = character.GetSecondSlot();
        CharacterSlot slot3 = character.GetThirdSlot();
        CharacterSlot slot4 = character.GetFourSlot();

        switch (rand)
        {
            case 0:
                damage -= slot1.GetArmor();
                Debug.Log("캐릭터 1공격");
                break;
            case 1:
                damage -= slot2.GetArmor();
                Debug.Log("캐릭터 2공격");
                break;
            case 2:
                damage -= slot3.GetArmor();
                Debug.Log("캐릭터 3공격");
                break;
            case 3:
                damage -= slot4.GetArmor();
                Debug.Log("캐릭터 4공격");
                break;
        }

        //방어력이 공격력보다 높은 경우
        if (damage <= 0)
        {
            damage = 0;
        }

        manager.currentHealth -= damage; 
        Debug.Log(this.name + "공격완료 현재체력 = " + manager.currentHealth);
        count = GetCount();
        yield return StartCoroutine(manager.healthBar.SetHealth(manager.currentHealth));
    }

    public int GetHealth() => MonsterStats.Health;
    public int GetDamage() => MonsterStats.Damage;
    public int GetCount() => MonsterStats.Count;
}
