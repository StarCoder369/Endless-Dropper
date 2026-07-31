using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Values")]
    public InputActionProperty moveAction;
    public InputActionProperty abilityAction;
    public InputActionProperty secondaryAbilityAction;

    public float moveSpeed;

    Rigidbody rb;
    CapsuleCollider capsule;
    float startSpeed;


    [Header("Raycast")]
    public GameObject landIndicator;
    public LayerMask landLayer;


    public enum Abilities
    {
        None,
        Slow,
        Indicator,
        Dash
    }

    public Abilities currentAbility = Abilities.None;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        startSpeed = moveSpeed;
    }


    void OnEnable()
    {
        moveAction.action?.Enable();
        abilityAction.action?.Enable();
        secondaryAbilityAction.action?.Enable();
    }


    void OnDisable()
    {
        moveAction.action?.Disable();
        abilityAction.action?.Disable();
        secondaryAbilityAction.action?.Disable();
    }


    void Update()
    {
        HandleSlowAbility();
        HandleIndicatorAbility();
    }


    private void HandleSlowAbility()
    {
        if (currentAbility != Abilities.Slow)
        {
            return;
        }


        if (abilityAction.action.IsPressed() || secondaryAbilityAction.action.IsPressed())
        {
            GameManager.Instance.usingAbility = true;

            if (GameManager.Instance.energy > GameManager.Instance.energyDrainRate)
            {
                GameManager.Instance.Slow();
            }
            else
            {
                GameManager.Instance.StopSlow();
            }
        }
        else
        {
            GameManager.Instance.usingAbility = false;
            GameManager.Instance.StopSlow();
        }
    }


    private void HandleIndicatorAbility()
    {
        landIndicator.SetActive(currentAbility == Abilities.Indicator);
    }


    void FixedUpdate()
    {
        if (GameManager.Instance.playerFrozen)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        moveSpeed = startSpeed * GameManager.Instance.moveMult;

        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();

        Vector3 movementDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (GameManager.Instance.reverse)
        {
            movementDir.z *= -1;
        }

        rb.linearVelocity = new Vector3(
            movementDir.x * moveSpeed,
            rb.linearVelocity.y,
            movementDir.z * moveSpeed
        );


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
            CoinUIManager.Instance.ClearCoins();
            Die();
        }
        else if (other.CompareTag("Score"))
        {
            GameManager.Instance.IncreaseScore(1);
            CoinUIManager.Instance.AddCoins(5);
        }
        else if (other.CompareTag("Reverse"))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.OnReverse();
            CoinUIManager.Instance.AddCoins(5);
        }
    }


    public void Die()
    {
        GameManager.Instance.Die();
    }
}