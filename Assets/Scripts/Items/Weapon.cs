using NUnit.Framework;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Weapon : MonoBehaviour, Equipment {


    public event System.EventHandler<Vitality> OnWeaponHit;

    [Header("Custom Weapon Properties")]
    public List<Enchantment> enchantments;
    public float baseDamage = 1;
    public bool areaOfEffect = true;

    public Sprite icon;

    [Header("Required References")]
    public Collider hitArea;


    protected Vitality owner;

    public override string ToString() {
        if (enchantments.Count > 0) {
            return enchantments[0].name + "blade";
        }
        return "Basic Weapon";
    }

    public Vitality GetOwner() {
        return owner;
    }

    private void Start() {
        //make sure this is a child of the owner object!
        owner = GetComponentInParent<Vitality>();
        hitArea.enabled = false;
    }


    void OnTriggerEnter(Collider other) {
        Vitality toHit = null;
        //make sure 1. it's got a vitality, 2. it's not ourselves, and 3. it's not an AI trigger
        if (other.TryGetComponent(out toHit) && toHit != owner && other.gameObject.layer != LayerMask.NameToLayer("AI Trigger")) {
            /////// disable hitArea and handle enchantment/combat in the Vitality
            if (!areaOfEffect) {
                hitArea.enabled = false;
            }
            toHit.Attacked(this);
            OnWeaponHit?.Invoke(this, toHit);
        }
    }


    public virtual void StartSwinging() {
        hitArea.enabled = true;
    }


    public virtual void StopSwinging() {
        hitArea.enabled = false;
    }


}
