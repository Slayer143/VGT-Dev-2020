using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VGTDataStore.Core;

namespace VGTDataStore.InMemory
{
    public class VGTUserDataStore : IVGTUserDataStore
    {
        private string _connectionString = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VGTBD;Integrated Security=true;";

        public IDictionary<Guid, VGTUser> Users { get; }

        public VGTUserDataStore()
        {
            Users = GetUsers();
        }

        public Dictionary<Guid, VGTUser> GetUsers()
        {
            var users = new Dictionary<Guid, VGTUser>();

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var res = connection.Query<VGTUser>("SELECT * FROM Users").ToList();

                foreach (var user in res)
                {
                    users.Add(user.UserId, user);
                }
            }

            return users;
        }

        public Guid AddUser(VGTUserRestricted user)
        {
            var newUser = new VGTUser(user);

            newUser.Chips = 10000;

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = "INSERT INTO Users" +
                    "(UserId, Login, Password, Email, Chips, RoleId) VALUES" +
                    "(@UserId, @Login, @Password, @Email, @Chips, @RoleId)";
                connection.Execute(query, newUser);
            }

            Users.Add(newUser.UserId, newUser);

            return newUser.UserId;
        }

        public async void ChangeUserChips(Guid UserId, int Chips)
        {
            var NewChips = Users[UserId].Chips + Chips;

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE Users set Chips = @NewChips WHERE UserId = @UserId";
                
                await connection.ExecuteAsync(query, new { NewChips, UserId });
            }

            Users[UserId].Chips = NewChips;
        }
    } 
}
