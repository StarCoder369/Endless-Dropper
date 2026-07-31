using UnityEngine;
using UnityEngine.UI;

public class DynamicUIScreen : MonoBehaviour
{
    public Selectable customFirstSelection;

    public GameObject GetFirstSelectable()
    {
        if (customFirstSelection != null && customFirstSelection.gameObject.activeInHierarchy)
        {
            return customFirstSelection.gameObject;
        }

        Selectable[] selectables = GetComponentsInChildren<Selectable>(false);

        foreach (Selectable selectable in selectables)
        {
            if (selectable.gameObject.activeInHierarchy && selectable.interactable)
            {
                return selectable.gameObject;
            }
        }

        return null;
    }
}
