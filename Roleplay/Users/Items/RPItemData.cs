namespace Plus.Roleplay.Users.Items;
public class RPItemData
{
    public int Id { get; }
    public string ItemName { get; }
    public int AllowInventoryStack { get; }
    public int IsEquippable { get; }
    public string EquippableType { get; }
    public int HandItemId { get; }
    public int SellPrice { get; }

    public RPItemData(int id, string itemName, int allowInventoryStack, int isEquippable, string equippableType, int handItemId, int sellPrice)
    {
        Id = id;
        ItemName = itemName;
        AllowInventoryStack = allowInventoryStack;
        IsEquippable = isEquippable;
        EquippableType = equippableType;
        HandItemId = handItemId;
        SellPrice = sellPrice;
    }
}

