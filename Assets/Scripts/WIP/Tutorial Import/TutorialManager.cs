using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    public DynamicPanelManager panelManager;

    [Header("Persistence")]
    public string tutorialKey = "TutorialStep";

    [Header("Behavior")]
    [Tooltip("Current tutorial step (can set default in Inspector)")]
    public int step = 0;

    [Header("Advance Timing")]
    [Tooltip("Delay in seconds between requesting the next step and actually advancing")]
    public float stepDelay = 2f;

    // internal
    private bool isAdvancing = false; // prevents multiple concurrent advance requests

    void Start()
    {
        // restore saved step
        step = PlayerPrefs.GetInt(tutorialKey, step);

        // apply UI for restored step
        if (panelManager != null)
            panelManager.ApplyStep(step);
    }

    void Update()
    {
        // clicks advance for steps (start the delayed advance)
        if (Input.GetMouseButtonDown(0))
        {
            RequestAdvanceToNextStep();
        }
    }

    /// <summary>
    /// Public request to advance to the next step. Starts the delay coroutine if not already advancing.
    /// </summary>
    public void RequestAdvanceToNextStep()
    {
        if (panelManager == null) return;
        if (isAdvancing) return; // already waiting to advance

        int maxStep = panelManager.steps != null ? panelManager.steps.Length - 1 : step;
        if (step >= maxStep)
        {
            // already at final step; just save progress and return
            SaveStep();
            return;
        }

        StartCoroutine(AdvanceToNextStepCoroutine());
    }

    /// <summary>
    /// Coroutine that waits stepDelay seconds then advances the step.
    /// </summary>
    private IEnumerator AdvanceToNextStepCoroutine()
    {
        isAdvancing = true;
        yield return new WaitForSeconds(stepDelay);

        // perform the actual advance
        AdvanceToNextStepImmediate();

        isAdvancing = false;
    }

    /// <summary>
    /// Immediate step advancement logic (no delay). Kept separate so SetStep can still set instantly.
    /// </summary>
    private void AdvanceToNextStepImmediate()
    {
        if (panelManager == null) return;

        int maxStep = panelManager.steps != null ? panelManager.steps.Length - 1 : step;
        if (step < maxStep)
        {
            step++;
            panelManager.ApplyStep(step);
            SaveStep();
        }
        else
        {
            // reached final configured step: save progress
            SaveStep();
        }
    }

    /// <summary>
    /// Directly set the tutorial step (immediate, no delay).
    /// </summary>
    public void SetStep(int newStep)
    {
        if (panelManager == null) return;

        int maxStep = panelManager.steps != null ? panelManager.steps.Length - 1 : newStep;
        newStep = Mathf.Clamp(newStep, 0, maxStep);
        if (newStep == step) return;

        step = newStep;
        panelManager.ApplyStep(step);
        SaveStep();
    }

    void SaveStep()
    {
        PlayerPrefs.SetInt(tutorialKey, step);
        PlayerPrefs.Save();
    }
}
