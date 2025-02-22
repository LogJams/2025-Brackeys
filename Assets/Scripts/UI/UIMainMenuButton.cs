using UnityEngine;

public class UIMainMenuButton : MonoBehaviour {

    public GameObject creditsPanel;
    public GameObject optionsPanel;

    public int idNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OnClick() {
        GetComponent<AudioSource>().Play();
    }


    public void Activate() {
        //start button
        if (idNum == 0) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        //options button
        if (idNum == 1) {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
        //credits button
        if (idNum == 2) {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
        }
        //quit button
        if (idNum == 3) {
            Application.Quit();
        }
    }


}
