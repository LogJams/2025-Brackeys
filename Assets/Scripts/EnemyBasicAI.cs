using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBasicAI : MonoBehaviour {

    NavMeshAgent nav;

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

    }
}
