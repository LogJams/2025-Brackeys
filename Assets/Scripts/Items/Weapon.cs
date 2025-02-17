using NUnit.Framework;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Weapon : MonoBehaviour, Equipment {


    [Header("Custom Weapon Properties")]
    public List<Enchantment> enchantments;
    public float baseDamage = 1;

    [Header("Required References")]
    public Collider hitArea;


    Vitality owner;

    public override string ToString() {
        return enchantments[0].name + "blade";
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
            hitArea.enabled = false;
            toHit.Attacked(this);

        }
    }


    public void StartSwinging() {
        hitArea.enabled = true;
    }


    public void StopSwinging() {
        hitArea.enabled = false;
    }


}
