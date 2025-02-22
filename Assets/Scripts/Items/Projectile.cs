using UnityEngine;

public class Projectile : MonoBehaviour {

    Transform target;
    Vitality parent;

    public float linSpeed = 4.5f;
    public float rotSpeed = 30;
    public bool homing = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
    }

    public void Configure(Transform target, Vitality parent) {
        this.target = target;
        this.parent = parent;
        transform.rotation.SetLookRotation(target.position - transform.position);
        parent.OnDeath += Delete;
    }

    // Update is called once per frame
    void Update() {
        if (homing && target != null) {
            Quaternion goTo = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, goTo, rotSpeed * Time.deltaTime);
        }

        transform.position = transform.position + transform.forward * linSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponentInParent<Vitality>() == parent) return;

        Vitality hitThing;

        if (other.TryGetComponent(out hitThing)) {
            hitThing.Attacked(parent.GetComponentInChildren<Shooter>());
            parent.OnDeath -= Delete; //clean up reference
            Destroy(this.gameObject);
        }

        if (other.GetComponent<Weapon>() != null) {
            Destroy(this.gameObject);
        }
    }

    void Delete(System.Object src, System.EventArgs e) {
        if (this != null) {
            Destroy(gameObject);
        }
    }
}
