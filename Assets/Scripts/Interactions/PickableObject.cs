using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable {

    public event System.EventHandler OnEndInteraction;
    public event System.EventHandler<string> OnPickObject;


    public List<Weapon> unlockedWeapon;
    public List<Armor> unlockedArmor;

    public void Interact() {
        OnPickObject?.Invoke(this, gameObject.name);
        foreach (Weapon w in unlockedWeapon) {
            UnlockTracker.instance.UnlockWeapon(w);
        }
        foreach (Armor a in unlockedArmor) {
            UnlockTracker.instance.UnlockArmor(a);
        }
        //delete the 3D model
        foreach (var mesh in GetComponentsInChildren<MeshRenderer>()) {
            Destroy(mesh.gameObject);
        }
    }

    public bool EndInteraction() {
        OnEndInteraction?.Invoke(this, System.EventArgs.Empty);
        DestroyImmediate(gameObject);
        return true;
    }

    public string GetDescription() {
        return "Take " + gameObject.name;
    }
}
