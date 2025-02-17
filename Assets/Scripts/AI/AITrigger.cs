using UnityEngine;

// Unity does weird stuff with OnTriggerEnter, so I put the "aggro" and attack triggers on different objects on different higherarchy branches
// they send events to the enemy AI in order to do useful stuff
public class AITrigger : MonoBehaviour {

    public event System.EventHandler<Vector3> OnTrigger;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            OnTrigger?.Invoke(this, other.transform.position);
        }
    }
}
