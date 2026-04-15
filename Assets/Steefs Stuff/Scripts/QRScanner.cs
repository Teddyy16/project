using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QRScanner : MonoBehaviour
{
    WebCamTexture webcamTexture;
    string QrCode = string.Empty;

    void Start()
    {
        // List all cameras
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"Camera {i}: {devices[i].name} | FrontFacing: {devices[i].isFrontFacing}");
        }

        // Your original camera setup
        var renderer = GetComponent<RawImage>();
        webcamTexture = new WebCamTexture(512, 512);
        renderer.texture = webcamTexture;

        StartCoroutine(GetQRCode());
    }

    IEnumerator GetQRCode()
    {
        IBarcodeReader barCodeReader = new BarcodeReader();
        webcamTexture.Play();

        var snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);

        while (true)
        {
            try
            {
                snap.SetPixels32(webcamTexture.GetPixels32());
                var Result = barCodeReader.Decode(
                    snap.GetRawTextureData(),
                    webcamTexture.width,
                    webcamTexture.height,
                    RGBLuminanceSource.BitmapFormat.ARGB32
                );

                if (Result != null)
                {
                    QrCode = Result.Text;
                    Debug.Log("DECODED TEXT: " + QrCode);

                    if (IsNumeric(QrCode))
                    {
                        Debug.Log("Numeric barcode detected → +1 coin");
                        FindObjectOfType<CoinManager>().AddCoins(1);
                    }

                    StartCoroutine(ClearAndRestart());
                    yield break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }

            yield return null;
        }
    }

    IEnumerator ClearAndRestart()
    {
        yield return new WaitForSeconds(2f);
        QrCode = string.Empty;

        StartCoroutine(GetQRCode());
    }

    bool IsNumeric(string text)
    {
        foreach (char c in text)
        {
            if (!char.IsDigit(c))
                return false;
        }
        return true;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        GUI.Label(rect, QrCode, style);
    }
}