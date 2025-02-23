using UnityEngine;
using UnityEngine.UI;

public class StatusInfoRow : MonoBehaviour {

    Image icon;
    TMPro.TMP_Text text;


    private void Awake() {
        icon = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMPro.TMP_Text>();
    }


    public void Set(Sprite icon, string text) {
        this.icon.sprite = icon;
        this.text.text = text;
    }



}
