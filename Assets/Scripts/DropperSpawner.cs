using UnityEngine;

public class DropperSpawner : MonoBehaviour
{
    public float moduleDistance;

    float distanceSinceLastSpawn;

    GameObject enabledModule;
    GameObject lastEnabledModule;

    public Material[] dropperMaterials;
    public Material noHeadacheMat;

    void Start()
    {
        SpawnModule();
    }
    void Update()
    {
        distanceSinceLastSpawn += GameManager.Instance.currentSpeed * Time.deltaTime;

        while (distanceSinceLastSpawn >= moduleDistance)
        {
            distanceSinceLastSpawn -= moduleDistance;
            SpawnModule();
        }
    }

    public void SpawnModule()
    {
        enabledModule = GameManager.Instance.modulePool.GetObject();
        if (!GameManager.Instance.noHeadacheMode)
        {
            Material randomMat = dropperMaterials[Random.Range(0, dropperMaterials.Length)];
            enabledModule.GetComponent<MeshRenderer>().material = randomMat;
        }
        else
        {
            enabledModule.GetComponent<MeshRenderer>().material = noHeadacheMat;
        }


        enabledModule.transform.position = transform.position;
        if (lastEnabledModule != null)
        {
            enabledModule.transform.position = new Vector3(enabledModule.transform.position.x, lastEnabledModule.transform.position.y - moduleDistance, enabledModule.transform.position.z);
            enabledModule.GetComponent<DropperMovement>().lastModule = lastEnabledModule;
            enabledModule.GetComponent<DropperMovement>().moduleDistance = moduleDistance;
        }
        lastEnabledModule = enabledModule;
    }
}