using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "DungeonDoors/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Starting Player Stats")]
    public int startingHP = 100;
    public int startingDamage = 10;
    public float startingAttackSpeed = 1f;
    [Range(0f, 1f)] public float startingCritChance = 0.05f;
    public int startingGold = 0;

    [Header("Enemy Scaling (by Room Number)")]
    public int normalEnemyBaseHP = 30;
    public int normalEnemyHPPerRoom = 5;
    public int normalEnemyBaseDamage = 5;
    public int normalEnemyDamagePerRoom = 1;
    public float normalEnemyAttackSpeed = 1f;

    [Header("Elite Enemy Multipliers")]
    public float eliteHPMultiplier = 3f;
    public float eliteDamageMultiplier = 2f;
    public int eliteGoldReward = 50;
    public int eliteSoulReward = 1;

    [Header("Boss Enemy Multipliers")]
    public float bossHPMultiplier = 8f;
    public float bossDamageMultiplier = 3f;
    public int bossGoldReward = 150;
    public int bossSoulReward = 3;
    public int bossRoomInterval = 10;

    [Header("Normal Enemy Reward (random range)")]
    public int normalGoldMin = 10;
    public int normalGoldMax = 30;

    [Header("Normal Room Enemy Count")]
    public int normalEnemyCountMin = 1;
    public int normalEnemyCountMax = 3;

    [Header("Door Rewards")]
    public float doorDamageBonusPercent = 0.15f;
    public float doorAttackSpeedBonusPercent = 0.15f;
    public float doorMaxHPBonusPercent = 0.20f;
    public float doorRestoreHPPercent = 0.20f;
    public float doorGoldGainBonusPercent = 0.25f;
    [Range(0f, 1f)] public float doorCritChanceBonus = 0.05f;

    [Header("Room Type Weights (Normal/Elite/Shop — Boss overrides)")]
    public int weightNormal = 5;
    public int weightElite = 3;
    public int weightShop = 2;

    [Header("Shop Items")]
    public ShopItemConfig[] shopItems = new ShopItemConfig[]
    {
        new ShopItemConfig { itemName = "Sword",  cost = 50, description = "+5 Damage",         effect = ShopItemEffect.Damage,      effectValue = 5f  },
        new ShopItemConfig { itemName = "Armor",  cost = 50, description = "+25 Max HP",        effect = ShopItemEffect.MaxHP,       effectValue = 25f },
        new ShopItemConfig { itemName = "Gloves", cost = 75, description = "+10% Attack Speed", effect = ShopItemEffect.AttackSpeed, effectValue = 0.10f },
        new ShopItemConfig { itemName = "Ring",   cost = 75, description = "+5% Crit Chance",   effect = ShopItemEffect.CritChance,  effectValue = 0.05f },
    };

    [Header("Meta Upgrades")]
    public UpgradeConfig[] upgrades = new UpgradeConfig[]
    {
        new UpgradeConfig { upgradeName = "Damage Upgrade", costPerLevel = 1, description = "+1 Starting Damage per level", effect = UpgradeEffect.StartingDamage, effectPerLevel = 1f },
        new UpgradeConfig { upgradeName = "HP Upgrade",     costPerLevel = 1, description = "+5 Starting HP per level",     effect = UpgradeEffect.StartingHP,     effectPerLevel = 5f },
        new UpgradeConfig { upgradeName = "Gold Upgrade",   costPerLevel = 1, description = "+5 Starting Gold per level",   effect = UpgradeEffect.StartingGold,   effectPerLevel = 5f },
    };
}

[System.Serializable]
public class ShopItemConfig
{
    public string itemName;
    public int cost;
    [TextArea(1, 2)] public string description;
    public ShopItemEffect effect;
    public float effectValue;
}

[System.Serializable]
public class UpgradeConfig
{
    public string upgradeName;
    public int costPerLevel;
    [TextArea(1, 2)] public string description;
    public UpgradeEffect effect;
    public float effectPerLevel;
}

public enum ShopItemEffect  { Damage, MaxHP, AttackSpeed, CritChance }
public enum UpgradeEffect   { StartingDamage, StartingHP, StartingGold }
public enum RoomType        { Normal, Elite, Shop, Boss }
public enum RewardType      { Damage, AttackSpeed, MaxHP, RestoreHP, GoldGain, CritChance }
