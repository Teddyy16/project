using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;

public class QRScanner : MonoBehaviour
{
    // Global webcam instance shared across scenes
    public static WebCamTexture webcamTexture;

    Texture2D snap;
    string QrCode = string.Empty;
    Coroutine scanRoutine;

    [Header("UI Output")]
    public TextMeshProUGUI scannedCodeText;   // Drag your TMP text here

    void Awake()
    {
        // Create webcam ONCE globally
        if (webcamTexture == null)
        {
            webcamTexture = new WebCamTexture(1920, 1080);
            Debug.Log("Created global webcam texture");
        }

        // Assign webcam feed to RawImage
        GetComponent<RawImage>().texture = webcamTexture;
    }

    void OnEnable()
    {
        // Start camera if not already running
        if (!webcamTexture.isPlaying)
        {
            webcamTexture.Play();
            Debug.Log("Webcam started");
        }

        // Start scanning
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
            Debug.Log("Webcam stopped");
        }

        // Only destroy webcam on quit
        if (quitting)
        {
            webcamTexture = null;
            Debug.Log("Webcam destroyed on quit");
        }
    }

    IEnumerator GetQRCode()
    {
        IBarcodeReader reader = new BarcodeReader();

        // Wait for camera to initialize
        while (webcamTexture.width < 100)
            yield return null;

        // Create snap texture AFTER webcam is ready
        snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        while (true)
        {
            snap.SetPixels32(webcamTexture.GetPixels32());

            var result = reader.Decode(
                snap.GetRawTextureData(),
                webcamTexture.width,
                webcamTexture.height,
                RGBLuminanceSource.BitmapFormat.ARGB32
            );

            if (result != null)
            {
                QrCode = result.Text;
                Debug.Log("DECODED: " + QrCode);

                // Show scanned code in UI
                if (scannedCodeText != null)
                    scannedCodeText.text = QrCode;

                // Add coins if numeric
                if (IsNumeric(QrCode))
                {
                    var coinManager = FindAnyObjectByType<CoinManager>();
                    if (coinManager != null)
                        coinManager.AddCoins(80);
                }

                // Wait before clearing
                yield return new WaitForSeconds(2f);

                QrCode = "";
                if (scannedCodeText != null)
                    scannedCodeText.text = "";
            }

            yield return null;
        }
    }

    bool IsNumeric(string text)
    {
        foreach (char c in text)
            if (!char.IsDigit(c))
                return false;
        return true;
    }
}
