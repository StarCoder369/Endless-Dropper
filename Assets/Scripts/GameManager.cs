using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public ObjectPool modulePool;
    public DropperSpawner spawner;
    public GameObject player;

    [Header("Menus")]
    public GameObject diePanel;
    public GameObject mainMenu;

    public float score = 0;

    public TMP_Text scoreTxt;

    [Header("Speed")]
    public float minSpeed;
    public float maxSpeed;
    public float currentSpeed;
    public float timeToMaxSpeed = 60f;

    [Header("Reverse")]
    public float reverseDuration = 10f;
    public float transitionTime = 1f;

    [Header("Slow Motion")]
    public float slowTimeScale = 0.35f;
    public float timeScaleSmoothTime = 0.2f;

    public bool noHeadacheMode = false;
    public bool reverse = false;

    public float moveMult;
    public float elapsedTime;

    private bool transitioningDown;
    private bool transitioningUp;

    private float transitionTimer;
    private float transitionStartSpeed;
    private float reverseTimer;
    private float scoreTimer;

    private Quaternion rotationStart;
    private Quaternion permRotationStart;
    private Quaternion rotationTarget;

    private bool canIncreaseScore = true;

    public bool isPlaying;

    private float targetTimeScale = 0f;
    private float timeScaleVelocity;

    public float energy;
    public float maxEnergy;
    public float energyDrainRate;

    bool usingAbility = false;

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
        scoreTxt.text = score.ToString();
        score = 0;
        currentSpeed = minSpeed;

        rotationStart = player.transform.rotation;
        permRotationStart = player.transform.rotation;
        rotationTarget = rotationStart;

        targetTimeScale = 0f;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (isPlaying)
        {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref timeScaleVelocity, timeScaleSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

            if (Mathf.Abs(Time.timeScale - targetTimeScale) < 0.001f)
            {
                Time.timeScale = targetTimeScale;
            }
        }

        if (usingAbility)
        {
            if (energy > 0)
            {
                energy -= 0.01f;
            }
        }

        if (!reverse && isPlaying)
        {
            elapsedTime += Time.deltaTime;
        }

        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Clamp01(elapsedTime / timeToMaxSpeed));

        if (transitioningDown)
        {
            transitionTimer += Time.deltaTime;

            float t = Mathf.Clamp01(transitionTimer / transitionTime);

            currentSpeed = Mathf.Lerp(transitionStartSpeed, 0f, t);

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
                {
                    reverseTimer = reverseDuration;
                }
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

        if (scoreTimer > 0)
        {
            scoreTimer -= Time.deltaTime;

            if (scoreTimer <= 0)
            {
                canIncreaseScore = true;
            }
        }

        moveMult = currentSpeed / minSpeed;
    }

    public void OnReverse()
    {
        if (reverse || transitioningDown || transitioningUp)
        {
            return;
        }

        transitionStartSpeed = currentSpeed;

        rotationStart = player.transform.rotation;
        rotationTarget = rotationStart * Quaternion.AngleAxis(180f, player.transform.right);

        transitioningDown = true;
        transitionTimer = 0f;
    }

    public void IncreaseScore(int scoreIncrease)
    {
        if (canIncreaseScore)
        {
            scoreTimer = 1f;
            canIncreaseScore = false;

            score += scoreIncrease;
            scoreTxt.text = score.ToString();
        }
    }

    public void Die()
    {
        isPlaying = false;

        targetTimeScale = 0f;
        timeScaleVelocity = 0f;
        Time.timeScale = 0f;

        DropperMovement[] modules = FindObjectsByType<DropperMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (DropperMovement module in modules)
        {
            module.ReturnModule();
        }

        diePanel.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ReturnToMenu()
    {
        isPlaying = false;

        targetTimeScale = 0f;
        timeScaleVelocity = 0f;
        Time.timeScale = 0f;

        diePanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Slow()
    {
        if (!isPlaying)
        {
            return;
        }

        targetTimeScale = slowTimeScale;
    }

    public void StopSlow()
    {
        if (!isPlaying)
        {
            return;
        }

        targetTimeScale = 1f;
    }

    public void Play()
    {
        DropperMovement[] modules = FindObjectsByType<DropperMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (DropperMovement module in modules)
        {
            module.ReturnModule();
        }

        elapsedTime = 0f;
        moveMult = 1f;
        score = 0f;
        scoreTxt.text = score.ToString();
        currentSpeed = minSpeed;
        player.transform.rotation = permRotationStart;

        reverse = false;
        transitioningDown = false;
        transitioningUp = false;

        targetTimeScale = 1f;
        timeScaleVelocity = 0f;
        Time.timeScale = 1f;

        isPlaying = true;

        diePanel.SetActive(false);
        mainMenu.SetActive(false);
    }
}