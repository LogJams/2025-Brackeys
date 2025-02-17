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

}


