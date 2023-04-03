using Plus.Communication.Packets.Outgoing.RP.Users;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.RP.Users;

internal class GetRPUserCurrentInventoryEvent : IPacketEvent
{
    public Task Parse(GameClient session, IIncomingPacket packet)
    {
        var userId = packet.ReadInt();
        var habbo = PlusEnvironment.GetHabboById(userId);
        if (habbo == null || userId != session.GetHabbo().Id)
            return Task.CompletedTask;
        session.Send(new SendRPUserInventoryComposer(habbo));
        return Task.CompletedTask;
    }
}