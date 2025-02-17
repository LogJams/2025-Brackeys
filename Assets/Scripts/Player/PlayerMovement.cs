using UnityEngine;

// This is the old movement class

public class PlayerMovement : MonoBehaviour
{
    [Header("Test Variables")]
    public bool moveToMouse = false;
    [Header("Player Variables")]
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //default movement
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Quaternion rotation = Quaternion.AngleAxis(45f, new Vector3(0, 1, 0));
        //always look at the mouse
        Vector3 lookAt = MouseWorldPoint();
        if (lookAt.y >= -0.1)
        {
            lookAt -= transform.position;
            lookAt.y = 0;
            //look at the mouse raycast point
            transform.rotation = Quaternion.LookRotation(lookAt, Vector3.up);
        }
        //move toward mouse movement
        if (moveToMouse)
        {
            rotation = Quaternion.AngleAxis(Mathf.Atan2(lookAt.x, lookAt.z) * 180 / Mathf.PI, new Vector3(0, 1, 0));
            movement.x = 0;
        }
        //move
        transform.position = transform.position + rotation * movement * speed * Time.deltaTime;
    }
    public Vector3 MouseWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            return hitInfo.point;
        }
        return new Vector3(0, -1000, 0);
    }
}


