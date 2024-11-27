using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

     private CharacterController controller;
     private Vector3 direction;
     public float forwardSpeed;

     private int desiredLane = 1;//0:left, 1:middle, 2:right
     public float laneDistance = 2.5f;//The distance between tow lanes

     public float jumpForce;
     public float gravity = -12f;





    // Start is called before the first frame update
    void Start()
    {
     controller = GetComponent<CharacterController>();
        Time.timeScale = 1.2f;   
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;

        

        if (controller.isGrounded)
        {
            direction.y = -1;
            if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           Jump(); 
        }

        }
        else 
        {
            direction.y += gravity*Time.deltaTime;
        }

        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }

        //Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        //transform.position = targetPosition;
        if (transform.position != targetPosition)
        {
            Vector3 diff = targetPosition - transform.position;
            Vector3 moveDir = diff.normalized * 30 * Time.deltaTime;
            if (moveDir.sqrMagnitude < diff.magnitude)
                controller.Move(moveDir);
            else
                controller.Move(diff);
        }

     //   controller.Move(move * Time.deltaTime);

    }

    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;

    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Obstacle")
        {
            PlayerManager.gameOver = true;
            
        }
    }
}
