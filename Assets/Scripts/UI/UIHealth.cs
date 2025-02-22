using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour {

    public Vitality playerVtl;

    public GameObject healthIcon;

    public float animTime = 0.5f;
    public float dmgSpeedFactor = 4;

    void Awake() {
        playerVtl.OnDamage += OnDamage;
        playerVtl.OnHeal += OnDamage;
    }

    public void OnDamage(System.Object src, float dmg) {

        int prevHp = transform.childCount;
        int dH = (int) (playerVtl.hp - prevHp);

        //net healing
        for (int i = 0; i < dH; i++) {
            StartCoroutine(AddHeart());
        }

        //we not ded yet
        if (playerVtl.hp > 0) {
            //net damage
            for (int i = prevHp; i > prevHp + dH; i--) {
                StartCoroutine(DeleteHeart(transform.GetChild(i - 1).gameObject));
            }
        }
        //we ded
        else {
            foreach (Transform tf in transform) {
                StartCoroutine(DeleteHeart(tf.gameObject));
            }
        }

    }


    IEnumerator AddHeart() {
        GameObject newHeart = Instantiate(healthIcon, transform);

        float t0 = Time.time;
        while (Time.time - t0 <= animTime) {
            newHeart.transform.localScale = Vector3.one * Mathf.Lerp(0.001f, 1, (Time.time - t0) / animTime);
            yield return null;
        }
        newHeart.transform.localScale = Vector3.one;
        yield return null;
    }


    IEnumerator DeleteHeart(GameObject heart) {
        float t0 = Time.time;
        while (Time.time - t0 <= animTime/ dmgSpeedFactor) {
            heart.transform.localScale = Vector3.one * Mathf.Lerp(1, 0.001f, (Time.time - t0) / (animTime/ dmgSpeedFactor));
            yield return null;
        }

        Destroy(heart);
        yield return null;
    }



}
