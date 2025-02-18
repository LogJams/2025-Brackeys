using UnityEngine;

public interface IInteractable {

    public event System.EventHandler OnEndInteraction;

    GameObject gameObject { get; }

    public void Interact();

    public void EndInteraction();

    public string GetDescription();

}
