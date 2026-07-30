using System.Collections.Generic;
using UnityEngine;

public class DropperSpawner : MonoBehaviour
{
    public float moduleDistance;

    private float distanceSinceLastSpawn;

    public Material[] dropperMaterials;
    public Material noHeadacheMat;

    public readonly List<DropperMovement> chain = new();

    void Start()
    {
        if (GameManager.Instance.isPlaying)
        {
            SpawnModule();
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying)
        {
            return;
        }
        distanceSinceLastSpawn += GameManager.Instance.currentSpeed * Time.deltaTime;

        while (distanceSinceLastSpawn >= moduleDistance)
        {
            distanceSinceLastSpawn -= moduleDistance;
            SpawnModule();
        }
    }

    void SpawnModule()
    {
        if (!GameManager.Instance.isPlaying)
        {
            return;
        }

        GameObject obj = GameManager.Instance.modulePool.GetObject();

        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

        renderer.material = GameManager.Instance.noHeadacheMode ? noHeadacheMat : dropperMaterials[Random.Range(0, dropperMaterials.Length)];

        DropperMovement module = obj.GetComponent<DropperMovement>();

        module.spawner = this;
        module.followOffset = Vector3.down * moduleDistance;

        if (chain.Count == 0)
        {
            obj.transform.position = transform.position;
        }
        else
        {
            obj.transform.position = chain[^1].transform.position + module.followOffset;
        }

        chain.Add(module);
    }

    public void Remove(DropperMovement module)
    {
        chain.Remove(module);
    }
}