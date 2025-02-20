using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class UIDebugText : MonoBehaviour {

    public TMP_Text text;

    public GearManager player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        player.OnWeaponChange += OnEquipmentChange;
        player.OnArmorChange += OnEquipmentChange;
        player.GetComponent<Vitality>().OnAttributeChange += OnAttrbChange;

        Weapon pw = player.weaponSlot.GetComponentInChildren<Weapon>();
        Armor pa = player.armorSlot.GetComponentInChildren<Armor>();
        OnEquipmentChange(this, (pw, pa));
    }

    // Update is called once per frame
    void Update() {
        
    }


    void OnEquipmentChange(System.Object source, (Weapon, Armor) eq) {
        string debugText = "Equipment\n";

        debugText += "1. " + player.GetWeapon().ToString() + "\n";
        debugText += "2. " + player.GetArmor().ToString() + "\n";

        var atb = player.GetComponent<Vitality>().GetAttributes();
        if (atb.Count > 0) {
            debugText += "\n";
            foreach (var a in atb) {
                debugText += "{" + a + "}\n";
            }
        }


        text.text = debugText;
    }

    void OnAttrbChange(System.Object src, System.EventArgs e) {
        string debugText = "Equipment\n";

        debugText += "1. " + player.GetWeapon().ToString() + "\n";
        debugText += "2. " + player.GetArmor().ToString() + "\n";

        var atb = player.GetComponent<Vitality>().GetAttributes();
        if (atb.Count > 0) {
            debugText += "\n";
            foreach (var a in atb) {
                debugText += "{" + a + "}\n";
            }
        }

        text.text = debugText;
    }

}
