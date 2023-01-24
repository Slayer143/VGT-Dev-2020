using Assets.StaticInfo;
using UnityEngine;
using UnityEngine.UI;

public class Switcher : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainMenu;

    [SerializeField]
    private GameObject _pokerMenu;

    [SerializeField]
    private GameObject _mainPokerMenu;

    [SerializeField]
    private GameObject _pokerSettings;

    [SerializeField]
    private GameObject _registration;

    [SerializeField]
    private GameObject _authorization;

    [SerializeField]
    private GameObject _regResult;

    [SerializeField]
    private GameObject _regGoFurther;

    [SerializeField]
    private GameObject _regLogin;

    [SerializeField]
    private GameObject _regPassword;

    [SerializeField]
    private GameObject _regPasswordRepeat;

    [SerializeField]
    private GameObject _regEmail;

    [SerializeField]
    private GameObject _regButton;

    [SerializeField]
    private GameObject _regChange;

    [SerializeField]
    private GameObject _authResult;

    [SerializeField]
    private GameObject _authGoFurther;

    [SerializeField]
    private GameObject _authLogin;
    
    [SerializeField]
    private GameObject _authPassword;

    [SerializeField]
    private GameObject _authButton;

    [SerializeField]
    private GameObject _authChange;

    [SerializeField]
    private GameObject _pokerGame;

    private string _callObject;

    public void SwitchToAuthorization()
    {
        _registration.SetActive(false);
        _authLogin.SetActive(true);
        _authPassword.SetActive(true);
        _authButton.SetActive(true);
        _authChange.SetActive(true);
        _authGoFurther.SetActive(false);
        _authResult.SetActive(false);
        _authorization.SetActive(true);
    }

    public void SwitchToRegistration()
    {
        _registration.SetActive(true);
        _regLogin.SetActive(true);
        _regPassword.SetActive(true);
        _regPasswordRepeat.SetActive(true);
        _regEmail.SetActive(true);
        _regButton.SetActive(true);
        _regChange.SetActive(true);
        _regGoFurther.SetActive(false);
        _regResult.SetActive(false);
        _authorization.SetActive(false);
    }

    public void SwitchToPokerMenu()
    {
        _mainMenu.SetActive(false);
        _pokerMenu.SetActive(true);
        _mainPokerMenu.SetActive(true);
    }

    public void SwitchToPokerSession()
    {
        try
        {
            _pokerSettings.SetActive(false);
            _pokerMenu.SetActive(false);
            Instantiate(_pokerGame).SetActive(true);
        }
        catch (System.Exception)
        {}

    }

    public void SwitchFromPokerSession()
    {
        _pokerMenu.SetActive(true);
        _mainPokerMenu.SetActive(true);
        _pokerGame.SetActive(false);
    }

    public void SwitchToPokerSettings()
    {
        _mainPokerMenu.SetActive(false);
        _pokerSettings.SetActive(true);
    }

    public void SwitchFromPokerSettings()
    {
        _mainPokerMenu.SetActive(true);
        _pokerSettings.SetActive(false);
    }

    public void SwitchFromPokerMenu()
    {
        _mainMenu.SetActive(true);
        _pokerMenu.SetActive(false);
    }

    public void SwitchToMainMenu(Text result)
    {
        if (result.text == "All data is correct" || result.text == "Added successfully")
        {
            print(MainInformation.PlayerInformation.Login);

            _registration.SetActive(false);
            _regGoFurther.SetActive(false);
            _regResult.SetActive(false);

            _authorization.SetActive(false);
            _authGoFurther.SetActive(false);
            _authResult.SetActive(false);

            _mainMenu.SetActive(true);
        }
        else
        {
            if (_callObject == "Registration")
            {
                _registration.SetActive(true);
                _regLogin.SetActive(true);
                _regPassword.SetActive(true);
                _regPasswordRepeat.SetActive(true);
                _regEmail.SetActive(true);
                _regButton.SetActive(true);
                _regChange.SetActive(true);
                _regGoFurther.SetActive(false);
                _regResult.SetActive(false);
                _authorization.SetActive(false);
                _authLogin.SetActive(false);
                _authPassword.SetActive(false);
                _authButton.SetActive(false);
                _authChange.SetActive(false);
                _authGoFurther.SetActive(false);
                _authResult.SetActive(false);
            }

            if (_callObject == "Authorization")
            {
                _authorization.SetActive(true);
                _authLogin.SetActive(true);
                _authPassword.SetActive(true);
                _authButton.SetActive(true);
                _authChange.SetActive(true);
                _authGoFurther.SetActive(false);
                _authResult.SetActive(false);
                _registration.SetActive(false);
                _regLogin.SetActive(false);
                _regPassword.SetActive(false);
                _regPasswordRepeat.SetActive(false);
                _regEmail.SetActive(false);
                _regButton.SetActive(false);
                _regChange.SetActive(false);
                _regGoFurther.SetActive(false);
                _regResult.SetActive(false);
            }
        }

    }

    public void Exit()
    {
        MainInformation.PlayerInformation = null;
        MainInformation.SessionInformation = null;

        _mainMenu.SetActive(false);
        _pokerMenu.SetActive(false);
        _registration.SetActive(true);
    }

    public void GetCallObjectLocation(string location)
    {
        _callObject = location;
    }
}
