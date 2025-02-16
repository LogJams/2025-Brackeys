using System.Collections;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {

    public Weapon weapon;

    bool busy = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        

        //todo: maybe we can play a "wind up" animation on button down, then swing on button up
        if (Input.GetButtonUp("Swing") && !busy) {
            Debug.Log("Swing!");
            StartCoroutine(SwingWeapon(0.5f));
        }

    }



    IEnumerator SwingWeapon(float swingTime) {
        busy = true;
        float t0 = Time.time;
        Quaternion initialRotation = weapon.transform.parent.localRotation;

        weapon.StartSwinging();

        while (Time.time - t0 <= swingTime) {

            weapon.transform.parent.localRotation = Quaternion.Lerp(initialRotation, Quaternion.AngleAxis(-90f, Vector3.up), (Time.time - t0) / swingTime);
            yield return null;
        }

        weapon.StopSwinging();

        t0 = Time.time;
        while (Time.time - t0 <= swingTime/2f) {

            weapon.transform.parent.localRotation = Quaternion.Lerp(Quaternion.AngleAxis(-90f, Vector3.up), initialRotation, 2 * (Time.time - t0) / swingTime);
            yield return null;
        }

        weapon.transform.parent.localRotation = initialRotation;
        busy = false;
        yield return null;
    }

}
