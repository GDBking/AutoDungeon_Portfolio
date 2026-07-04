using UnityEngine;

public class Illusion : UnitDefault
{
    public Scarecrow scarecrow;
    public float duration;

    protected override void Awake()
    {
        base.Awake();

        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        skillTargetPos = SkillTarget.transform.position;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;

        Scarecrow scarecrowComp = Instantiate(scarecrow, pos + (SkillTarget != null ? new Vector2(SkillTarget.GetComponent<UnitDefault>().curFlipX ? -1f : 1f, 0f) : Vector2.zero), Quaternion.identity, enemyField);
        scarecrowComp.duration = duration;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 적 유닛 뒤에 모든 적을 도발하는 허수아비를 생성합니다.\n" +
                    $"(최대 <color=yellow>{duration}초</color> 동안 유지)";

        ENSkillDesc = $"Creates a scarecrow behind the attacked enemy that taunts all enemies.\n" +
                      $"(Lasts up to <color=yellow>{duration} seconds</color>)";

        FRSkillDesc = $"Crée un épouvantail derrière l'ennemi attaqué qui attire tous les ennemis.\n" +
                      $"(Dure jusqu'à <color=yellow>{duration} secondes</color>)";

        ITSkillDesc = $"Crea uno spaventapasseri dietro il nemico attaccato che provoca tutti i nemici.\n" +
                      $"(Dura fino a <color=yellow>{duration} secondi</color>)";

        DESkillDesc = $"Erstellt eine Vogelscheuche hinter dem angegriffenen Gegner, die alle Feinde provoziert.\n" +
                      $"(Hält bis zu <color=yellow>{duration} Sekunden</color>)";

        ESSkillDesc = $"Crea un espantapájaros detrás del enemigo atacado que provoca a todos los enemigos.\n" +
                      $"(Dura hasta <color=yellow>{duration} segundos</color>)";

        JASkillDesc = $"攻撃中の敵の背後に、すべての敵を挑発するかかしを生成します。\n" +
                      $"（最大<color=yellow>{duration}秒</color>持続）";

        PT_BRSkillDesc = $"Cria um espantalho atrás do inimigo atacado que provoca todos os inimigos.\n" +
                         $"(Dura até <color=yellow>{duration} segundos</color>)";

        RUSkillDesc = $"Создаёт пугало позади атакуемого врага, которое провоцирует всех врагов.\n" +
                      $"(Длится до <color=yellow>{duration} секунд</color>)";

        ZH_HANSSkillDesc = $"在被攻击的敌人身后生成一个嘲讽所有敌人的稻草人。\n" +
                           $"（持续时间最大为 <color=yellow>{duration} 秒</color>）";
    }
}
