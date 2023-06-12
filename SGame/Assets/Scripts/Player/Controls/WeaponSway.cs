using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
public class WeaponSway : NetworkBehaviour
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
    [SerializeField] private GameObject playerCamera;
    private float sensitivity;
    // Start sets initial values
    void Start()
    {
        if (!IsOwner) this.enabled = false;
    }
    /// <summary>
    /// Sets the intial rotation, position, and sensitivity of the sway after the camera has been assigned
    /// </summary>
    private void SetInitialValues()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        sensitivity = playerCamera.GetComponent<MouseLook>().mouseSensitivity;
    }
    //Update calls tilt and rotation
    void Update()
    {
        TiltSway();
        RotationSway();
    }
    /// <summary>
    /// Sways the current item by the input of the mouse axis
    /// </summary>
    private void TiltSway()
    {
        float moveX = Input.GetAxis("Mouse X") * amount * sensitivity;
        float moveY = Input.GetAxis("Mouse Y") * amount * sensitivity;
        moveX = Mathf.Clamp(moveX, -maxSway, maxSway);
        moveY = Mathf.Clamp(moveY, -maxSway, maxSway);
        Vector3 finalPos = new Vector3(-moveX, 0, moveY);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPosition, Time.deltaTime * smoothAmount);
    }
    /// <summary>
    /// Rotates the current item by the inuput of the mouse axis
    /// </summary>
    private void RotationSway()
    {
        float tiltX = Input.GetAxis("Mouse X") * tiltAmount * sensitivity;
        float tiltY = Input.GetAxis("Mouse Y") * tiltAmount * sensitivity;
        tiltY = Mathf.Clamp(tiltY, -maxTiltSway, maxTiltSway);
        tiltX = Mathf.Clamp(tiltX, -maxTiltSway, maxTiltSway);
        Quaternion finalRotation = Quaternion.Euler(new Vector3(-tiltX, tiltY, tiltY));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothAmountTilt);
    }
    /// <summary>
    /// Sets the camera to use for sensitivity
    /// </summary>
    /// <param name="cameraToSet">The camera gameobject reference</param>
    public void SetCamera(GameObject cameraToSet)
    {
        playerCamera = cameraToSet;
        SetInitialValues();
    }
}
