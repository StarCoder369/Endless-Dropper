using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CoinUIManager : MonoBehaviour
{
    public static CoinUIManager Instance;

    public RectTransform coinContainer;
    public RectTransform coinTarget;
    public GameObject coinPrefab;

    public float spawnOffset = 100f;
    public float popDuration = 0.1f;
    public float rotateDuration = 0.3f;
    public float waitDuration = 0.1f;
    public float moveDuration = 0.4f;

    public Ease popEase = Ease.OutBack;
    public Ease moveEase = Ease.InBack;

    private List<GameObject> activeCoins = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SpawnCoin()
    {
        GameObject coin = Instantiate(coinPrefab, coinContainer);
        if (GameManager.Instance.muted)
        {
            coin.GetComponent<AudioSource>().enabled = false;
        }
        else
        {
            coin.GetComponent<AudioSource>().enabled = true;
            coin.GetComponent<AudioSource>().volume = Random.Range(0.3f, 0.7f);
            coin.GetComponent<AudioSource>().pitch = Random.Range(0.6f, 1f);
        }

        activeCoins.Add(coin);

        RectTransform rect = coin.GetComponent<RectTransform>();

        rect.position = coinTarget.position;
        rect.anchoredPosition += new Vector2(Random.Range(-spawnOffset, spawnOffset), Random.Range(-spawnOffset, spawnOffset));
        rect.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence().SetId(coin);

        seq.Append(rect.DOScale(1f, popDuration).SetEase(popEase));
        seq.Join(rect.DORotate(new Vector3(0, 0, 360), rotateDuration, RotateMode.FastBeyond360));
        seq.AppendInterval(waitDuration);
        seq.Append(rect.DOMove(coinTarget.position, moveDuration).SetEase(moveEase));

        seq.OnComplete(() =>
        {
            if (coin != null)
            {
                activeCoins.Remove(coin);
                Destroy(coin);
            }
        });
    }

    public void ClearCoins()
    {
        foreach (GameObject coin in activeCoins)
        {
            if (coin != null)
            {
                DOTween.Kill(coin);
                Destroy(coin);
            }
        }

        activeCoins.Clear();

        coinTarget.DOKill();
        coinTarget.localScale = Vector3.one;
    }

    public void AddCoins(int amount)
    {
        GameManager.Instance.IncreaseCoins(amount);

        for (int i = 0; i < amount; i++)
        {
            SpawnCoin();
        }
    }
}