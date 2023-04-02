using Plus.Database;
using Plus.Database.Interfaces;
using Plus.Roleplay.Users;
using System.Data;

namespace Plus.HabboHotel.Users.UserData
{
    internal class LoadRPUserDataLoginTask : IUserDataLoadingTask
    {
        private readonly IDatabase _database;

        public LoadRPUserDataLoginTask(IDatabase database)
        {
            _database = database;
        }

        public async Task Load(Habbo habbo)
        {
            DataRow dataRow;
            using (var dbClient = _database.GetQueryReactor())
            {
                dataRow = await GetRPUserData(dbClient, habbo.Id);
                if (dataRow == null)
                {
                    await InsertRPUserData(dbClient, habbo.Id);
                    dataRow = await GetRPUserData(dbClient, habbo.Id);
                }
            }

            if (dataRow != null)
            {
                habbo.RPHabboData = new RPHabboData(
                    Convert.ToInt32(dataRow["user_id"]),
                    Convert.ToInt32(dataRow["level"]),
                    Convert.ToInt32(dataRow["health"]),
                    Convert.ToInt32(dataRow["max_health"]),
                    Convert.ToInt32(dataRow["stamina"]),
                    Convert.ToInt32(dataRow["max_stamina"]),
                    Convert.ToInt32(dataRow["aggression"]));
            }
        }

        private async Task<DataRow> GetRPUserData(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery(
                "SELECT `user_id`, `level`, `health`, `max_health`, `stamina`, `max_stamina`, `aggression` FROM `rp_user_data` WHERE `user_id` = @user_id LIMIT 1");
            dbClient.AddParameter("user_id", userId);
            return dbClient.GetRow();
        }

        private async Task InsertRPUserData(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("INSERT INTO `rp_user_data` (`user_id`) VALUES (@user_id)");
            dbClient.AddParameter("user_id", userId);
            dbClient.RunQuery();
        }
    }
}
