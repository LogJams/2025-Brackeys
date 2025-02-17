using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBasicAI : MonoBehaviour {

    [Header("Put Objects with Trigger Colliders Here")]
    public AITrigger aggroHandler;
    public AITrigger attackHandler;

    [Header("Aggro Characteristics")]
    [Range(2, 50)]
    public float aggroRadius = 4f;
    public bool leash = true;
    Vector3 p0;

    [Header("Weapon References")]
    Weapon weapon;
    public float swingT = 0.15f;


    //set this to true when leashing
    bool ignoreAggro = false;

    bool busy = false;
    NavMeshAgent nav;


    private void Awake() {
        nav = GetComponent<NavMeshAgent>();
        weapon = GetComponentInChildren<Weapon>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        aggroHandler.OnTrigger += OnAggro;
        attackHandler.OnTrigger += OnAttack;
        GetComponent<Vitality>().OnDeath += OnDeath;
        p0 = transform.position;
        nav.SetDestination(p0);
    }

    // Update is called once per frame
    void Update() {
        //make no decisions if we're busy attacking right now
        if (busy) {
            return;
        }

        //random walk within our aggro radius of p0
        if (nav.remainingDistance <= 0.01) {
            //next waypoint is somewhere random in the aggro radius
            Quaternion rot = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0));
            float r = Random.Range(0, aggroRadius) / 2;

            nav.SetDestination(p0 + rot*(new Vector3(0, 0, r)));
            ignoreAggro = false; //if we reach p0 we can ignore aggro, otherwise this should always be true
        }        
        //leash back to p0 if we exit the aggro radius
        if (leash && Vector3.SqrMagnitude(transform.position - p0) >= aggroRadius*aggroRadius) {
            nav.SetDestination(p0);
            ignoreAggro = true;
        }

    }


    void OnAggro(System.Object src, Vector3 location) {
        if (ignoreAggro) return;

        //stay a little bit away from the target and hit it
        Vector3 dp = 5 * nav.radius * (location - transform.position).normalized;
        //move to attacj!
        nav.SetDestination(location - dp);
    }

    void OnAttack(System.Object src, Vector3 location) {
        if (!busy) {
            StartCoroutine(SwingWeapon(swingT));
        }
    }

    void OnDeath(System.Object src, System.EventArgs e) {
        aggroHandler.OnTrigger -= OnAggro;
        attackHandler.OnTrigger -= OnAttack;
        GetComponent<Vitality>().OnDeath -= OnDeath;
    }



    //called when we update values in the inspector
    private void OnValidate() {
        aggroHandler.GetComponent<SphereCollider>().radius = aggroRadius;
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
