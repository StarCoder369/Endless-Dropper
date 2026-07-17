using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ObjectPool modulePool;
    public GameObject player;

    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float timeToMaxSpeed = 60f;

    private float elapsedTime;

    public float moveMult;

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
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / timeToMaxSpeed);
        currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);

        moveMult = currentSpeed / minSpeed;
    }
}