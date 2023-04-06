using Plus.Roleplay.Users.Inventory;
using System.Data;

namespace Plus.Roleplay.Users.Items;

public static class RPItemLoader
{
    public static List<InventoryItem> GetItemsForUser(int userId)
    {
        var itemList = new List<InventoryItem>();

        using var dbClient = PlusEnvironment.DatabaseManager.GetQueryReactor();
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

        return itemList;
    }

    public static void DeleteAllInventoryItemsForUser(int userId)
    {
        using var dbClient = PlusEnvironment.DatabaseManager.GetQueryReactor();
        dbClient.SetQuery("DELETE FROM `rp_user_items` WHERE `user_id` = @uid;");
        dbClient.AddParameter("uid", userId);
        dbClient.RunQuery();
    }
}