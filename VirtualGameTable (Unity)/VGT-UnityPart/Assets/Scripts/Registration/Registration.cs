using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.UI;
using Assets.Scripts.Registration;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Assets.AnswerModels;
using Assets.ServerStateControl;
using System;
using Assets.StaticInfo;
using System.Security.Cryptography;

public class Registration : MonoBehaviour
{
    [SerializeField]
    private InputField _login;

    [SerializeField]
    private InputField _password;

    [SerializeField]
    private InputField _passwordRepeat;

    [SerializeField]
    private InputField _email;

    [SerializeField]
    private Text _result;

    [SerializeField]
    private GameObject _loginObj;

    [SerializeField]
    private GameObject _passwordObj;

    [SerializeField]
    private GameObject _passwordRepeatObj;

    [SerializeField]
    private GameObject _emailObj;

    [SerializeField]
    private GameObject _confirmButton;

    [SerializeField]
    private GameObject _switchButton;

    [SerializeField]
    private GameObject _resultObj;

    [SerializeField]
    private GameObject _goFurtherButton;

    public void RegisterButton()
    {
        _switchButton.SetActive(false);

        if (new ServerInfo().IsActive)
        {
            if (_login.text != string.Empty
            && _password.text != string.Empty
            && _passwordRepeat.text != string.Empty
            && _email.text != string.Empty)
            {
                var userInfo = new RegistrationFormModel(_login.text, _password.text, _passwordRepeat.text, _email.text);

                if (userInfo.CheckPass())
                {
                    if (userInfo.CheckEmail())
                    {
                        var answer = GetResponse(new RegistrationUserModel(userInfo.Login, userInfo.Password, userInfo.Email));

                        _result.text = answer.result;
                        ShowResult();

                        if (answer.userId != Guid.Empty)
                            MainInformation.PlayerInformation = new PlayerInfo(answer.userId);
                    }
                    else
                    {
                        _result.text = "Incorrect email";
                        ShowResult();
                    }
                }
                else
                {
                    _result.text = "Password and it`s repeat are not equal";
                    ShowResult();
                }
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

    public RegistrationAndAuthorizationAnswerModel GetResponse(RegistrationUserModel model)
    {
        string answer;

        var request = WebRequest.Create($"http://localhost:5000/api/users/register");
        request.ContentType = "application/json";
        request.Method = "POST";

        var data = JsonConvert.SerializeObject(model);

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

        return JsonConvert.DeserializeObject<RegistrationAndAuthorizationAnswerModel>(answer);
    }

    public void ShowResult()
    {
        _loginObj.SetActive(false);
        _passwordObj.SetActive(false);
        _passwordRepeatObj.SetActive(false);
        _emailObj.SetActive(false);
        _confirmButton.SetActive(false);
        _goFurtherButton.SetActive(true);
        _resultObj.SetActive(true);
    }
}
