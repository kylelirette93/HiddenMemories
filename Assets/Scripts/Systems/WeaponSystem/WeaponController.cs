using UnityEngine;

public class WeaponController : MonoBehaviour
{
    ItemDataSO currentWeapon;


    private void Awake()
    {
        InteractableActions.AddWeapon += AddWeapon;
    }

    private void AddWeapon(ItemDataSO weapon)
    {        
        currentWeapon = weapon;
        Debug.Log("Added weapon " + weapon);
    }
}
