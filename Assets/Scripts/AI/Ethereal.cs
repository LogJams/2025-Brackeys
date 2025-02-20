using System.Collections;
using UnityEngine;

public class Ethereal : MonoBehaviour {

    Vitality owner;

    public float delay = 0.5f;

    Coroutine fade;

    void Awake() {
        owner = GetComponentInParent<Vitality>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            StartCoroutine(FadeAway());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            owner.RemoveStatusEffect(EFFECTS.ethereal);
        }
    }

    IEnumerator FadeAway() {
        yield return new WaitForSeconds(delay);
        owner.AddStatusEffect(EFFECTS.ethereal, -1);
        yield return null;
    }
    

}
