using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/Items", order = 1)]
public abstract class ItemDataSO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;

    public virtual void Use()
    {
        Debug.Log("Using item: " + itemName);
    }
}

public enum ItemType
{
    HealthPotion,
    Ammo,
    Weapon,
    Cash,
    Key
}
