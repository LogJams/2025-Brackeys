using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour {

    public Vitality playerVtl;

    Slider hpSlider;

    private void Awake() {
        hpSlider = GetComponent<Slider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        hpSlider.maxValue = playerVtl.max_hp;
        hpSlider.value = playerVtl.hp;

        playerVtl.OnDamage += OnDamage;
        playerVtl.OnHeal += OnDamage;

    }


    public void OnDamage(System.Object src, float dmg) {
        //we can put up some 3D text "3 damage! 5 damage! critical hit!"
        hpSlider.value = playerVtl.hp;
    }
}
