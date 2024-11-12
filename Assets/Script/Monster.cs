using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    public HealthBar HealthBar;
    private CharacterRotate character;
    public ParticleSystem AttackParticle;
    public int NowHp;
    public int MaxHealth;
    [SerializeField] private int MonsterNumber;
    [SerializeField] private RawImage image;
    [SerializeField] private UnityEngine.UI.Image targeting;

    private MonsterStat MonsterStats;
    private void Start()
    {
        SystemManager system = SystemManager.Instance;
        character = FindAnyObjectByType<CharacterRotate>();
        MonsterStats = system.GetMonsterStat(MonsterNumber);
        SetMonster(MonsterNumber);
        MaxHealth = MonsterStats.Health;
        NowHp = MaxHealth;
        HealthBar.SetMaxHealth(MaxHealth);

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

    public int GetHealth() => MonsterStats.Health;
    public int GetDamage() => MonsterStats.Damage;
}
