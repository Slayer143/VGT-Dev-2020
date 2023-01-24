using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account.Manage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using VGTDataStore.Core;
using VGTDataStore.Core.Interfaces;
using VGTDataStore.Core.Models;
using VGTDataStore.Core.Models.Enums;
using VGTServer.Answers;
using VGTServer.Models;

namespace VGTServer.Controllers
{
    [Route("/api/sessions/")]
    [ApiController]
    public class VGTSessionsController : ControllerBase
    {
        private IGameSessionsDataStore _gameSessionsDataStore;

        private IPlayingCardsDataStore _playingCardsDataStore;

        public VGTSessionsController(
            IGameSessionsDataStore gameSessionsDataStore,
            IPlayingCardsDataStore playingCardsDataStore)
        {
            _gameSessionsDataStore = gameSessionsDataStore;
            _playingCardsDataStore = playingCardsDataStore;
        }

        [HttpGet]
        public IActionResult GetGameSessions()
        {
            if (_gameSessionsDataStore.GameSessions != null)
                return Ok(_gameSessionsDataStore.GameSessions);

            return NotFound();
        }

        [HttpGet("status/{sessionId}")]
        public IActionResult GetSessionStatus(Guid sessionId)
        {
            if (!_gameSessionsDataStore.GameSessions.ContainsKey(sessionId))
                return BadRequest("No session with that ID");

            return Ok(_gameSessionsDataStore.GameSessions[sessionId].SessionStatusId);
        }

        [HttpGet("open")]
        public async Task<IActionResult> GetOpenGameSessions()
        {
            if (_gameSessionsDataStore.GameSessions != null)
            {
                var list = new List<PokerGameSession>();

                await Task.Run(() =>
                {
                    foreach (var item in _gameSessionsDataStore
                        .GameSessions
                        .Where(x => x.Value.RoomSize >
                        _gameSessionsDataStore
                        .GameSessionUsers
                        .Where(y => y.Value.SessionId == x.Value.SessionId).Count()
                        && x.Value.SessionStatusId == GameSessionStatus.NotStaffed))
                    {
                        list.Add(item.Value);
                    }
                });

                return Ok(list);
            }

            return NotFound();
        }

        [HttpGet("size/{id}")]
        public IActionResult GetSize(Guid id)
        {
            return Ok(_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value.RoomSize);
        }

        [HttpGet("places/free/{id}")]
        public async Task<IActionResult> GetFreePlaces(Guid id)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value == null)
                return BadRequest("No session");

            var size = _gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value.RoomSize;

            var arr = new int[size];

            await Task.Run(() =>
            {
                for (int i = 0; i < size; i++)
                {
                    if (_gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == id && x.Value.SeatPlace == i + 1).Value != null)
                        arr[i] = 1;
                    else
                        arr[i] = 0;
                }
            });

            return Ok(arr);
        }

        [HttpGet("isFilled/{id}")]
        public async Task<IActionResult> IsFilled(Guid id)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value == null)
                return BadRequest("No session");

            return Ok(await Task.Run(() =>
            {
                return _gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value.SessionStatusId == GameSessionStatus.Staffed;
            }));
        }

        [HttpGet("places/{id}")]
        public async Task<IActionResult> GetPlaces(Guid id)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value == null)
                return BadRequest("No session");

            return Ok(await _gameSessionsDataStore.GetUsersInGame(id));
        }

        [HttpGet("nowChips/{sessionId}&{userId}")]
        public IActionResult GetNowChips(Guid sessionId, Guid userId)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value == null)
                return BadRequest("No session");

            return Ok(_gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == sessionId && x.Value.UserId == userId).Value.NowChips);
        }

        [HttpGet("bet/{sessionId}&{userId}")]
        public IActionResult GetBet(Guid sessionId, Guid userId)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value == null)
                return BadRequest("No session");

            return Ok(_gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.SessionId == sessionId && x.Value.UserId == userId).Value.Bet);
        }

        [HttpGet("statuses/{sessionId}")]
        public async Task<IActionResult> CheckForReady(Guid sessionId)
        {
            bool areAllReady = true;

            await Task.Run(() =>
            {
                foreach (var user in _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId))
                {
                    if (user.PlayerStatusId != PokerPlayerStatus.ReadyForStart)
                        areAllReady = false;
                }
            });

            if (areAllReady)
                return Ok("All players are ready");
            else
                return Ok("Not all players are ready");
        }

        [HttpGet("statuses/inGame/{SessionId}")]
        public async Task<IActionResult> CheckStatuses(Guid sessionId)
        {
            if (!_gameSessionsDataStore.GameSessions.ContainsKey(sessionId))
                return BadRequest();

            var statuses = new Dictionary<Guid, PokerPlayerStatus>();

            await Task.Run(() =>
            {
                foreach (var user in _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId))
                {
                    statuses.Add(user.UserId, user.PlayerStatusId);
                }
            });

            return Ok(statuses);
        }
        
        private void OneWinner(Guid userId, Guid sessionId)
        {
            Guid winner = userId;

            var bank = _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId).Sum(x => x.NowChips);

            var request = WebRequest.Create($"http://localhost:5000/api/users/changeChips/{winner}&{bank}");
            string answer;
            request.Method = "PATCH";

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    answer = reader.ReadToEnd();
                }
            }

            foreach (var user in _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId && x.UserId != winner))
            {
                var userNowChips = user.NowChips * (-1);

                request = WebRequest.Create($"http://localhost:5000/api/users/changeChips/{user.UserId}&{userNowChips}");

                request.Method = "PATCH";

                response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        answer = reader.ReadToEnd();
                    }
                }
            }
        }

        private void TwoWinners(List<Guid> userId, Guid sessionId)
        {
            var bank = _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId).Sum(x => x.NowChips);

            var half = bank / 2;

            foreach (var user in userId)
            {
                var request = WebRequest.Create($"http://localhost:5000/api/users/changeChips/{user}&{half}");
                string answer;
                request.Method = "PATCH";

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        answer = reader.ReadToEnd();
                    }
                }
            }

            foreach (var user in _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId && x.UserId != userId[0] && x.UserId != userId[1]))
            {
                var userNowChips = user.NowChips * (-1);

                var request = WebRequest.Create($"http://localhost:5000/api/users/changeChips/{user.UserId}&{userNowChips}");
                string answer;
                request.Method = "PATCH";

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        answer = reader.ReadToEnd();
                    }
                }
            }
        }

        [HttpGet("results/{sessionId}")]
        public IActionResult GetResults(Guid sessionId)
        {
            if (!_playingCardsDataStore.PlayerResults.ContainsKey(sessionId))
                return NotFound("No results for that session");

            if (_playingCardsDataStore.PlayerResults.FirstOrDefault(x => x.Key == sessionId).Value == null)
                return NotFound("No results for that game");

            var winnersCount = _playingCardsDataStore
                .PlayerResults[sessionId]
                .Where(x => x.Combination == _playingCardsDataStore.PlayerResults[sessionId].Max(y => y.Combination))
                .ToList();

            var winners = new Guid[2];

            if (winnersCount.Count == 1)
            {
                OneWinner(winnersCount.First().UserId, sessionId);
            }
            else
            {
                var strongest = winnersCount.Where(x => x.Value == winnersCount.Max(y => y.Value)).ToList();

                if (strongest.Count == 1)
                    OneWinner(winnersCount.First().UserId, sessionId);
                else
                {
                    var usersId = new List<Guid>() { strongest[0].UserId, strongest[1].UserId };

                    TwoWinners(usersId, sessionId);
                }
            }

            foreach (var user in _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId))
            {
                _gameSessionsDataStore.ChangeChips(user.SessionId, user.UserId, 0);

                var request = WebRequest.Create($"http://localhost:5000/api/users/info/chips/{user.UserId}");
                string answer;

                var response = request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        answer = reader.ReadToEnd();
                    }
                }

                var chipsForGame = JsonConvert.DeserializeObject<int>(answer);

                _gameSessionsDataStore.ChangeChipsForGame(user.SessionId, user.UserId, 0, chipsForGame);
            }

            var result = _playingCardsDataStore.PlayerResults.FirstOrDefault(x => x.Key == sessionId).Value;

            _playingCardsDataStore.PlayerResults.Remove(sessionId);

            return Ok(result);
        }

        [HttpPost("prepareCards/{roleId}")]
        public IActionResult PrepareCards(int roleId, [FromBody] List<GameSessionUsers> lobbyPlayers)
        {
            if (!_gameSessionsDataStore.GameSessions.ContainsKey(lobbyPlayers[0].SessionId))
                return BadRequest("No session with that ID");

            if (roleId == 1)
            {
                _playingCardsDataStore.FormCards(lobbyPlayers);

                return Ok(_playingCardsDataStore.PlayerCards);
            }
            else if (_playingCardsDataStore.PlayerCards != null)
                return Ok(_playingCardsDataStore.PlayerCards);
            else
                return BadRequest("Cards are not prepared");
        }

        [HttpPost("register/{roomSize}&{roomName}&{roomPassword}")]
        public async Task<IActionResult> RegisterGameSession(
            string roomSize,
            string roomName,
            string roomPassword,
            [FromBody] PokerGamePlayer player)
        {
            if (player == null)
                return BadRequest();

            var session = new PokerGameSession(Convert.ToInt32(roomSize) + 1, roomName, roomPassword, player);
            var id = Guid.Empty;

            await Task.Run(async () =>
            {
                id = await _gameSessionsDataStore.AddSession(session);

                await _gameSessionsDataStore.AddGameSessionUser(id, player.PlayerId, player.SeatPlace, player.UserRole, player.ChipsForGame);
            });

            return Ok(new Answer(id, "Session created, first user added"));
        }

        [HttpPost("{sessionId}")]
        public IActionResult AddUserToTheGameSession(Guid sessionId, [FromBody] PokerGamePlayer player)
        {
            if (player == null)
                return BadRequest("Not a player");

            if (!_gameSessionsDataStore.GameSessions.ContainsKey(sessionId))
                return BadRequest("No session");

            if (_gameSessionsDataStore.GameSessionUsers.Values.FirstOrDefault(x => x.UserId == player.PlayerId && x.SessionId == sessionId) != null)
                return BadRequest(new Answer(player.PlayerId, "Is already in the game"));

            var session = _gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value;

            var sessionUsers = _gameSessionsDataStore.GameSessionUsers.Where(x => x.Value.SessionId == sessionId).Count();

            if (session.RoomSize == sessionUsers)
                return BadRequest(new Answer(player.PlayerId, "Cannot be added cause of room size"));

            var rawId = _gameSessionsDataStore.AddGameSessionUser(sessionId, player.PlayerId, player.SeatPlace, player.UserRole, player.ChipsForGame);

            return Ok(new PokerGameSessionAnswer(player, sessionId, "Player added to the poker game session"));
        }

        [HttpPatch("SessionId={sessionId}&UserId={userId}&StatusCode={status}")]
        public IActionResult ChangeUsersStatus(Guid sessionId, Guid userId, PokerPlayerStatus status)
        {
            _gameSessionsDataStore.ChangeStatus(sessionId, userId, status);

            var current = _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId);

            if (status == PokerPlayerStatus.Check)
            {
                if (current
                .Where(x => x.PlayerStatusId == PokerPlayerStatus.Check)
                .Count() == current
                .Where(x => x.PlayerStatusId != PokerPlayerStatus.Fall && x.UserRoleId != UserRolesForPoker.Stickman)
                .Count())
                    return Ok(EndRound(sessionId));
                else
                    return Ok(SetNextPlayer(sessionId, userId));
            }

            if (current.Max(x => x.Bet) != 0
                && current
                .Where(x => x.Bet == current.Max(y => y.Bet) && x.PlayerStatusId != PokerPlayerStatus.Fall)
                .Count() == current
                .Where(x => x.PlayerStatusId != PokerPlayerStatus.Fall && x.UserRoleId != UserRolesForPoker.Stickman)
                .Count())
                return Ok(EndRound(sessionId));

            if (status == PokerPlayerStatus.WaitingForMove)
                return Ok(SetNextPlayer(sessionId, userId));

            if (current.Where(x => x.PlayerStatusId == PokerPlayerStatus.Fall).Count() != 0
                && current.Where(x => x.PlayerStatusId == PokerPlayerStatus.Fall).Count() == current.Count() - 2)
                return Ok(EndGame(sessionId));

            return Ok(status);
        }

        [HttpPatch("setFirst/{sessionId}&{stickmanPos}")]
        public async Task<IActionResult> SetFirstPlayer(Guid sessionId, int stickmanPos)
        {
            var positions = _gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value.RoomSize;

            int playerPos;

            if (stickmanPos == positions)
                playerPos = 1;
            else
                playerPos = stickmanPos + 1;

            await Task.Run(() => _gameSessionsDataStore.ChangeStatus(
                sessionId,
                _gameSessionsDataStore
                .GameSessionUsers
                .FirstOrDefault(x => x.Value.SeatPlace == playerPos)
                .Value
                .UserId,
                PokerPlayerStatus.MakesMove));

            return Ok("Changed");
        }

        [HttpPatch("setNext/{sessionId}&{userId}")]
        public async Task<IActionResult> SetNextPlayer(Guid sessionId, Guid userId)
        {
            var positions = _gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value.RoomSize;

            int playerPos = _gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.UserId == userId).Value.SeatPlace;

            int nextPlayerPos;

            if (playerPos == positions)
                nextPlayerPos = 1;
            else
                nextPlayerPos = playerPos + 1;

            while (_gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.SeatPlace == nextPlayerPos).Value.UserRoleId == UserRolesForPoker.Stickman
                || _gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.SeatPlace == nextPlayerPos).Value.PlayerStatusId == PokerPlayerStatus.Fall)
            {
                if (nextPlayerPos == positions)
                    nextPlayerPos = 1;
                else
                    nextPlayerPos++;
            }

            await Task.Run(() =>
            {
                _gameSessionsDataStore.ChangeStatus(
                    sessionId,
                    _gameSessionsDataStore
                    .GameSessionUsers
                    .FirstOrDefault(x => x.Value.SeatPlace == nextPlayerPos)
                    .Value
                    .UserId,
                    PokerPlayerStatus.MakesMove);
            });


            return Ok("Changed");
        }

        [HttpPatch("changeRole/{userId}&{sessionId}&{roleId}")]
        public async Task<IActionResult> ChangeUserRole(Guid userId, Guid sessionid, UserRolesForPoker roleId)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionid).Value == null)
                return BadRequest("No session with that ID");

            if (_gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.UserId == userId).Value == null)
                return BadRequest("No user with tath ID");

            await Task.Run(() => _gameSessionsDataStore.ChangeRole(userId, sessionid, roleId));

            return Ok("Changed");
        }

        [HttpPatch("SessionId={sessionId}&UserId={userId}&Chips={chips}")]
        public async Task<IActionResult> ChangeUserNowChips(Guid sessionId, Guid userId, int chips)
        {
            if (sessionId == Guid.Empty
                || userId == Guid.Empty)
                return BadRequest();

            if (!_gameSessionsDataStore.GameSessions.ContainsKey(sessionId))
                return BadRequest();

            await Task.Run(() => _gameSessionsDataStore.ChangeChips(sessionId, userId, chips));

            var minusChips = chips * (-1);

            var request = WebRequest.Create($"http://localhost:5000/api/users/changeChips/{userId}&{minusChips}");
            string answer;
            request.Method = "PATCH";

            var response = request.GetResponse();

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    answer = reader.ReadToEnd();
                }
            }

            return Ok(ChangeUsersStatus(sessionId, userId, PokerPlayerStatus.WaitingForMove));
        }

        [HttpPatch("SessionId={sessionId}&UserId={userId}&Bet={bet}")]
        public async Task<IActionResult> ChangeUserBet(Guid sessionId, Guid userId, int bet)
        {
            if (sessionId == Guid.Empty
                || userId == Guid.Empty)
                return BadRequest();

            if (!_gameSessionsDataStore.GameSessions.ContainsKey(sessionId))
                return BadRequest();

            await Task.Run(() => _gameSessionsDataStore.ChangeBet(sessionId, userId, bet));

            return Ok();
        }

        [HttpPatch("start/{id}")]
        public IActionResult StartGame(Guid id)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value == null)
                return BadRequest("No session with that ID");

            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == id).Value.SessionStatusId == GameSessionStatus.NotStaffed)
                return BadRequest("Can not start because session is not staffed");

            _gameSessionsDataStore.ChangeStatus(id, GameSessionStatus.BetRound);
            return Ok("Started");
        }

        [HttpPatch("endRound/{sessionId}")]
        public async Task<IActionResult> EndRound(Guid sessionId)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value == null)
                return BadRequest("No session with that ID");

            var status = _gameSessionsDataStore.GameSessions[sessionId].SessionStatusId;

            await Task.Run(() =>
            {
                switch (status)
                {
                    case GameSessionStatus.BetRound:
                        _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.FirstRound);
                        break;
                    case GameSessionStatus.FirstRound:
                        _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.SecondRound);
                        break;
                    case GameSessionStatus.SecondRound:
                        _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.ThirdRound);
                        break;
                    case GameSessionStatus.ThirdRound:
                        _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.Finished);
                        break;
                }
            });

            var startingPlace = 0;

            foreach (var user in _gameSessionsDataStore.GameSessionUsers.Values.Where(x => x.SessionId == sessionId).ToList())
            {
                if (user.UserRoleId != UserRolesForPoker.Stickman)
                {
                    _gameSessionsDataStore.ChangeBet(user.SessionId, user.UserId, user.Bet * (-1));
                    _gameSessionsDataStore.ChangeStatus(user.SessionId, user.UserId, PokerPlayerStatus.WaitingForMove);
                }
                else
                {
                    if (user.SeatPlace == _gameSessionsDataStore.GameSessions[sessionId].RoomSize)
                        startingPlace = 1;
                    else
                        startingPlace = user.SeatPlace + 1;
                }
            }

            if (_gameSessionsDataStore.GameSessions[sessionId].SessionStatusId == GameSessionStatus.Finished)
                return Ok("Game is finished");

            _gameSessionsDataStore.ChangeStatus(sessionId, _gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.SeatPlace == startingPlace).Value.UserId, PokerPlayerStatus.MakesMove);
            return Ok("New round");
        }

        [HttpDelete("leave/{userId}&{sessionId}")]
        public async Task<IActionResult> LeaveFromLobby(Guid userId, Guid sessionId)
        {
            if (_gameSessionsDataStore.GameSessions.FirstOrDefault(x => x.Key == sessionId).Value == null
                || _gameSessionsDataStore.GameSessionUsers.FirstOrDefault(x => x.Value.UserId == userId).Value == null)
                return BadRequest();

            if (_gameSessionsDataStore.GameSessions[sessionId].SessionStatusId != GameSessionStatus.NotStaffed
                && _gameSessionsDataStore.GameSessions[sessionId].SessionStatusId != GameSessionStatus.Staffed)
            {
                _gameSessionsDataStore.ChangeBet(
                sessionId,
                _gameSessionsDataStore
                .GameSessionUsers
                .Values.First(x => x.SessionId == sessionId && x.UserRoleId == UserRolesForPoker.Stickman).UserId,
                _gameSessionsDataStore.GameSessionUsers.Values.First(x => x.UserId == userId && x.SessionId == sessionId).NowChips);
            }
            
            await Task.Run(() => _gameSessionsDataStore.DeleteFromSession(sessionId, userId));

            return Ok();
        }

        [HttpPatch("endGame/{sessionId}")]
        public IActionResult EndGame(Guid sessionId)
        {
            _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.Finished);

            return Ok();
        }

        [HttpPatch("restart/{sessionId}")]
        public IActionResult RestartGame(Guid sessionId)
        {
            if (_gameSessionsDataStore.GameSessionUsers.Where(x => x.Value.SessionId == sessionId).Count() == _gameSessionsDataStore.GameSessions[sessionId].RoomSize)
                _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.Staffed);
            else
                _gameSessionsDataStore.ChangeStatus(sessionId, GameSessionStatus.NotStaffed);

            foreach (var item in _gameSessionsDataStore.GameSessionUsers.Where(x => x.Value.SessionId == sessionId))
            {
                _gameSessionsDataStore.ChangeStatus(sessionId, item.Value.UserId, PokerPlayerStatus.WaitingForStart);
            }

            return Ok();
        }
    }
}
