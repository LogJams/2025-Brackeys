using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipment : MonoBehaviour {

    GearManager player;

    public Image weaponIcon;
    public TMPro.TMP_Text weaponText;

    public Image armorIcon;
    public TMPro.TMP_Text armorText;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<GearManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        weaponText.text = player.GetWeapon().ToString();
        armorText.text = player.GetArmor().ToString();
        //todo: icons
        player.OnWeaponChange += OnChangeGear;
        player.OnArmorChange += OnChangeGear;
    }


    void OnChangeGear(System.Object src, (Weapon, Armor) prevItems) {
        weaponText.text = player.GetWeapon().ToString();
        armorText.text = player.GetArmor().ToString();
        //todo: icons
    }

}
