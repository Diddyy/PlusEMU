using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.RP.Inventory;
public class ItemOfferComposer : IServerPacket
{
    private readonly int _offerId;
    private readonly int _senderId;
    private readonly string _senderUsername;
    private readonly int _itemId;
    private readonly int _itemPrice;


    public uint MessageId => ServerPacketHeader.ItemOfferComposer;

    public ItemOfferComposer(int offerId, Habbo sender, int itemId, int itemPrice)
    {
        _offerId = offerId;
        _senderId = sender.Id;
        _senderUsername = sender.Username;
        _itemId = itemId;
        _itemPrice = itemPrice;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInteger(_offerId);
        packet.WriteInteger(_senderId);
        packet.WriteString(_senderUsername);
        packet.WriteInteger(_itemId);
        packet.WriteInteger(_itemPrice);
    }
}