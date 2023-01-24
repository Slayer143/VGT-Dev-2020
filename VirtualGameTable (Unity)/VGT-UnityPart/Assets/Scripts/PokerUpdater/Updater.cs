using Assets.Scripts.PokerSession;
using Assets.Scripts.PokerUpdater;
using Assets.StaticInfo;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Updater : MonoBehaviour
{
    [SerializeField]
    private GameObject _lobbyList;

    [SerializeField]
    private Font _font;

    [SerializeField]
    private GameObject _friendsList;

    [SerializeField]
    private GameObject _pokerMenu;

    [SerializeField]
    private GameObject _pokerGame;

    void Start()
    {
        CreateLobbyList();
    }

    void FixedUpdate()
    {
        CreateLobbyList();
    }

    private void SwitchToPokerSession()
    {
        _pokerMenu.SetActive(false);
        _pokerGame.SetActive(true);
    }

    private int GetPlace(Guid id)
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/places/free/{id}");
        string answer;

        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        print(answer);
        print(JsonConvert.DeserializeObject<int[]>(answer).Length);

        return JsonConvert.DeserializeObject<int[]>(answer).Length;
    }

    public void EnterLobby(Guid id)
    {
        MainInformation.PlayerInformation = new PlayerInfo(new Guid("FCD8C2F0-0AF4-40D8-B08C-BCAF674D83AA"));

        string answer;

        var player = new PokerGamePlayer();

        player.PlayerId = MainInformation.PlayerInformation.UserId;

        print(player.PlayerId);

        player.SeatPlace = GetPlace(id);

        print(player.SeatPlace);

        var request = WebRequest.Create($"http://localhost:5000/api/sessions/{id}");
        request.ContentType = "application/json";
        request.Method = "POST";

        var data = JsonConvert.SerializeObject(player);

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

        SwitchToPokerSession();

        MainInformation.SessionInformation = new SessionInfo(id, player.SeatPlace, player.Status, player.UserRole, player.ChipsForGame, player.NowChips);
    }

    private void CreateLobbyList()
    {
        var list = GetLobbyList();

        foreach (var item in _lobbyList.GetComponentsInChildren<UnityEngine.UI.Button>().ToList())
        {
            if (list.FirstOrDefault(x => x.SessionId.ToString().ToUpper() == item.name.ToUpper()) == null)
                Destroy(item.transform.gameObject);
        }

        if (list.Count > 0)
        {
            foreach (var lobby in list)
            {
                if (_lobbyList.GetComponentsInChildren<UnityEngine.UI.Button>().ToList().FirstOrDefault(x => x.name == lobby.SessionId.ToString()) == null)
                {
                    var lobbyLink = new GameObject(lobby.SessionId.ToString(), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button), typeof(LayoutElement));

                    lobbyLink.transform.SetParent(_lobbyList.transform);

                    lobbyLink.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { EnterLobby(lobby.SessionId); });

                    var text = new GameObject(lobby.RoomName, typeof(Text));
                    text.transform.SetParent(lobbyLink.transform);
                    text.GetComponent<Text>().text = lobby.RoomName;
                    text.GetComponent<Text>().font = _font;
                    text.GetComponent<Text>().fontSize = 30;
                    text.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    var rectTransform = text.GetComponent<RectTransform>();

                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    rectTransform.sizeDelta = new Vector2(0, 0);

                    text.GetComponent<Text>().color = Color.black;
                }
            }
        }
    }

    private List<GameSessionModel> GetLobbyList()
    {
        var request = WebRequest.Create($"http://localhost:5000/api/sessions/open");
        var response = request.GetResponse();
        string answer;

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<List<GameSessionModel>>(answer);
    }

    private void GetFriendsList()
    {

    }
}
