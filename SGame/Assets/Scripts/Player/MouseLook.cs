using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using KinematicCharacterController;
public class MouseLook : NetworkBehaviour
{
    public UIManager uiManager;
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;
    private PlayerCharacterController controller;
    private MovePlayer movePlr;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        if (!IsOwner)
        {
            GetComponent<Camera>().enabled = false;
             
        }
        controller = playerBody.GetComponent<PlayerCharacterController>();
        movePlr = playerBody.GetComponent<MovePlayer>();
    }

    // Update is called once per frame
    Vector3 rotation;
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
            rotation += (Vector3.up * mouseX);
            Quaternion rot = Quaternion.Euler(rotation);
            //Call the character input after assigning our rotation so things don't feel delayed
            movePlr.CharacterInput(rot);

            }
            
           
        
    }
}
