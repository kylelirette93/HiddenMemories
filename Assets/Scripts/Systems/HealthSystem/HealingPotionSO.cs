using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/HealingPotion", order = 1)]
public class HealingPotionSO : ItemDataSO
{
    public int HealAmount = 20;
}
