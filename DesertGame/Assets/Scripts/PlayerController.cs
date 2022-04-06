using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] Vector3 movement;
    [SerializeField] AudioClip stepAudio;
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    bool running = false;
    float currentMovementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        currentMovementSpeed = walkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        running = Input.GetButton("Run");
        if (running)
        {
            currentMovementSpeed = runningSpeed;
            animator.speed = runningSpeed / walkingSpeed;
        }
        else
        {
            currentMovementSpeed = walkingSpeed;
            animator.speed = 1;
        }
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");
        movement = new Vector3(movementX, movementY).normalized;
        if (movement == Vector3.zero)
        {
            animator.SetBool("WalkingHor", false);
            animator.SetBool("WalkingDown", false);
            animator.SetBool("WalkingUp", false);
            return;
        }
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            animator.SetBool("WalkingHor", true);
            animator.SetBool("WalkingDown", false);
            animator.SetBool("WalkingUp", false);
            spriteRenderer.flipX = (movement.x < 0);
        }
        else
        {
            if (movement.y > 0)
            {
                animator.SetBool("WalkingUp", true);
                animator.SetBool("WalkingDown", false);
            }
            else
            {
                animator.SetBool("WalkingDown", true);
                animator.SetBool("WalkingUp", false);
            }
            animator.SetBool("WalkingHor", false);
        }
    }

    private void FixedUpdate()
    {
        transform.position += movement * currentMovementSpeed * Time.deltaTime;
    }

    public void Step()
    {
        playerAudioSource.PlayOneShot(stepAudio);
    }
}
