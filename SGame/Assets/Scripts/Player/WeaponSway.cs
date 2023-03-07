using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{

    [Header("Tilt")]
    [SerializeField]private float amount;
    [SerializeField] private float maxSway;
    [SerializeField] private float smoothAmount;
    [Space]
    [Header("Rotation Sway")]
    [SerializeField] private float tiltAmount;
    [SerializeField] private float maxTiltSway;
    [SerializeField] private float smoothAmountTilt;

    Vector3 initialPosition;
    Quaternion initialRotation;
    [Header("Components")]
    [SerializeField] private GameObject camera;
    private float sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        sensitivity = camera.GetComponent<MouseLook>().mouseSensitivity;
    }

    // Update is called once per frame
    
    void Update()
    {
        TiltSway();
        RotationSway();
    }
    private void TiltSway()
    {
        float moveX = Input.GetAxis("Mouse X") * amount * sensitivity;
        float moveY = Input.GetAxis("Mouse Y") * amount * sensitivity;
        Debug.Log(moveX);
        moveX = Mathf.Clamp(moveX, -maxSway, maxSway);
        moveY = Mathf.Clamp(moveY, -maxSway, maxSway);
        Vector3 finalPos = new Vector3(-moveX, 0, moveY);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPosition, Time.deltaTime * smoothAmount);
    }
    private void RotationSway()
    {
        float tiltX = Input.GetAxis("Mouse X") * tiltAmount * sensitivity;
        float tiltY = Input.GetAxis("Mouse Y") * tiltAmount * sensitivity;
        tiltY = Mathf.Clamp(tiltY, -maxTiltSway, maxTiltSway);
        tiltX = Mathf.Clamp(tiltX, -maxTiltSway, maxTiltSway);
        Quaternion finalRotation = Quaternion.Euler(new Vector3(-tiltX, tiltY, tiltY));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothAmountTilt);
    }
}
