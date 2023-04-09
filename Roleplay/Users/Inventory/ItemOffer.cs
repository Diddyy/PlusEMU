namespace Plus.Roleplay.Users.Inventory;
public class ItemOffer
{
    public int OfferId { get; set; }
    public int SenderId { get; set; }
    public int TargetId { get; set; }
    public int ItemId { get; set; }
    public int Cost { get; set; }

    public ItemOffer(int offerId, int senderId, int targetId, int itemId, int cost)
    {
        OfferId = offerId;
        SenderId = senderId;
        TargetId = targetId;
        ItemId = itemId;
        Cost = cost;
    }
}
