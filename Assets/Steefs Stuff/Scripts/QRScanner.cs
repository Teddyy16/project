using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;

public class QRScanner : MonoBehaviour
{
    public static WebCamTexture webcamTexture;

    Texture2D snap;
    string QrCode = string.Empty;
    Coroutine scanRoutine;

    [Header("UI Output")]
    public TextMeshProUGUI scannedCodeText;

    void Awake()
    {
        if (webcamTexture == null)
        {
            string selectedCam = SelectCorrectCamera();
            webcamTexture = new WebCamTexture(selectedCam, 1920, 1080);
            Debug.Log("📸 Using camera: " + selectedCam);
        }

        GetComponent<RawImage>().texture = webcamTexture;
    }

    void OnEnable()
    {
        if (!webcamTexture.isPlaying)
            webcamTexture.Play();

        // FIX: Wait for real frame before rotating
        StartCoroutine(ApplyRotationWhenReady());

        scanRoutine = StartCoroutine(GetQRCode());
    }

    void OnDisable() => StopScanner();
    void OnDestroy() => StopScanner();
    void OnApplicationQuit() => StopScanner(true);

    void StopScanner(bool quitting = false)
    {
        if (scanRoutine != null)
        {
            StopCoroutine(scanRoutine);
            scanRoutine = null;
        }

        if (webcamTexture != null && webcamTexture.isPlaying)
            webcamTexture.Stop();

        if (quitting)
            webcamTexture = null;
    }

    // ---------------------------------------------------------
    // CAMERA PRIORITY:
    // 1. Phone back camera
    // 2. Phone front camera
    // 3. Laptop front camera
    // 4. Everything else (OBS, virtual cams)
    // ---------------------------------------------------------
    string SelectCorrectCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        Debug.Log("=== Available Cameras ===");
        foreach (var cam in devices)
            Debug.Log(cam.name + " | Front: " + cam.isFrontFacing);

        // PRIORITY 1 — Phone BACK camera
        foreach (var cam in devices)
        {
            string n = cam.name.ToLower();
            if (!cam.isFrontFacing &&
                (n.Contains("back") || n.Contains("rear") || n.Contains("iphone") || n.Contains("android")))
            {
                return cam.name;
            }
        }

        // PRIORITY 2 — Phone FRONT camera
        foreach (var cam in devices)
        {
            string n = cam.name.ToLower();
            if (cam.isFrontFacing &&
                (n.Contains("iphone") || n.Contains("android") || n.Contains("front")))
            {
                return cam.name;
            }
        }

        // PRIORITY 3 — Laptop front-facing webcam
        foreach (var cam in devices)
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

        // PRIORITY 4 — OBS / virtual cams
        foreach (var cam in devices)
        {
            string n = cam.name.ToLower();
            if (n.Contains("obs") || n.Contains("virtual"))
            {
                return cam.name;
            }
        }

        // FINAL FALLBACK
        if (devices.Length > 0)
            return devices[0].name;

        return null;
    }

    // ---------------------------------------------------------
    // ROTATION + MIRROR FIX (correct on first try)
    // ---------------------------------------------------------
    IEnumerator ApplyRotationWhenReady()
    {
        var raw = GetComponent<RawImage>();

        // Wait until camera actually produces a frame
        while (webcamTexture.width < 100)
            yield return null;

        yield return new WaitForEndOfFrame();

        int angle = webcamTexture.videoRotationAngle;

        // iPhone sometimes reports 90 when it means 270
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            angle = (angle + 180) % 360;

        raw.rectTransform.localEulerAngles = new Vector3(0, 0, -angle);

        // FIX: Always mirror on phone (front/back both need it)
        bool mirror = webcamTexture.videoVerticallyMirrored;

        raw.rectTransform.localScale = new Vector3(
            1f,
            mirror ? -1f : 1f,
            1f
        );

        Debug.Log($"📐 Rotation applied: {angle}, Mirrored: {mirror}");
    }

    // ---------------------------------------------------------
    // SCANNING LOGIC (unchanged)
    // ---------------------------------------------------------
    IEnumerator GetQRCode()
    {
        IBarcodeReader reader = new BarcodeReader();

        while (webcamTexture.width < 100)
            yield return null;

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

                if (scannedCodeText != null)
                    scannedCodeText.text = QrCode;

                if (IsNumeric(QrCode))
                {
                    var coinManager = FindAnyObjectByType<CoinManager>();
                    if (coinManager != null)
                        coinManager.AddCoins(80);
                }

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
