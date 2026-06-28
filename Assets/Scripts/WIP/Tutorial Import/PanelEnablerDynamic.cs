using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicPanelManager : MonoBehaviour
{
    [System.Serializable]
    public class StepPanels
    {
        [Tooltip("GameObjects (panels) to disable when this step is active.")]
        public GameObject[] panelsToHide;

        [Tooltip("Image GameObjects (panel with text child) to show for this step.")]
        public GameObject[] explanationObjects;

        [TextArea]
        [Tooltip("Optional texts to assign to the explanationObjects (order matches explanationObjects). If empty, existing child text is kept.")]
        public string[] explanationTexts;
    }

    [Tooltip("Steps configuration (per-room).")]
    public StepPanels[] steps;

    [Tooltip("Root that contains all textbox Image GameObjects. All Image children here will be hidden at start.")]
    public GameObject groupRoot;

    [Tooltip("Seconds to wait before verifying a textbox remains enabled after showing it.")]
    public float verifyDelay = 0.08f;

    [Tooltip("How many times to retry re-enabling a textbox if something else hides it.")]
    public int verifyRetries = 3;

    // internal cache of registered explanation objects (from steps)
    private HashSet<GameObject> registeredExplanationObjects = new HashSet<GameObject>();

    void Awake()
    {
        BuildRegisteredExplanationObjects();

        // Hide every Image child under groupRoot (treat them as textboxes)
        if (groupRoot != null)
        {
            foreach (var t in groupRoot.GetComponentsInChildren<Transform>(true))
            {
                if (t == null) continue;
                var go = t.gameObject;
                if (go == groupRoot) continue;
                if (go.GetComponent<Image>() != null)
                {
                    if (go.activeSelf) go.SetActive(false);
                }
            }
        }
        else
        {
            // fallback: hide registered explanation objects only
            foreach (var go in registeredExplanationObjects)
                if (go != null && go.activeSelf) go.SetActive(false);
        }
    }

    private void BuildRegisteredExplanationObjects()
    {
        registeredExplanationObjects.Clear();
        if (steps == null) return;
        foreach (var s in steps)
        {
            if (s == null || s.explanationObjects == null) continue;
            foreach (var e in s.explanationObjects)
                if (e != null) registeredExplanationObjects.Add(e);
        }

        // If groupRoot is set, filter to its descendants only
        if (groupRoot != null)
        {
            var allowed = new HashSet<GameObject>();
            foreach (var t in groupRoot.GetComponentsInChildren<Transform>(true))
                if (t != null) allowed.Add(t.gameObject);

            registeredExplanationObjects.RemoveWhere(go => go == null || !allowed.Contains(go));
        }
    }

    /// <summary>
    /// Apply the given step index:
    /// - disable only the panels listed in panelsToHide for that step,
    /// - enable only the explanationObjects for that step (set text if provided),
    /// - hide other registered explanation objects.
    /// </summary>
    public void ApplyStep(int stepIndex)
    {
        if (steps == null || steps.Length == 0)
        {
            Debug.LogWarning("DynamicPanelManager: no steps configured.");
            return;
        }

        if (stepIndex < 0 || stepIndex >= steps.Length)
        {
            Debug.LogWarning($"DynamicPanelManager: stepIndex {stepIndex} out of range.");
            return;
        }

        BuildRegisteredExplanationObjects();

        // 1) Disable only the panels listed for this step
        var toHide = steps[stepIndex].panelsToHide;
        if (toHide != null)
        {
            foreach (var p in toHide)
            {
                if (p == null) continue;
                if (p.activeSelf)
                {
                    p.SetActive(false);
                    Debug.Log($"DynamicPanelManager: hid panel {p.name}");
                }
            }
        }

        // 2) Show only the explanation objects for this step (set text if provided)
        var boxes = steps[stepIndex].explanationObjects;
        var texts = steps[stepIndex].explanationTexts;
        HashSet<GameObject> currentBoxes = new HashSet<GameObject>();

        if (boxes != null)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                var go = boxes[i];
                if (go == null) continue;
                currentBoxes.Add(go);

                // set text if provided (prefer TMP, fallback to legacy Text)
                string textToSet = (texts != null && i < texts.Length) ? texts[i] : null;
                if (!string.IsNullOrEmpty(textToSet))
                {
                    var tmp = go.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (tmp != null) tmp.text = textToSet;
                    else
                    {
                        var legacy = go.GetComponentInChildren<Text>(true);
                        if (legacy != null) legacy.text = textToSet;
                    }
                }

                // Ensure parents and UI components are active/enabled before setting active
                ActivateParents(go);
                EnsureUIComponentsEnabled(go);

                if (!go.activeSelf)
                {
                    go.SetActive(true);
                    Debug.Log($"DynamicPanelManager: showed explanation {go.name}");
                }

                // start verification coroutine to ensure it stays enabled
                StartCoroutine(VerifyRemainsEnabled(go, verifyDelay, verifyRetries));
            }
        }

        // 3) Hide all other registered explanation objects
        foreach (var go in registeredExplanationObjects)
        {
            if (go == null) continue;
            if (currentBoxes.Contains(go)) continue;
            if (go.activeSelf)
            {
                go.SetActive(false);
                Debug.Log($"DynamicPanelManager: hid other explanation {go.name}");
            }
        }
    }

    /// <summary>
    /// Hide every registered explanation object, then show only the allowed one.
    /// Optionally set its child text.
    /// </summary>
    public void ShowOnlyExplanation(GameObject allowed, string optionalText = null)
    {
        BuildRegisteredExplanationObjects();

        // hide all registered explanation objects
        foreach (var go in registeredExplanationObjects)
            if (go != null && go.activeSelf) go.SetActive(false);

        if (allowed == null)
        {
            Debug.LogWarning("ShowOnlyExplanation: allowed GameObject is null. All explanation objects hidden.");
            return;
        }

        // set text if provided
        if (!string.IsNullOrEmpty(optionalText))
        {
            var tmp = allowed.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null) tmp.text = optionalText;
            else
            {
                var legacy = allowed.GetComponentInChildren<Text>(true);
                if (legacy != null) legacy.text = optionalText;
            }
        }

        ActivateParents(allowed);
        EnsureUIComponentsEnabled(allowed);

        if (!allowed.activeSelf)
        {
            allowed.SetActive(true);
            Debug.Log($"DynamicPanelManager: ShowOnlyExplanation showed {allowed.name}");
        }

        StartCoroutine(VerifyRemainsEnabled(allowed, verifyDelay, verifyRetries));
    }

    /// <summary>
    /// Convenience: find an explanation GameObject by name under groupRoot (or among registered)
    /// and show only it.
    /// </summary>
    public void ShowOnlyExplanationByName(string name, string optionalText = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("ShowOnlyExplanationByName: name is empty.");
            return;
        }

        BuildRegisteredExplanationObjects();

        GameObject found = null;

        if (groupRoot != null)
        {
            foreach (var t in groupRoot.GetComponentsInChildren<Transform>(true))
            {
                if (t == null) continue;
                if (t.gameObject.name == name) { found = t.gameObject; break; }
            }
        }

        if (found == null)
        {
            foreach (var go in registeredExplanationObjects)
            {
                if (go == null) continue;
                if (go.name == name) { found = go; break; }
            }
        }

        if (found == null)
        {
            Debug.LogWarning($"ShowOnlyExplanationByName: no GameObject named '{name}' found under groupRoot or registered explanation objects.");
            return;
        }

        ShowOnlyExplanation(found, optionalText);
    }

    // Verify the object remains enabled; if something else hides it, try to re-enable a few times and log.
    private IEnumerator VerifyRemainsEnabled(GameObject go, float delay, int retries)
    {
        if (go == null) yield break;

        int attempts = 0;
        while (attempts < retries)
        {
            yield return new WaitForSeconds(delay);

            if (go == null) yield break;
            if (go.activeSelf)
            {
                // success — it stayed enabled
                yield break;
            }
            else
            {
                // it was hidden by something else — try to re-enable and log
                Debug.LogWarning($"DynamicPanelManager: '{go.name}' was hidden after being shown. Re-enabling (attempt {attempts + 1}/{retries}).");
                ActivateParents(go);
                EnsureUIComponentsEnabled(go);
                go.SetActive(true);
            }

            attempts++;
        }

        // final check
        if (go != null && !go.activeSelf)
            Debug.LogError($"DynamicPanelManager: '{go.name}' could not be kept enabled after {retries} retries. Something else is overriding its active state.");
    }

    // Helper: activate all parent GameObjects up the hierarchy
    private void ActivateParents(GameObject go)
    {
        if (go == null) return;
        Transform t = go.transform.parent;
        while (t != null)
        {
            if (!t.gameObject.activeSelf)
                t.gameObject.SetActive(true);
            t = t.parent;
        }
    }

    // Helper: ensure common UI components are enabled so the object can render
    private void EnsureUIComponentsEnabled(GameObject go)
    {
        if (go == null) return;

        // enable Image component if present
        var img = go.GetComponent<Image>();
        if (img != null && !img.enabled) img.enabled = true;

        // enable CanvasGroup on this object (not parent) if present and set alpha to 1
        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            if (cg.alpha == 0f) cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        // enable text components under the object
        var tmp = go.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null && !tmp.enabled) tmp.enabled = true;
        else
        {
            var legacy = go.GetComponentInChildren<Text>(true);
            if (legacy != null && !legacy.enabled) legacy.enabled = true;
        }

        // ensure scale is not zero
        if (go.transform.localScale == Vector3.zero) go.transform.localScale = Vector3.one;
    }

    // Optional helper to hide all explanation objects immediately
    public void HideAllExplanationObjects()
    {
        BuildRegisteredExplanationObjects();
        foreach (var go in registeredExplanationObjects)
            if (go != null && go.activeSelf) go.SetActive(false);
    }
}
