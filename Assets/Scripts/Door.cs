using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Door : MonoBehaviour
{
    public List<Vitality> dieList;
    public float openSpeed = 1.0f; // Controls how fast the door opens
    private float currentRotation = 0f;
    private bool isRotating = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    //private bool Stop = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        bool doorOpen = true;
        for (int i = 0; i < dieList.Count; i++)
        {
            if (dieList[i] != null)
            {
                doorOpen = false;
                break;
            }
        }

        if (doorOpen && !isRotating)
        {
            isRotating = true;
            //Stop = true;
        }

        if (isRotating)
        {
            Vector3 pivotPoint;
            // Check if we're nearly done before doing any more rotation
            if (Mathf.Abs(currentRotation - 90f) < 0.01f)
            {
                // Do one final rotation to exactly 90 degrees
                transform.position = originalPosition;
                transform.rotation = originalRotation;
                pivotPoint = transform.position + (transform.forward * transform.localScale.z / 2);
                transform.RotateAround(pivotPoint, transform.right, 90f);

                isRotating = false;
                enabled = false;
                return;
            }

            currentRotation = Mathf.Lerp(currentRotation, 90f, Time.deltaTime * openSpeed);

            transform.position = originalPosition;
            transform.rotation = originalRotation;

            pivotPoint = transform.position + (transform.forward * transform.localScale.z / 2);
            transform.RotateAround(pivotPoint, transform.right, currentRotation);
        }
    }
}