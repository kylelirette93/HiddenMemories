using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/Items", order = 1)]
public class ItemDataSO : ScriptableObject
{
    public ItemType itemType;
}

public enum ItemType
{
    HealthPotion,
    Ammo,
    Weapon,
    Cash
}
