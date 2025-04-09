using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, INoisy
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
    [SerializeField] GameObject lampLight;
    [SerializeField] GameObject naturalLight;
    bool lampOn = false;
    NoiseMaker noiseMaker;
    Inventory playerInventory;

    private void Awake()
    {
        noiseMaker = GetComponentInChildren<NoiseMaker>();
        playerInventory = GetComponent<Inventory>();
    }
    void Start()
    {
        currentMovementSpeed = walkingSpeed;
        GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    void Update()
    {
        #region Inputs
        running = Input.GetButton("Run");
        if (Input.GetKeyDown(KeyCode.F)) ToggleLampState();

        #endregion

        if (running)
        {
            currentMovementSpeed = runningSpeed;
            animator.speed = runningSpeed / walkingSpeed;
            SetLampState(false);
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
        if (isInvulnerable) return;
        isInvulnerable = true;
        StartCoroutine(Invulnerable());
    }

    public void Step()
    {
        float stepVolume = 0.5f;
        NoiseLevel stepNoiseLevel = NoiseLevel.low;
        if (running)
        {
            stepVolume = 1.0f;
            stepNoiseLevel = NoiseLevel.high;
        }

        playerAudioSource.PlayOneShot(stepAudio, stepVolume);
        NoiseWasMade(new Noise(stepNoiseLevel, transform.position));
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

    void ToggleLampState()
    {
        SetLampState(!lampOn);
    }

    void SetLampState(bool newState)
    {
        lampOn = newState;
        naturalLight.SetActive(!lampOn);
        lampLight.SetActive(lampOn);
    }

    public void NoiseWasMade(Noise noise)
    {
        noiseMaker.AlertCloseNoiseCatchers(noise);
    }

    public Inventory GetPlayerInventory()
    {
        return playerInventory;
    }
}
