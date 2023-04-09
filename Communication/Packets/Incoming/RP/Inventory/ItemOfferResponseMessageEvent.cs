using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.Chat.Commands.User.RP.Corporations;

namespace Plus.Communication.Packets.Incoming.Rooms.Inventory;

internal class ItemOfferResponseMessageEvent : IPacketEvent
{
    private readonly SellCommand _sellCommand;

    public ItemOfferResponseMessageEvent(SellCommand sellCommand)
    {
        _sellCommand = sellCommand;
    }

    public async Task Parse(GameClient session, IIncomingPacket packet)
    {
        int offerId = packet.ReadInt();
        bool accepted = packet.ReadBool();

        await _sellCommand.HandleItemOfferResponse(session, offerId, accepted);
    }
}