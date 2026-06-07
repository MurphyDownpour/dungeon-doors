using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// God-object. Owns all game state and drives screen transitions.
/// Attach to a single "GameManager" GameObject in the scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ── Inspector References ─────────────────────────────────────────────────

    [Header("Config")]
    [SerializeField] private GameConfig config;

    [Header("Systems")]
    [SerializeField] private CombatSystem combatSystem;

    [Header("UI Panels")]
    [SerializeField] private MainMenuUI       mainMenuUI;
    [SerializeField] private DoorSelectionUI  doorSelectionUI;
    [SerializeField] private CombatUI         combatUI;
    [SerializeField] private ShopUI           shopUI;
    [SerializeField] private DeathScreenUI    deathScreenUI;
    [SerializeField] private MetaProgressionUI metaProgressionUI;

    // ── Persistent State (PlayerPrefs) ───────────────────────────────────────

    private int Souls
    {
        get => PlayerPrefs.GetInt("Souls", 0);
        set => PlayerPrefs.SetInt("Souls", value);
    }

    private int MetaLevel(int index) => PlayerPrefs.GetInt($"MetaLevel_{index}", 0);
    private void SetMetaLevel(int index, int value) => PlayerPrefs.SetInt($"MetaLevel_{index}", value);

    // ── Runtime State ────────────────────────────────────────────────────────

    private RuntimePlayerStats _player;
    private int _currentRoom;
    private int _soulsEarnedThisRun;
    private int _totalGoldEarnedThisRun;

    // Tracks the selected door for the current room
    private DoorData _selectedDoor;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        combatSystem.OnCombatLog       += combatUI.AppendLog;
        combatSystem.OnPlayerHPChanged += combatUI.SetPlayerHP;
        combatSystem.OnEnemyHPChanged  += combatUI.SetEnemyHP;
    }

    private void Start() => ShowMainMenu();

    // ── Screen Transitions ───────────────────────────────────────────────────

    private void ShowMainMenu()
    {
        HideAll();
        mainMenuUI.Show();
    }

    public void StartRun()
    {
        InitPlayerForNewRun();
        _currentRoom = 1;
        _soulsEarnedThisRun = 0;
        _totalGoldEarnedThisRun = 0;
        ShowDoorSelection();
    }

    private void ShowDoorSelection()
    {
        HideAll();
        var doors = _currentRoom % config.bossRoomInterval == 0
            ? new List<DoorData> { new DoorData { roomType = RoomType.Boss, reward = PickRandomReward() } }
            : GenerateThreeDoors();

        doorSelectionUI.Show(_currentRoom, _player, doors, OnDoorSelected);
    }

    private void OnDoorSelected(DoorData door)
    {
        ApplyDoorReward(door);
        _selectedDoor = door;

        if (door.roomType == RoomType.Shop)
        {
            ShowShop();
        }
        else
        {
            StartCombat(door.roomType);
        }
    }

    private void StartCombat(RoomType roomType)
    {
        HideAll();
        EnemyStats enemy = BuildEnemy(roomType, _currentRoom);
        combatUI.Show(_currentRoom, enemy.name);
        combatUI.SetPlayerHP(_player.currentHP, _player.maxHP);
        combatUI.SetEnemyHP(enemy.currentHP, enemy.maxHP);
        combatSystem.StartCombat(_player, enemy, won => OnCombatEnd(won, enemy));
    }

    private void OnCombatEnd(bool playerWon, EnemyStats enemy)
    {
        if (!playerWon)
        {
            OnPlayerDeath();
            return;
        }

        int gold = Mathf.RoundToInt(enemy.goldReward * _player.goldGainMultiplier);
        _player.gold += gold;
        _totalGoldEarnedThisRun += gold;
        _soulsEarnedThisRun += enemy.soulReward;
        Souls += enemy.soulReward;

        _currentRoom++;
        ShowDoorSelection();
    }

    private void ShowShop()
    {
        HideAll();
        var items = PickShopItems(3);
        shopUI.Show(_player, items, OnItemPurchased, OnLeaveShop);
    }

    private void OnItemPurchased(ShopItemConfig item)
    {
        if (_player.gold < item.cost) return;
        _player.gold -= item.cost;
        ApplyShopItem(item);
        shopUI.RefreshGold(_player.gold);
    }

    private void OnLeaveShop()
    {
        _currentRoom++;
        ShowDoorSelection();
    }

    private void OnPlayerDeath()
    {
        HideAll();
        deathScreenUI.Show(_currentRoom - 1, _totalGoldEarnedThisRun, _soulsEarnedThisRun,
            OnGoToMetaProgression, StartRun);
    }

    private void OnGoToMetaProgression()
    {
        HideAll();
        metaProgressionUI.Show(Souls, config.upgrades, GetMetaLevels(), OnBuyUpgrade, StartRun);
    }

    private void OnBuyUpgrade(int upgradeIndex)
    {
        UpgradeConfig upg = config.upgrades[upgradeIndex];
        if (Souls < upg.costPerLevel) return;
        Souls -= upg.costPerLevel;
        SetMetaLevel(upgradeIndex, MetaLevel(upgradeIndex) + 1);
        metaProgressionUI.Refresh(Souls, GetMetaLevels());
    }

    private void HideAll()
    {
        mainMenuUI.Hide();
        doorSelectionUI.Hide();
        combatUI.Hide();
        shopUI.Hide();
        deathScreenUI.Hide();
        metaProgressionUI.Hide();
    }

    // ── Player Initialization ────────────────────────────────────────────────

    private void InitPlayerForNewRun()
    {
        int[] levels = GetMetaLevels();
        float bonusDmg  = config.upgrades[0].effectPerLevel * levels[0];
        float bonusHP   = config.upgrades[1].effectPerLevel * levels[1];
        float bonusGold = config.upgrades[2].effectPerLevel * levels[2];

        _player = new RuntimePlayerStats
        {
            maxHP              = config.startingHP + bonusHP,
            damage             = config.startingDamage + bonusDmg,
            attackSpeed        = config.startingAttackSpeed,
            critChance         = config.startingCritChance,
            goldGainMultiplier = 1f,
            gold               = config.startingGold + (int)bonusGold,
        };
        _player.currentHP = _player.maxHP;
    }

    private int[] GetMetaLevels()
    {
        int[] levels = new int[config.upgrades.Length];
        for (int i = 0; i < levels.Length; i++) levels[i] = MetaLevel(i);
        return levels;
    }

    // ── Door Generation ──────────────────────────────────────────────────────

    private List<DoorData> GenerateThreeDoors()
    {
        var doors = new List<DoorData>();
        var roomTypes = WeightedRoomTypePick(3);
        foreach (var rt in roomTypes)
            doors.Add(new DoorData { roomType = rt, reward = PickRandomReward() });
        return doors;
    }

    private List<RoomType> WeightedRoomTypePick(int count)
    {
        var pool = new List<RoomType>();
        for (int i = 0; i < config.weightNormal; i++) pool.Add(RoomType.Normal);
        for (int i = 0; i < config.weightElite;  i++) pool.Add(RoomType.Elite);
        for (int i = 0; i < config.weightShop;   i++) pool.Add(RoomType.Shop);

        var result = new List<RoomType>();
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            // Don't remove — duplicates allowed, matches GDD intent
        }
        return result;
    }

    private RewardType PickRandomReward()
    {
        var values = System.Enum.GetValues(typeof(RewardType));
        return (RewardType)values.GetValue(Random.Range(0, values.Length));
    }

    // ── Door Reward Application ──────────────────────────────────────────────

    private void ApplyDoorReward(DoorData door)
    {
        switch (door.reward)
        {
            case RewardType.Damage:
                _player.damage *= (1f + config.doorDamageBonusPercent);
                break;
            case RewardType.AttackSpeed:
                _player.attackSpeed *= (1f + config.doorAttackSpeedBonusPercent);
                break;
            case RewardType.MaxHP:
                float bonus = _player.maxHP * config.doorMaxHPBonusPercent;
                _player.maxHP    += bonus;
                _player.currentHP += bonus;
                break;
            case RewardType.RestoreHP:
                float restore = _player.maxHP * config.doorRestoreHPPercent;
                _player.currentHP = Mathf.Min(_player.currentHP + restore, _player.maxHP);
                break;
            case RewardType.GoldGain:
                _player.goldGainMultiplier *= (1f + config.doorGoldGainBonusPercent);
                break;
            case RewardType.CritChance:
                _player.critChance += config.doorCritChanceBonus;
                break;
        }
    }

    // ── Enemy Building ───────────────────────────────────────────────────────

    private EnemyStats BuildEnemy(RoomType roomType, int room)
    {
        float baseHP  = config.normalEnemyBaseHP  + config.normalEnemyHPPerRoom  * room;
        float baseDmg = config.normalEnemyBaseDamage + config.normalEnemyDamagePerRoom * room;

        switch (roomType)
        {
            case RoomType.Normal:
                int count = Random.Range(config.normalEnemyCountMin, config.normalEnemyCountMax + 1);
                int gold  = Random.Range(config.normalGoldMin, config.normalGoldMax + 1);
                // For normal rooms with multiple enemies, stack their HP for a combined "encounter" HP
                return new EnemyStats
                {
                    name        = count > 1 ? $"{count} Enemies" : "Enemy",
                    maxHP       = baseHP * count,
                    currentHP   = baseHP * count,
                    damage      = baseDmg,
                    attackSpeed = config.normalEnemyAttackSpeed,
                    goldReward  = gold,
                    soulReward  = 0,
                };
            case RoomType.Elite:
                return new EnemyStats
                {
                    name        = "Elite",
                    maxHP       = baseHP  * config.eliteHPMultiplier,
                    currentHP   = baseHP  * config.eliteHPMultiplier,
                    damage      = baseDmg * config.eliteDamageMultiplier,
                    attackSpeed = config.normalEnemyAttackSpeed,
                    goldReward  = config.eliteGoldReward,
                    soulReward  = config.eliteSoulReward,
                };
            case RoomType.Boss:
                return new EnemyStats
                {
                    name        = $"Boss (Room {room})",
                    maxHP       = baseHP  * config.bossHPMultiplier,
                    currentHP   = baseHP  * config.bossHPMultiplier,
                    damage      = baseDmg * config.bossDamageMultiplier,
                    attackSpeed = config.normalEnemyAttackSpeed,
                    goldReward  = config.bossGoldReward,
                    soulReward  = config.bossSoulReward,
                };
            default:
                return null;
        }
    }

    // ── Shop ─────────────────────────────────────────────────────────────────

    private List<ShopItemConfig> PickShopItems(int count)
    {
        var pool = new List<ShopItemConfig>(config.shopItems);
        var result = new List<ShopItemConfig>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }

    private void ApplyShopItem(ShopItemConfig item)
    {
        switch (item.effect)
        {
            case ShopItemEffect.Damage:
                _player.damage += item.effectValue;
                break;
            case ShopItemEffect.MaxHP:
                _player.maxHP    += item.effectValue;
                _player.currentHP = Mathf.Min(_player.currentHP + item.effectValue, _player.maxHP);
                break;
            case ShopItemEffect.AttackSpeed:
                _player.attackSpeed *= (1f + item.effectValue);
                break;
            case ShopItemEffect.CritChance:
                _player.critChance += item.effectValue;
                break;
        }
    }
}

// ── Data Structs ─────────────────────────────────────────────────────────────

public class DoorData
{
    public RoomType   roomType;
    public RewardType reward;
}
