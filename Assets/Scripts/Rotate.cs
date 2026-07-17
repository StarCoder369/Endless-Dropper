using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float minRotateSpeed;
    public float maxRotateSpeed;
    public float rotateChance;

    float rotateSpeed;
    bool rotate;
    void OnEnable()
    {
        rotate = Random.Range(0, 100) <= rotateChance;
        rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
    }

    void FixedUpdate()
    {
        if (rotate)
        {
            transform.Rotate(0, rotateSpeed, 0);
        }
    }
}
