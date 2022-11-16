using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public bool moving = false;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public LayerMask groundMask;

    public CraftingManager cManager;
    Vector3 velocity;
    bool isGrounded;
    // Update is called once per frame
    void Update()
    {

        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x == 0 || z == 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        Vector3 move = transform.right * x + transform.forward * z;
        if (!cManager.isTyping)
        {
            controller.Move(move * speed * Time.deltaTime);
        }
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity*Time.deltaTime);
    }
}
