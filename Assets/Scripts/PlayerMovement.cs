using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionProperty moveAction;

    public float moveSpeed;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        moveAction.action?.Enable();
    }

    void OnDisable()
    {
        moveAction.action?.Disable();
    }

    void FixedUpdate()
    {
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();

        Vector3 movementDir = new Vector3(inputVector.x, 0, inputVector.y);

        rb.linearVelocity = new Vector3(movementDir.x * moveSpeed, rb.linearVelocity.y, movementDir.z * moveSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died");
    }
}
