using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ObjectPool modulePool;
    public GameObject player;

    public float score = 0;

    [Header("Speed")]
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float timeToMaxSpeed = 60f;

    [Header("Reverse")]
    public float reverseDuration = 10f;
    public float transitionTime = 1f;

    public bool noHeadacheMode = false;
    public bool reverse = false;

    public float moveMult;

    private float elapsedTime;

    private bool transitioningDown;
    private bool transitioningUp;

    private float transitionTimer;
    private float transitionStartSpeed;
    private float reverseTimer;

    private Quaternion rotationStart;
    private Quaternion rotationTarget;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentSpeed = minSpeed;
        rotationStart = player.transform.rotation;
        rotationTarget = rotationStart;
    }

    private void Update()
    {
        if (!reverse)
        {
            elapsedTime += Time.deltaTime;
        }

        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Clamp01(elapsedTime / timeToMaxSpeed));

        if (transitioningDown)
        {
            transitionTimer += Time.deltaTime;

            float t = Mathf.Clamp01(transitionTimer / transitionTime);

            currentSpeed = Mathf.Lerp(transitionStartSpeed, 0f, t);

            // Start rotating halfway through slowdown
            if (t >= 0.5f)
            {
                float rotateT = (t - 0.5f) / 0.5f;
                player.transform.rotation = Quaternion.Slerp(rotationStart, rotationTarget, rotateT);
            }

            if (transitionTimer >= transitionTime)
            {
                currentSpeed = 0f;
                player.transform.rotation = rotationTarget;

                transitioningDown = false;
                transitioningUp = true;
                transitionTimer = 0f;

                reverse = !reverse;

                if (reverse)
                    reverseTimer = reverseDuration;
            }
        }
        else if (transitioningUp)
        {
            transitionTimer += Time.deltaTime;

            float t = Mathf.Clamp01(transitionTimer / transitionTime);

            currentSpeed = Mathf.Lerp(0f, targetSpeed, t);

            if (transitionTimer >= transitionTime)
            {
                currentSpeed = targetSpeed;
                transitioningUp = false;
            }
        }
        else
        {
            currentSpeed = targetSpeed;

            if (reverse)
            {
                reverseTimer -= Time.deltaTime;

                if (reverseTimer <= 0f)
                {
                    transitionStartSpeed = currentSpeed;

                    rotationStart = player.transform.rotation;
                    rotationTarget = rotationStart * Quaternion.AngleAxis(180f, player.transform.right);

                    transitioningDown = true;
                    transitionTimer = 0f;
                }
            }
        }

        moveMult = currentSpeed / minSpeed;
    }

    public void OnReverse()
    {
        if (reverse || transitioningDown || transitioningUp)
            return;

        transitionStartSpeed = currentSpeed;

        rotationStart = player.transform.rotation;
        rotationTarget = rotationStart * Quaternion.AngleAxis(180f, player.transform.right);

        transitioningDown = true;
        transitionTimer = 0f;
    }
}