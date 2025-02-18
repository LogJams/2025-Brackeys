using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class ReadableObject : MonoBehaviour, IInteractable {

    public event System.EventHandler OnEndInteraction;

    public string textToDisplay = "[PLACEHOLDER TEXT]";

    public event System.EventHandler<string> OnReadObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Interact() {
        OnReadObject?.Invoke(this, textToDisplay);
    }

    public void EndInteraction() {
        OnEndInteraction?.Invoke(this, System.EventArgs.Empty);
    }

    public string GetDescription() {
        return "Read " + gameObject.name;
    }
}
