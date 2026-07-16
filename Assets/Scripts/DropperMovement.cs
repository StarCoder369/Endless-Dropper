using UnityEngine;

public class DropperMovement : MonoBehaviour
{
    public Vector3 targetDirection = Vector3.up;

    Vector3 normalizedDirection;

    void OnEnable()
    {
        int randomChild = Random.Range(0, transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            if (i == randomChild)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        normalizedDirection = targetDirection.normalized;
        transform.position += GameManager.Instance.dropperSpeed * Time.deltaTime * normalizedDirection;
    }

    void Update()
    {
        if (Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) > 2000f)
        {
            GameManager.Instance.modulePool.ReturnObject(gameObject);
        }

        transform.position += GameManager.Instance.dropperSpeed * Time.deltaTime * normalizedDirection;
    }
}
