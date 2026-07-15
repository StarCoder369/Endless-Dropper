using UnityEngine;

public class DropperMovement : MonoBehaviour
{
    public Vector3 targetDirection = Vector3.up;

    void Update()
    {
        Vector3 normalizedDirection = targetDirection.normalized;

        transform.position += GameManager.Instance.dropperSpeed * Time.deltaTime * normalizedDirection;
    }
}
