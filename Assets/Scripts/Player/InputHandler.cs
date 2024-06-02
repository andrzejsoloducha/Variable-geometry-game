using System;
using JetBrains.Annotations;
using Unity.Barracuda;
using UnityEngine; 
public class InputHandler : MonoBehaviour
{
    private WeaponSwitcher weaponSwitcher;

    private GameObject GetCurrentPlayerObject()
    {
        return GameManager.Instance.CurrentPlayer;
    }
   
    [CanBeNull]
    private Player CurrentPlayer()
    {
        return GetCurrentPlayerObject()?.GetComponent<Player>();
    }

    private WeaponSwitcher GetWeaponSwitcher()
    {
        return GetCurrentPlayerObject().GetComponent<WeaponSwitcher>();
    }

    private bool WeaponUsed()
    {
        return GameManager.Instance.weaponUsed;
    }

    private void Update()
    {
        var cp = CurrentPlayer();
        if (!cp) return;
        
        cp.TryMove();

        if (!WeaponUsed())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetWeaponSwitcher().SwitchWeaponTo(0);
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GetWeaponSwitcher().SwitchWeaponTo(1);
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GetWeaponSwitcher().SwitchWeaponTo(2);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            cp.TryJump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            cp.ExtendedJump();
        }

        var currentWeapon = GetWeaponSwitcher().GetCurrentWeapon();
        var bazookaEquipped = currentWeapon?.name == "bazooka";
        var didNotUseWeaponThisRound = !WeaponUsed();
        
        if (Input.GetButtonDown("Fire1") && didNotUseWeaponThisRound && bazookaEquipped)
        {
            cp.TryShoot();
        }

        var constructorEquipped = currentWeapon?.name == "constructor";
        if (constructorEquipped && didNotUseWeaponThisRound)
        {
            currentWeapon?
                .GetComponent<WeaponConstructor>()
                .OnMouseInput();
        }
    }
}