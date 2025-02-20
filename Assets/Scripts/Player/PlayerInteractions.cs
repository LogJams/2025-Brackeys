using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [Header("Attack Properties")]
    public float windUpTime = 0.2f;       // Reduced from 0.3f for snappier response
    public float swingTime = 0.15f;       // Reduced from 0.2f
    public float recoveryTime = 0.2f;     // Reduced from 0.25f
    public AnimationCurve swingCurve = new AnimationCurve(  // Adjusted curve for more impact
        new Keyframe(0, 0, 3, 3),         // Faster initial movement
        new Keyframe(0.4f, 0.8f, 1, 1),   // Earlier peak
        new Keyframe(1, 1, 0.5f, 0)       // Sharper end
    );

    [Header("Dodge Properties")]
    public float dodgeDuration = 0.25f;    // Quick dodge duration
    public float baseDodgeSpeed = 16f;     // Base dodge speed
    public float dodgeCooldown = 0.4f;
    public float iFrameDuration = 0.2f;
    public AnimationCurve dodgeCurve = new AnimationCurve(
        new Keyframe(0, 1, 2, 2),      // Sharp initial acceleration
        new Keyframe(0.3f, 0.9f),      // Maintain high speed
        new Keyframe(1, 0, -2, -2)     // Smooth deceleration
    );

    [Header("Combat State")]
    private bool isAttacking = false;
    private bool canCancelAttack = false;
    private bool isDodging = false;
    private bool canDodge = true;
    private Vector3 lastMoveDirection;
    private Vector3 currentDodgeVelocity;
    private float currentMoveSpeed;
    private AttackPhase currentAttackPhase = AttackPhase.None;

    private PlayerMovement playerMovement;
    private GearManager equipment;
    private CharacterController characterController;
    private Vector3 dodgeDirection;

    //interactions
    public event System.EventHandler<IInteractable> OnInteractionStart;
    public event System.EventHandler<IInteractable> OnInteractionEnd;


    public event System.EventHandler<IInteractable> OnInteractionEvent;
    public event System.EventHandler<IInteractable> OnInteractionClose;


    private IInteractable currentInteraction = null;
    bool interacting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }


    private enum AttackPhase
    {
        None,
        WindUp,
        Swing,
        Recovery
    }

    private void Awake()
    {
        equipment = GetComponent<GearManager>();
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();


        if (playerMovement == null || characterController == null)
        {
            Debug.LogError("Required components missing on player!");
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.E) && currentInteraction != null && !isAttacking && !isDodging) {
            interacting = !interacting;

            if (interacting) {
                OnInteractionEvent?.Invoke(this, currentInteraction);
                currentInteraction.Interact();
            } else {
                currentInteraction.EndInteraction();
                OnInteractionClose?.Invoke(this, currentInteraction);
            }


        }
        if (interacting) return; //do nothing if we're in the middle of an interaction

        // Store the current movement direction for dodge calculations
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (moveInput.magnitude > 0.1f)
        {
            // Rotate input by 45 degrees to match your isometric view
            Quaternion inputRotation = Quaternion.AngleAxis(45f, Vector3.up);
            lastMoveDirection = (inputRotation * moveInput).normalized;
        }

        // Cycle weapon
        if (Input.GetKeyDown(KeyCode.A) && !isAttacking && !isDodging)
        {
            equipment.CycleWeapon();
        }

        // Cycle armor
        if (Input.GetKeyDown(KeyCode.Alpha2) && !isAttacking && !isDodging)
        {
            equipment.CycleArmor();
        }

        // Handle Attack Input
        if (Input.GetButtonDown("Swing") && !isAttacking && !isDodging)
        {
            StartCoroutine(ExecuteAttack());
        }

        // Improved Dodge Input handling
        if (Input.GetKeyDown(KeyCode.Mouse1) && canDodge)
        {
            Vector3 dodgeDirection;
            float dodgeSpeed = baseDodgeSpeed;

            // Determine dodge direction and speed based on current state
            if (moveInput.magnitude > 0.1f)
            {
                // Reverse the current movement direction and add speed bonus
                dodgeDirection = transform.forward;
                currentMoveSpeed = playerMovement.speed;
            }
            else
            {
                // If not moving, dodge in the direction opposite to facing
                dodgeDirection = transform.forward;
                currentMoveSpeed = 0f;
            }

            StartCoroutine(ExecuteImprovedDodge(dodgeDirection, dodgeSpeed));

            if (canCancelAttack)
            {
                StopCoroutine(ExecuteAttack());
                ResetAttackState();
            }
        }
    }

    private IEnumerator ExecuteAttack()
    {
        isAttacking = true;
        Quaternion initialRotation = equipment.GetWeapon().transform.parent.localRotation;
        Quaternion targetRotation = Quaternion.AngleAxis(60f, Vector3.up);

        // Wind-up phase
        currentAttackPhase = AttackPhase.WindUp;
        float t0 = Time.time;
        while (Time.time - t0 <= windUpTime)
        {
            float t = (Time.time - t0) / windUpTime;
            equipment.GetWeapon().transform.parent.localRotation =
                Quaternion.Lerp(initialRotation, Quaternion.AngleAxis(-80f, Vector3.up), t);
            yield return null;
        }

        // Active swing phase
        currentAttackPhase = AttackPhase.Swing;
        equipment.GetWeapon().StartSwinging();
        t0 = Time.time;
        while (Time.time - t0 <= swingTime)
        {
            float t = (Time.time - t0) / swingTime;
            float curveValue = swingCurve.Evaluate(t);
            equipment.GetWeapon().transform.parent.localRotation =
                Quaternion.Lerp(Quaternion.AngleAxis(-80f, Vector3.up), targetRotation, curveValue);
            yield return null;
        }
        equipment.GetWeapon().StopSwinging();

        // Recovery phase with faster initial return
        currentAttackPhase = AttackPhase.Recovery;
        canCancelAttack = true;
        t0 = Time.time;
        while (Time.time - t0 <= recoveryTime)
        {
            float t = (Time.time - t0) / recoveryTime;
            // Added smoothstep for faster initial return
            float smoothT = t * t * (3f - 2f * t);
            equipment.GetWeapon().transform.parent.localRotation =
                Quaternion.Lerp(targetRotation, initialRotation, smoothT);
            yield return null;
        }

        ResetAttackState();
    }

    private IEnumerator ExecuteImprovedDodge(Vector3 direction, float speed)
    {
        isDodging = true;
        canDodge = false;

        // Store original state
        bool wasMoving = currentMoveSpeed > 0;
        Vector3 originalVelocity = lastMoveDirection * currentMoveSpeed;

        // Temporarily disable normal movement
        playerMovement.enabled = false;

        //check confidence for direction
        int dir = GetComponent<Vitality>().QueryStatusEffect(EFFECTS.confident) ? -1 : 1;

        // Start invincibility
        StartCoroutine(ApplyInvincibilityFrames());

        // Execute the dodge movement
        float t0 = Time.time;
        while (Time.time - t0 <= dodgeDuration)
        {
            float t = (Time.time - t0) / dodgeDuration;

            // Use the dodge curve for speed modulation
            float speedMultiplier = dodgeCurve.Evaluate(t);

            // Calculate dodge velocity
            Vector3 dodgeVelocity = -1 * direction * speed * speedMultiplier;

            // If we were moving, blend with original velocity
            if (wasMoving)
            {
                float blendFactor = 2f; // Gradually reduce original velocity influence
                dodgeVelocity += originalVelocity * blendFactor * 0.5f;
            }

            // Apply movement
            characterController.Move(dir * dodgeVelocity * Time.deltaTime);

            yield return null;
        }

        // Re-enable normal movement
        playerMovement.enabled = true;
        isDodging = false;

        // Handle cooldown
        yield return new WaitForSeconds(dodgeCooldown - dodgeDuration);
        canDodge = true;
    }


    private void OnTriggerEnter(Collider other) {
        IInteractable inter;
        if (other.TryGetComponent(out inter)) {
            currentInteraction = inter;
            OnInteractionStart?.Invoke(this, inter);
        }
    }

    private void OnTriggerExit(Collider other) {
        IInteractable inter;
        if (other.TryGetComponent(out inter) && currentInteraction == inter) {
            //if we're currently interacting
            if (interacting) {
                interacting = false;
                currentInteraction.EndInteraction();
            }
            //clear out current interaction and notify the UI
            currentInteraction = null;
            OnInteractionEnd?.Invoke(this, inter);

        }
    }

    private IEnumerator ApplyInvincibilityFrames()
    {
        // TODO: Implement your invincibility logic here
        yield return new WaitForSeconds(iFrameDuration);
    }

    private void ResetAttackState()
    {
        isAttacking = false;
        canCancelAttack = false;
        currentAttackPhase = AttackPhase.None;
        equipment.GetWeapon().transform.parent.localRotation =
            equipment.GetWeapon().transform.parent.localRotation;
    }
}