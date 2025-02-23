using System.Collections;
using UnityEngine;

public class Ethereal : MonoBehaviour {

    Vitality owner;

    public float delay = 0.5f;

    public float min_alpha = 0.05f;

    Material mat;

    Coroutine fade;

    void Awake() {
        owner = GetComponentInParent<Vitality>();
        mat = owner.GetComponent<MeshRenderer>().material;

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
        Color c = mat.color;
        c.a = 1;
        mat.color = c;
    }

    IEnumerator FadeAway() {
        float t0 = Time.time;

        Color c = mat.color;

        while (Time.time - t0 <= delay) {
            c.a = Mathf.Lerp(1, min_alpha, (Time.time - t0) / delay);
            mat.color = c;
            yield return null;
        }

        c.a = min_alpha;
        
        owner.AddStatusEffect(EFFECTS.ethereal, -1);
        yield return null;
    }
    

}
