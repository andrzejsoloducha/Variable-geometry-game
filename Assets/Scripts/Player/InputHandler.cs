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

        if (bazookaEquipped)
        {
            //var target = GameManager.Instance.currentTarget;
            currentWeapon.GetComponent<Bazooka>().RotateBazookaToPoint(Input.mousePosition);
        }
        
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

        if (currentWeapon && currentWeapon.name == "driller")
        {
            var lastDirection = Vector3.zero;
            var direction = Input.GetAxisRaw("Horizontal");
            if (direction != 0)
            {
                lastDirection = new Vector3(direction, 0, 0).normalized;
            }

            currentWeapon.GetComponent<WeaponDriller>().FlipDriller(lastDirection.x);
            
            if (GameManager.Instance.currentTime <= 0.1f)
            {
                currentWeapon.GetComponent<WeaponDriller>().ResetDriller();
            }
        }
    }
}