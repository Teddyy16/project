using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;

public class QRScanner : MonoBehaviour
{
    public static WebCamTexture webcamTexture;
public AnimalData animalData;
public WeeklyQuest weeklyQuest;
  

    private Texture2D snap;
    private string qrCode = string.Empty;
    private Coroutine scanRoutine;

    [Header("UI Output")]
    public TextMeshProUGUI scannedCodeText;

    [Header("Reward")]
    public int rewardCoins = 10;
    public bool allowSameCodeOnlyOnce = true;

    void Awake()
    {
        if (webcamTexture == null)
        {
            string selectedCam = SelectCorrectCamera();

            if (string.IsNullOrEmpty(selectedCam))
            {
                Debug.LogError("No camera found.");
                return;
            }

            webcamTexture = new WebCamTexture(selectedCam, 1920, 1080);
            Debug.Log("Using camera: " + selectedCam);
        }

        RawImage rawImage = GetComponent<RawImage>();

        if (rawImage != null)
        {
            rawImage.texture = webcamTexture;
        }
        else
        {
            Debug.LogError("QRScanner must be attached to an object with RawImage.");
        }
    }

    void OnEnable()
    {
        if (webcamTexture == null)
        {
            Debug.LogError("WebCamTexture is missing.");
            return;
        }

        if (!webcamTexture.isPlaying)
        {
            webcamTexture.Play();
        }

        StartCoroutine(ApplyRotationWhenReady());

        scanRoutine = StartCoroutine(GetQRCode());
    }

    void OnDisable()
    {
        StopScanner();
    }

    void OnDestroy()
    {
        StopScanner();
    }

    void OnApplicationQuit()
    {
        StopScanner(true);
    }

    void StopScanner(bool quitting = false)
    {
        if (scanRoutine != null)
        {
            StopCoroutine(scanRoutine);
            scanRoutine = null;
        }

        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }

        if (quitting)
        {
            webcamTexture = null;
        }
    }

    string SelectCorrectCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        Debug.Log("=== Available Cameras ===");

        foreach (WebCamDevice cam in devices)
        {
            Debug.Log(cam.name + " | Front: " + cam.isFrontFacing);
        }

        foreach (WebCamDevice cam in devices)
        {
            string n = cam.name.ToLower();

            if (!cam.isFrontFacing &&
                (n.Contains("back") || n.Contains("rear") || n.Contains("iphone") || n.Contains("android")))
            {
                return cam.name;
            }
        }

        foreach (WebCamDevice cam in devices)
        {
            string n = cam.name.ToLower();

            if (cam.isFrontFacing &&
                (n.Contains("iphone") || n.Contains("android") || n.Contains("front")))
            {
                return cam.name;
            }
        }

        foreach (WebCamDevice cam in devices)
        {
            string n = cam.name.ToLower();

            if (cam.isFrontFacing ||
                n.Contains("integrated") ||
                n.Contains("built") ||
                n.Contains("webcam") ||
                n.Contains("hd camera") ||
                n.Contains("face"))
            {
                return cam.name;
            }
        }

        foreach (WebCamDevice cam in devices)
        {
            string n = cam.name.ToLower();

            if (n.Contains("obs") || n.Contains("virtual"))
            {
                return cam.name;
            }
        }

        if (devices.Length > 0)
        {
            return devices[0].name;
        }

        return null;
    }

    IEnumerator ApplyRotationWhenReady()
    {
        RawImage raw = GetComponent<RawImage>();

        if (raw == null)
        {
            yield break;
        }

        while (webcamTexture != null && webcamTexture.width < 100)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        int angle = webcamTexture.videoRotationAngle;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            angle = (angle + 180) % 360;
        }

        raw.rectTransform.localEulerAngles = new Vector3(0, 0, -angle);

        bool mirror = webcamTexture.videoVerticallyMirrored;

        raw.rectTransform.localScale = new Vector3(
            1f,
            mirror ? -1f : 1f,
            1f
        );

        Debug.Log("Rotation applied: " + angle + ", Mirrored: " + mirror);
    }

    IEnumerator GetQRCode()
    {
        IBarcodeReader reader = new BarcodeReader();

        while (webcamTexture != null && webcamTexture.width < 100)
        {
            yield return null;
        }

        snap = new Texture2D(
            webcamTexture.width,
            webcamTexture.height,
            TextureFormat.ARGB32,
            false
        );

        while (true)
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
                qrCode = result.Text;

                Debug.Log("DECODED: " + qrCode);

                if (scannedCodeText != null)
                {
                    if (weeklyQuest.unlocked.Count >= weeklyQuest.maxIcons)
                    {
                        animalData.UnlockRabbit();
                    }
                    var coinManager = FindAnyObjectByType<CoinManager>();
                    if (coinManager != null)
                        coinManager.AddCoins(80);
                    scannedCodeText.text = qrCode;
                }

                TryRewardPlayer(qrCode);

                yield return new WaitForSeconds(2f);

                qrCode = "";

                if (scannedCodeText != null)
                {
                    scannedCodeText.text = "";
                }
            }

            yield return null;
        }
    }

    private void TryRewardPlayer(string scannedCode)
    {
        if (string.IsNullOrEmpty(scannedCode))
        {
            return;
        }

        if (!IsNumeric(scannedCode))
        {
            Debug.Log("Scanned code is not numeric. No coins added: " + scannedCode);
            return;
        }

        if (allowSameCodeOnlyOnce)
        {
            string saveKey = "ScannedReceipt_" + scannedCode;

            if (PlayerPrefs.GetInt(saveKey, 0) == 1)
            {
                Debug.Log("This receipt/barcode was already scanned: " + scannedCode);

                if (scannedCodeText != null)
                {
                    scannedCodeText.text = "Already scanned";
                }

                return;
            }

            PlayerPrefs.SetInt(saveKey, 1);
        }

        int currentCoins = PlayerPrefs.GetInt("Coin", 0);
        currentCoins += rewardCoins;

        PlayerPrefs.SetInt("Coin", currentCoins);
        PlayerPrefs.Save();

        Debug.Log("Added " + rewardCoins + " coins. Current coins: " + currentCoins);

        if (scannedCodeText != null)
        {
            scannedCodeText.text = "Scanned!\n+" + rewardCoins + " coins";
        }
    }

    bool IsNumeric(string text)
    {
        foreach (char c in text)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}