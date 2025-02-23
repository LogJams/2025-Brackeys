using UnityEngine;

public class UIAnnouncement : MonoBehaviour {

    public TMPro.TMP_Text text;
    public GameObject root;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        root.SetActive(false);
        ProgressionManager.instance.OnAnnouncement += OnAnnouncement;
        ProgressionManager.instance.EndAnnouncement += EndAnnouncement;

    }


    void OnAnnouncement(System.Object src, Announcement anc) {
        root.SetActive(true);
        text.text = anc.message;
    }

    void EndAnnouncement(System.Object src, System.EventArgs e) {
        root.SetActive(false);
    }

}
