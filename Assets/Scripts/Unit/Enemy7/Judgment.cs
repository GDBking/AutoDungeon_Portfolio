using UnityEngine;

public class Judgment : UnitDefault
{
    [Header("심판")]
    public float duration;
    public float size;

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(size / 2f, Target))
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        CameraShake.instance.Shake(1f, 2f);

        GameObject effect = Instantiate(attackObj, transform.position, Quaternion.identity);
        effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);
        effect.transform.localScale *= size;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<UnitDefault>().OnStunAnim(duration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 반경 <color=#AEEAFF>{size}X{size}</color>범위 이내에 있는 적 유닛들을 <color=yellow>{duration}초</color> 동안 기절시킵니다.";
        
        ENSkillDesc = $"Stuns enemy units within a radius of <color=#AEEAFF>{size}X{size}</color> around self for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Étourdit les unités ennemies dans un rayon de <color=#AEEAFF>{size}X{size}</color> autour de soi pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Stordisce le unità nemiche entro un raggio di <color=#AEEAFF>{size}X{size}</color> intorno a sé per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Betäubt feindliche Einheiten im Umkreis von <color=#AEEAFF>{size}X{size}</color> um sich selbst für <color=yellow>{duration} Sekunden</color>.";

        ESSkillDesc = $"Aturde a las unidades enemigas dentro de un radio de <color=#AEEAFF>{size}X{size}</color> alrededor de uno mismo durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"自身の半径<color=#AEEAFF>{size}X{size}</color>範囲内にいる敵ユニットを<color=yellow>{duration}秒</color>間気絶させます。";

        PT_BRSkillDesc = $"Aturde as unidades inimigas dentro de um raio de <color=#AEEAFF>{size}X{size}</color> ao redor de si mesmo por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Оглушает вражеские юниты в радиусе <color=#AEEAFF>{size}X{size}</color> вокруг себя на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"使自身半径<color=#AEEAFF>{size}X{size}</color>范围内的敌方单位眩晕<color=yellow>{duration}秒</color>。";
    }
}