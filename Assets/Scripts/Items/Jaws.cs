using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Jaws : Weapon, Equipment {


    public int damage = 1;

    public Animator anim;

    List<Vitality> containedItems;

    Vitality player;

    private void Awake() {
        //make sure this is a child of the owner object!
        owner = GetComponentInParent<Vitality>();
        containedItems = new List<Vitality>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Vitality>();
    }

    void OnTriggerEnter(Collider other) {
        Vitality vit;
        if (other.TryGetComponent(out vit) && vit != owner) {
            containedItems.Add(vit);
            vit.OnDeath += OnTargetDeath;
        }
    }

    void OnTriggerExit(Collider other) {
        Vitality vit;
        if (other.TryGetComponent(out vit) && containedItems.Contains(vit)) {
            RemoveItem(vit);
        }
    }

    void OnTargetDeath(System.Object src, System.EventArgs e) {
        RemoveItem((Vitality) src);
    }

    void RemoveItem(Vitality vit) {
        vit.OnDeath -= OnTargetDeath;
        if (containedItems.Contains(vit)) {
            containedItems.Remove(vit);
        }
    }

    public override void StartSwinging() {
        //attack animation is playing here
        anim.SetTrigger("Attack");
    }


    public override void StopSwinging() {
        //do damage
        if (containedItems.Contains(player)) {
            Debug.Log("Player attacked!");
            player.Attacked(this);
        }
        else {
            Debug.Log("No player");
        }
        
    }


}
