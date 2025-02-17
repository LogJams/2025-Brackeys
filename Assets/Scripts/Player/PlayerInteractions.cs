using System.Collections;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{

    public Weapon weapon;
    public float swingT = 0.1f;
    public float dashDuration = 0.2f;  // How long the dash lasts
    public float dashSpeedMultiplier = 4.0f;  // How much faster the dash is

    bool busy = false;
    private PlayerMovement playerMovement;

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
    void Update()
    {


        //todo: maybe we can play a "wind up" animation on button down, then swing on button up
        if (Input.GetButtonUp("Swing") && !busy)
        {
            Debug.Log("Swing!");
            StartCoroutine(SwingWeapon(swingT));
        }

        if (Input.GetButtonUp("Dash") && !busy)
        {
            Debug.Log("Dash!");
            StartCoroutine(DashPlayer(dashSpeedMultiplier));
        }



        IEnumerator SwingWeapon(float swingTime)
        {
            busy = true;
            float t0 = Time.time;
            Quaternion initialRotation = weapon.transform.parent.localRotation;

            weapon.StartSwinging();

            while (Time.time - t0 <= swingTime)
            {

                weapon.transform.parent.localRotation = Quaternion.Lerp(initialRotation, Quaternion.AngleAxis(-90f, Vector3.up), (Time.time - t0) / swingTime);
                yield return null;
            }

            weapon.StopSwinging();

            t0 = Time.time;
            while (Time.time - t0 <= swingTime / 2f)
            {

                weapon.transform.parent.localRotation = Quaternion.Lerp(Quaternion.AngleAxis(-90f, Vector3.up), initialRotation, 2 * (Time.time - t0) / swingTime);
                yield return null;
            }

            weapon.transform.parent.localRotation = initialRotation;
            busy = false;
            yield return null;
        }

        IEnumerator DashPlayer(float dashSpeed)
        {
            if (playerMovement == null) yield break;

            busy = true;

            // Store the original speed
            float originalSpeed = playerMovement.speed;

            // Set dash speed
            playerMovement.speed *= dashSpeed;

            // Wait for dash duration
            yield return new WaitForSeconds(dashDuration);

            // Restore original speed
            playerMovement.speed = originalSpeed;

            busy = false;
        }

    }

}


