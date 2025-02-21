using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable {

    public event System.EventHandler OnEndInteraction;
    public event System.EventHandler<string> OnPickObject;


    public List<Weapon> unlockedWeapon;

    public void Interact() {
        OnPickObject?.Invoke(this, gameObject.name);
        foreach (Weapon w in unlockedWeapon) {
            UnlockTracker.instance.UnlockWeapon(w);
        }
        //delete the 3D model
        foreach (Transform tf in transform) {
            Destroy(tf.gameObject);
        }
    }

    public bool EndInteraction() {
        OnEndInteraction?.Invoke(this, System.EventArgs.Empty);
        Destroy(gameObject);
        return true;
    }

    public string GetDescription() {
        return "Take " + gameObject.name;
    }
}
