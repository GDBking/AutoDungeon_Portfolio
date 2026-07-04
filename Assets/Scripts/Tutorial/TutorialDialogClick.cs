using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DialogSystem))]
public class TutorialDialogClick : TutorialBase
{
    [SerializeField]
    private Button targetButton;

    private DialogSystem dialogSystem;
    private bool dialogFinished;
    private bool buttonClicked;

    public override void Enter()
    {
        dialogSystem = GetComponent<DialogSystem>();
        dialogFinished = false;
        buttonClicked = false;

        dialogSystem.Setup();

        targetButton.onClick.RemoveListener(OnButtonClick);
    }

    public override void Exeute(TutorialController controller)
    {
        if (!dialogFinished)
        {
            dialogFinished = dialogSystem.UpdateDialog();

            if (dialogFinished)
            {
                targetButton.onClick.AddListener(OnButtonClick);
            }

            return;
        }

        if (!buttonClicked)
            return;

        dialogSystem.HideUI();
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
        targetButton.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        buttonClicked = true;
    }
}