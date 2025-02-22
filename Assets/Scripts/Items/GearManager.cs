using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GearManager : MonoBehaviour {

    //events to tell other classes when we do things
    public event System.EventHandler<(Weapon, Armor)> OnWeaponChange;
    public event System.EventHandler<(Weapon, Armor)> OnArmorChange;

    [Header("Ref. to Empty Object Holding Weapons")]
    public Transform weaponSlot;
    List<Weapon> weapons;
    int weaponIndex = 0;

    [Header("Ref. to Empty Object Holding Armor")]
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

    void Start() {
        //unlock initially equipped weapon
        UnlockTracker.instance.UnlockWeapon(weapons[weaponIndex]);
        UnlockTracker.instance.UnlockArmor(armors[armorIndex]);
        UnlockTracker.instance.onUnlockWeapon += SetWeapon;
        UnlockTracker.instance.onUnlockArmor += SetArmor;
        UnlockTracker.instance.onLoseWeapon += DropWeapon;
        UnlockTracker.instance.onLoseArmor += DropArmor;
    }

    public void SetWeapon(System.Object src, Weapon weap) {
        Weapon oldWeapon = weapons[weaponIndex];
        weaponIndex = weapons.IndexOf(weap);

            for (int i = 0; i < weapons.Count; i++) {
                weapons[i].gameObject.SetActive(i == weaponIndex);
            }
            OnWeaponChange?.Invoke(this, (oldWeapon, armors[armorIndex]));
    }

    public void SetArmor(System.Object src, Armor arm) {
        Armor oldArmor = armors[armorIndex];
        armorIndex = armors.IndexOf(arm);

        for (int i = 0; i < armors.Count; i++) {
            armors[i].gameObject.SetActive(i == armorIndex);
        }
        OnArmorChange?.Invoke(this, (weapons[weaponIndex], oldArmor));
    }

    void DropWeapon(System.Object src, Weapon weap) {
        int idx = weapons.IndexOf(weap);
        if (idx == weaponIndex) {
            CycleWeapon();
        }
    }

    void DropArmor(System.Object src, Armor arm) {
        int idx = armors.IndexOf(arm);
        if (idx == armorIndex) {
            CycleArmor();
        }
    }


    public void CycleWeapon() {
        int prevWeapon = weaponIndex; //get ref to weaponIndex then increment to ntext
        Weapon oldWeapon = weapons[prevWeapon];

        weaponIndex = (weaponIndex + 1) % weapons.Count;
        while (!UnlockTracker.instance.IsWeaponUnlocked(weapons[weaponIndex])) {
            weaponIndex = (weaponIndex + 1) % weapons.Count;
        }
        //only do this if we actually switch
        if (prevWeapon != weaponIndex) {
            for (int i = 0; i < weapons.Count; i++) {
                weapons[i].gameObject.SetActive(i == weaponIndex);
            }
            OnWeaponChange?.Invoke(this, (oldWeapon, armors[armorIndex]));
        }
    }

    public void CycleArmor() {
        int prevArmor = armorIndex;
        Armor oldArmor = armors[armorIndex];
        armorIndex = (armorIndex + 1) % armors.Count;

        while (!UnlockTracker.instance.IsArmorUnlocked(armors[armorIndex])) {
            armorIndex = (armorIndex + 1) % armors.Count;
        }
        if (prevArmor != armorIndex) {
            for (int i = 0; i < armors.Count; i++) {
                armors[i].gameObject.SetActive(i == armorIndex);
            }
            OnArmorChange?.Invoke(this, (weapons[weaponIndex], oldArmor));
        }
    }



    public Weapon GetWeapon() {
        return weapons[weaponIndex];
    }

    public Armor GetArmor() {
        return armors[armorIndex];
    }
}
