using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public ObjectPool modulePool;
    public DropperSpawner spawner;
    public GameObject player;

    [Header("Other Values")]
    public int coins;
    public TMP_Text coinTxt;

    [Header("UI")]
    public GameObject diePanel;
    public GameObject mainMenu;
    public Slider energySlider;

    public int score = 0;
    public int highScore = 0;
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

    [Header("Energy")]
    public float energy;
    public float maxEnergy;
    public float energyDrainRate = 0.01f;

    public bool usingAbility = false;
    public bool isPlaying;

    private bool transitioningDown;
    private bool transitioningUp;
    private bool canIncreaseScore = true;

    private float transitionTimer;
    private float transitionStartSpeed;
    private float reverseTimer;
    private float scoreTimer;

    private float targetTimeScale;
    private float timeScaleVelocity;

    private Quaternion rotationStart;
    private Quaternion permRotationStart;
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
        SaveManager.Instance.Load();
        scoreTxt.text = score.ToString();
        score = 0;

        currentSpeed = minSpeed;

        rotationStart = player.transform.rotation;
        permRotationStart = player.transform.rotation;
        rotationTarget = rotationStart;

        targetTimeScale = 0f;
        Time.timeScale = 0f;

        mainMenu.SetActive(true);

        coins = SaveManager.Instance.GetCoins();
        highScore = SaveManager.Instance.GetHighScore();
    }


    private void Update()
    {
        UpdateUI();

        //Requires playing functions
        if (!isPlaying)
        {
            return;
        }

        UpdateTimeScale();
        UpdateEnergy();
        UpdateElapsedTime();
        UpdateMovementSpeed();
        UpdateScoreTimer();
    }


    public void UpdateHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            SaveManager.Instance.SetHighScore(highScore);
        }
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref timeScaleVelocity, timeScaleSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        if (Mathf.Abs(Time.timeScale - targetTimeScale) < 0.001f)
        {
            Time.timeScale = targetTimeScale;
        }
    }


    private void UpdateEnergy()
    {
        if (usingAbility)
        {
            if (energy > 0)
            {
                energy -= energyDrainRate;
            }
        }
        else
        {
            if (energy < maxEnergy)
            {
                energy += energyDrainRate / 2;
            }
        }

        energySlider.value = energy / maxEnergy;
    }


    private void UpdateElapsedTime()
    {
        if (!reverse && !usingAbility)
        {
            elapsedTime += Time.deltaTime;
        }
    }


    private void UpdateMovementSpeed()
    {
        float targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Clamp01(elapsedTime / timeToMaxSpeed));


        if (transitioningDown)
        {
            HandleTransitionDown();
        }
        else if (transitioningUp)
        {
            HandleTransitionUp(targetSpeed);
        }
        else
        {
            HandleNormalMovement(targetSpeed);
        }

        moveMult = currentSpeed / minSpeed;
    }


    private void HandleTransitionDown()
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


    private void HandleTransitionUp(float targetSpeed)
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
    private void HandleNormalMovement(float targetSpeed)
    {
        currentSpeed = targetSpeed;

        if (reverse)
        {
            reverseTimer -= Time.deltaTime;

            targetTimeScale = 0.9f;

            if (reverseTimer <= 0f)
            {
                StartReverseTransition();
            }
        }
    }


    private void StartReverseTransition()
    {
        transitionStartSpeed = currentSpeed;

        rotationStart = rotationTarget;

        rotationTarget = rotationStart * Quaternion.AngleAxis(180f, rotationStart * Vector3.right);

        transitioningDown = true;
        transitionTimer = 0f;
    }


    private void UpdateScoreTimer()
    {
        if (scoreTimer > 0)
        {
            scoreTimer -= Time.deltaTime;

            if (scoreTimer <= 0)
            {
                canIncreaseScore = true;
            }
        }
    }


    private void UpdateUI()
    {
        coinTxt.text = coins.ToString();
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

        ResetModules();

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
        if (!isPlaying && !reverse)
        {
            return;
        }

        targetTimeScale = 1f;
    }


    public void Play()
    {
        ResetModules();

        elapsedTime = 0f;

        moveMult = 1f;

        score = 0;

        scoreTxt.text = score.ToString();

        currentSpeed = minSpeed;

        player.transform.rotation = permRotationStart;

        rotationStart = permRotationStart;
        rotationTarget = permRotationStart;

        reverse = false;

        transitioningDown = false;
        transitioningUp = false;

        targetTimeScale = 1f;

        timeScaleVelocity = 0f;

        Time.timeScale = 1f;

        isPlaying = true;

        diePanel.SetActive(false);
        mainMenu.SetActive(false);
        SaveManager.Instance.Save();
    }


    private void ResetModules()
    {
        DropperMovement[] modules = FindObjectsByType<DropperMovement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);


        foreach (DropperMovement module in modules)
        {
            module.ReturnModule();
        }
    }

    public void IncreaseCoins(int increaseAmount)
    {
        coins += increaseAmount;
    }
}