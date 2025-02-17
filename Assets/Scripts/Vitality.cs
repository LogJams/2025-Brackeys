using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Vitality : MonoBehaviour {

    public event System.EventHandler OnDeath;
    public event System.EventHandler<EFFECTS> OnGainEffect;
    public event System.EventHandler<EFFECTS> OnLoseEffect;

    public float max_hp = 5;
    private float _hp;

    public float hp {
        get { return _hp; } 
        private set { _hp = value;
            if (hp <= 0) { 
                OnDeath?.Invoke(this, System.EventArgs.Empty);
                SceneManager.instance.OnTick -= OnTick;
                Destroy(gameObject);
            }
        }
    }

    public List<ATTRIBUTE> attributes;
    List<EFFECTS> statusEffects;


    public void AddStatusEffect(EFFECTS effect) {
        if (!statusEffects.Contains(effect)) {
            Debug.Log(gameObject.name + " now suffers from " + effect + "!");
            statusEffects.Add(effect);
            OnGainEffect?.Invoke(this, effect);
        }
    }


    private void Awake() {
        hp = max_hp;
        statusEffects = new List<EFFECTS>();
    }

    private void Start() {
        SceneManager.instance.OnTick += OnTick;
    }

    void OnTick(System.Object src, uint count) {
        foreach (EFFECTS effect in statusEffects) {
            int hpChange = SceneManager.instance.EffectHealthChange(effect);
            hp += hpChange;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Weapon weap = null;

        if (other.TryGetComponent(out weap)) {
            int dmg = weap.OnHit(this);

            if (statusEffects.Contains(EFFECTS.harmless)) {
                dmg = 0;
                statusEffects.Remove(EFFECTS.harmless);
            }

            hp -= dmg;
        }
    }

}
