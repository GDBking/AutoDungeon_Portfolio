using UnityEngine;

public class DashAttack : UnitDefault
{
    [Header("진격")]
    public float size;
    public float duration;

    bool isRush;

    protected override void FixedUpdate()
    {
        if (isRush) {
            OnMove();
            if (Target == null || isDeath) {
                isRush = false;
                SetStat(speedStat, 3f, false, false);
                rigid2D.mass = 1f;
            }
            else if (CheckForTargetRange(AttackRange, Target)) {
                isRush = false;
                SetStat(speedStat, 3f, false, false);
                rigid2D.mass = 1f;

                SoundPlay(skillSoundClip);
                attackSize = size;
                CreateAttackBox(-1f, null, transform.position, AttackStyle.range);
                attackSize = 0.5f;

                Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
                foreach (Collider2D enemy in hitEnemys) {
                    enemy.GetComponent<UnitDefault>().OnStunAnim(duration);
                }
            }
            sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);
            IsFlipX();
        }
        else
            base.FixedUpdate();
    }

    protected override void UseSkill()
    {
        if (Target == null)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (Target == null) 
            return;

        isRush = true;
        SetStat(speedStat, 3f, false, true);
        rigid2D.mass = 3f;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"가장 가까운 적 유닛에게 돌진하여 반경 <color=#AEEAFF>{size}x{size}</color>범위의 적들을 <color=yellow>{duration}초</color> 동안 기절시킵니다.";

        ENSkillDesc = $"Dashes toward the nearest enemy unit and stuns enemies within a <color=#AEEAFF>{size}x{size}</color> area for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Se précipite vers l'unité ennemie la plus proche et étourdit les ennemis dans une zone de <color=#AEEAFF>{size}x{size}</color> pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Scatta verso l'unità nemica più vicina e stordisce i nemici in un'area di <color=#AEEAFF>{size}x{size}</color> per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Stürzt auf die nächstgelegene feindliche Einheit zu und betäubt Gegner in einem Bereich von <color=#AEEAFF>{size}x{size}</color> für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Embiste hacia la unidad enemiga más cercana y aturde a los enemigos en un área de <color=#AEEAFF>{size}x{size}</color> durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"最も近い敵ユニットに突進し、<color=#AEEAFF>{size}x{size}</color>の範囲の敵を<color=yellow>{duration}秒</color>間気絶させます。";

        PT_BRSkillDesc = $"Avança em direção à unidade inimiga mais próxima e atordoa inimigos em uma área de <color=#AEEAFF>{size}x{size}</color> por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Рывок к ближайшему вражескому юниту и оглушение врагов в области <color=#AEEAFF>{size}x{size}</color> на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"向最近的敌方单位突进，使<color=#AEEAFF>{size}x{size}</color>范围的敌人眩晕<color=yellow>{duration}秒</color>。";
    }
}