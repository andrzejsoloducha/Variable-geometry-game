using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    private List<GameObject> weapons = new();

    private void Start()
    {
        var weaponHolder = transform.Find("weaponHolder");

        foreach (Transform weapon in weaponHolder)
        {
            weapons.Add(weapon.gameObject);
            weapon.gameObject.SetActive(false);
        }
    }

    public void SwitchWeaponTo(int idx)
    {
        for (var i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetActive(i == idx);
        }
    }

    [CanBeNull]
    public GameObject GetCurrentWeapon()
    {
        return weapons.Find(w => w.activeSelf);
    }
}