using UnityEngine;

public class Frenzy : UnitDefault
{
    [Header("광폭")]
    public float dotDamage;
    public float size;

    float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= 1f && !isDeath) {
            elapsedTime = 0f;
            SoundPlay(skillSoundClip);
            attackSize = size;
            CreateAttackBox(dotDamage, null, transform.position, AttackStyle.range, true, true);
            attackSize = 0.5f;
        }
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"본인 반경 <color=#AEEAFF>{size}x{size}</color>범위의 적들에게 매 초 <color=red>{dotDamage}</color>의 방어력 관통 피해를 입힙니다.";

        ENSkillDesc = $"Deals {""} <color=red>{dotDamage}</color> defense-piercing damage per second to enemies within a <color=#AEEAFF>{size}x{size}</color> area around this unit.";

        FRSkillDesc = $"Inflige <color=red>{dotDamage}</color> de dégâts contournant la défense par seconde aux ennemis dans une zone de <color=#AEEAFF>{size}x{size}</color> autour de cette unité.";

        ITSkillDesc = $"Infligge <color=red>{dotDamage}</color> danni che ignorano la difesa al secondo ai nemici in un'area di <color=#AEEAFF>{size}x{size}</color> attorno a questa unità.";

        DESkillDesc = $"Verursacht <color=red>{dotDamage}</color> verteidigungsdurchdringenden Schaden pro Sekunde an Gegnern in einem <color=#AEEAFF>{size}x{size}</color> Bereich um diese Einheit.";

        ESSkillDesc = $"Inflige <color=red>{dotDamage}</color> de daño que ignora la defensa por segundo a los enemigos dentro de un área de <color=#AEEAFF>{size}x{size}</color> alrededor de esta unidad.";

        JASkillDesc = $"このユニットの周囲<color=#AEEAFF>{size}x{size}</color>範囲の敵に毎秒<color=red>{dotDamage}</color>の防御貫通ダメージを与えます。";

        PT_BRSkillDesc = $"Causa <color=red>{dotDamage}</color> de dano que perfura a defesa por segundo aos inimigos dentro de uma área de <color=#AEEAFF>{size}x{size}</color> ao redor desta unidade.";

        RUSkillDesc = $"Наносит <color=red>{dotDamage}</color> урона с пробитием защиты в секунду врагам в области <color=#AEEAFF>{size}x{size}</color> вокруг этой единицы.";

        ZH_HANSSkillDesc = $"对该单位周围<color=#AEEAFF>{size}x{size}</color>区域内的敌人每秒造成<color=red>{dotDamage}</color>点无视防御的伤害。";
    }
}
