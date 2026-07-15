using UnityEngine;

public class DropperSpawner : MonoBehaviour
{
    public GameObject dropper;

    public float moduleDistance;

    float distanceSinceLastSpawn;

    void Update()
    {
        distanceSinceLastSpawn += GameManager.Instance.dropperSpeed * Time.deltaTime;

        while (distanceSinceLastSpawn >= moduleDistance)
        {
            distanceSinceLastSpawn -= moduleDistance;
            SpawnModule();
        }
    }

    public void SpawnModule()
    {
        Instantiate(dropper, transform.position, Quaternion.identity);
    }
}
