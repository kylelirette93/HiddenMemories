using System.Collections.Generic;

/// <summary>
/// Ammo Manager saves and loads ammo based on weapon cycling.
/// </summary>
public class AmmoManager
{
    Dictionary<int, int> ammoCounts = new Dictionary<int, int>();
    public void Save(int index, int ammoCount)
    {
        ammoCounts[index] = ammoCount;
    }

    public int Load(int index, int ammoCount)
    {
        return ammoCounts.TryGetValue(index, out int savedAmmo) ? savedAmmo : ammoCount;
    }
}
