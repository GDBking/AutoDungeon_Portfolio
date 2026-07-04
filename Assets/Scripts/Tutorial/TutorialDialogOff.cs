using UnityEngine;

[RequireComponent(typeof(DialogSystem))]
public class TutorialDialogOff : TutorialBase
{
    [SerializeField]
    private GameObject DialogObject;

    private DialogSystem dialogSystem;
    private bool dialogFinished;

    public override void Enter()
    {
        dialogSystem = GetComponent<DialogSystem>();
        dialogFinished = false;
        dialogSystem.Setup();
    }

    public override void Exeute(TutorialController controller)
    {
        // 1. 다이얼로그 진행
        if (!dialogFinished)
        {
            dialogFinished = dialogSystem.UpdateDialog();
            return;
        }

        // 2. 다이얼로그 완료 후, 타겟이 꺼지면 진행
        if (DialogObject != null && !DialogObject.activeSelf)
        {
            dialogSystem.HideUI();
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}