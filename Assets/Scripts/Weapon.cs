using NUnit.Framework;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public Vitality owner;
    public Collider hitArea;

    public List<Enchantment> enchantments;

    private void Start() {
        hitArea.enabled = false;
    }

    public void StartSwinging() {
        hitArea.enabled = true;
    }


    public void StopSwinging() {
        hitArea.enabled = false;
    }

    public int OnHit(Vitality target) {
        int damage = 1;
        ///// block of code to handle enchantments

        //first check for curses
        bool curseEffect = false;
        foreach (Enchantment ench in enchantments) {
            foreach (Curse curse in ench.curses) {
                //check if the curse applies
                if (target.attributes.Contains(curse.trigger)) {
                    curseEffect = true;
                    //apply (beneficial?) curses to targets
                    if (curse.target == TARGETS.target) {
                        target.AddStatusEffect(curse.effect);
                    }
                    //apply cuse to ourselves
                    else {
                        owner.AddStatusEffect(curse.effect);
                    }


                }
            }
        }


        //todo: check how our enchantment affects OnHit based on target properties
        if (!curseEffect) {
            foreach (Enchantment ench in enchantments) {
                //do stuff to the target if it applies
                if (ench.target == TARGETS.target && HasValidAttribute(target, ench.attribute)) {
                    target.AddStatusEffect(ench.effect);
                }
                else if (ench.target == TARGETS.self && HasValidAttribute(owner, ench.attribute)) {
                    owner.AddStatusEffect(ench.effect);
                }
            }
        }

        /////// end of enchantment block, disable hitArea and do damage
        hitArea.enabled = false;
        return damage;
    }


    private bool HasValidAttribute(Vitality vit, ATTRIBUTE atb) {
        if (atb == ATTRIBUTE.ANY || vit.attributes.Contains(atb)) {
            return true;
        }

        return false;
    }

}
