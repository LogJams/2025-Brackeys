using UnityEngine;

public enum MOVEMENT_TYPE {
    STOLEN_TIME, ADI_MOUSE, WASD
}


public class PlayerMovement : MonoBehaviour {

    private CharacterController charactercontroller;

    [Header("Test Different Input Configurations Here")]
    public MOVEMENT_TYPE mType = MOVEMENT_TYPE.ADI_MOUSE;
    public bool useRawInputs = true;
    public bool snappyStops = true;
    [Range(0, 1)]
    public float snappyStopThreshold = 0.1f;

    public bool smoothRotations = false;

    [Range(2, 100)]
    public float rotation_responsiveness = 10;

    Quaternion q0;

    private Animator anim;
    
    [Header("Player Variables")]
    public float base_speed = 6;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charactercontroller = GetComponent<CharacterController>();
        q0 = transform.rotation;
        anim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update() {
        //Get movement vector and rotate the inputs 45 degrees to match the camera
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (useRawInputs || (movement.sqrMagnitude <= snappyStopThreshold* snappyStopThreshold)) {
            movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }


        Quaternion inputRotation = Quaternion.AngleAxis(45f, new Vector3(0, 1, 0));
        q0 = transform.rotation;

        Vector3 lookAtMouse = MouseWorldPoint();       
        lookAtMouse -= transform.position;
        lookAtMouse.y = 0;

        switch (mType) {
            case (MOVEMENT_TYPE.STOLEN_TIME):
                //look at the mouse
                LookAt(lookAtMouse);
                break;
            case (MOVEMENT_TYPE.ADI_MOUSE):
                //look at the mouse
                LookAt(lookAtMouse);
                //only move forward/backward
                inputRotation = Quaternion.AngleAxis(Mathf.Atan2(lookAtMouse.x, lookAtMouse.z) * 180 / Mathf.PI, new Vector3(0, 1, 0));
                movement.x = 0;
                break;
            case (MOVEMENT_TYPE.WASD):
                //always look toward movement direction
                if (movement.sqrMagnitude > 0.01) {
                    LookAt(inputRotation * movement);
                }
                break;
        }

        //from above, move the character at speed
        float scale = GetComponent<Vitality>().GetAttributes().Contains(ATTRIBUTE.Speedy) ? 2 : 1;
        charactercontroller.SimpleMove(inputRotation * movement * scale * base_speed);
        anim.SetFloat("Speed", scale*movement.z);
    }

    private void LookAt(Vector3 target) {
        if (smoothRotations) {
            transform.rotation = Quaternion.Lerp(q0, Quaternion.LookRotation(target, Vector3.up), rotation_responsiveness * Time.deltaTime);
        } else {
            transform.rotation = Quaternion.LookRotation(target, Vector3.up);
        }
    }

    public Vector3 MouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.NameToLayer("GROUND_PLANE_ONLY"), QueryTriggerInteraction.Ignore))
        {
            return hitInfo.point;
        }
        return new Vector3(0, -1000, 0);
    }
}


