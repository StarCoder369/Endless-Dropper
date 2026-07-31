using UnityEngine;

public class DropperMovement : MonoBehaviour
{
    public Vector3 targetDirection = Vector3.up;

    private Vector3 direction;

    public Vector3 followOffset;

    public int reverseChance = 6;

    [HideInInspector]
    public DropperSpawner spawner;

    private MeshRenderer[] childRenderers;

    void OnEnable()
    {
        direction = targetDirection.normalized;

        int randomChild = Random.Range(0, transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        Transform child = transform.GetChild(randomChild);
        child.gameObject.SetActive(true);

        child.localRotation = Quaternion.Euler(child.localEulerAngles.x, Random.Range(0, 8) * 45f, child.localEulerAngles.z);

        childRenderers = child.GetComponentsInChildren<MeshRenderer>(true);

        foreach (Transform t in child.GetComponentsInChildren<Transform>(true))
        {
            if (GameManager.Instance.elapsedTime >= 7.5f)
            {
                if (t.CompareTag("Reverse"))
                {
                    t.gameObject.SetActive(Random.Range(0, reverseChance) == 0);
                }
            }
            else
            {
                if (t.CompareTag("Reverse"))
                {
                    t.gameObject.SetActive(false);
                }
            }
        }


    }

    void Update()
    {
        Material mat = GetComponent<MeshRenderer>().material;

        foreach (MeshRenderer r in childRenderers)
        {
            if (!r.CompareTag("Score") && !r.CompareTag("Reverse") && r.material != mat)
            {
                r.material = mat;
            }
        }

        if ((GameManager.Instance.playerMove.transform.position - transform.position).sqrMagnitude > 3000f * 3000f)
        {
            ReturnModule();
            return;
        }

        int index = spawner.chain.IndexOf(this);

        if (index == 0)
        {
            Vector3 moveDir = GameManager.Instance.reverse ? -direction : direction;

            transform.position += moveDir * GameManager.Instance.currentSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = spawner.chain[index - 1].transform.position + followOffset;
        }
    }

    public void ReturnModule()
    {
        spawner.Remove(this);
        GameManager.Instance.modulePool.ReturnObject(gameObject);
        return;
    }
}