using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum ItemState
    {
        Locked,
        NotEquipped,
        Equipped
    }

    [Header("References")]
    public ShopManager shopManager;

    [Header("Base Values")]
    public string itemName;
    public int itemCost;
    public Sprite itemIcon;

    [Header("Item References")]
    public TMP_Text itemNameTxt;
    public TMP_Text itemCostTxt;
    public Image itemImg;
    public Image coinImg;
    public TMP_Text stateTxt;

    [Header("Internal Values")]
    public ItemState currentState = ItemState.Locked;

    void Start()
    {
        itemNameTxt.text = itemName;
        itemCostTxt.text = itemCost.ToString();
        itemImg.sprite = itemIcon;
    }

    public void TryClick()
    {
        if (currentState == ItemState.Locked)
        {
            if (1 >= itemCost)
            {
                // GameManager.Instance.coins -= itemCost;
                currentState = ItemState.NotEquipped;
                coinImg.gameObject.SetActive(false);
                itemCostTxt.gameObject.SetActive(false);
                stateTxt.gameObject.SetActive(true);
                stateTxt.text = "Not Equipped";
            }
        }
        else if (currentState == ItemState.NotEquipped)
        {
            shopManager.SetAllNotEquipped(this);
            currentState = ItemState.Equipped;
            coinImg.gameObject.SetActive(false);
            itemCostTxt.gameObject.SetActive(false);
            stateTxt.gameObject.SetActive(true);
            stateTxt.text = "Equipped";
        }
        else if (currentState == ItemState.Equipped)
        {
            currentState = ItemState.Equipped;
            coinImg.gameObject.SetActive(false);
            itemCostTxt.gameObject.SetActive(false);
            stateTxt.gameObject.SetActive(true);
            stateTxt.text = "Equipped";
        }
    }
}
