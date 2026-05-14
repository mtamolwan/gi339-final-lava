using TMPro;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance;

    public TextMeshProUGUI questText;

    private int buttonsPressed = 0;
    private int totalButtons = 3;

    private bool generatorFixed = false;

    void Awake()
    {
        Instance = this;
        RefreshUI();
    }

    public void UpdateButtons(int amount)
    {
        buttonsPressed = amount;
        RefreshUI();
    }

    public void CompleteGenerator()
    {
        generatorFixed = true;
        RefreshUI();
    }

    void RefreshUI()
    {
        string buttonStatus =
            buttonsPressed >= totalButtons
            ? "DONE"
            : "INCOMPLETE";

        string generatorStatus =
            generatorFixed
            ? "DONE"
            : "INCOMPLETE";

        questText.text =
    "OBJECTIVES\n\n" +

    (buttonsPressed >= totalButtons
    ? "<color=green>- Activate Reactor Buttons (3/3) : DONE</color>\n"
    : "- Activate Reactor Buttons (" + buttonsPressed + "/" + totalButtons + ") : INCOMPLETE\n")

    +

    (generatorFixed
       ? "<color=green>- Repair Generator [Press E] : DONE</color>"
       : "- Repair Generator [Press E] : INCOMPLETE");
    }
}