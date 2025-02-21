using UnityEngine;

public class UIAnnouncement : MonoBehaviour {

    public TMPro.TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        text.gameObject.SetActive(false);
        ProgressionManager.instance.OnAnnouncement += OnAnnouncement;
        ProgressionManager.instance.EndAnnouncement += EndAnnouncement;

    }


    void OnAnnouncement(System.Object src, Announcement anc) {
        text.gameObject.SetActive(true);
        text.text = anc.message;
    }

    void EndAnnouncement(System.Object src, System.EventArgs e) {
        text.gameObject.SetActive(false);
    }

}
