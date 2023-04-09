using Dapper;
using Plus.Communication.Packets.Outgoing.RP.Users;
using Plus.Database;
using Plus.HabboHotel.Users;
using System.Collections.Concurrent;

namespace Plus.Roleplay.Users.Inventory;

public class InventoryItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public int ItemSlot { get; set; }
    public int Quantity { get; set; }
    public int Health { get; set; }
    public int RemainingUses { get; set; }
    public InventoryItem(int id, int itemId, int itemSlot, int quantity)
    {
        Id = id;
        ItemId = itemId;
        ItemSlot = itemSlot;
        Quantity = quantity;
    }
}

public class RPInventoryComponent
{
    private readonly ConcurrentDictionary<int, InventoryItem> _items;

    private readonly IDatabase _database;
    public IReadOnlyDictionary<int, InventoryItem> Items => _items;

    public RPInventoryComponent(IDatabase database, IEnumerable<InventoryItem> inventoryItems)
    {
        _database = database;
        _items = new ConcurrentDictionary<int, InventoryItem>(inventoryItems.ToDictionary(i => i.Id));
    }

    public IEnumerable<InventoryItem> AllItems => _items.Values;

    public void ClearItems()
    {
        _items.Clear();
    }

    public InventoryItem? GetItem(int itemId)
    {
        if (_items.TryGetValue(itemId, out var item))
            return item;
        return null;
    }

    public bool AddItem(InventoryItem item)
    {
        return _items.TryAdd(item.Id, item);
    }
    public async Task<bool> AddNewItem(Habbo habbo, int itemId)
    {
        int freeSlot = FindFreeSlot(habbo);
        if (freeSlot == -1)
        {
            return false;
        }

        using var connection = _database.Connection();
        int newItemId = await connection.ExecuteScalarAsync<int>("INSERT INTO `RP_user_items` (`user_id`, `item_id`, `item_slot`, `quantity`) VALUES (@userId, @itemId, @itemSlot, @quantity); SELECT LAST_INSERT_ID();", new
        {
            userId = habbo.Id,
            itemId = itemId,
            itemSlot = freeSlot,
            quantity = 1
        });

        var newItem = new InventoryItem(newItemId, itemId, freeSlot, 1);
        habbo.RPInventory.AddItem(newItem);
        habbo.Client.Send(new SendRPUserInventoryComposer(habbo));
        return true;
    }

    public async Task UpdateItemPosition(int itemId, int position)
    {
        var item = GetItem(itemId);
        if (item == null) return;

        item.ItemSlot = position;

        using var connection = _database.Connection();
        var query = "UPDATE RP_user_items SET item_slot = @ItemSlot WHERE Id = @Id";
        await connection.ExecuteAsync(query, new { ItemSlot = position, Id = itemId });
    }

    public bool HasItem(int itemId) => _items.ContainsKey(itemId);

    public bool RemoveItem(int itemId) => _items.TryRemove(itemId, out _);

    private int FindFreeSlot(Habbo habbo)
    {
        for (int slot = 3; slot <= 10; slot++)
        {
            bool slotTaken = false;
            foreach (var item in habbo.RPInventory.AllItems)
            {
                if (item.ItemSlot == slot)
                {
                    slotTaken = true;
                    break;
                }
            }

            if (!slotTaken)
            {
                return slot;
            }
        }

        return -1; // No free slot found
    }
}
