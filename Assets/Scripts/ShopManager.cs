using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public ShopItem[] shopItems;

    public void SetAllNotEquipped(ShopItem shopItem)
    {
        foreach (ShopItem item in shopItems)
        {
            if (item.currentState == ShopItem.ItemState.Equipped)
            {
                item.currentState = ShopItem.ItemState.NotEquipped;
            }

            if (item == shopItem)
            {
                item.currentState = ShopItem.ItemState.Equipped;
            }
            item.UpdateUI();
            item.UpdateSaves();
        }
    }
}
