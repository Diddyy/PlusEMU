using Plus.Database;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.UserData;
using Plus.Roleplay.Users.Items;

namespace Plus.Roleplay.Users.Inventory;

internal class LoadRPUserInventoryTask : IUserDataLoadingTask
{
    private readonly IDatabase _database;

    public LoadRPUserInventoryTask(IDatabase database) => _database = database;

    public async Task Load(Habbo habbo)
    {
        var items = RPItemLoader.GetItemsForUser(habbo.Id);
        habbo.RPInventory = new RPInventoryComponent(_database, items);
    }
}