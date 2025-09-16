using UnityEngine;

public class ItemDataSO : ScriptableObject
{
    ItemType itemType;

    void UseItem()
    {
        switch (itemType)
        {
            case ItemType.HealthPotion:
                // Use health potion.
                break;
            case ItemType.Ammo:
                // Add ammo.
                break;
            case ItemType.Weapon:
                // Pickup weapon.
                break;
        }
    }
}

public enum ItemType
{
    HealthPotion,
    Ammo,
    Weapon
}
