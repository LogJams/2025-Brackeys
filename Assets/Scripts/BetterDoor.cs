using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BetterDoor : MonoBehaviour {


    public List<Vitality> killList;
    int counter = 0;

    public Transform leftHinge;
    public Transform rightHinge;
    public float openTime = 0.5f;
    
    private void Start() {
        foreach (var vitality in killList) {
            vitality.OnDeath += OnDeath;
        }
        counter = killList.Count;
        if (counter == 0) {
            Open();
        }
    }


    public void OnDeath(System.Object src, System.EventArgs e) {
        counter--;

        if (counter == 0) {
            Open();
        }
    }

    void Open() {
        StartCoroutine(SwingOpen(leftHinge, -1));
        StartCoroutine(SwingOpen(rightHinge, 1));
    }

    IEnumerator SwingOpen(Transform target, int dir) {
        Quaternion q0 = target.localRotation;
        Quaternion qf = q0 * Quaternion.AngleAxis(dir * 90f, target.up);
        float t0 = Time.time;
        while (Time.time - t0 < openTime) {
            target.localRotation = Quaternion.Lerp(q0, qf, (Time.time - t0) / openTime);
            yield return null;
        }

        target.localRotation = qf;

        yield return null;
    }


}
