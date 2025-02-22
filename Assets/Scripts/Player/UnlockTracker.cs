using System.Collections.Generic;
using UnityEngine;

public class UnlockTracker : MonoBehaviour {

    public event System.EventHandler<Weapon> onUnlockWeapon;
    public event System.EventHandler<Armor> onUnlockArmor;

    public event System.EventHandler<Weapon> onLoseWeapon;
    public event System.EventHandler<Armor> onLoseArmor;


    public static UnlockTracker instance;
    Dictionary<Weapon, bool> unlockedWeapons;
    Dictionary<Armor, bool> unlockedArmor;


    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        unlockedWeapons = new Dictionary<Weapon, bool>();
        unlockedArmor = new Dictionary<Armor, bool>();
    }

    public bool IsWeaponUnlocked(Weapon weap) {
        return (unlockedWeapons.ContainsKey(weap) && unlockedWeapons[weap]);
    }

    public void UnlockWeapon(Weapon weap) {
        unlockedWeapons.Add(weap, true);
        onUnlockWeapon?.Invoke(this, weap);
    }

    public void LockWeapon(Weapon weap) {
        if (unlockedWeapons.ContainsKey(weap)) {
            unlockedWeapons[weap] = false;
            onLoseWeapon?.Invoke(this, weap);
        }
    }

    public bool IsArmorUnlocked(Armor arm) {
        return (unlockedArmor.ContainsKey(arm) && unlockedArmor[arm]);
    }

    public void UnlockArmor(Armor arm) {
        unlockedArmor.Add(arm, true);
        onUnlockArmor?.Invoke(this, arm);
    }

    public void LockArmor(Armor arm) {
        if (unlockedArmor.ContainsKey(arm)) {
            unlockedArmor[arm] = false;
            onLoseArmor?.Invoke(this, arm);
        }
    }

}
