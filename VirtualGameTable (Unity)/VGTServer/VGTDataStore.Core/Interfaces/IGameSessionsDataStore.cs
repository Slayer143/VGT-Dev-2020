using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VGTDataStore.Core.Models;
using VGTDataStore.Core.Models.Enums;

namespace VGTDataStore.Core.Interfaces
{
    public interface IGameSessionsDataStore
    {
        IDictionary<Guid, PokerGameSession> GameSessions { get; }

        IDictionary<Guid, GameSessionUsers> GameSessionUsers { get; }

        void GetSessions();

        Task<Dictionary<Guid, PokerGameSession>> GetSessionsAsync();

        void GetSessionUsers();

        Task<Dictionary<Guid, GameSessionUsers>> GetSessionUsersAsync();

        Task<Guid> AddSession(PokerGameSession session);

        Task<Guid> AddGameSessionUser(Guid sessionId, Guid userId, int place, UserRolesForPoker role, int startingChips);

        void ChangeStatus(Guid sessionId, Guid userId, PokerPlayerStatus status);

        void ChangeChips(Guid sessionId, Guid userId, int chips);

        void ChangeBet(Guid sessionId, Guid userId, int bet);

        void ChangeStatus(Guid sessionId, GameSessionStatus status);

        void PushResults(Guid userId, Guid sessionId, int playerChips, int gain = 0);

        void DeleteFromSession(Guid sessionId, Guid userId);

        Task<List<GameSessionUsers>> GetUsersInGame(Guid gameId);

        void ChangeRole(Guid UserId, Guid SessionId, UserRolesForPoker UserRole);

        void ChangeChipsForGame(Guid SessionId, Guid UserId, int Bet, int ChipsForGame);
    }
}
