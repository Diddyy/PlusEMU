using Dapper;
using Plus.Database;
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

    public async Task UpdateItemPosition(int itemId, int position)
    {
        Console.WriteLine($"Updating item position: itemId={itemId}, position={position}");
        var item = GetItem(itemId);
        if (item == null) return;

        item.ItemSlot = position;

        using var connection = _database.Connection();
        var query = "UPDATE RP_user_items SET item_slot = @ItemSlot WHERE Id = @Id";
        await connection.ExecuteAsync(query, new { ItemSlot = position, Id = itemId });
    }

    public bool HasItem(int itemId) => _items.ContainsKey(itemId);

    public bool RemoveItem(int itemId) => _items.TryRemove(itemId, out _);
}
