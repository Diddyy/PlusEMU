using Plus.Database.Interfaces;
using Plus.Roleplay.Users.Inventory;
using System.Collections.Concurrent;
using System.Data;

namespace Plus.Roleplay.Users.Items;

public static class RPItemLoader
{
    private static readonly ConcurrentDictionary<int, List<InventoryItem>> UserItemsCache = new ConcurrentDictionary<int, List<InventoryItem>>();
    private static readonly ConcurrentDictionary<int, RPItemData> ItemDataCache = new ConcurrentDictionary<int, RPItemData>();

    public static List<InventoryItem> GetItemsForUser(int userId)
    {
        if (UserItemsCache.TryGetValue(userId, out List<InventoryItem> itemList))
        {
            return itemList;
        }

        itemList = new List<InventoryItem>();
        using (IQueryAdapter dbClient = PlusEnvironment.DatabaseManager.GetQueryReactor())
        {
            dbClient.SetQuery("SELECT * FROM `rp_user_items` WHERE `user_id` = @uid;");
            dbClient.AddParameter("uid", userId);

            DataTable items = dbClient.GetTable();

            if (items != null)
            {
                foreach (DataRow row in items.Rows)
                {
                    itemList.Add(new InventoryItem(
                        Convert.ToInt32(row["id"]),
                        Convert.ToInt32(row["item_id"]),
                        Convert.ToInt32(row["item_slot"]),
                        Convert.ToInt32(row["quantity"])
                    )
                    {
                        Health = row["health"] != DBNull.Value ? Convert.ToInt32(row["health"]) : 0,
                        RemainingUses = row["remaining_uses"] != DBNull.Value ? Convert.ToInt32(row["remaining_uses"]) : 0
                    });
                }
            }
        }

        UserItemsCache.TryAdd(userId, itemList);

        return itemList;
    }

    public static void DeleteAllInventoryItemsForUser(int userId)
    {
        using (IQueryAdapter dbClient = PlusEnvironment.DatabaseManager.GetQueryReactor())
        {
            dbClient.SetQuery("DELETE FROM `rp_user_items` WHERE `user_id` = @uid;");
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }

        UserItemsCache.TryRemove(userId, out _);
    }

    public static string GetItemNameById(int itemId)
    {
        if (ItemDataCache.TryGetValue(itemId, out RPItemData itemData))
        {
            return itemData.ItemName;
        }

        using (IQueryAdapter dbClient = PlusEnvironment.DatabaseManager.GetQueryReactor())
        {
            dbClient.SetQuery("SELECT `id`, `item_name`, `allow_inventory_stack`, `is_equippable`, `equippable_type`, `handitem_id`, `sell_price` FROM `rp_items` WHERE `id` = @id;");
            dbClient.AddParameter("id", itemId);

            var row = dbClient.GetRow();
            if (row != null)
            {
                itemData = new RPItemData(
                    Convert.ToInt32(row["id"]),
                    Convert.ToString(row["item_name"]),
                    Convert.ToInt32(row["allow_inventory_stack"]),
                    Convert.ToInt32(row["is_equippable"]),
                    Convert.ToString(row["equippable_type"]),
                    Convert.ToInt32(row["handitem_id"] != DBNull.Value ? row["handitem_id"] : null),
                    Convert.ToInt32(row["sell_price"] != DBNull.Value ? row["sell_price"] : null)
                );
            }
        }

        if (itemData != null)
        {
            ItemDataCache.TryAdd(itemId, itemData);
            return itemData.ItemName;
        }

        return null;
    }

    public static RPItemData GetItemDataByHandItemId(int handItemId)
    {
        if (ItemDataCache.TryGetValue(handItemId, out RPItemData itemData))
        {
            return itemData;
        }

        using (IQueryAdapter dbClient = PlusEnvironment.DatabaseManager.GetQueryReactor())
        {
            dbClient.SetQuery("SELECT `id`, `item_name`, `allow_inventory_stack`, `is_equippable`, `equippable_type`, `sell_price` FROM `rp_items` WHERE `handitem_id` = @handItemId;");
            dbClient.AddParameter("handItemId", handItemId);

            var row = dbClient.GetRow();
            if (row != null)
            {
                itemData = new RPItemData(
                    Convert.ToInt32(row["id"]),
                    Convert.ToString(row["item_name"]),
                    Convert.ToInt32(row["allow_inventory_stack"]),
                    Convert.ToInt32(row["is_equippable"]),
                    Convert.ToString(row["equippable_type"]),
                    handItemId,
                    Convert.ToInt32(row["sell_price"] != DBNull.Value ? row["sell_price"] : null)
                );
            }
        }

        if (itemData != null)
        {
            ItemDataCache.TryAdd(handItemId, itemData);
        }

        return itemData;
    }
}
