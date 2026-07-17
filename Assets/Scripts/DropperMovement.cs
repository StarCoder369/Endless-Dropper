using UnityEngine;

public class DropperMovement : MonoBehaviour
{
    public Vector3 targetDirection = Vector3.up;

    Vector3 normalizedDirection;

    int randomChild;
    Transform child;
    MeshRenderer[] childRenderers;

    void OnEnable()
    {
        randomChild = Random.Range(0, transform.childCount);
        int degreeMult = Random.Range(1, 9);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);

            if (i == randomChild)
            {
                child = transform.GetChild(i);
                child.gameObject.SetActive(true);
                child.localRotation = Quaternion.Euler(
                    child.localEulerAngles.x,
                    45f * degreeMult,
                    child.localEulerAngles.z);

                // Use the child's MeshRenderer if it has one,
                // otherwise use all MeshRenderers in its children.
                MeshRenderer renderer = child.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    childRenderers = new MeshRenderer[] { renderer };
                }
                else
                {
                    childRenderers = child.GetComponentsInChildren<MeshRenderer>(true);
                }
            }
        }
    }

    void Start()
    {
        normalizedDirection = targetDirection.normalized;
        transform.position += GameManager.Instance.currentSpeed * Time.deltaTime * normalizedDirection;
    }

    void Update()
    {
        Material targetMaterial = GetComponent<MeshRenderer>().material;

        foreach (MeshRenderer renderer in childRenderers)
        {
            if (renderer.material != targetMaterial)
            {
                renderer.material = targetMaterial;
            }
        }

        if (Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) > 2000f)
        {
            GameManager.Instance.modulePool.ReturnObject(gameObject);
        }

        transform.position += GameManager.Instance.currentSpeed * Time.deltaTime * normalizedDirection;
    }
}