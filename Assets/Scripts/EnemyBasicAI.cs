using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBasicAI : MonoBehaviour {

    NavMeshAgent nav;

    public Weapon weapon;

    public float swingT = 0.15f;

    bool busy = false;

    private void Awake() {
        nav = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        nav.SetDestination(transform.position);
    }

    // Update is called once per frame
    void Update() {

        if (nav.remainingDistance <= 0.01) {
            //next waypoint
            nav.SetDestination(new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)));
        }

        //todo: maybe we can play a "wind up" animation on button down, then swing on button up
        if (!busy) {
            StartCoroutine(SwingWeapon(swingT));
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
        while (Time.time - t0 <= swingTime / 2f) {

            weapon.transform.parent.localRotation = Quaternion.Lerp(Quaternion.AngleAxis(-90f, Vector3.up), initialRotation, 2 * (Time.time - t0) / swingTime);
            yield return null;
        }

        weapon.transform.parent.localRotation = initialRotation;
        yield return new WaitForSeconds(0.5f);

        busy = false;
        yield return null;
    }
}
