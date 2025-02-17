using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStatusEffects : MonoBehaviour {

    TMP_Text text;
    public Vitality player;

    private void Awake() {
        text = GetComponent<TMP_Text>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        text.text = "";

        player.OnGainEffect += OnStatusChange;
        player.OnLoseEffect += OnStatusChange;
    }

    
    public void OnStatusChange(System.Object source, EFFECTS E) {
        List<EFFECTS> eff = player.GetEffects();

        string str = "";

        foreach (var e in eff) {
            str += "[" + e + "]\n";
        }

        text.text = str;
    }

}
