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

public class SessionCreating : MonoBehaviour
{
    [SerializeField]
    private InputField _lobbyName;

    [SerializeField]
    private InputField _lobbySize;

    [SerializeField]
    private InputField _lobbyPassword;

    [SerializeField]
    private GameObject _okButton;

    void FixedUpdate()
    {
        if (_lobbyName.text != null
            && _lobbyName.text != string.Empty
            && _lobbySize.text != string.Empty
            && Convert.ToInt32(_lobbySize.text) > 0
            && Convert.ToInt32(_lobbySize.text) <= 10)
            _okButton.SetActive(true);
        else
            _okButton.SetActive(false);
    }

    public void CreateSession()
    {
        string answer;

        if (_lobbyPassword.text == string.Empty)
            _lobbyPassword.text = "_no_pass_";

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/register/{_lobbySize.text}&{_lobbyName.text}&{_lobbyPassword.text}");

        request.ContentType = "application/json";
        request.Method = "POST";

        MainInformation.PlayerInformation = new PlayerInfo(new Guid("FCD8C2F0-0AF4-40D8-B08C-BCAF674D83AA"));

        var nowPlayer = new PokerGamePlayer();

        nowPlayer.UserRole = 1;

        var data = JsonConvert.SerializeObject(nowPlayer);

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

        print(answer);

        MainInformation.SessionInformation = new SessionInfo(
            JsonConvert.DeserializeObject<PokerSessionAnswerModel>(answer).userId,
            nowPlayer.SeatPlace,
            nowPlayer.Status,
            nowPlayer.UserRole,
            nowPlayer.ChipsForGame,
            nowPlayer.NowChips);
    }
}

