using UnityEngine;

public class ChangeArmorColor : MonoBehaviour {

    GearManager gear;

    public Material cloth;
    public Material darkLeather;
    public Material lightLeather;
    public Material metal;



    private void Awake() {
        gear = GetComponent<GearManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        gear.OnArmorChange += UpdatedArmor;
        UpdateColors(gear.GetArmor().colors);
    }

    void UpdatedArmor(System.Object src, (Weapon, Armor) info) {
        UpdateColors(gear.GetArmor().colors);
    }

    void UpdateColors(ArmorPaint colors) {
        cloth.color = colors.cloth;
        darkLeather.color = colors.darkLeather;
        lightLeather.color = colors.lightLeather;
        metal.color = colors.metal;
    }

}
