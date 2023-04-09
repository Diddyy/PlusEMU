using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.RP.Inventory;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;
using Plus.Roleplay.Users.Inventory;
using System.Collections.Concurrent;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.RP.Corporations;

internal class SellCommand : ITargetChatCommand
{
    public string Key => "sell";
    public string PermissionRequired => "command_sell";

    public string Parameters => "%username%";

    public string Description => "Sell the item you're holding to a target user";
    public bool MustBeInSameRoom => true;

    private readonly ConcurrentDictionary<int, ItemOffer> _itemOffers;

    public SellCommand()
    {
        _itemOffers = new ConcurrentDictionary<int, ItemOffer>();
    }

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
                int offerId = _itemOffers.Count + 1; // Generate a new offer ID
                int itemId = 30; // MedKit ID
                int cost = 50; // MedKit cost
                _itemOffers.TryAdd(offerId, new ItemOffer(offerId, thisUser.UserId, target.Id, itemId, cost));

                room.SendPacket(new ChatComposer(thisUser.VirtualId, $"*Offers {targetUser.GetUsername()} a {itemId} for {cost} coins", 0, thisUser.LastBubble));
                targetUser.GetClient().Send(new ItemOfferComposer(offerId, thisUser.GetClient().GetHabbo(), itemId, cost));
            }
        }
        else
        {
            session.SendWhisper("That user is not close enough to you");
        }
    }

    public async Task HandleItemOfferResponse(GameClient session, int offerId, bool accepted)
    {
        if (!_itemOffers.TryGetValue(offerId, out var offer))
        {
            session.SendWhisper("An error occurred while processing your response. The offer might have expired.");
            return;
        }

        _itemOffers.TryRemove(offerId, out _);

        var sender = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(offer.SenderId);
        var room = session.GetHabbo().CurrentRoom;
        var target = room.GetRoomUserManager().GetRoomUserByHabbo(PlusEnvironment.GetUsernameById(offer.TargetId));

        if (accepted)
        {
            int cost = offer.Cost;
            if (target.GetClient().GetHabbo().Credits >= cost)
            {
                bool itemAdded = await target.GetClient().GetHabbo().RPInventory.AddNewItem(target.GetClient().GetHabbo(), offer.ItemId);
                if (itemAdded)
                {
                    target.GetClient().GetHabbo().Credits -= cost;
                    target.GetClient().Send(new CreditBalanceComposer(target.GetClient().GetHabbo().Credits));

                    room.SendPacket(new ChatComposer(target.VirtualId, $"*Accepts {sender.GetUsername()}'s offer", 0, target.LastBubble));
                }
                else
                {
                    sender.GetClient().SendWhisper("Unable to offer a medkit as user has no free inventory slots.");
                    target.GetClient().SendWhisper("Unable to accept a medkit as you have no free inventory slots.");
                }
            }
            else
            {
                sender.GetClient().SendWhisper($"{target.GetUsername()} does not have enough credits to complete the transaction.");
                target.GetClient().SendWhisper("You do not have enough credits to complete the transaction.");
            }
        }
        else
        {
            room.SendPacket(new ChatComposer(target.VirtualId, $"*Declines {sender.GetUsername()}'s offer", 0, target.LastBubble));
        }
    }
}