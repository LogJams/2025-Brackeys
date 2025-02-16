using UnityEngine;

public class Weapon : MonoBehaviour {

    public Collider hitArea;

    public void StartSwinging() {
        hitArea.enabled = true;
    }


    public void StopSwinging() {
        hitArea.enabled = false;
    }

    public int OnHit(Vitality target) {
        int damage = 1;

        //todo: check how our enchantment affects OnHit based on target properties

        hitArea.enabled = false;

        return damage;
    }


}
