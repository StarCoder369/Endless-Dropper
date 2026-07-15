using UnityEngine;

public class DropperSpawner : MonoBehaviour
{
    public float moduleDistance;

    float distanceSinceLastSpawn;

    GameObject enabledModule;
    GameObject lastEnabledModule;

    void Start()
    {
        SpawnModule();
    }
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
        enabledModule = GameManager.Instance.modulePool.GetObject();
        enabledModule.transform.position = transform.position;
        if (lastEnabledModule != null)
        {
            enabledModule.transform.position = new Vector3(enabledModule.transform.position.x, lastEnabledModule.transform.position.y - moduleDistance, enabledModule.transform.position.z);
        }
        lastEnabledModule = enabledModule;
    }
}
