using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using UnityEngine;
using UnityEngine.Networking;
using TMPro;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;

using Vuforia;
using Vuforia.EditorClasses;

/// <summary>
/// Used to recognize frames and decode QR-codes within them
/// </summary>
public class QRCodeScan : MonoBehaviour
{
    //Used to initialize coroutine only once/control their reinitialization
    bool decodeCoroutineState = true;
    bool getDataCoroutineState = true;

    private PIXEL_FORMAT _mPixelFormat = PIXEL_FORMAT.UNKNOWN_FORMAT;

    TextMeshPro dataSheet;

    UnityWebRequest webRequest;
    string patientUUID;

    public IEnumerator OnTrackingFound()
    {
        decodeCoroutineState = true;
        yield return null;
        IBarcodeReader reader = new BarcodeReader();
        var image = CameraDevice.Instance.GetCameraImage(_mPixelFormat);
        if (image == null)
        {
            decodeCoroutineState = false;
            yield break;
        }
        var pixMap = image.Pixels;
        var result = reader.Decode(pixMap, image.Width, image.Height, RGBLuminanceSource.BitmapFormat.RGB24);
        if (result == null){
            GetComponent<Renderer>().enabled = false;
            decodeCoroutineState = false;
            yield break;
        }
        patientUUID = result.ToString();
        this.GetComponent<Renderer>().enabled = true;
        GameObject.Find("DebugTextBox").GetComponent<TextMesh>().text = "UUID: " + patientUUID;
        getDataCoroutineState = false;
}

    public IEnumerator getPatientFromServer()
    {
        getDataCoroutineState = true;
        Patient patient = new Patient();
        webRequest = new UnityWebRequest(storedData.URL + storedData.patientRequest + patientUUID, "GET"); //sending token in header
        webRequest.SetRequestHeader("token", storedData.token);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        string json = webRequest.downloadHandler.text; //receiving json with all the data. Look Patient class for reference
        patient = JsonUtility.FromJson<Patient>(json);
        DisplayPatientData.SetPatientData(patient, dataSheet);
        enabled = false;
    }

    public void OnTrackingLost()
    {
    }

    public void startScan()
    {
        decodeCoroutineState = false;
        enabled = true;
    }

    void Start()
    {
#if UNITY_EDITOR
        _mPixelFormat = PIXEL_FORMAT.GRAYSCALE;
#else
        _mPixelFormat = PIXEL_FORMAT.RGB888;
#endif
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        dataSheet = GameObject.Find("DataSheet").GetComponent<TextMeshPro>();
        VuforiaRuntime.Instance.InitVuforia();
    }

    private void OnVuforiaStarted()
    {
        var isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(_mPixelFormat, true);
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        enabled = false;
    }


    private void Update()
    {
        if (!decodeCoroutineState)
        {
            StartCoroutine(OnTrackingFound());
        }
        else if (!getDataCoroutineState)
        {
            StartCoroutine(getPatientFromServer());
        }
    }
}
