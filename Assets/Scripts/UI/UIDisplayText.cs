using UnityEngine;

public class UIDisplayText : MonoBehaviour {

    public TMPro.TMP_Text displayText;


    private void Start() {

        foreach (var interactable in FindObjectsByType<ReadableObject>(FindObjectsSortMode.None)) {
            interactable.OnReadObject += OnDisplayText;
        }
        foreach (var interactable in FindObjectsByType<PickableObject>(FindObjectsSortMode.None)) {
            interactable.OnPickObject += OnDisplayText;
        }


        displayText.gameObject.SetActive(false);

    }


    public void OnDisplayText(System.Object source, string text) {
        displayText.text = text;
        displayText.gameObject.SetActive(true);
        ((IInteractable)source).OnEndInteraction += HideDisplay;
    }

    public void HideDisplay(System.Object source, System.EventArgs e) {
        ((IInteractable)source).OnEndInteraction -= HideDisplay;
        displayText.gameObject.SetActive(false);
    }

}
