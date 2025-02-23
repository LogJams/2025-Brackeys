using UnityEngine;
using UnityEngine.UI;

public class StatusInfoRow : MonoBehaviour {

    public Image icon;
    TMPro.TMP_Text text;


    private void Awake() {
        text = GetComponentInChildren<TMPro.TMP_Text>();
    }


    public void Set(Sprite icon, string text) {
        this.icon.sprite = icon;
        this.text.text = text;
    }



}
