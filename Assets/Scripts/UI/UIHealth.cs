using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour {

    public Vitality playerVtl;

    public GameObject healthIcon;


    void Awake() {
        playerVtl.OnDamage += OnDamage;
        playerVtl.OnHeal += OnDamage;
    }

    public void OnDamage(System.Object src, float dmg) {
        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < playerVtl.hp; i++) {
            Instantiate(healthIcon, transform);
        }
    }
}
