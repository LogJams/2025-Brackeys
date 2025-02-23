using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

[Serializable]
public struct ArmorPaint {
    public Color cloth;
    public Color darkLeather;
    public Color lightLeather;
    public Color metal;
}

public class Armor : MonoBehaviour, Equipment {

    public Sprite icon;

    public List<ATTRIBUTE> attributes;

    public int bonus_hp = 0;

    public ArmorPaint colors;
    


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

        return description + gameObject.name;
    }


}
