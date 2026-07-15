using UnityEngine;

public class DropperMovement : MonoBehaviour
{
    public Vector3 targetDirection = Vector3.up;

    Vector3 normalizedDirection;

    public Transform lastModule;
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

        // if (lastModule != null)
        // {
        //     transform.position = new Vector3(transform.position.x, lastModule.transform.position.y, transform.position.z);
        //     return;
        // }
        transform.position += GameManager.Instance.dropperSpeed * Time.deltaTime * normalizedDirection;
    }
}
