using System.Collections.Generic;
using UnityEngine;

public class UnlockTracker : MonoBehaviour {

    public static UnlockTracker instance;
    Dictionary<Weapon, bool> unlockedWeapons;


    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        unlockedWeapons = new Dictionary<Weapon, bool>();
    }

    public bool IsWeaponUnlocked(Weapon weap) {
        return (unlockedWeapons.ContainsKey(weap) && unlockedWeapons[weap]);
    }

    public void UnlockWeapon(Weapon weap) {
        unlockedWeapons.Add(weap, true);
    }

}
