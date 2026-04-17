using System.Collections.Generic;
using System.Text;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AuditionMinigame : MonoBehaviour
{
    [Header("UI")]
    public Text arrowText;
    public Slider timerSlider;

    [Header("Round Settings")]
    public float timeLimit = 4f;
    public int sequenceLength = 5;
    public int totalRounds = 3;
    public int additionalArrowsPerRound = 1;
    public float timerSpeed = 1f;
    public float timerSpeedIncreasePerRound = 0f;

    [Header("Round Indicator")]
    public RectTransform roundIndicatorContainer;
    public Vector2 roundIndicatorSize = new Vector2(28f, 28f);
    public float roundIndicatorSpacing = 12f;
    public Color roundIndicatorInactiveColor = new Color(1f, 1f, 1f, 0.25f);
    public Color roundIndicatorCompleteColor = new Color(1f, 1f, 1f, 0.95f);

    private readonly List<KeyCode> sequence = new List<KeyCode>();
    private readonly List<Image> roundIndicators = new List<Image>();

    private int currentIndex = 0;
    private int currentRound = 0;
    private float timer;
    private float currentRoundTimeLimit;
    private float currentTimerSpeed;
    private bool isPlaying = false;
    private bool controlsLockedForMinigame = false;
    private bool previousCursorLocked = true;
    private bool previousLookInputEnabled = true;
    private bool previousControllerEnabled = true;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    public bool IsCompleted { get; private set; }

    void Awake()
    {
        ConfigureArrowText();
        EnsureRoundIndicatorContainer();
        EnsureRoundIndicators();
    }

    public void SetupGame()
    {
        IsCompleted = false;
        LockPlayerControls();
        currentRound = 0;
        BeginRound();
    }

    void OnDisable()
    {
        isPlaying = false;
        RestorePlayerControls();
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        if (sequence.Count > 0 && currentIndex >= sequence.Count)
        {
            CompleteRound();
            return;
        }

        timer -= Time.deltaTime * currentTimerSpeed;

        if (timerSlider != null)
        {
            timerSlider.value = Mathf.Clamp01(timer / currentRoundTimeLimit);
        }

        if (timer <= 0f)
        {
            Fail();
            return;
        }

        if (WasAnyKeyPressedThisFrame())
        {
            if (WasKeyPressedThisFrame(sequence[currentIndex]))
            {
                currentIndex++;
                RefreshArrowDisplay();

                if (currentIndex >= sequence.Count)
                {
                    CompleteRound();
                }
            }
            else if (IsAnyArrowPressed())
            {
                Fail();
            }
        }
    }

    private void BeginRound()
    {
        isPlaying = true;
        currentIndex = 0;
        currentRoundTimeLimit = Mathf.Max(0.1f, timeLimit);
        currentTimerSpeed = Mathf.Max(0.1f, timerSpeed + (currentRound * timerSpeedIncreasePerRound));
        timer = currentRoundTimeLimit;

        if (timerSlider != null)
        {
            timerSlider.value = 1f;
        }

        GenerateSequence();
        RefreshRoundIndicators();
    }

    private void GenerateSequence()
    {
        sequence.Clear();

        KeyCode[] keys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        int arrowsThisRound = Mathf.Max(1, sequenceLength + (currentRound * additionalArrowsPerRound));

        for (int i = 0; i < arrowsThisRound; i++)
        {
            sequence.Add(keys[Random.Range(0, keys.Length)]);
        }

        RefreshArrowDisplay();
    }

    private void CompleteRound()
    {
        currentRound++;
        RefreshRoundIndicators();

        if (currentRound >= Mathf.Max(1, totalRounds))
        {
            Success();
            return;
        }

        BeginRound();
    }

    private bool IsAnyArrowPressed()
    {
        return WasKeyPressedThisFrame(KeyCode.UpArrow) ||
               WasKeyPressedThisFrame(KeyCode.DownArrow) ||
               WasKeyPressedThisFrame(KeyCode.LeftArrow) ||
               WasKeyPressedThisFrame(KeyCode.RightArrow);
    }

    private void Success()
    {
        isPlaying = false;
        IsCompleted = true;
        Debug.Log("Audition minigame complete.");
        gameObject.SetActive(false);
    }

    private void Fail()
    {
        isPlaying = false;
        IsCompleted = false;
        Debug.Log("Audition minigame failed. Closing minigame.");
        gameObject.SetActive(false);
    }

    private void RefreshArrowDisplay()
    {
        if (arrowText == null)
        {
            return;
        }

        ConfigureArrowText();

        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < sequence.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(' ');
            }

            string arrow = GetArrowSymbol(sequence[i]);

            if (i < currentIndex)
            {
                builder.Append("<color=#22C55E>").Append(arrow).Append("</color>");
            }
            else
            {
                builder.Append(arrow);
            }
        }

        arrowText.text = builder.ToString();
    }

    private void ConfigureArrowText()
    {
        if (arrowText == null)
        {
            return;
        }

        arrowText.supportRichText = true;
        arrowText.resizeTextForBestFit = true;
        arrowText.resizeTextMaxSize = Mathf.Max(arrowText.fontSize, 18);
        arrowText.resizeTextMinSize = 18;
        arrowText.horizontalOverflow = HorizontalWrapMode.Wrap;
        arrowText.verticalOverflow = VerticalWrapMode.Overflow;
        arrowText.alignment = TextAnchor.MiddleCenter;
    }

    private static string GetArrowSymbol(KeyCode keyCode)
    {
        return keyCode switch
        {
            KeyCode.UpArrow => "\u2191",
            KeyCode.DownArrow => "\u2193",
            KeyCode.LeftArrow => "\u2190",
            KeyCode.RightArrow => "\u2192",
            _ => "?"
        };
    }

    private void EnsureRoundIndicatorContainer()
    {
        if (roundIndicatorContainer != null)
        {
            ConfigureRoundIndicatorContainer(roundIndicatorContainer);
            return;
        }

        GameObject containerObject = new GameObject("RoundIndicators", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(ContentSizeFitter));
        containerObject.transform.SetParent(transform, false);

        roundIndicatorContainer = containerObject.GetComponent<RectTransform>();
        roundIndicatorContainer.anchorMin = new Vector2(0.5f, 0.5f);
        roundIndicatorContainer.anchorMax = new Vector2(0.5f, 0.5f);
        roundIndicatorContainer.pivot = new Vector2(0.5f, 0.5f);
        roundIndicatorContainer.anchoredPosition = new Vector2(0f, 0f);
        roundIndicatorContainer.sizeDelta = new Vector2(0f, roundIndicatorSize.y);

        ConfigureRoundIndicatorContainer(roundIndicatorContainer);
    }

    private void ConfigureRoundIndicatorContainer(RectTransform container)
    {
        HorizontalLayoutGroup layoutGroup = container.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = container.gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = roundIndicatorSpacing;

        ContentSizeFitter fitter = container.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = container.gameObject.AddComponent<ContentSizeFitter>();
        }

        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void EnsureRoundIndicators()
    {
        EnsureRoundIndicatorContainer();

        int desiredCount = Mathf.Max(1, totalRounds);

        while (roundIndicators.Count < desiredCount)
        {
            GameObject indicatorObject = new GameObject($"RoundIndicator_{roundIndicators.Count + 1}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            indicatorObject.transform.SetParent(roundIndicatorContainer, false);

            RectTransform indicatorRect = indicatorObject.GetComponent<RectTransform>();
            indicatorRect.sizeDelta = roundIndicatorSize;

            Image indicatorImage = indicatorObject.GetComponent<Image>();
            indicatorImage.raycastTarget = false;
            indicatorImage.color = roundIndicatorInactiveColor;

            roundIndicators.Add(indicatorImage);
        }

        while (roundIndicators.Count > desiredCount)
        {
            Image indicatorImage = roundIndicators[roundIndicators.Count - 1];
            roundIndicators.RemoveAt(roundIndicators.Count - 1);

            if (indicatorImage != null)
            {
                Destroy(indicatorImage.gameObject);
            }
        }
    }

    private void RefreshRoundIndicators()
    {
        EnsureRoundIndicators();

        for (int i = 0; i < roundIndicators.Count; i++)
        {
            if (roundIndicators[i] == null)
            {
                continue;
            }

            roundIndicators[i].color = i < currentRound ? roundIndicatorCompleteColor : roundIndicatorInactiveColor;
        }
    }

    private void LockPlayerControls()
    {
        if (controlsLockedForMinigame)
        {
            ResetPlayerInputState();
            return;
        }

        CachePlayerReferences();

        if (starterAssetsInputs != null)
        {
            previousCursorLocked = starterAssetsInputs.cursorLocked;
            previousLookInputEnabled = starterAssetsInputs.cursorInputForLook;

            starterAssetsInputs.cursorLocked = false;
            starterAssetsInputs.cursorInputForLook = false;
        }

        if (thirdPersonController != null)
        {
            previousControllerEnabled = thirdPersonController.enabled;
            thirdPersonController.enabled = false;
        }

        ResetPlayerInputState();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        controlsLockedForMinigame = true;
    }

    private void RestorePlayerControls()
    {
        if (!controlsLockedForMinigame)
        {
            return;
        }

        CachePlayerReferences();

        if (thirdPersonController != null)
        {
            thirdPersonController.enabled = previousControllerEnabled;
        }

        if (starterAssetsInputs != null)
        {
            starterAssetsInputs.cursorLocked = previousCursorLocked;
            starterAssetsInputs.cursorInputForLook = previousLookInputEnabled;
        }

        ResetPlayerInputState();
        Cursor.lockState = previousCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !previousCursorLocked;
        controlsLockedForMinigame = false;
    }

    private void CachePlayerReferences()
    {
        if (thirdPersonController == null)
        {
            thirdPersonController = FindFirstObjectByType<ThirdPersonController>();
        }

        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = FindFirstObjectByType<StarterAssetsInputs>();
        }
    }

    private void ResetPlayerInputState()
    {
        if (starterAssetsInputs == null)
        {
            return;
        }

        starterAssetsInputs.MoveInput(Vector2.zero);
        starterAssetsInputs.LookInput(Vector2.zero);
        starterAssetsInputs.JumpInput(false);
        starterAssetsInputs.SprintInput(false);
    }

    private bool WasAnyKeyPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
#else
        return Input.anyKeyDown;
#endif
    }

    private bool WasKeyPressedThisFrame(KeyCode keyCode)
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return false;
        }

        return keyCode switch
        {
            KeyCode.UpArrow => keyboard.upArrowKey.wasPressedThisFrame,
            KeyCode.DownArrow => keyboard.downArrowKey.wasPressedThisFrame,
            KeyCode.LeftArrow => keyboard.leftArrowKey.wasPressedThisFrame,
            KeyCode.RightArrow => keyboard.rightArrowKey.wasPressedThisFrame,
            _ => false
        };
#else
        return Input.GetKeyDown(keyCode);
#endif
    }
}
