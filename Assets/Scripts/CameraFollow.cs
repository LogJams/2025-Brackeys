using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform player;

    public float zoomDist = 90f;

    Vector3 viewNormal = new Vector3(-0.49f, 0.71f, -0.49f);

    

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }


    // Update is called once per frame
    void LateUpdate() {
        if (player != null) {
            transform.position = player.position + viewNormal * zoomDist;
        }
    }
}
