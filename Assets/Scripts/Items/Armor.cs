using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class Armor : MonoBehaviour, Equipment {

    public List<ATTRIBUTE> attributes;

    //todo: improve this
    public int bonus_hp = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public override string ToString() {
        string description = "";
        foreach (var attribute in attributes) {
            description += attribute + " ";
        }

        return description + "Armor";
    }


}
