using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using TMPro;

/// <summary>
/// Used to collect data from InputFields and then get Token from the server
/// </summary>
public class logToServer : MonoBehaviour
{
    [SerializeField]
    private GameObject IPInputFieldObject;
    private TMP_InputField IPInputField;

    [SerializeField]
    private GameObject LoginInputFieldObject;
    private TMP_InputField LoginInputField;

    [SerializeField]
    private GameObject PasswordInputFieldObject;
    private TMP_InputField PasswordInputField;

    [SerializeField]
    private GameObject ErrorTextBoxObject;
    private TMP_Text ErrorTextBox;

    [SerializeField]
    private GameObject RememberLoginToggleObject;
    private UnityEngine.UI.Toggle RememberLoginToggle;

    private UnityWebRequest webRequest;

    //Used to initialize coroutine only once
    private bool coroutineState = true;

    //Temporary class for json parse
    [System.Serializable]
    class tmpClass {
        public string token;
    }

    public IEnumerator getToken()
    {
        coroutineState = true;
        webRequest = new UnityWebRequest("http://" + IPInputField.text + storedData.tokenRequest, "GET"); //Getting token requires login + password in header
        webRequest.SetRequestHeader("login", LoginInputField.text);
        webRequest.SetRequestHeader("password", PasswordInputField.text);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.DataProcessingError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            ErrorTextBox.text = webRequest.error;
        }
        else
        {
            var tmp = webRequest.downloadHandler.data;
            string json = webRequest.downloadHandler.text;
            storedData.token = JsonUtility.FromJson<tmpClass>(json).token; //Request returns token in a form of json
            ErrorTextBox.text = "Successfully logged in. Loading...";
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);

            storedData.IP = IPInputField.text;
            storedData.URL = "http://" + IPInputField.text;
            if (RememberLoginToggle.isOn)
            {
                PlayerPrefs.SetString("login", LoginInputField.text);
                PlayerPrefs.SetInt("rememberLogin", 1);
            }
            else
            {
                PlayerPrefs.DeleteKey("login");
                PlayerPrefs.DeleteKey("rememberLogin");
            }
            PlayerPrefs.SetString("IP", storedData.IP);
        }
        enabled = false;
    }

    /// <summary>
    /// Used to call getToken coroutine
    /// </summary>
    public void getTokenCall()
    {
        coroutineState = false;
        enabled = true;
        ErrorTextBox.text = "Logging in...";
    }

    private void Start()
    {
        //Getting saved data from previous session and getting references to InputFields and TextBoxes
        IPInputField = IPInputFieldObject.GetComponent<TMP_InputField>();
        IPInputField.text = PlayerPrefs.GetString("IP");
        LoginInputField = LoginInputFieldObject.GetComponent<TMP_InputField>();
        PasswordInputField = PasswordInputFieldObject.GetComponent<TMP_InputField>();
        ErrorTextBox = ErrorTextBoxObject.GetComponent<TMP_Text>();
        RememberLoginToggle = RememberLoginToggleObject.GetComponent<UnityEngine.UI.Toggle>();
        if (PlayerPrefs.GetInt("rememberLogin", 0) == 1)
        {
            RememberLoginToggle.isOn = true;
            LoginInputField.text = PlayerPrefs.GetString("login");
        }
        enabled = false;
    }

    private void Update()
    {
        if (!coroutineState)
        {
            StartCoroutine(getToken());
        }
    }
}
