using System.Collections.Generic;
using UnityEngine;

public class GearManager : MonoBehaviour {

    //events to tell other classes when we do things
    public event System.EventHandler<(Weapon, Armor)> OnWeaponChange;
    public event System.EventHandler<(Weapon, Armor)> OnArmorChange;

    public Transform weaponSlot;
    List<Weapon> weapons;
    int weaponIndex = 0;

    public Transform armorSlot;
    int armorIndex = 0;
    List<Armor> armors;

    

    private void Awake() {
        weapons = new List<Weapon>();
        weapons.AddRange(weaponSlot.GetComponentsInChildren<Weapon>());

        armors = new List<Armor>();
        armors.AddRange(armorSlot.GetComponentsInChildren<Armor>());

        for (int i = 0; i < armors.Count; i++) {
            armors[i].gameObject.SetActive(i==armorIndex);
        }
        for (int i = 0; i < weapons.Count; i++) {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }
    }



    public void CycleWeapon() {
        Weapon oldWeapon = weapons[weaponIndex];
        weaponIndex = (weaponIndex + 1) % weapons.Count;
        for (int i = 0; i < armors.Count; i++) {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }
        OnWeaponChange?.Invoke(this, (oldWeapon, armors[armorIndex]));
    }

    public void CycleArmor() {
        Armor oldArmor = armors[armorIndex];
        armorIndex = (armorIndex + 1) % armors.Count;
        for (int i = 0; i < armors.Count; i++) {
            armors[i].gameObject.SetActive(i == armorIndex);
        }
        OnArmorChange?.Invoke(this, (weapons[weaponIndex], oldArmor));
    }



    public Weapon GetWeapon() {
        return weapons[weaponIndex];
    }

    public Armor GetArmor() {
        return armors[armorIndex];
    }
}
