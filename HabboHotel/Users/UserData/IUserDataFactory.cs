namespace Plus.HabboHotel.Users.UserData;

public interface IUserDataFactory
{
    Task<Habbo?> Create(int userId);
    Task<string> GetUsernameForHabboById(int userId);
    Task<bool> HabboExists(int userId);
    Task<bool> HabboExists(string username);
    Task<Habbo?> GetUserDataByIdAsync(int userId);
}