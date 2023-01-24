using Assets.AnswerModels;
using Assets.Scripts.PokerSession;
using Assets.StaticInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unity.MPE;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SessionController : MonoBehaviour
{
    [SerializeField]
    private Text _login;

    [SerializeField]
    private Text _chips;

    [SerializeField]
    private Text _nowChips;

    [SerializeField]
    private Text _myBet;

    [SerializeField]
    private Text _buttonDeal;

    [SerializeField]
    private Text _buttonStickmanDeal;

    [SerializeField]
    private GameObject _game;

    [SerializeField]
    private GameObject _otherPlayer;

    [SerializeField]
    private GameObject _playerHand;

    [SerializeField]
    private GameObject _stickmanHand;

    [SerializeField]
    private GameObject _stickmanInfoCanvas;

    [SerializeField]
    private GameObject _otherPlayersCanvas;

    [SerializeField]
    private GameObject _actions;

    [SerializeField]
    private GameObject _becomeStickmanButton;

    [SerializeField]
    private GameObject _playerReadyStateButton;

    [SerializeField]
    private GameObject _stickmanReadyStateButton;

    [SerializeField]
    private GameObject _checkButton;

    [SerializeField]
    private GameObject _allInButton;

    [SerializeField]
    private GameObject _callButton;

    [SerializeField]
    private GameObject _fallButton;

    [SerializeField]
    private GameObject _smallBlindButton;

    [SerializeField]
    private GameObject _bigBlindButton;

    [SerializeField]
    private InputField _cheepsAmount;

    [SerializeField]
    private Text _stickmanDeckText;

    [SerializeField]
    private Text _stickmanHandText;

    [SerializeField]
    private GameObject[] _playerCardsInHand;

    [SerializeField]
    private GameObject[] _cardsOnTable;

    [SerializeField]
    private GameObject _playersCardsOnTable;

    private GameObject _newPlayer;

    private List<InGameUsersAnswerModel> _playersBeforeUpdate;

    private Dictionary<Guid, List<Card>> _playersCards = null;

    private string _resoursesPath = "PlayingCards/Image/PlayingCards/";

    private bool _startUpdate = false;

    private int _smallBlind = 2;

    private int _bigBlind = 3;

    private int _smallBlindValue = 0;

    private int _bigBlindValue = 0;

    private int _round;

    [SerializeField]
    private Scrollbar _scrollbar;

    private void CreateMainInfo()
    {
        MainInformation.PlayerInformation = new PlayerInfo();
        MainInformation.PlayerInformation.UserId = Guid.Empty;
        MainInformation.PlayerInformation.RoleId = 0;
        MainInformation.PlayerInformation.Login = "Local tester";
        MainInformation.PlayerInformation.Chips = 10000;
        MainInformation.PlayerInformation.Email = "localtest";

        MainInformation.SessionInformation = new SessionInfo();
        MainInformation.SessionInformation.ChipsForGame = MainInformation.PlayerInformation.Chips;
        MainInformation.SessionInformation.NowChips = 0;
        MainInformation.SessionInformation.SeatPlace = 1;
        MainInformation.SessionInformation.Status = 0;
        MainInformation.SessionInformation.UserRole = MainInformation.PlayerInformation.RoleId;
        MainInformation.SessionInformation.NowSession = new Guid("30CC5CDF-3D1D-446F-8628-3751F834ABC6");
    }

    void Start()
    {
        //CreateMainInfo();

        PrepareGameField();
    }

    public void Scroller()
    {
        //_nowChips.text = "Ваша ставка: " + Convert.ToInt32(MainInformation.PlayerInformation.Chips * _scrollbar.value) + MainInformation.SessionInformation.NowChips;
        if (MainInformation.SessionInformation.Status == 2)
            _cheepsAmount.text = Convert.ToInt32(MainInformation.PlayerInformation.Chips * _scrollbar.value).ToString();
        else
        {
            if (Convert.ToInt32(MainInformation.PlayerInformation.Chips * _scrollbar.value) < _smallBlindValue)
                _cheepsAmount.text = _smallBlindValue.ToString();
            else
                _cheepsAmount.text = Convert.ToInt32(MainInformation.PlayerInformation.Chips * _scrollbar.value).ToString();
        }
    }

    public void SetValueToNowChips()
    {
        _nowChips.text = "Ваша общая ставка: " + (Convert.ToInt32(_cheepsAmount.text) + MainInformation.SessionInformation.NowChips).ToString();
        _myBet.text = "Bet: " + _cheepsAmount.text;
    }

    public void SetSmallBlindValue()
    {
        _smallBlindValue = 200;
        _cheepsAmount.text = _smallBlindValue.ToString();
    }

    public void SetBigBlindValue()
    {
        _bigBlindValue = _smallBlindValue * 2;
        _cheepsAmount.text = _bigBlindValue.ToString();
    }

    public void Call()
    {

    }

    public void PushChips()
    {
        if (_smallBlind == MainInformation.SessionInformation.SeatPlace && MainInformation.SessionInformation.Status == 2)
        {
            if (Convert.ToInt32(_cheepsAmount.text) >= 200)
            {
                _smallBlindValue = Convert.ToInt32(_cheepsAmount.text);
                PushBet();
            }
            else
                SetSmallBlindValue();
        }
        if (_bigBlind == MainInformation.SessionInformation.SeatPlace && MainInformation.SessionInformation.Status == 2)
        {
            if (Convert.ToInt32(_cheepsAmount.text) >= _smallBlindValue * 2)
            {
                _bigBlindValue = Convert.ToInt32(_cheepsAmount.text);
                PushBet();
            }
            else
                SetBigBlindValue();
        }
        else
        {
            if (Convert.ToInt32(_cheepsAmount.text) >= _smallBlindValue)
            {
                PushBet();
            }
            else
                _cheepsAmount.text = _smallBlindValue.ToString();
        }
    }

    private void PushBet()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&Bet={Convert.ToInt32(_cheepsAmount.text)}");
        request.Method = "PATCH";

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        PushNowChips();

        if (MainInformation.SessionInformation.Status == 2 && _bigBlind != MainInformation.SessionInformation.SeatPlace)
            SetNextPlayer();
        else if (MainInformation.SessionInformation.Status == 2 && _bigBlind == MainInformation.SessionInformation.SeatPlace)
            EndRound();
        else
        {
            if (_playersBeforeUpdate
                .Where(
                x => x.Bet == _playersBeforeUpdate
                .First(y => y.UserRoleId != 1)
                .Bet && x.PlayerStatusId != 4 
                && x.Bet != 0)
                .Count() == _playersBeforeUpdate
                .Where(x => x.PlayerStatusId != 4 && x.UserRoleId != 1)
                .Count())
                EndRound();
            else
                SetNextPlayer();
        }
    }

    public void Fall()
    {

    }

    private void EndRound()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/endRound/{MainInformation.SessionInformation.NowSession}");
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

    public void PushNowChips()
    {
        string answer;

        MainInformation.SessionInformation.NowChips += Convert.ToInt32(_cheepsAmount.text);

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&Chips={MainInformation.SessionInformation.NowChips}");
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

    private void SetNextPlayer()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/setNext/{MainInformation.SessionInformation.NowSession}&{MainInformation.PlayerInformation.UserId}");
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

    public void AllIn()
    {
        _scrollbar.value = 1;
    }

    private void ShowActions()
    {
        if (MainInformation.SessionInformation.Status == 2 && MainInformation.SessionInformation.SeatPlace == _smallBlind)
        {
            _smallBlindButton.SetActive(true);
            _bigBlindButton.SetActive(false);
        }

        if (MainInformation.SessionInformation.Status == 2 && MainInformation.SessionInformation.SeatPlace == _bigBlind)
        {
            _bigBlindButton.SetActive(true);
            _smallBlindButton.SetActive(false);
        }

        if (MainInformation.SessionInformation.Status != 2)
        {
            _allInButton.SetActive(true);

            if (_playersBeforeUpdate
                .Where(x => x.Bet > _playersBeforeUpdate.First(y => y.UserId == MainInformation.PlayerInformation.UserId).Bet)
                .Count() == 0
                && _playersBeforeUpdate.First(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet != 0)
                _checkButton.SetActive(false);
            else
                _checkButton.SetActive(true);

            if (_playersBeforeUpdate.Where(x => x.Bet != 0).Count() == 0)
                _callButton.SetActive(false);
            else
                _callButton.SetActive(true);

            _fallButton.SetActive(true);

            _bigBlindButton.SetActive(false);
            _smallBlindButton.SetActive(false);
        }
    }

    private int GetSize()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/size/{MainInformation.SessionInformation.NowSession}");
        string answer;

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<int>(answer);
    }

    private async void PrepareGameField()
    {
        _playersBeforeUpdate = GetPlayersAmount();

        if (_playersBeforeUpdate.FirstOrDefault() != null)
        {
            var sessionPlayers = _playersBeforeUpdate;

            if (MainInformation.SessionInformation.UserRole == 1)
            {
                _playerHand.SetActive(false);
                _stickmanInfoCanvas.SetActive(false);
                _stickmanHand.SetActive(true);

                if (MainInformation.SessionInformation.SeatPlace == GetSize())
                    _smallBlind = 1;
                else
                    _smallBlind = MainInformation.SessionInformation.SeatPlace + 1;

                if (_smallBlind == GetSize())
                    _bigBlind = 1;
                else
                    _bigBlind = _smallBlind + 1;


                _stickmanHandText.text = (await GetInfo(MainInformation.PlayerInformation.UserId)).Login;

                for (int i = 0; i < sessionPlayers.Count; i++)
                {
                    if (sessionPlayers[i].UserId != MainInformation.PlayerInformation.UserId)
                    {
                        _newPlayer = Instantiate(_otherPlayer);

                        _newPlayer.name = sessionPlayers[i].UserId.ToString();

                        _newPlayer.transform.SetParent(_otherPlayersCanvas.transform);

                        _newPlayer.SetActive(true);

                        _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "Login").text = (await GetInfo(sessionPlayers[i].UserId)).Login;
                    }
                }
            }
            else
            {
                _playerHand.SetActive(true);
                _stickmanHand.SetActive(false);

                _login.text = (await GetInfo(MainInformation.PlayerInformation.UserId)).Login;
                _chips.text = MainInformation.SessionInformation.ChipsForGame + " VGT-coins";

                for (int i = 0; i < sessionPlayers.Count; i++)
                {
                    if (sessionPlayers[i].UserId != MainInformation.PlayerInformation.UserId)
                    {
                        if (sessionPlayers[i].UserRoleId != 1)
                        {
                            _newPlayer = Instantiate(_otherPlayer);

                            _newPlayer.name = sessionPlayers[i].UserId.ToString();

                            _newPlayer.transform.SetParent(_otherPlayersCanvas.transform);

                            _newPlayer.SetActive(true);

                            _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "Login").text = (await GetInfo(sessionPlayers[i].UserId)).Login;

                            _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "AllChips").text = (await GetInfo(sessionPlayers[i].UserId)).Chips.ToString() + " VGT-coins";
                        }
                        else
                        {
                            _stickmanInfoCanvas.SetActive(true);
                            _stickmanDeckText.text = (await GetInfo(sessionPlayers[i].UserId)).Login;

                            if (sessionPlayers[i].SeatPlace == GetSize())
                                _smallBlind = 1;
                            else
                                _smallBlind = sessionPlayers[i].SeatPlace + 1;

                            if (_smallBlind == GetSize())
                                _bigBlind = 1;
                            else
                                _bigBlind = _smallBlind + 1;
                        }
                    }
                }
            }

            _startUpdate = true;
        }
    }

    private async void AddNew(InGameUsersAnswerModel item)
    {
        if (item.UserId != MainInformation.PlayerInformation.UserId)
        {
            if (item.UserRoleId == 0)
            {
                _newPlayer = Instantiate(_otherPlayer);

                _newPlayer.name = item.UserId.ToString();

                _newPlayer.transform.SetParent(_otherPlayersCanvas.transform);

                _newPlayer.SetActive(true);

                _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "Login").text = (await GetInfo(item.UserId)).Login;

                _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "AllChips").text = (await GetInfo(item.UserId)).Chips.ToString() + " VGT-coins";
            }
            else
            {
                _stickmanDeckText.text = (await GetInfo(item.UserId)).Login;

                if (item.SeatPlace == GetSize())
                    _smallBlind = 1;
                else
                    _smallBlind = item.SeatPlace + 1;

                if (_smallBlind == GetSize())
                    _bigBlind = 1;
                else
                    _bigBlind = _smallBlind + 1;
            }

            _playersBeforeUpdate.Add(item);
        }
    }

    private async void DeleteOld(InGameUsersAnswerModel item)
    {
        if (item.UserRoleId == 0)
        {
            var obj = _otherPlayersCanvas.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name.ToUpper() == item.UserId.ToString().ToUpper());

            Destroy(obj.transform.gameObject);
        }
        else
            _stickmanDeckText.text = (await GetInfo(item.UserId)).Login;

        _playersBeforeUpdate.Remove(item);
    }

    private int GetSessionStatus()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/status/{MainInformation.SessionInformation.NowSession}");
        string answer;

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<int>(answer);
    }

    private async void UpdateUIWithPlayers()
    {
        MainInformation.SessionInformation.Status = GetSessionStatus();

        print($"Now session status: {MainInformation.SessionInformation.Status}");

        var updatedUsersList = GetPlayersAmount();

        var union = _playersBeforeUpdate.Union(updatedUsersList).Distinct();

        var bothHave = new List<InGameUsersAnswerModel>();
        var beforeDoesNotHave = new List<InGameUsersAnswerModel>();
        var isGone = new List<InGameUsersAnswerModel>();

        foreach (var item in union)
        {
            if (_playersBeforeUpdate.FirstOrDefault(x => x.UserId == item.UserId) != null)
                bothHave.Add(item);
            else
                beforeDoesNotHave.Add(item);
        }

        foreach (var item in _playersBeforeUpdate)
        {
            if (updatedUsersList.FirstOrDefault(x => x.UserId == item.UserId) == null)
                isGone.Add(item);
        }

        if (beforeDoesNotHave.Count > 0)
        {
            foreach (var item in beforeDoesNotHave)
            {
                AddNew(item);
            }
        }

        if (isGone.Count > 0)
        {
            foreach (var item in isGone)
            {
                DeleteOld(item);
            }
        }

        foreach (var item in bothHave)
        {
            _playersBeforeUpdate.FirstOrDefault(x => x.UserId == item.UserId).Update(item);

            if (MainInformation.SessionInformation.Status > 1)
            {
                if (item.UserRoleId != 1 && item.UserId != MainInformation.PlayerInformation.UserId)
                {
                    var playerCanvas = _otherPlayersCanvas
                    .GetComponentsInChildren<Canvas>()
                    .FirstOrDefault(x => x.name.ToUpper() == item.UserId.ToString().ToUpper());

                    if (playerCanvas != null)
                    {
                        if (item.PlayerStatusId == 2)
                        {
                            playerCanvas
                            .GetComponentsInChildren<Text>()
                            .FirstOrDefault(x => x.name == "Status").text = "Ходит";
                        }
                        else if (item.PlayerStatusId == 4)
                        {
                            playerCanvas
                            .GetComponentsInChildren<Text>()
                            .FirstOrDefault(x => x.name == "Status").text = "Fall";
                        }
                        else
                        {
                            playerCanvas
                            .GetComponentsInChildren<Text>()
                            .FirstOrDefault(x => x.name == "Status").text = string.Empty;
                        }

                        playerCanvas
                        .GetComponentsInChildren<Text>()
                        .FirstOrDefault(x => x.name == "NowChips").text = "Общая ставка " + (await GetNowChips(item.UserId)).ToString();

                        playerCanvas
                        .GetComponentsInChildren<Text>()
                        .FirstOrDefault(x => x.name == "Bet").text = "Bet " + (await GetBet(item.UserId)).ToString();
                    }
                }
            }
        }
    }

    public async void BecomeStickman()
    {
        MainInformation.SessionInformation.UserRole = 1;
        MainInformation.PlayerInformation.RoleId = 1;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/changeRole/{MainInformation.PlayerInformation.UserId}&{MainInformation.SessionInformation.NowSession}&{MainInformation.SessionInformation.UserRole}");
        request.Method = "PATCH";

        await request.GetResponseAsync();

        _playerHand.SetActive(false);
        _stickmanInfoCanvas.SetActive(false);

        _stickmanHand.SetActive(true);
        _stickmanHandText.text = (await GetInfo(MainInformation.PlayerInformation.UserId)).Login;

        print(MainInformation.PlayerInformation.RoleId);
        print(MainInformation.SessionInformation.UserRole);
    }

    void FixedUpdate()
    {
        if (_startUpdate)
        {
            if (MainInformation.PlayerInformation != null && MainInformation.SessionInformation != null && _playersBeforeUpdate != null)
            {
                if (_playersBeforeUpdate.FirstOrDefault(x => x.UserRoleId == 1) == null && MainInformation.PlayerInformation.RoleId != 1)
                    _becomeStickmanButton.SetActive(true);
                else
                    _becomeStickmanButton.SetActive(false);

                if (_playersCards == null && _playersBeforeUpdate.FirstOrDefault(x => x.PlayerStatusId == 0) == null)
                {
                    _playerReadyStateButton.SetActive(false);
                    _stickmanReadyStateButton.SetActive(false);

                    print("CARDS COUNT IS ZERO, ALL PLAYERS ARE READY, GAME CAN BE STARTED");
                    if (MainInformation.PlayerInformation.RoleId == 1)
                        StartGameStickmanPart();
                    else
                        StartGamePlayerPart();

                    //if (_playersBeforeUpdate.Count > 0)
                    //    ShowPlayerCards();
                }

                if (_playersBeforeUpdate.FirstOrDefault(x => x.PlayerStatusId == 2) != null)
                {
                    if (_playersBeforeUpdate.FirstOrDefault(x => x.PlayerStatusId == 2).UserId == MainInformation.PlayerInformation.UserId)
                    {
                        if (_playersBeforeUpdate.FirstOrDefault(x => x.Bet != 0) != null)
                        {
                            _smallBlindValue = _playersBeforeUpdate.FirstOrDefault(x => x.Bet != 0).Bet;
                        }

                        _actions.SetActive(true);
                        ShowActions();
                    }
                    else
                        _actions.SetActive(false);
                }
                else
                    _actions.SetActive(false);

                UpdateUIWithPlayers();

                if (_smallBlindValue == 0)
                    _smallBlindValue = _playersBeforeUpdate.Min(x => x.Bet);
                if (_bigBlindValue == 0 || _bigBlindValue < _smallBlindValue * 2)
                    _bigBlindValue = _playersBeforeUpdate.Max(x => x.Bet);
            }
        }
    }

    private void ShowPlayerCards()
    {
        if (MainInformation.SessionInformation.UserRole != 1)
        {
            PlayerRole();
        }
        else
        {
            StickmanRole();
        }
    }

    private void PlayerRole()
    {
        _actions.SetActive(true);

        for (int i = 0; i < 2; i++)
        {
            _playerCardsInHand[i].GetComponent<Image>().sprite = Resources
            .Load<Sprite>(_resoursesPath + _playersCards.FirstOrDefault(x => x.Key == MainInformation.PlayerInformation.UserId).Value[i].CardId.ToString());
        }
    }

    private void StickmanRole()
    {
        for (int i = 0; i < 5; i++)
        {
            _cardsOnTable[i].GetComponent<Image>().sprite = Resources
            .Load<Sprite>(
            _resoursesPath +
            _playersCards
            .FirstOrDefault(
                x => x.Key == MainInformation.PlayerInformation.UserId).Value[i].CardId);
        }

        var usersCopy = _playersBeforeUpdate;
        usersCopy.Remove(usersCopy.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId));

        foreach (var user in usersCopy)
        {
            var cards = _otherPlayersCanvas
                .GetComponentsInChildren<Canvas>()
                .FirstOrDefault(
                x => x.name.ToUpper() == user.UserId.ToString().ToUpper()).GetComponentsInChildren<Canvas>();

            cards.FirstOrDefault(x => x.name == "FirstCard").GetComponent<Image>().sprite = Resources
            .Load<Sprite>(
            _resoursesPath +
            _playersCards
            .FirstOrDefault(
                x => x.Key == user.UserId).Value[0].CardId);

            cards.FirstOrDefault(x => x.name == "SecondCard").GetComponent<Image>().sprite = Resources
            .Load<Sprite>(
            _resoursesPath +
            _playersCards
            .FirstOrDefault(
                x => x.Key == user.UserId).Value[1].CardId);
        }
    }

    private void StartGamePlayerPart()
    {
        if (CanStart() && SetStartedStatusCode())
        {
            SetFirstPlayer();
            //_playersCardsOnTable.SetActive(true);

            //foreach (var card in _cardsOnTable)
            //{
            //    card.SetActive(true);
            //}

            //_playersCards = new Dictionary<Guid, List<Card>>(GetCards(0));
        }
    }

    private void StartGameStickmanPart()
    {
        if (CanStart() && SetStartedStatusCode())
        {
            SetFirstPlayer();
            //foreach (var card in _cardsOnTable)
            //{
            //    card.SetActive(true);
            //}

            //_playersCards = new Dictionary<Guid, List<Card>>(GetCards(1));
        }
    }

    private bool SetStartedStatusCode()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/start/{MainInformation.SessionInformation.NowSession}");
        request.Method = "PATCH";

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        MainInformation.SessionInformation.Status = 2;

        return answer == "Started";
    }

    private Dictionary<Guid, List<Card>> GetCards(int roleId)
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/prepareCards/{roleId}");
        request.ContentType = "application/json";
        request.Method = "POST";

        var data = JsonConvert.SerializeObject(_playersBeforeUpdate);

        var bytes = Encoding.UTF8.GetBytes(data);

        request.ContentLength = bytes.Length;

        using (var writer = request.GetRequestStream())
        {
            writer.Write(bytes, 0, bytes.Length);
        }

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<Dictionary<Guid, List<Card>>>(answer);
    }

    private bool CanStart()
    {
        bool res = false;

        if (IsLobbyFilled())
            foreach (var item in _playersBeforeUpdate)
            {
                print(item.PlayerStatusId);

                if (item.PlayerStatusId == 1)
                    res = true;
                else
                    res = false;
            }

        return res;
    }

    private bool IsLobbyFilled()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/isFilled/{MainInformation.SessionInformation.NowSession}");
        var response = request.GetResponse();
        string answer;

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<bool>(answer);
    }

    private void SetFirstPlayer()
    {
        var stickmanPos = _playersBeforeUpdate.FirstOrDefault(x => x.UserRoleId == 1).SeatPlace;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/setFirst/{MainInformation.SessionInformation.NowSession}&{stickmanPos}");
        request.Method = "PATCH";
        string answer;

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }
    }

    private async Task<InfoAnswerModel> GetInfo(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/users/info/{id}");
        string answer;

        var response = await request.GetResponseAsync();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = await reader.ReadToEndAsync();
            }
        }

        return JsonConvert.DeserializeObject<InfoAnswerModel>(answer);
    }

    private async Task<int> GetNowChips(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/nowChips/{MainInformation.SessionInformation.NowSession}&{id}");
        string answer;

        var response = await request.GetResponseAsync();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = await reader.ReadToEndAsync();
            }
        }

        return JsonConvert.DeserializeObject<int>(answer);
    }

    private async Task<int> GetBet(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/bet/{MainInformation.SessionInformation.NowSession}&{id}");
        string answer;

        var response = await request.GetResponseAsync();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = await reader.ReadToEndAsync();
            }
        }

        return JsonConvert.DeserializeObject<int>(answer);
    }

    public void ChangeStatus()
    {
        if (MainInformation.SessionInformation.UserRole != 1)
        {
            if (_buttonDeal.text == "Готов")
                SetReady();
            else
                SetNotReady();
        }
        else
        {
            if (_buttonStickmanDeal.text == "Готов")
                SetReady();
            else
                SetNotReady();
        }
    }

    private async void SetReady()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&StatusCode=1");
        request.Method = "PATCH";
        string answer;

        var response = await request.GetResponseAsync();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = await reader.ReadToEndAsync();
            }
        }

        MainInformation.SessionInformation.Status = 1;
        _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).PlayerStatusId = 1;

        if (MainInformation.SessionInformation.UserRole != 1)
            _buttonDeal.text = "Не готов";
        else
            _buttonStickmanDeal.text = "Не готов";
    }

    private async void SetNotReady()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&StatusCode=0");
        request.Method = "PATCH";

        string answer;

        var response = await request.GetResponseAsync();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = await reader.ReadToEndAsync();
            }
        }

        MainInformation.SessionInformation.Status = 0;
        _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).PlayerStatusId = 0;

        if (MainInformation.SessionInformation.UserRole != 1)
            _buttonDeal.text = "Готов";
        else
            _buttonStickmanDeal.text = "Готов";
    }

    private List<InGameUsersAnswerModel> GetPlayersAmount()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/places/{MainInformation.SessionInformation.NowSession}");
        string answer;

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<List<InGameUsersAnswerModel>>(answer);
    }

    public async void ExitFromLobby()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/leave/{MainInformation.PlayerInformation.UserId}&{MainInformation.SessionInformation.NowSession}");
        request.Method = "DELETE";

        await request.GetResponseAsync();

        _playersBeforeUpdate.Remove(_playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId));

        //Destroy(_game);
    }
}
