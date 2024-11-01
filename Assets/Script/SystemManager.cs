using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public bool turn;
    public int TotaltapCount;
    public int MaxHealth;
    public int currentHealth;
    public List<CharacterStat> clist = new List<CharacterStat>(); //캐릭터 정보 DB

    [SerializeField] private CharacterRotate crotate;
    public HealthBar healthBar;

    private Dictionary<int, CharacterStat> characterStats = new Dictionary<int, CharacterStat>();

    public static SystemManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCharacterStats();
        }
    }

    public void AddHealth(int health)
    {
        MaxHealth += health;
        healthBar.SetMaxHealth(MaxHealth);
        currentHealth = MaxHealth;
    }

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeD(20);
        }
    }

    public void TakeD(int d)
    {
        currentHealth -= d;
        healthBar.SetHealth(currentHealth);
    }
}