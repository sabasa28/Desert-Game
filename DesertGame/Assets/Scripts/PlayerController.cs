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
    bool isInvulnerable = false;
    [SerializeField] float invulnerabilityTime;
    [SerializeField] float invulnerabilityBlinkTime;
    // Start is called before the first frame update
    void Start()
    {
        currentMovementSpeed = walkingSpeed;
        GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;
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
        if (Mathf.Abs(movement.x) >= Mathf.Abs(movement.y))
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

    public void TakeDamage()
    {
        Debug.Log("ouch1");
        if (isInvulnerable) return;
        Debug.Log("ouch2");
        isInvulnerable = true;
        StartCoroutine(Invulnerable());
    }

    public void Step()
    {
        playerAudioSource.PlayOneShot(stepAudio);
    }

    IEnumerator Invulnerable()
    {
        float timer = 0.0f;
        while (timer < invulnerabilityTime)
        {
            timer += Time.deltaTime;
            if ((int)(timer / invulnerabilityBlinkTime % 2) == 0) spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
            else spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isInvulnerable = false;
    }
}
