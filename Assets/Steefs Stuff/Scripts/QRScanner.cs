using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZXing;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class QRScanner : MonoBehaviour
{
    public static WebCamTexture webcamTexture;

    [Header("Optional Game References")]
    public AnimalData animalData;
    public WeeklyQuest weeklyQuest;

    [Header("UI")]
    public RawImage cameraPreview;
    public TextMeshProUGUI scannedCodeText;

    [Header("Reward")]
    public int rewardCoins = 10;
    public bool allowSameCodeOnlyOnce = true;

    [Header("Scanner Settings")]
    [Tooltip("How often the camera is checked for a QR code.")]
    public float scanInterval = 0.25f;

    [Tooltip("Use the back camera when available.")]
    public bool preferBackCamera = true;

    private Texture2D snap;
    private Coroutine scannerRoutine;
    private Coroutine setupRoutine;

    private bool scannerReady;
    private bool showingMessage;

    private void Awake()
    {
        if (cameraPreview == null)
        {
            cameraPreview = GetComponent<RawImage>();
        }

        if (cameraPreview == null)
        {
            Debug.LogError("QRScanner needs a RawImage.");
        }
    }

    private void OnEnable()
    {
        setupRoutine = StartCoroutine(StartScanner());
    }

    private void OnDisable()
    {
        StopScanner();
    }

    private void OnDestroy()
    {
        StopScanner();
    }

    private IEnumerator StartScanner()
    {
        scannerReady = false;
        ShowMessage("Starting camera...");

        yield return StartCoroutine(RequestCameraPermission());

        if (!HasCameraPermission())
        {
            ShowMessage("Camera permission is required.");
            Debug.LogError("Camera permission was denied.");
            yield break;
        }

        string selectedCamera = SelectCorrectCamera();

        if (string.IsNullOrEmpty(selectedCamera))
        {
            ShowMessage("No camera found.");
            Debug.LogError("No camera device was found.");
            yield break;
        }

        if (webcamTexture == null || webcamTexture.deviceName != selectedCamera)
        {
            if (webcamTexture != null && webcamTexture.isPlaying)
            {
                webcamTexture.Stop();
            }

            webcamTexture = new WebCamTexture(selectedCamera, 1280, 720, 30);
        }

        if (cameraPreview != null)
        {
            cameraPreview.texture = webcamTexture;
        }

        webcamTexture.Play();

        float waitTime = 0f;

        while (webcamTexture != null && webcamTexture.width < 100 && waitTime < 10f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        if (webcamTexture == null || webcamTexture.width < 100)
        {
            ShowMessage("Camera could not start.");
            Debug.LogError("Camera did not initialise.");
            yield break;
        }

        ApplyCameraRotation();

        snap = new Texture2D(
            webcamTexture.width,
            webcamTexture.height,
            TextureFormat.ARGB32,
            false
        );

        scannerReady = true;
        ShowMessage("Scan a QR code");

        scannerRoutine = StartCoroutine(ScanQRCode());
    }

    private IEnumerator RequestCameraPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);

            float waitTime = 0f;

            while (!Permission.HasUserAuthorizedPermission(Permission.Camera) && waitTime < 10f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }
        }
#elif UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }
#else
        yield return null;
#endif
    }

    private bool HasCameraPermission()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
        return Application.HasUserAuthorization(UserAuthorization.WebCam);
#else
        return true;
#endif
    }

    private void ApplyCameraRotation()
    {
        if (cameraPreview == null || webcamTexture == null)
        {
            return;
        }

        int angle = webcamTexture.videoRotationAngle;

        cameraPreview.rectTransform.localEulerAngles =
            new Vector3(0f, 0f, -angle);

        float verticalScale = webcamTexture.videoVerticallyMirrored ? -1f : 1f;

        cameraPreview.rectTransform.localScale =
            new Vector3(1f, verticalScale, 1f);
    }

    private IEnumerator ScanQRCode()
    {
        IBarcodeReader reader = new BarcodeReader();

        while (scannerReady && webcamTexture != null && webcamTexture.isPlaying)
        {
            if (!showingMessage && webcamTexture.didUpdateThisFrame)
            {
                try
                {
                    snap.SetPixels32(webcamTexture.GetPixels32());

                    Result result = reader.Decode(
                        snap.GetRawTextureData(),
                        webcamTexture.width,
                        webcamTexture.height,
                        RGBLuminanceSource.BitmapFormat.ARGB32
                    );

                    if (result != null)
                    {
                        HandleScannedResult(result);
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarning("Scanner error: " + exception.Message);
                }
            }

            yield return new WaitForSeconds(scanInterval);
        }
    }

    private void HandleScannedResult(Result result)
    {
        if (result == null || string.IsNullOrEmpty(result.Text))
        {
            return;
        }

        Debug.Log(
            "Detected: " + result.Text +
            " | Format: " + result.BarcodeFormat
        );

        showingMessage = true;

        if (result.BarcodeFormat != BarcodeFormat.QR_CODE)
        {
            StartCoroutine(ShowTemporaryMessage(
                "Wrong code type.\nPlease scan a QR code.",
                2f
            ));

            return;
        }

        TryRewardPlayer(result.Text);
    }

    private void TryRewardPlayer(string qrText)
    {
        if (string.IsNullOrEmpty(qrText))
        {
            StartCoroutine(ShowTemporaryMessage("Invalid QR code.", 2f));
            return;
        }

        if (allowSameCodeOnlyOnce)
        {
            string scanKey = "ScannedQR_" + qrText;

            if (PlayerPrefs.GetInt(scanKey, 0) == 1)
            {
                StartCoroutine(ShowTemporaryMessage(
                    "This QR code was already scanned.",
                    2f
                ));

                return;
            }

            PlayerPrefs.SetInt(scanKey, 1);
        }

        int currentCoins = PlayerPrefs.GetInt("Coin", 0);
        currentCoins += rewardCoins;

        PlayerPrefs.SetInt("Coin", currentCoins);
        PlayerPrefs.Save();

        Debug.Log(
            "Added " + rewardCoins +
            " coins. Total: " + currentCoins
        );

        CheckAnimalUnlock();

        StartCoroutine(ShowTemporaryMessage(
            "QR scanned!\n+" + rewardCoins + " coins",
            2f
        ));
    }

    private void CheckAnimalUnlock()
    {
        if (weeklyQuest == null || animalData == null)
        {
            return;
        }

        if (weeklyQuest.unlocked.Count >= weeklyQuest.maxIcons)
        {
            animalData.UnlockRabbit();
        }
    }

    private IEnumerator ShowTemporaryMessage(string message, float seconds)
    {
        ShowMessage(message);

        yield return new WaitForSeconds(seconds);

        if (scannerReady)
        {
            ShowMessage("Scan a QR code");
        }

        showingMessage = false;
    }

    private void ShowMessage(string message)
    {
        if (scannedCodeText != null)
        {
            scannedCodeText.text = message;
        }
    }

    private string SelectCorrectCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices == null || devices.Length == 0)
        {
            return null;
        }

        foreach (WebCamDevice device in devices)
        {
            Debug.Log(
                "Camera: " + device.name +
                " | Front-facing: " + device.isFrontFacing
            );
        }

        if (preferBackCamera)
        {
            foreach (WebCamDevice device in devices)
            {
                if (!device.isFrontFacing)
                {
                    return device.name;
                }
            }
        }

        return devices[0].name;
    }

    private void StopScanner()
    {
        scannerReady = false;
        showingMessage = false;

        if (setupRoutine != null)
        {
            StopCoroutine(setupRoutine);
            setupRoutine = null;
        }

        if (scannerRoutine != null)
        {
            StopCoroutine(scannerRoutine);
            scannerRoutine = null;
        }

        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}