using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VGTDataStore.Core;
using VGTDataStore.Core.Interfaces;
using VGTDataStore.Core.Models;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.InMemory
{
    public class VGTGameSessionsDataStore : IGameSessionsDataStore
    {
        private string _connectionString = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VGTBD;Integrated Security=true;";

        public IDictionary<Guid, PokerGameSession> GameSessions { get; set; }

        public IDictionary<Guid, GameSessionUsers> GameSessionUsers { get; set; }

        public VGTGameSessionsDataStore()
        {
            GetSessions();

            GetSessionUsers();
        }

        public async void GetSessions()
        {
            GameSessions = await GetSessionsAsync();
        }

        public async Task<Dictionary<Guid, PokerGameSession>> GetSessionsAsync()
        {
            var sessions = new Dictionary<Guid, PokerGameSession>();

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var res = await connection.QueryAsync<PokerGameSession>("SELECT * FROM GameSessions");

                if (res == null)
                    return sessions;

                foreach (var session in res)
                {
                    sessions.Add(session.SessionId, session);
                }
            }

            return sessions;
        }

        public async void GetSessionUsers()
        {
            GameSessionUsers = await GetSessionUsersAsync();
        }

        public async Task<Dictionary<Guid, GameSessionUsers>> GetSessionUsersAsync()
        {
            var sessions = new Dictionary<Guid, GameSessionUsers>();

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var res = await connection.QueryAsync<GameSessionUsers>("SELECT * FROM GameSessionUsers");

                if (res == null)
                    return sessions;

                foreach (var session in res)
                {
                    sessions.Add(Guid.NewGuid(), session);
                }
            }

            return sessions;
        }

        public async Task<Guid> AddSession(PokerGameSession session)
        {
            var newSession = session;

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = "INSERT INTO GameSessions(SessionId, StartDate, GameId, SessionStatusId, RoomSize, RoomName, RoomPassword) VALUES(@SessionId, @StartDate, @GameId, @SessionStatusId, @RoomSize, @RoomName, @RoomPassword)";

                await connection.ExecuteAsync(query, new { newSession.SessionId, newSession.StartDate, newSession.GameId, newSession.SessionStatusId, newSession.RoomSize, newSession.RoomName, newSession.RoomPassword });
            }

            GameSessions.Add(newSession.SessionId, newSession);

            return newSession.SessionId;
        }

        public async Task<List<GameSessionUsers>> GetUsersInGame(Guid gameId)
        {
            return await Task.Run(() => GameSessionUsers.Values.Where(x => x.SessionId == gameId).ToList());
        }

        public async Task<Guid> AddGameSessionUser(
            Guid sessionId,
            Guid userId,
            int place,
            UserRolesForPoker role,
            int startingChips)
        {
            var newSessionUser = new GameSessionUsers(sessionId, userId, place, role, startingChips, 0, 0);

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = "INSERT INTO GameSessionUsers" +
                    "(UserId, SessionId, PlayerStatusId, SeatPlace, UserRoleId, StartingChips, NowChips, Bet) VALUES" +
                    "(@UserId, @SessionId, @PlayerStatusId, @SeatPlace, @UserRoleId, @StartingChips, @NowChips, @Bet)";
                await connection.ExecuteAsync(query, newSessionUser);
            }

            var rawId = Guid.NewGuid();

            GameSessionUsers.Add(rawId, newSessionUser);

            CheckForStaffed(sessionId);

            return rawId;
        }

        private void CheckForStaffed(Guid sessionId)
        {
            if (GameSessionUsers.Where(x => x.Value.SessionId == sessionId).Count() ==
                GameSessions.FirstOrDefault(x => x.Key == sessionId).Value.RoomSize)
                ChangeStatus(sessionId, GameSessionStatus.Staffed);
            else
                ChangeStatus(sessionId, GameSessionStatus.NotStaffed);
        }

        public async void ChangeRole(Guid UserId, Guid SessionId, UserRolesForPoker UserRoleId)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE GameSessionUsers set UserRoleId = @UserRoleId WHERE SessionId = @SessionId AND UserId = @UserId";
                await connection.ExecuteAsync(query, new { UserRoleId, SessionId, UserId });
            }

            GameSessionUsers.FirstOrDefault(x => x.Value.UserId == UserId && x.Value.SessionId == SessionId).Value.UserRoleId = UserRoleId;
        }

        public async void ChangeStatus(
            Guid SessionId,
            Guid UserId,
            PokerPlayerStatus PlayerStatusId)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE GameSessionUsers set PlayerStatusId = @PlayerStatusId WHERE SessionId = @SessionId AND UserId = @UserId";
                await connection.ExecuteAsync(query, new { PlayerStatusId, SessionId, UserId, });
            }

            GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == SessionId && x.Value.UserId == UserId).Value.PlayerStatusId = PlayerStatusId;
        }

        public async void ChangeChips(
            Guid SessionId,
            Guid UserId,
            int NowChips)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE GameSessionUsers set NowChips = @NowChips WHERE SessionId = @SessionId AND UserId = @UserId";
                await connection.ExecuteAsync(query, new { NowChips, SessionId, UserId, });
            }

            GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == SessionId && x.Value.UserId == UserId).Value.NowChips = NowChips;
        }

        public async void ChangeBet(
            Guid SessionId,
            Guid UserId,
            int Bet)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE GameSessionUsers set Bet = @Bet WHERE SessionId = @SessionId AND UserId = @UserId";
                await connection.ExecuteAsync(query, new { Bet, SessionId, UserId, });
            }

            GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == SessionId && x.Value.UserId == UserId).Value.Bet = Bet;
        }

        public async void ChangeStatus(Guid sessionId, GameSessionStatus status)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE GameSessions set SessionStatusId = @status WHERE SessionId = @SessionId";
                await connection.ExecuteAsync(query, new { status, sessionId });
            }

            GameSessions.FirstOrDefault(x => x.Key == sessionId).Value.SessionStatusId = status;
        }

        public async void DeleteFromSession(Guid sessionId, Guid userId)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var query = @"DELETE GameSessionUsers WHERE SessionId = @SessionId AND UserId = @UserId";
                await connection.ExecuteAsync(query, new { sessionId, userId });
            }

            GameSessionUsers.Remove(GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == sessionId && x.Value.UserId == userId));

            CheckForStaffed(sessionId);
        }

        public void PushResults(Guid userId, Guid sessionId, int playerChips, int gain = 0)
        {
            throw new NotImplementedException();
        }
    }
}
