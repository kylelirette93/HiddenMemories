using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currency;
    public List<UpgradeData> purchasedUpgrades;
    public List<InventoryData> inventoryData;
    public List<bool> doorsOpened;
    public int timesWon;
    public EnemyData enemyData;
    public GameData()
    {
        // Initialize values here.
        currency = 100;
        purchasedUpgrades = new List<UpgradeData>();
        inventoryData = new List<InventoryData>();
        doorsOpened = new List<bool> { false, false };
        timesWon = 0;
        enemyData = new EnemyData();
    }
}
