using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.RP.Corporations;

internal class SellCommand : ITargetChatCommand
{
    public string Key => "sell";
    public string PermissionRequired => "command_sell";

    public string Parameters => "%username%";

    public string Description => "Sell the item you're holding to a target user";
    public bool MustBeInSameRoom => true;

    public async Task Execute(GameClient session, Room room, Habbo target, string[] parameters)
    {
        if (target == session.GetHabbo())
        {
            session.SendWhisper("You cannot sell an item to yourself.");
            return;
        }

        var targetUser = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(target.Id);

        if (targetUser == null)
        {
            session.SendWhisper("An error occurred whilst finding that user, maybe they're not online or in this room.");
            return;
        }

        var thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);

        if (target.CurrentRoom!.Id == session.GetHabbo().CurrentRoom!.Id && Math.Abs(thisUser.X - targetUser.X) < 3 && Math.Abs(thisUser.Y - targetUser.Y) < 3)
        {
            if (thisUser.CarryItemId == 1013)
            {
                bool itemAdded = await target.RPInventory.AddNewItem(target, 30); // 30 is MedKit ID
                if (itemAdded)
                {
                    room.SendPacket(new ChatComposer(thisUser.VirtualId, $"*Offers {targetUser.GetUsername()} a medkit for 50 coins*", 0, thisUser.LastBubble));
                }
                else
                {
                    session.SendWhisper("Unable to offer a medkit as user has no free inventory slots.");
                }
            }
        }
        else
        {
            session.SendWhisper("That user is not close enough to you");
        }
    }
}