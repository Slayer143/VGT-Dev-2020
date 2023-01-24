using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;
using System;
using Assets.Scripts.Authorization;
using Newtonsoft.Json;
using System.IO;
using Assets.AnswerModels;
using Assets.ServerStateControl;
using Assets.StaticInfo;

public class Authorization : MonoBehaviour
{
    [SerializeField]
    private InputField _login;

    [SerializeField]
    private InputField _password;

    [SerializeField]
    private Text _result;

    [SerializeField]
    private GameObject _loginObj;

    [SerializeField]
    private GameObject _passwordObj;

    [SerializeField]
    private GameObject _resultObj;

    [SerializeField]
    private GameObject _confirmButton;

    [SerializeField]
    private GameObject _goFurtherButton;

    [SerializeField]
    private GameObject _switchButton;

    public void AuthorizationButton()
    {
        _switchButton.SetActive(false);

        if (new ServerInfo().IsActive)
        {
            if (_login.text != string.Empty
                && _password.text != string.Empty)
            {
                var answer = GetResponse(new AuthorizationModel(_login.text, _password.text));

                _result.text = answer.result;
                ShowResult();

                if (answer.userId != Guid.Empty)
                    MainInformation.PlayerInformation = new PlayerInfo(answer.userId);
            }
            else
            {
                _result.text = "All fields must be filled";
                ShowResult();
            }
        }
        else
        {
            _result.text = "Server is not active or you have some connection troubles";
            ShowResult();
        }
    }

    public RegistrationAndAuthorizationAnswerModel GetResponse(AuthorizationModel model)
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/users/auth/Login={model.Login}&Password={model.Password}");
        var response = request.GetResponse();

        using (var stream = response.GetResponseStream())
        {
            using (var reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
        }

        return JsonConvert.DeserializeObject<RegistrationAndAuthorizationAnswerModel>(answer);
    }

    public void ShowResult()
    {
        _loginObj.SetActive(false);
        _passwordObj.SetActive(false);
        _confirmButton.SetActive(false);
        _resultObj.SetActive(true);
        _goFurtherButton.SetActive(true);
    }
}
