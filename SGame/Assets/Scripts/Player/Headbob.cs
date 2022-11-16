using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    private float timer = 0;
    public float speedModifier = 0.1f;
    public float bobAmount = 0;
    public Camera playerCam;

    public MovePlayer movePlayer;
    // Start is called before the first frame update
    void Start()
    {
        movePlayer = GetComponent<MovePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 horizontalVelocity = movePlayer.controller.velocity;
        horizontalVelocity = new Vector3(movePlayer.controller.velocity.x, 0, movePlayer.controller.velocity.z);

        // The speed on the x-z plane ignoring any speed
        float horizontalSpeed = horizontalVelocity.magnitude;



        if (movePlayer.moving)
        {
            timer += movePlayer.speed*speedModifier;

            playerCam.transform.position = new Vector3(playerCam.transform.position.x, playerCam.transform.position.y + Mathf.Sin(timer) * bobAmount, playerCam.transform.position.z);
        }
        }
}
