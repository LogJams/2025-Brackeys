using UnityEngine;

public interface IInteractable {

    public event System.EventHandler OnEndInteraction;

    GameObject gameObject { get; }

    public void Interact();

    public bool EndInteraction(); //true if destroyed

    public string GetDescription();

}
