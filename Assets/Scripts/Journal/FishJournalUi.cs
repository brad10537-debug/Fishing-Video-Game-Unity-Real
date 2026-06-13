using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Opens/closes the Fish Journal with the old Input Manager.
// Attach this to the HUD Canvas and assign a panel GameObject plus Text.
public class FishJournalUi : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.J;
    public GameObject journalPanel;
    public Text journalText;
    public CanvasGroup journalCanvasGroup;
    public RectTransform journalRectTransform;
    public FishJournal journal;

    private bool isOpen;
    private bool initialized;
    private Coroutine animationRoutine;

    private void Awake()
    {
        if (journal == null)
        {
            journal = FindAnyObjectByType<FishJournal>();
        }

        SetOpen(false);
    }

    private void OnEnable()
    {
        if (journal != null)
        {
            journal.JournalChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (journal != null)
        {
            journal.JournalChanged -= Refresh;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            SetOpen(!isOpen);
        }
        else if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            SetOpen(false);
        }
    }

    public void SetOpen(bool open)
    {
        if (initialized && isOpen == open)
        {
            if (isOpen)
            {
                Refresh();
            }

            return;
        }

        bool wasInitialized = initialized;
        initialized = true;
        isOpen = open;

        if (journalPanel != null)
        {
            if (journalCanvasGroup == null)
            {
                journalCanvasGroup = journalPanel.GetComponent<CanvasGroup>();
            }

            if (journalRectTransform == null)
            {
                journalRectTransform = journalPanel.GetComponent<RectTransform>();
            }

            journalPanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            Refresh();
            PlayOpenFeedback();
        }
        else if (wasInitialized)
        {
            PlayCloseFeedback();
        }
    }

    public void Refresh()
    {
        if (journalText == null)
        {
            return;
        }

        journalText.text = journal != null
            ? journal.BuildJournalText()
            : "Fish Journal\nNo FishJournal component found on the player.";
    }

    private void PlayOpenFeedback()
    {
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayJournalOpen();
        }

        if (journalCanvasGroup != null && journalRectTransform != null)
        {
            StartPanelAnimation(0.92f, 1f, 0f, 1f);
        }
    }

    private void PlayCloseFeedback()
    {
        if (GameAudioFeedback.Instance != null)
        {
            GameAudioFeedback.Instance.PlayJournalClose();
        }
    }

    private void StartPanelAnimation(float fromScale, float toScale, float fromAlpha, float toAlpha)
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }

        animationRoutine = StartCoroutine(AnimatePanel(fromScale, toScale, fromAlpha, toAlpha));
    }

    private IEnumerator AnimatePanel(float fromScale, float toScale, float fromAlpha, float toAlpha)
    {
        float timer = 0f;
        const float seconds = 0.16f;

        while (timer < seconds)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / seconds);
            journalCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            journalRectTransform.localScale = Vector3.one * Mathf.Lerp(fromScale, toScale, t);
            yield return null;
        }

        journalCanvasGroup.alpha = toAlpha;
        journalRectTransform.localScale = Vector3.one * toScale;
        animationRoutine = null;
    }
}
