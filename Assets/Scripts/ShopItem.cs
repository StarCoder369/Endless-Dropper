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
    public PlayerMovement player;

    [Header("Base Values")]
    public string itemName;
    public int itemCost;
    public Sprite itemIcon;
    public bool slowItem;
    public bool landIndicatorItem;

    [Header("Item References")]
    public TMP_Text itemNameTxt;
    public TMP_Text itemCostTxt;
    public Image itemImg;
    public Image coinImg;
    public TMP_Text stateTxt;

    [Header("Internal Values")]
    public ItemState currentState = ItemState.Locked;

    private void OnEnable()
    {
        itemNameTxt.text = itemName;
        itemCostTxt.text = itemCost.ToString();
        itemImg.sprite = itemIcon;

        LoadState();
        UpdateUI();
    }

    private void LoadState()
    {
        currentState = ItemState.Locked;

        if (slowItem)
        {
            if (SaveManager.Instance.GetSlowToolUnlocked())
            {
                currentState = SaveManager.Instance.GetEquippedTool() == EquippedTool.Slow ? ItemState.Equipped : ItemState.NotEquipped;
            }
        }

        if (landIndicatorItem)
        {
            if (SaveManager.Instance.GetIndicatorToolUnlocked())
            {
                currentState = SaveManager.Instance.GetEquippedTool() == EquippedTool.Indicator ? ItemState.Equipped : ItemState.NotEquipped;
            }
        }

        if (currentState == ItemState.Equipped)
        {
            EquipTool();
        }
    }

    public void TryClick()
    {
        if (currentState == ItemState.Locked)
        {
            if (GameManager.Instance.coins >= itemCost)
            {
                GameManager.Instance.coins -= itemCost;
                currentState = ItemState.NotEquipped;

                SaveUnlock();
            }
        }
        else if (currentState == ItemState.NotEquipped)
        {
            shopManager.SetAllNotEquipped(this);

            currentState = ItemState.Equipped;

            EquipTool();

            UpdateSaves();
        }
        else if (currentState == ItemState.Equipped)
        {
            EquipTool();
        }

        UpdateUI();
    }

    private void EquipTool()
    {
        if (slowItem)
        {
            player.currentAbility = PlayerMovement.Abilities.Slow;
        }

        if (landIndicatorItem)
        {
            player.currentAbility = PlayerMovement.Abilities.Indicator;
        }
    }

    private void SaveUnlock()
    {
        if (slowItem)
        {
            SaveManager.Instance.SetSlowToolUnlocked(true);
        }

        if (landIndicatorItem)
        {
            SaveManager.Instance.SetIndicatorToolUnlocked(true);
        }

        SaveManager.Instance.SetCoins(GameManager.Instance.coins);
    }

    public void UpdateUI()
    {
        if (currentState == ItemState.Locked)
        {
            coinImg.gameObject.SetActive(true);
            itemCostTxt.gameObject.SetActive(true);
            stateTxt.gameObject.SetActive(false);
        }
        else if (currentState == ItemState.NotEquipped)
        {
            coinImg.gameObject.SetActive(false);
            itemCostTxt.gameObject.SetActive(false);
            stateTxt.gameObject.SetActive(true);
            stateTxt.text = "Not Equipped";
        }
        else if (currentState == ItemState.Equipped)
        {
            coinImg.gameObject.SetActive(false);
            itemCostTxt.gameObject.SetActive(false);
            stateTxt.gameObject.SetActive(true);
            stateTxt.text = "Equipped";
        }
    }

    public void UpdateSaves()
    {
        if (slowItem)
        {
            SaveManager.Instance.SetSlowToolUnlocked(currentState != ItemState.Locked);

            if (currentState == ItemState.Equipped)
            {
                SaveManager.Instance.SetEquippedTool(EquippedTool.Slow);
            }
        }

        if (landIndicatorItem)
        {
            SaveManager.Instance.SetIndicatorToolUnlocked(currentState != ItemState.Locked);

            if (currentState == ItemState.Equipped)
            {
                SaveManager.Instance.SetEquippedTool(EquippedTool.Indicator);
            }
        }

        SaveManager.Instance.SetCoins(GameManager.Instance.coins);
    }
}