using UnityEngine;

[RequireComponent(typeof(DialogSystem))]
public class TutorialDialog : TutorialBase
{
    private DialogSystem dialogSystem;

    public override void Enter()
    {
        dialogSystem = GetComponent<DialogSystem>();
        dialogSystem.Setup();
    }

    public override void Exeute(TutorialController controller)
    {
        // 현재 분기에 진행되는 대사 진행
        bool isCompleted = dialogSystem.UpdateDialog();

        // 현재 분기의 대사 진행이 완료되면
        if (isCompleted == true)
        {
            dialogSystem.HideUI();
            // 다음 튜토리얼로 이동
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
