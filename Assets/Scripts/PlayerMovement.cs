using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Values")]
    public InputActionProperty moveAction;

    public float moveSpeed;

    Rigidbody rb;
    float startSpeed;


    [Header("Raycast")]
    public GameObject landIndicator;
    public LayerMask landLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startSpeed = moveSpeed;
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
        moveSpeed = startSpeed * GameManager.Instance.moveMult;
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();

        Vector3 movementDir = new Vector3(inputVector.x, 0, inputVector.y);

        rb.linearVelocity = new Vector3(movementDir.x * moveSpeed, rb.linearVelocity.y, movementDir.z * moveSpeed);

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();

        Vector3 center = transform.TransformPoint(capsule.center);
        float radius = capsule.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
        float height = Mathf.Max(capsule.height * transform.lossyScale.y, radius * 2);

        Vector3 top = center + Vector3.up * (height / 2 - radius);
        Vector3 bottom = center - Vector3.up * (height / 2 - radius);

        if (Physics.CapsuleCast(top, bottom, radius, Vector3.down, out RaycastHit hit, 1000f, landLayer))
        {
            landIndicator.transform.position = new Vector3(hit.point.x, hit.point.y + 5f, hit.point.z);
        }
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
        Time.timeScale = 0f;
    }
}
