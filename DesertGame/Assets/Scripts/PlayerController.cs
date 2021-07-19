using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] Vector3 movement;
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
            currentMovementSpeed = runningSpeed;
        else
            currentMovementSpeed = walkingSpeed;
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");
        movement = new Vector3(movementX, movementY).normalized;
        
    }

    private void FixedUpdate()
    {
        transform.position += movement * currentMovementSpeed * Time.deltaTime;
    }
}
