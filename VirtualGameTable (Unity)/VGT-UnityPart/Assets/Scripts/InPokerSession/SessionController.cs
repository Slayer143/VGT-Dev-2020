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
using UnityEngine;
using UnityEngine.UI;

public class SessionController : MonoBehaviour
{
    [SerializeField]
    private GameObject _menu;

    [SerializeField]
    private GameObject _game;

    [SerializeField]
    private Text _login;

    [SerializeField]
    private Text _chips;

    [SerializeField]
    private Text _bank;

    [SerializeField]
    private Text _myBet;

    [SerializeField]
    private Text _buttonDeal;

    [SerializeField]
    private Text _buttonStickmanDeal;

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
    private Text _minusSmallBlindButton;

    [SerializeField]
    private Text _plusSmallBlindButton;

    [SerializeField]
    private GameObject _smallBlindButtons;

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

    [SerializeField]
    private Scrollbar _scrollbar;

    private List<Result> _results;

    void Start()
    {
        PrepareGameField();
    }

    public void Scroller()
    {
        _cheepsAmount.text = Convert.ToInt32(_playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).StartingChips * _scrollbar.value).ToString();
    }

    public void SetValueToNowChips()
    {
        if (MainInformation.SessionInformation.SeatPlace == _bigBlind
            && _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet == 0
            && Convert.ToInt32(_cheepsAmount.text) < _smallBlindValue * 2)
            SetBigBlindValue();

        if (MainInformation.SessionInformation.SeatPlace == _smallBlind
            && _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet == 0
            && Convert.ToInt32(_cheepsAmount.text) < 10)
            SetSmallBlindValue();

        if (Convert.ToInt32(_cheepsAmount.text) < _playersBeforeUpdate.Max(x => x.Bet))
            _cheepsAmount.text = (_playersBeforeUpdate.Max(x => x.Bet) - _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet).ToString();
    }

    public void SetSmallBlindValue()
    {
        _smallBlindValue = 10;
        _cheepsAmount.text = _smallBlindValue.ToString();
    }

    public void SetBigBlindValue()
    {
        _bigBlindValue = _smallBlindValue * 2;
        _cheepsAmount.text = _bigBlindValue.ToString();
    }

    public void Call()
    {
        _cheepsAmount.text = (_playersBeforeUpdate.Max(x => x.Bet) - _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet).ToString();
    }

    public void Check()
    {
        SetCheckStatus();
    }

    public void PushChips()
    {
        if (Convert.ToInt32(_cheepsAmount.text) >= 10)
        {
            PushBet();
        }
        else
            SetSmallBlindValue();
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

        MainInformation.SessionInformation.ChipsForGame -= Convert.ToInt32(_cheepsAmount.text);

        _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet += Convert.ToInt32(_cheepsAmount.text);

        PushNowChips();
    }

    public void Fall()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&StatusCode=4");
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

    public void SetCheckStatus()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&StatusCode=5");
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

    public void AllIn()
    {
        _cheepsAmount.text = _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).StartingChips.ToString();
    }

    private void ShowActions()
    {
        if (MainInformation.SessionInformation.Status == 2 && MainInformation.SessionInformation.SeatPlace == _smallBlind && _playersBeforeUpdate.Where(x => x.Bet == 0).Count() == _playersBeforeUpdate.Count)
        {
            _smallBlindButton.SetActive(true);

            _bigBlindButton.SetActive(false);
            _callButton.SetActive(false);
            _checkButton.SetActive(false);
            _allInButton.SetActive(false);
            _fallButton.SetActive(false);

            _smallBlindButtons.SetActive(false);
        }
        else if (MainInformation.SessionInformation.Status == 2 && MainInformation.SessionInformation.SeatPlace == _smallBlind)
        {
            _smallBlindButton.SetActive(true);
            _callButton.SetActive(true);
            _allInButton.SetActive(true);
            _fallButton.SetActive(true);
            _smallBlindButtons.SetActive(true);
            _bigBlindButton.SetActive(true);

            _plusSmallBlindButton.text = "+" + _smallBlindValue.ToString();
            _minusSmallBlindButton.text = "-" + _smallBlindValue.ToString();

            _checkButton.SetActive(false);

            if (_playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet == _playersBeforeUpdate.Max(x => x.Bet))
                _checkButton.SetActive(true);
        }

        if (MainInformation.SessionInformation.Status == 2 && MainInformation.SessionInformation.SeatPlace == _bigBlind)
        {
            _bigBlindButton.SetActive(true);
            _smallBlindButton.SetActive(false);
            _allInButton.SetActive(true);
            _smallBlindButtons.SetActive(true);

            _plusSmallBlindButton.text = "+" + _smallBlindValue.ToString();
            _minusSmallBlindButton.text = "-" + _smallBlindValue.ToString();

            _callButton.SetActive(false);
            _checkButton.SetActive(false);
            _fallButton.SetActive(false);

            if (_playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet == _playersBeforeUpdate.Max(x => x.Bet))
                _checkButton.SetActive(true);
        }
        else
        {
            _allInButton.SetActive(true);

            if (_playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).Bet == _playersBeforeUpdate.Max(x => x.Bet))
                _checkButton.SetActive(true);
            else
                _checkButton.SetActive(false);

            if (_playersBeforeUpdate.FirstOrDefault(x => x.Bet == _playersBeforeUpdate.Max(y => y.Bet)).UserId != MainInformation.PlayerInformation.UserId
                && _playersBeforeUpdate.Where(x => x.Bet == 0).Count() != 0)
                _callButton.SetActive(true);
            else
                _callButton.SetActive(false);

            _fallButton.SetActive(true);

            _smallBlindButtons.SetActive(true);

            _plusSmallBlindButton.text = "+" + _smallBlindValue.ToString();
            _minusSmallBlindButton.text = "-" + _smallBlindValue.ToString();

            _bigBlindButton.SetActive(false);
            _smallBlindButton.SetActive(false);
        }
    }

    public void PlusSmallBlind()
    {
        _cheepsAmount.text = (Convert.ToInt32(_cheepsAmount.text) + _smallBlindValue).ToString();
    }

    public void MinusSmallBlind()
    {
        _cheepsAmount.text = (Convert.ToInt32(_cheepsAmount.text) - _smallBlindValue).ToString();
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

    private void PrepareGameField()
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


                _stickmanHandText.text = GetInfo(MainInformation.PlayerInformation.UserId).Login;

                for (int i = 0; i < sessionPlayers.Count; i++)
                {
                    if (sessionPlayers[i].UserId != MainInformation.PlayerInformation.UserId)
                    {
                        _newPlayer = Instantiate(_otherPlayer);

                        _newPlayer.name = sessionPlayers[i].UserId.ToString();

                        _newPlayer.transform.SetParent(_otherPlayersCanvas.transform);

                        _newPlayer.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name == "FirstCard").enabled = false;
                        _newPlayer.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name == "SecondCard").enabled = false;

                        _newPlayer.SetActive(true);

                        _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "Login").text = GetInfo(sessionPlayers[i].UserId).Login;

                        _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "AllChips").text = GetInfo(sessionPlayers[i].UserId).Chips.ToString() + " VGT-coins";
                    }
                }
            }
            else
            {
                _playerHand.SetActive(true);
                _stickmanHand.SetActive(false);

                _login.text = GetInfo(MainInformation.PlayerInformation.UserId).Login;
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

                            _newPlayer.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name == "FirstCard").enabled = false;
                            _newPlayer.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name == "SecondCard").enabled = false;

                            _newPlayer.SetActive(true);

                            _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "Login").text = GetInfo(sessionPlayers[i].UserId).Login;

                            _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "AllChips").text = GetInfo(sessionPlayers[i].UserId).Chips.ToString() + " VGT-coins";
                        }
                        else
                        {
                            _stickmanInfoCanvas.SetActive(true);
                            _stickmanDeckText.text = GetInfo(sessionPlayers[i].UserId).Login;

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

    private void AddNew(InGameUsersAnswerModel item)
    {
        if (item.UserId != MainInformation.PlayerInformation.UserId)
        {
            if (item.UserRoleId == 0)
            {
                _newPlayer = Instantiate(_otherPlayer);

                _newPlayer.name = item.UserId.ToString();

                _newPlayer.transform.SetParent(_otherPlayersCanvas.transform);

                _newPlayer.SetActive(true);

                _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "Login").text = GetInfo(item.UserId).Login;

                _newPlayer.GetComponentsInChildren<Text>().FirstOrDefault(x => x.name == "AllChips").text = GetInfo(item.UserId).Chips.ToString() + " VGT-coins";

                _newPlayer.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name == "FirstCard").enabled = false;
                _newPlayer.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name == "SecondCard").enabled = false;
            }
            else
            {
                _stickmanDeckText.text = GetInfo(item.UserId).Login;

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

    private void DeleteOld(InGameUsersAnswerModel item)
    {
        if (item.UserRoleId == 0)
        {
            var obj = _otherPlayersCanvas.GetComponentsInChildren<Canvas>().FirstOrDefault(x => x.name.ToUpper() == item.UserId.ToString().ToUpper());

            Destroy(obj.transform.gameObject);
        }
        else
            _stickmanDeckText.text = GetInfo(item.UserId).Login;

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

    private int GetChips()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/users/info/chips/{MainInformation.PlayerInformation.UserId}");
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

    private void UpdateUIWithPlayers()
    {
        MainInformation.SessionInformation.Status = GetSessionStatus();
        MainInformation.PlayerInformation.Chips = GetChips();
        MainInformation.SessionInformation.ChipsForGame = MainInformation.PlayerInformation.Chips;

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

        if (_playersBeforeUpdate.Sum(x => x.NowChips) != 0)
            _bank.text = _playersBeforeUpdate.Sum(x => x.NowChips).ToString();
        else
            _bank.text = string.Empty;

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
                        .FirstOrDefault(x => x.name == "Bet").text = "Bet " + GetBet(item.UserId).ToString();
                    }
                }
                else if (item.UserRoleId != 1 && item.UserId != MainInformation.PlayerInformation.UserId)
                {
                    _chips.text = item.StartingChips.ToString();
                    _myBet.text = item.Bet.ToString();
                }
            }
        }
    }

    public void BecomeStickman()
    {
        MainInformation.SessionInformation.UserRole = 1;
        MainInformation.PlayerInformation.RoleId = 1;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/changeRole/{MainInformation.PlayerInformation.UserId}&{MainInformation.SessionInformation.NowSession}&{MainInformation.SessionInformation.UserRole}");
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

        _playerHand.SetActive(false);
        _stickmanInfoCanvas.SetActive(false);

        _stickmanHand.SetActive(true);
        _stickmanHandText.text = GetInfo(MainInformation.PlayerInformation.UserId).Login;

        print(MainInformation.PlayerInformation.RoleId);
        print(MainInformation.SessionInformation.UserRole);
    }

    void FixedUpdate()
    {
        if (_startUpdate)
        {
            if (MainInformation.PlayerInformation != null && MainInformation.SessionInformation != null && _playersBeforeUpdate != null)
            {
                UpdateUIWithPlayers();

                if (_playersBeforeUpdate.FirstOrDefault(x => x.UserRoleId == 1) == null && MainInformation.PlayerInformation.RoleId != 1)
                    _becomeStickmanButton.SetActive(true);
                else
                    _becomeStickmanButton.SetActive(false);

                if (_playersCards == null
                    && _playersBeforeUpdate.Where(x => x.PlayerStatusId == 0).Count() == 0
                    && MainInformation.SessionInformation.Status == 1
                    && _playersBeforeUpdate.Count == GetSize())
                {
                    _playerReadyStateButton.SetActive(false);
                    _stickmanReadyStateButton.SetActive(false);

                    if (MainInformation.PlayerInformation.RoleId == 1)
                        StartGameStickmanPart();
                    else
                        StartGamePlayerPart();

                    if (MainInformation.SessionInformation.UserRole != 1)
                        _playersCardsOnTable.SetActive(true);
                }
                if (_playersCards != null
                    && _playersBeforeUpdate.Where(x => x.PlayerStatusId == 0).Count() == 0
                    && _playersBeforeUpdate.Count == GetSize())
                {
                    if (_playersBeforeUpdate.Count > 0)
                        ShowPlayerCards();

                    foreach (var card in _cardsOnTable)
                    {
                        card.SetActive(true);
                    }

                    foreach (var user in _playersBeforeUpdate)
                    {
                        if (user.UserId != MainInformation.PlayerInformation.UserId && user.UserRoleId != 1)
                        {
                            var cards = _otherPlayersCanvas
                            .GetComponentsInChildren<Canvas>()
                            .FirstOrDefault(
                            x => x.name.ToUpper() == user.UserId.ToString().ToUpper()).GetComponentsInChildren<Canvas>();

                            cards.FirstOrDefault(x => x.name == "FirstCard").enabled = true;
                            cards.FirstOrDefault(x => x.name == "SecondCard").enabled = true;
                        }
                    }

                    if (MainInformation.SessionInformation.Status == 2 && _smallBlindValue == 0 || _bigBlindValue == 0)
                    {
                        if (_playersBeforeUpdate.Min(x => x.Bet) != 0)
                            _smallBlindValue = _playersBeforeUpdate.Min(x => x.Bet);
                        else
                            _smallBlindValue = _playersBeforeUpdate.Max(x => x.Bet);

                        if (_smallBlindValue == _playersBeforeUpdate.Max(x => x.Bet))
                            _bigBlindValue = _smallBlindValue * 2;
                        else
                            _bigBlindValue = _playersBeforeUpdate.Max(x => x.Bet);
                    }

                    ShowCards();

                    if (_playersBeforeUpdate.FirstOrDefault(x => x.PlayerStatusId == 2) != null)
                    {
                        if (_playersBeforeUpdate.FirstOrDefault(x => x.PlayerStatusId == 2).UserId == MainInformation.PlayerInformation.UserId)
                        {
                            _actions.SetActive(true);
                            ShowActions();
                        }
                        else
                            _actions.SetActive(false);
                    }
                    else
                        _actions.SetActive(false);

                    if (MainInformation.SessionInformation.Status == 6)
                    {
                        _results = GetResults();

                        var winCount = _results.Where(x => x.Combination == _results.Max(y => y.Combination) && x.Value == _results.Max(y => y.Value)).Count();

                        if (winCount == 1)
                        {
                            if (_results
                            .Where(x => x.Combination == _results.Max(y => y.Combination)
                            && x.Value == _results.Max(y => y.Value))
                            .ToList()[0]
                            .UserId == MainInformation.PlayerInformation.UserId)
                                print("I AM THE WINNER");
                            else
                            {
                                print("I AM NOT THE WINNER");
                                print($"{_results.Where(x => x.Combination == _results.Max(y => y.Combination) && x.Value == _results.Max(y => y.Value)).ToList()[0].UserId} IS THE WINNER");
                            }

                        }
                        else
                        {
                            print($"{_results.Where(x => x.Combination == _results.Max(y => y.Combination) && x.Value == _results.Max(y => y.Value)).ToList()[0].UserId} IS THE FIRST WINNER");
                            print($"{_results.Where(x => x.Combination == _results.Max(y => y.Combination) && x.Value == _results.Max(y => y.Value)).ToList()[1].UserId} IS THE SECOND WINNER");
                        }

                        System.Threading.Thread.Sleep(10000);

                        ClearTable();
                        RestartSession();
                    }
                }
            }
        }
    }

    public void ClearTable()
    {
        foreach (var item in _cardsOnTable)
        {
            item.GetComponent<Image>().sprite = Resources.Load<Sprite>(_resoursesPath + "BackColor_Black");
            item.SetActive(false);
        }

        foreach (var user in _playersBeforeUpdate)
        {
            if (user.UserId != MainInformation.PlayerInformation.UserId && user.UserRoleId != 1)
            {
                var cards = _otherPlayersCanvas
                .GetComponentsInChildren<Canvas>()
                .FirstOrDefault(
                x => x.name.ToUpper() == user.UserId.ToString().ToUpper()).GetComponentsInChildren<Canvas>();

                cards.FirstOrDefault(x => x.name == "FirstCard").GetComponent<Image>().sprite = Resources.Load<Sprite>(_resoursesPath + "BackColor_Black");
                cards.FirstOrDefault(x => x.name == "SecondCard").GetComponent<Image>().sprite = Resources.Load<Sprite>(_resoursesPath + "BackColor_Black");

                cards.FirstOrDefault(x => x.name == "FirstCard").enabled = false;
                cards.FirstOrDefault(x => x.name == "SecondCard").enabled = false;
            }
        }

        if (MainInformation.SessionInformation.UserRole != 1)
        {
            foreach (var item in _playerCardsInHand)
            {
                item.GetComponent<Image>().sprite = Resources.Load<Sprite>(_resoursesPath + "BackColor_Black");
            }

            _playersCardsOnTable.SetActive(false);

            _actions.SetActive(false);

            _smallBlindValue = 0;
            _bigBlindValue = 0;
            _playersCards = null;
            _results = null;
        }
    }

    public void RestartSession()
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/restart/{MainInformation.SessionInformation.NowSession}");
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

    public List<Result> GetResults()
    {
        string answer;

        print(MainInformation.SessionInformation.NowSession);

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/results/{MainInformation.SessionInformation.NowSession}");

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<List<Result>>(answer);
    }

    public void ShowCards()
    {
        var stickmanId = _playersBeforeUpdate.FirstOrDefault(x => x.UserRoleId == 1).UserId;

        if (MainInformation.SessionInformation.UserRole != 1 && MainInformation.SessionInformation.Status == 3)
        {
            for (int i = 0; i < 2; i++)
            {
                _playerCardsInHand[i].GetComponent<Image>().sprite = Resources
                .Load<Sprite>(_resoursesPath + _playersCards.FirstOrDefault(x => x.Key == MainInformation.PlayerInformation.UserId).Value[i].CardId.ToString());
            }
        }

        if (MainInformation.SessionInformation.Status == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                _cardsOnTable[i].GetComponent<Image>().sprite = Resources
                .Load<Sprite>(
                _resoursesPath +
                _playersCards
                .FirstOrDefault(
                x => x.Key == stickmanId).Value[i].CardId);
            }
        }
        if (MainInformation.SessionInformation.Status == 4)
        {
            _cardsOnTable[3].GetComponent<Image>().sprite = Resources
                .Load<Sprite>(
                _resoursesPath +
                _playersCards
                .FirstOrDefault(
                x => x.Key == stickmanId).Value[3].CardId);
        }
        if (MainInformation.SessionInformation.Status == 5)
        {
            _cardsOnTable[4].GetComponent<Image>().sprite = Resources
                .Load<Sprite>(
                _resoursesPath +
                _playersCards
                .FirstOrDefault(
                x => x.Key == stickmanId).Value[4].CardId);
        }
        if (MainInformation.SessionInformation.Status == 6)
        {
            for (int i = 0; i < 5; i++)
            {
                _cardsOnTable[i].GetComponent<Image>().sprite = Resources
                .Load<Sprite>(
                _resoursesPath +
                _playersCards
                .FirstOrDefault(
                x => x.Key == stickmanId).Value[i].CardId);
            }

            foreach (var user in _playersBeforeUpdate)
            {
                if (user.UserId != MainInformation.PlayerInformation.UserId && user.UserRoleId != 1)
                {
                    var cards = _otherPlayersCanvas
                    .GetComponentsInChildren<Canvas>()
                    .FirstOrDefault(
                    x => x.name.ToUpper() == user.UserId.ToString().ToUpper()).GetComponentsInChildren<Canvas>();

                    cards.FirstOrDefault(x => x.name == "FirstCard").GetComponent<Image>().sprite = Resources.Load<Sprite>(_resoursesPath + _playersCards.FirstOrDefault(x => x.Key == user.UserId).Value[0].CardId);
                    cards.FirstOrDefault(x => x.name == "SecondCard").GetComponent<Image>().sprite = Resources.Load<Sprite>(_resoursesPath + _playersCards.FirstOrDefault(x => x.Key == user.UserId).Value[1].CardId);

                    cards.FirstOrDefault(x => x.name == "FirstCard").enabled = true;
                    cards.FirstOrDefault(x => x.name == "SecondCard").enabled = true;
                }
            }
        }
    }

    private void ShowPlayerCards()
    {
        if (MainInformation.SessionInformation.UserRole != 1)
            PlayerRole();
    }

    private void PlayerRole()
    {
        _actions.SetActive(true);
    }

    private void StartGamePlayerPart()
    {
        if (CanStart())
        {
            _playersCardsOnTable.SetActive(true);

            foreach (var card in _cardsOnTable)
            {
                card.SetActive(true);
            }

            _playersCards = new Dictionary<Guid, List<Card>>(GetCards(0));
        }
    }

    private void StartGameStickmanPart()
    {
        if (CanStart() && SetStartedStatusCode())
        {
            SetFirstPlayer();

            foreach (var card in _cardsOnTable)
            {
                card.SetActive(true);
            }

            _playersCards = new Dictionary<Guid, List<Card>>(GetCards(1));
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

    private InfoAnswerModel GetInfo(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/users/info/{id}");
        string answer;

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<InfoAnswerModel>(answer);
    }

    private int GetNowChips(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/nowChips/{MainInformation.SessionInformation.NowSession}&{id}");
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

    private int GetBet(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/bet/{MainInformation.SessionInformation.NowSession}&{id}");
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

    private void SetReady()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&StatusCode=1");
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

        MainInformation.SessionInformation.Status = 1;
        _playersBeforeUpdate.FirstOrDefault(x => x.UserId == MainInformation.PlayerInformation.UserId).PlayerStatusId = 1;

        if (MainInformation.SessionInformation.UserRole != 1)
            _buttonDeal.text = "Не готов";
        else
            _buttonStickmanDeal.text = "Не готов";
    }

    private void SetNotReady()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/SessionId={MainInformation.SessionInformation.NowSession}&UserId={MainInformation.PlayerInformation.UserId}&StatusCode=0");
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

    public void Exit()
    {
        try
        {
            var request = WebRequest.Create($"http://localhost:5000/api/sessions/leave/{MainInformation.PlayerInformation.UserId}&{MainInformation.SessionInformation.NowSession}");
            request.Method = "DELETE";

            request.GetResponseAsync();

            _game.SetActive(false);
            Destroy(_game);

            _menu.SetActive(true);

            MainInformation.SessionInformation = null;
        }
        catch (Exception)
        {
            _game.SetActive(false);
            Destroy(_game);

            _menu.SetActive(true);

            MainInformation.SessionInformation = null;
        }
    }
}
