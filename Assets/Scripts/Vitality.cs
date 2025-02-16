using UnityEngine;

public class Vitality : MonoBehaviour {

    public event System.EventHandler OnDeath;

    public float max_hp = 5;
    private float hp;


    public void StatusEffect() {
        //todo: implement
    }


    private void Awake() {
        hp = max_hp;
    }

    private void OnTriggerEnter(Collider other) {
        Weapon weap = null;

        if (other.TryGetComponent(out weap)) {
            int dmg = weap.OnHit(this);

            hp -= dmg;

            if (hp <= 0) {
                OnDeath?.Invoke(this.gameObject, System.EventArgs.Empty);
                Destroy(gameObject);
            }
        }
    }

}
