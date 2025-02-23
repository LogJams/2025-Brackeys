using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBasicAI : MonoBehaviour {

    [Header("Put Objects with Trigger Colliders Here")]
    public AITrigger aggroHandler;
    public AITrigger attackHandler;
    public bool animateSpeed = false;
    [Header("Aggro Characteristics")]
    [Range(2, 50)]
    public float aggroRadius = 4f;
    public bool leash = true;
    Vector3 p0;

    [Header("Weapon References")]
    Weapon weapon;

    [Header("Attack Properties")]
    public float windUpTime = 0.2f;       // Reduced from 0.3f for snappier response
    public float swingTime = 0.15f;       // Reduced from 0.2f
    public float recoveryTime = 0.2f;     // Reduced from 0.25f
    public AnimationCurve swingCurve = new AnimationCurve(  // Adjusted curve for more impact
        new Keyframe(0, 0, 3, 3),         // Faster initial movement
        new Keyframe(0.4f, 0.8f, 1, 1),   // Earlier peak
        new Keyframe(1, 1, 0.5f, 0)       // Sharper end
    );

    public bool separate_attack = false;

    //set this to true when leashing
    bool ignoreAggro = false;

    bool busy = false;
    NavMeshAgent nav;

    Animator anim;

    private void Awake() {
        nav = GetComponent<NavMeshAgent>();
        weapon = GetComponentInChildren<Weapon>();
        anim = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //add the aggro behavior if we have an AI trigger and navigation
        if (aggroHandler != null && nav != null) {
            aggroHandler.OnTrigger += OnAggro;
        }
        //add the attack behavior if we have an AI trigger and a weapon
        if (attackHandler != null && weapon != null) {
            attackHandler.OnTrigger += OnAttack;
        }
        GetComponent<Vitality>().OnDeath += OnDeath;
        p0 = transform.position;
        //only do this if we move
        if (nav != null) {
            nav.SetDestination(p0);
        }
    }

    // Update is called once per frame
    void Update() {

        if (animateSpeed && anim != null && nav != null) {
            anim.SetFloat("Speed", nav.velocity.magnitude / nav.speed / 2f);
        }

        //make no decisions if we're busy attacking right now (and we are capable of moving!)
        if (busy || nav==null) {
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
            if (!separate_attack) {
                StartCoroutine(ExecuteAttack());
            }
            else {
                StartCoroutine(AttackTimer());
            }
        }
    }


    IEnumerator AttackTimer() {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(windUpTime);
        weapon.StartSwinging();
        yield return new WaitForSeconds(swingTime);
        weapon.StopSwinging();
        yield return null;
    }

    void OnDeath(System.Object src, System.EventArgs e) {
        if (aggroHandler != null) {
            aggroHandler.OnTrigger -= OnAggro;
        }
        if (attackHandler != null) {
            attackHandler.OnTrigger -= OnAttack;
        }
        GetComponent<Vitality>().OnDeath -= OnDeath;
    }

    private IEnumerator ExecuteAttack()
    {
        busy = true;
        Quaternion initialRotation = weapon.transform.parent.localRotation;
        Quaternion targetRotation = Quaternion.AngleAxis(60f, Vector3.up);

        // Wind-up phase
        float t0 = Time.time;
        while (Time.time - t0 <= windUpTime)
        {
            float t = (Time.time - t0) / windUpTime;
            weapon.transform.parent.localRotation =
                Quaternion.Lerp(initialRotation, Quaternion.AngleAxis(-80f, Vector3.up), t);
            yield return null;
        }

        // Active swing phase
        weapon.StartSwinging();
        t0 = Time.time;
        while (Time.time - t0 <= swingTime)
        {
            float t = (Time.time - t0) / swingTime;
            float curveValue = swingCurve.Evaluate(t);
            weapon.transform.parent.localRotation =
                Quaternion.Lerp(Quaternion.AngleAxis(-80f, Vector3.up), targetRotation, curveValue);
            yield return null;
        }
        weapon.StopSwinging();

        // Recovery phase with faster initial return
        t0 = Time.time;
        while (Time.time - t0 <= recoveryTime)
        {
            float t = (Time.time - t0) / recoveryTime;
            // Added smoothstep for faster initial return
            float smoothT = t * t * (3f - 2f * t);
            weapon.transform.parent.localRotation =
                Quaternion.Lerp(targetRotation, initialRotation, smoothT);
            yield return null;
        }
        busy = false; //shouldn't this be outside the loop so we can't immediately start another attack?

    }

    //called when we update values in the inspector
    private void OnValidate() {
        if (aggroHandler != null) {
            aggroHandler.GetComponent<SphereCollider>().radius = aggroRadius;
        }
    }



    //IEnumerator SwingWeapon(float swingTime) {
    //    busy = true;
    //    float t0 = Time.time;
    //    Quaternion initialRotation = weapon.transform.parent.localRotation;

    //    weapon.StartSwinging();

    //    while (Time.time - t0 <= swingTime) {

    //        weapon.transform.parent.localRotation = Quaternion.Lerp(initialRotation, Quaternion.AngleAxis(-90f, Vector3.up), (Time.time - t0) / swingTime);
    //        yield return null;
    //    }

    //    weapon.StopSwinging();

    //    t0 = Time.time;
    //    while (Time.time - t0 <= swingTime / 2f) {

    //        weapon.transform.parent.localRotation = Quaternion.Lerp(Quaternion.AngleAxis(-90f, Vector3.up), initialRotation, 2 * (Time.time - t0) / swingTime);
    //        yield return null;
    //    }

    //    weapon.transform.parent.localRotation = initialRotation;
    //    yield return new WaitForSeconds(0.5f);

    //    busy = false;
    //    yield return null;
    //}
}
