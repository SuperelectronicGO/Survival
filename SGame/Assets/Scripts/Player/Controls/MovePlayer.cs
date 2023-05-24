using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using KinematicCharacterController;
public class MovePlayer : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    public PlayerCharacterController Character;
    public CraftingManager cManager;
    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";
    //CharacterInput is called by MouseLook after assigning camera rotation so things don't feel delayed
    public void CharacterInput(Quaternion cameraRotation)
    {
        if (!cManager.isTyping)
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            //Create struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
            characterInputs.CameraRotation = cameraRotation;
            Character.SetInputs(ref characterInputs);
        }
    }









}
    /*
    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            
            Vector3 move = transform.right * x + transform.forward * z;
            if (!cManager.isTyping)
            {
                controller.Move(move * speed * Time.deltaTime);
                moving = (controller.velocity != Vector3.zero);
            }
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }


            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
    */