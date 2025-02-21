using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MagicEffect {
    public EFFECTS effect;
    public int duration;
}

public class ReadableObject : MonoBehaviour, IInteractable {

    public event System.EventHandler OnEndInteraction;

    public string textToDisplay = "[PLACEHOLDER TEXT]";

    public event System.EventHandler<string> OnReadObject;

    [Header("Optional")]
    public List<MagicEffect> effectOnPlayer;
    Vitality player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Vitality>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Interact() {
        OnReadObject?.Invoke(this, textToDisplay);
        foreach (var me in effectOnPlayer) {
            player.AddStatusEffect(me.effect, me.duration);
        }
        OnReadObject?.Invoke(this, textToDisplay);        
    }

    public bool EndInteraction() {
        OnEndInteraction?.Invoke(this, System.EventArgs.Empty);
        return false;
    }

    public string GetDescription() {
        return "Examine " + gameObject.name;
    }
}
