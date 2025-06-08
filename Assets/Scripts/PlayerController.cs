using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //movement variables
    private Vector2 input;
    private CharacterController controller;
    private Vector3 dir;
    [SerializeField] private float speed = 5f;

    // Gravity and jumping
    private float gravity = -9.81f;
    [SerializeField] private float gravMultiplier = 3.0f;
    [SerializeField] private float jumpPower;
    private float velocity;

    void Awake()
    {
        //Assigning player character controller to controller object
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        ApplyGravity();
        ApplyMovement();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speed = 10f;
        }
        else if (context.canceled)
        {
            speed = 5f;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!isGrounded()) return;

        velocity += jumpPower;
    }
    private void ApplyGravity()
    {
        if (isGrounded() && velocity < 0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += gravity * gravMultiplier * Time.deltaTime;
        }
        dir.y = velocity;
    }
    private void ApplyMovement()
    {
        // Get the movement direction relative to the player’s facing direction
        Vector3 move = transform.TransformDirection(new Vector3(input.x, 0, input.y));
        move.y = velocity; // Apply vertical movement separately

        controller.Move(move * speed * Time.deltaTime);
    }
    public void Move(InputAction.CallbackContext context)
    {
        //Taking player input and accordinly setting the direction in which player needs to move
        input = context.ReadValue<Vector2>();
        dir = new Vector3(input.x, 0.0f, input.y);

    }

    private bool isGrounded() => controller.isGrounded;

}