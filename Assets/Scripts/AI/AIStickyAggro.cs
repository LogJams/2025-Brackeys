using UnityEngine;

//sticks to the player forever
public class AIStickyAggro : MonoBehaviour {

    Transform player;

    private void LateUpdate() {
        if (player != null) {
            transform.position = player.position;
        }
    }

    private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
            player = other.transform;
        }
    }

}
