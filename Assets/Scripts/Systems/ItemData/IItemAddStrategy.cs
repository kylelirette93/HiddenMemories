using System;

public interface IItemAddStrategy
{
    void Execute(ItemDataSO item, PlayerInventory inventory);
}

public interface IItemRemoveStrategy
{
    void Execute(ItemDataSO item, PlayerInventory inventory);
}

public class KeyAddStrategy : IItemAddStrategy
{
    public void Execute(ItemDataSO item, PlayerInventory inventory)
    {
        if (item is KeyDataSO key)
        {
            inventory.Keys.Add(key);
        }
        inventory.OnKeyCountChanged?.Invoke(inventory.Keys.Count);
    }
}

public class PotionAddStrategy : IItemAddStrategy
{
    public void Execute(ItemDataSO item, PlayerInventory inventory)
    {
        if (item is HealingPotionSO potion)
        {
            inventory.HealingPotions.Add(potion);
        }
        inventory.OnPotionCountChanged?.Invoke(inventory.HealingPotions.Count);
    }
}

public class WeaponAddStrategy : IItemAddStrategy
{
    public void Execute(ItemDataSO item, PlayerInventory inventory)
    {
        if (item is WeaponDataSO weapon && !inventory.availableWeapons.Contains(weapon))
        {
            inventory.availableWeapons.Add(weapon);
            WeaponManager.Instance.EquipWeapon(weapon);
            weapon.IsUnlocked = true;
            WeaponActions.UnlockWeapon?.Invoke(weapon);
        }
    }
}

public class KeyRemoveStrategy : IItemRemoveStrategy
{
    public void Execute(ItemDataSO item, PlayerInventory inventory)
    {
        if (item is KeyDataSO key && inventory.Keys.Contains(key))
        {
            inventory.Keys.Remove(key);
        }
        inventory.OnKeyCountChanged?.Invoke(inventory.Keys.Count);
    }
}
public class PotionRemoveStrategy : IItemRemoveStrategy
{
    public void Execute(ItemDataSO item, PlayerInventory inventory)
    {
        if (item is HealingPotionSO potion && inventory.HealingPotions.Contains(potion))
        {
            inventory.HealingPotions.Remove(potion);
        }
        inventory.OnPotionCountChanged?.Invoke(inventory.HealingPotions.Count);
    }
}
