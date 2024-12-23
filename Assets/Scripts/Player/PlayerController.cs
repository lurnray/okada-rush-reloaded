using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed = 6;

    private int desiredLane = 1; // 0: Left, 1: Middle, 2: Right
    public float laneDistance = 2.5f; // Distance between two lanes

    public float gravity = -12.0f;
    public float jumpHeight = 6.0f; // Controls how high the player jumps
    private Vector3 velocity;

    public ParticleSystem collisionParticle;
    // public ParticleSystem fireworksParticle;

    private AudioSource playerAudio;
    public AudioClip obstacleCollision;
    // public AudioClip explodeSound;

    private bool isSliding = false;
    public float slideDuration = 1.5f;

    // Limits for Y position
    public float topLimit = 6.0f;   // Maximum height the player can reach
    public float lowerLimit = 1.2f; // Ground level

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Time.timeScale = 1.2f;
        playerAudio = GetComponent<AudioSource>();

        // Ensure player starts at lowerLimit
        transform.position = new Vector3(transform.position.x, lowerLimit, transform.position.z);
    }

    void Update()
    {
        if (!PlayerManager.isGameStarted)
            return;

        // Apply gravity
        if (transform.position.y > lowerLimit) // Apply gravity if above ground
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (transform.position.y <= lowerLimit) // Reset velocity when on ground
        {
            velocity.y = -1f;
            transform.position = new Vector3(transform.position.x, lowerLimit, transform.position.z);
        }

        // Prevent the player from exceeding the top limit
        if (transform.position.y >= topLimit)
        {
            transform.position = new Vector3(transform.position.x, topLimit, transform.position.z);
        }

        // Handle Jump
        if (SwipeManager.swipeUp)
            Jump();

        // Handle Slide
        if (SwipeManager.swipeDown && !isSliding)
            StartCoroutine(Slide());

        // Apply movement
        controller.Move(velocity * Time.deltaTime);

        // Handle Lane Switching
        if (SwipeManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2;
        }
        if (SwipeManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0;
        }

        // Calculate target lane position
        Vector3 targetPosition = transform.position.z * Vector3.forward + transform.position.y * Vector3.up;
        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        // Smoothly move player to target lane
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 30 * Time.deltaTime;
        if (diff.magnitude > moveDir.magnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);

        // Constant forward movement
        direction.z = forwardSpeed;
        controller.Move(direction * Time.deltaTime);
    }

    private void Jump()
    {
        if (Mathf.Abs(transform.position.y - lowerLimit) < 0.2f) // Allow jump if close to ground
        {
            Debug.Log("Jump Triggered");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Apply jump force
        }
        else
        {
            Debug.Log("Cannot Jump - Player is not grounded");
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;

        yield return new WaitForSeconds(slideDuration);

        controller.center = Vector3.zero;
        controller.height = 2;
        isSliding = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Collision detected with: " + hit.transform.name);
        if (hit.transform.tag == "Obstacle")
        {
            Debug.Log("Playing collision particle effect.");
            collisionParticle.Play();
            playerAudio.PlayOneShot(obstacleCollision, 1.0f);
            Debug.Log("Game Over - Hit Obstacle");
            PlayerManager.gameOver = true;
        }
    }
}
