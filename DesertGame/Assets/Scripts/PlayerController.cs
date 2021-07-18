using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] Vector3 movement;
    float currentMovementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        currentMovementSpeed = walkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = Input.GetAxisRaw("Horizontal");
        float movementY = Input.GetAxisRaw("Vertical");
        movement = new Vector3(movementX, movementY).normalized;
        
    }

    private void FixedUpdate()
    {
        transform.position += movement * currentMovementSpeed * Time.deltaTime;
    }
}
