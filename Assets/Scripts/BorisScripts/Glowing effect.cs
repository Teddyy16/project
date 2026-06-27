using UnityEngine;
using UnityEngine.UI;

public class GlowingEffect : MonoBehaviour
{
    [Header("Glow")]
    public Color glowColor = new Color(1f, 0.8f, 0.15f, 1f);
    public float minGlow = 0.4f;
    public float maxGlow = 3f;
    public float pulseSpeed = 2f;

    [Header("Light")]
    public bool createPointLight = true;
    public float minLightIntensity = 0.3f;
    public float maxLightIntensity = 2f;
    public float lightRange = 2f;
    public Vector3 lightOffset = new Vector3(0f, 0.5f, 0f);

    [Header("UI Glow")]
    public bool useUIGlow = false;
    public Image uiGlowImage;
    public float minUIAlpha = 0.15f;
    public float maxUIAlpha = 0.75f;

    private Renderer[] objectRenderers;
    private Material[][] objectMaterials;
    private Light glowLight;

    private void Awake()
    {
        objectRenderers = GetComponentsInChildren<Renderer>(true);
        objectMaterials = new Material[objectRenderers.Length][];

        for (int i = 0; i < objectRenderers.Length; i++)
        {
            objectMaterials[i] = objectRenderers[i].materials;

            foreach (Material material in objectMaterials[i])
            {
                if (material != null && material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                }
            }
        }

        // Point Light e samo za 3D obekti.
        // Pri UI butoni v Canvas ne ni trqbva.
        if (createPointLight && !useUIGlow)
        {
            CreateGlowLight();
        }
    }

    private void Update()
    {
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        float glowAmount = Mathf.Lerp(minGlow, maxGlow, pulse);
        float lightAmount = Mathf.Lerp(minLightIntensity, maxLightIntensity, pulse);

        UpdateGlow(glowAmount);
        UpdateUIGlow(pulse);

        if (glowLight != null)
        {
            glowLight.intensity = lightAmount;
        }
    }

    private void CreateGlowLight()
    {
        GameObject lightObject = new GameObject("Glow Light");

        lightObject.transform.SetParent(transform);
        lightObject.transform.localPosition = lightOffset;
        lightObject.transform.localRotation = Quaternion.identity;

        glowLight = lightObject.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = glowColor;
        glowLight.range = lightRange;
        glowLight.shadows = LightShadows.None;
    }

    private void UpdateGlow(float intensity)
    {
        for (int i = 0; i < objectMaterials.Length; i++)
        {
            foreach (Material material in objectMaterials[i])
            {
                if (material != null && material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", glowColor * intensity);
                }
            }
        }
    }

    private void UpdateUIGlow(float pulse)
    {
        if (!useUIGlow || uiGlowImage == null)
            return;

        Color color = glowColor;
        color.a = Mathf.Lerp(minUIAlpha, maxUIAlpha, pulse);

        uiGlowImage.color = color;
    }

    public void StopGlow()
    {
        enabled = false;

        if (glowLight != null)
        {
            glowLight.enabled = false;
        }

        if (uiGlowImage != null)
        {
            Color color = uiGlowImage.color;
            color.a = 0f;
            uiGlowImage.color = color;
        }

        UpdateGlow(0f);
    }

    public void StartGlow()
    {
        enabled = true;

        if (glowLight != null)
        {
            glowLight.enabled = true;
        }
    }
}