using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Tutorial
{
    [Serializable]
    public class TutorialBlock
    {
        public string heading;
        public string storyText;
        public string instructionText;
        [JsonIgnore]
        public Action onShow;
        [JsonIgnore]
        public Action onHide;
        public float duration;
        public TutorialBlock() { }
        public TutorialBlock(string heading, string storyText, string instructionText, Action onShow = null, Action onHide = null, float duration = 14)
        {
            this.heading = heading;
            this.storyText = storyText;
            this.instructionText = instructionText;
            this.onShow = onShow;
            this.onHide = onHide;
            this.duration = duration;
        }
        public TutorialBlock(string instructionText, Action onShow = null, Action onHide = null, float duration = 14)
        {
            heading = "";
            storyText = "";
            this.instructionText = instructionText;
            this.onShow = onShow;
            this.onHide = onHide;
            this.duration = duration;
        }
    }
    public static bool tutorialActive;
    private static UIItem tutorialPanel;
    private static ButtonTemplate esc;

    public static void SetupTutorial()
    {
        tutorialPanel = new UIItem(GameObject.Find("Tutorial"), ToolsSystem.PauseType.Inventory, ToolsSystem.PauseType.InGame, ToolsSystem.PauseType.Animation);
        tutorialPanel.IsInLevel = true;
        tutorialPanel.Go.GetComponent<RectTransform>().transform.localPosition = new Vector2(0, -1080);
        tutorialPanel.AddTransition(new Lerpable(1, new Vector2(0, -540), ToolsUI.easeIn, false, true, null, () =>
        {
            tutorialActive = false;
        }), "reveal");
        esc = new ButtonTemplate(GameObject.Find("EscButton"), true, true, null, null, ToolsSystem.PauseType.Inventory, ToolsSystem.PauseType.InGame);
        esc.OnMouseDown = (UIItem button) => { Tutorial.CloseTutorial(); };
        esc.OnMouseUp = (UIItem button) => { (button as ButtonTemplate).Deselect(); };
    }


    public static void ShowTutorial(TutorialBlock block)
    {
        if (block.onShow != null)
        {
            block.onShow();
        }
        tutorialActive = true;
        tutorialPanel.GetTransitable("reveal").onReturnEnd = () =>
        {
            tutorialActive = false;
            if (block.onHide != null)
            {
                block.onHide();
            }
        };
        tutorialPanel.StartTransition("reveal", true);
        if (block.heading == "" && block.storyText == "")
        {
            tutorialPanel.Go.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>().margin = new Vector4(100, 25, 183.4996f, 25);
        }
        else
        {
            tutorialPanel.Go.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>().margin = new Vector4(100, 240.0917f, 183.4996f, 25);
        }
        tutorialPanel.Go.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = block.heading;
        tutorialPanel.Go.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = block.storyText;
        tutorialPanel.Go.transform.GetChild(1).GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = block.instructionText;
        tutorialPanel.AddAction(new ItemAction((ActionHandler item, object[] parameters) => { (item as UIItem).ReturnTransition("reveal"); }, ToolsMath.SecondsToFrames(block.duration), ItemAction.ExecutionType.OnlyFirstTime, ItemAction.OnLeaveType.KeepRunning));
    }
    public static void CloseTutorial()
    {
        tutorialPanel.ReturnTransition("reveal");
    }
}

