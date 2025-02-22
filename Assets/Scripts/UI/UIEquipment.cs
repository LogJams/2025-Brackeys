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
        weaponIcon.sprite = player.GetWeapon().icon;
        armorText.text = player.GetArmor().ToString();
        armorIcon.sprite = player.GetArmor().icon;
        //todo: icons
        player.OnWeaponChange += OnChangeGear;
        player.OnArmorChange += OnChangeGear;
    }


    void OnChangeGear(System.Object src, (Weapon, Armor) prevItems) {
        weaponText.text = player.GetWeapon().ToString();
        weaponIcon.sprite = player.GetWeapon().icon;
        armorText.text = player.GetArmor().ToString();
        armorIcon.sprite = player.GetArmor().icon;
        //todo: icons
    }

}
