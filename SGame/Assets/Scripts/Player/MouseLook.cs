using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MouseLook : NetworkBehaviour
{
    public UIManager uiManager;
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        if (!IsOwner)
        {
            GetComponent<Camera>().enabled = false;
             
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

            //  float mouseX = Input.GetAxis("Mouse X")*mouseSensitivity*Time.deltaTime;
            //float mouseY = Input.GetAxis("Mouse Y")*mouseSensitivity*Time.deltaTime;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            if (!uiManager.inventoryOpen)
            {
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 80f);

                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
            }
        
    }
}
