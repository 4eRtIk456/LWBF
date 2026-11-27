
using System.Collections;
using UnityEngine;

public class OptionsPopup : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private RectTransform _optionsPanel;
    [SerializeField] private CanvasGroup _darkOverlay;

    [Header("Animation")]
    [SerializeField] private float duration = 0.3f;

    private CanvasGroup panelGroup;
    private bool isAnimating = false;

    private void Awake()
    {
        panelGroup = _optionsPanel.GetComponent<CanvasGroup>();

        _optionsPanel.localScale = Vector3.one * 0.65f;
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;
        _optionsPanel.gameObject.SetActive(false);

        _darkOverlay.alpha = 0f;
        _darkOverlay.blocksRaycasts = false;
    }

    public void OpenOptions()
    {
        if (isAnimating || _optionsPanel.gameObject.activeSelf)
            return;

        _optionsPanel.gameObject.SetActive(true);
        _darkOverlay.blocksRaycasts = true;

        StartCoroutine(AnimatePopup(true));
    }

    public void CloseOptions()
    {
        if (isAnimating || !_optionsPanel.gameObject.activeSelf)
            return;

        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;

        StartCoroutine(AnimatePopup(false));
    }

    private IEnumerator AnimatePopup(bool opening)
    {
        isAnimating = true;

        float startScale = _optionsPanel.localScale.x;
        float endScale = opening ? 1f : 0.65f;

        float startAlpha = panelGroup.alpha;
        float endAlpha = opening ? 1f : 0f;

        float startDark = _darkOverlay.alpha;
        float endDark = opening ? 0.7f : 0f;

        float elapsed = 0f;
        float animationTime = Mathf.Max(0.01f, duration);

        while (elapsed < animationTime)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / animationTime);

            float smooth = 1f - Mathf.Pow(1f - t, 3f);

            float scale = Mathf.Lerp(startScale, endScale, smooth);

            if (opening)
                scale += Mathf.Sin(t * Mathf.PI) * 0.04f;

            _optionsPanel.localScale = Vector3.one * scale;

            panelGroup.alpha = Mathf.Lerp(
                startAlpha, endAlpha, smooth
            );

            _darkOverlay.alpha = Mathf.Lerp(
                startDark, endDark, smooth
            );

            yield return null;
        }

        _optionsPanel.localScale = Vector3.one * endScale;
        panelGroup.alpha = endAlpha;
        _darkOverlay.alpha = endDark;

        if (opening)
        {
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;
        }
        else
        {
            _optionsPanel.gameObject.SetActive(false);
            _darkOverlay.blocksRaycasts = false;
        }

        isAnimating = false;
    }
}
