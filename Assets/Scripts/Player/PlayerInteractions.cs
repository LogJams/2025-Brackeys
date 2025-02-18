using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {


    //player properties
    public float swingT = 0.1f;
    public float dashDuration = 0.2f;  // How long the dash lasts
    public float dashSpeedMultiplier = 4.0f;  // How much faster the dash is

    bool busy = false;
    bool dashing = false;
    private PlayerMovement playerMovement;

    private GearManager equipment;

    //interactions
    public event System.EventHandler<IInteractable> OnInteractionStart;
    public event System.EventHandler<IInteractable> OnInteractionEnd;


    public event System.EventHandler<IInteractable> OnInteractionEvent;
    public event System.EventHandler<IInteractable> OnInteractionClose;


    private IInteractable currentInteraction = null;
    bool interacting = false;

    private void Awake() {
        equipment = GetComponent<GearManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get reference to the PlayerMovement component
        playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found!");
        }


    }

    // Update is called once per frame
    void Update() {

        //interact by hitting E, it pauses everything else in update
        if (Input.GetKeyDown(KeyCode.E) && currentInteraction != null && !busy) {
            interacting = !interacting;

            if (interacting) {
                OnInteractionEvent?.Invoke(this, currentInteraction);
                currentInteraction.Interact();
            }
            else {
                currentInteraction.EndInteraction();
                OnInteractionClose?.Invoke(this, currentInteraction);
            }


        }
        if (interacting) return; //do nothing if we're in the middle of an interaction


        //todo: maybe we can play a "wind up" animation on button down, then swing on button up
        if (Input.GetButtonUp("Swing") && !busy) {
            StartCoroutine(SwingWeapon(swingT));
        }

        if (Input.GetButton("Dash") && !dashing) {
            StartCoroutine(DashPlayer(dashSpeedMultiplier));
        }


        if (Input.GetKeyDown(KeyCode.Alpha1) && !busy) {
            equipment.CycleWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !busy) {
            equipment.CycleArmor();
        }

    }

    //TODO: we can probably put swing time as a property of the individual weapons
    IEnumerator SwingWeapon(float swingTime)
    {
        busy = true;
        float t0 = Time.time;
        Quaternion initialRotation = equipment.GetWeapon().transform.parent.localRotation;

        equipment.GetWeapon().StartSwinging();

        while (Time.time - t0 <= swingTime)
        {

            equipment.GetWeapon().transform.parent.localRotation = Quaternion.Lerp(initialRotation, Quaternion.AngleAxis(-90f, Vector3.up), (Time.time - t0) / swingTime);
            yield return null;
        }

        equipment.GetWeapon().StopSwinging();

        t0 = Time.time;
        while (Time.time - t0 <= swingTime / 2f)
        {

            equipment.GetWeapon().transform.parent.localRotation = Quaternion.Lerp(Quaternion.AngleAxis(-90f, Vector3.up), initialRotation, 2 * (Time.time - t0) / swingTime);
            yield return null;
        }

        equipment.GetWeapon().transform.parent.localRotation = initialRotation;
        busy = false;
        yield return null;
    }

    IEnumerator DashPlayer(float dashSpeed)
    {
        dashing = true;
        if (playerMovement == null) yield break;


        // Store the original speed
        float originalSpeed = playerMovement.speed;

        // Set dash speed
        playerMovement.speed *= dashSpeed;

        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);

        // Restore original speed
        playerMovement.speed = originalSpeed;

        dashing = false;
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

}


