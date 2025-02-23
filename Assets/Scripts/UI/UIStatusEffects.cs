using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[Serializable]
public struct EffectIcon {
    public EFFECTS effect;
    public Sprite icon;
}

[Serializable]
public struct AttributeIcon {
    public ATTRIBUTE attribute;
    public Sprite icon;
}

public class UIStatusEffects : MonoBehaviour {

    public Vitality player;

    public GameObject statusUIPrefab;

    [Header("Select icons here:")]
    public List<EffectIcon> effects;
    public List<AttributeIcon> attributes;

    Dictionary<EFFECTS, Sprite> effectIcons;
    Dictionary<ATTRIBUTE, Sprite> attributeIcons;


    private void Awake() {
        effectIcons = new Dictionary<EFFECTS, Sprite>();
        attributeIcons = new Dictionary<ATTRIBUTE, Sprite>();

        foreach (EffectIcon ei in effects) {
            effectIcons.Add(ei.effect, ei.icon);
        }
        foreach (AttributeIcon ai in attributes) {
            attributeIcons.Add(ai.attribute, ai.icon);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

        player.OnGainEffect += OnStatusChange;
        player.OnLoseEffect += OnStatusChange;

        player.OnAttributeChange += OnAttributeChange;

        UpdateUI();
    }


    public void OnStatusChange(System.Object source, EFFECTS E) {
        UpdateUI();
    }
    public void OnAttributeChange(System.Object soruce, EventArgs E) {
        UpdateUI();
    }


    public void UpdateUI() {

        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }

        List<ATTRIBUTE> att = player.GetAttributes();
        foreach (ATTRIBUTE a in att) {
            if (attributeIcons.ContainsKey(a)) {
                GameObject go = Instantiate(statusUIPrefab, this.transform);
                go.GetComponent<StatusInfoRow>().Set(attributeIcons[a], a.ToString());
            }
        }

        List<EFFECTS> eff = player.GetEffects();
        foreach (EFFECTS e in eff) {
            if (effectIcons.ContainsKey(e)) {
                GameObject go = Instantiate(statusUIPrefab, this.transform);
                go.GetComponent<StatusInfoRow>().Set(effectIcons[e], e.ToString());
            }
        }

    }
}
