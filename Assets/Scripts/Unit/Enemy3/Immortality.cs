using System.Collections;
using UnityEngine;

public class Immortality : UnitDefault
{
    [Header("불사")]
    public float resurrectionHealthPer;
    public float incLifeStealPer;
    public float decHealthAmount;

    bool isRevive;
    float elapsedTime;

    private void Update()
    {
        if (!isRevive)
            return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 1f) {
            elapsedTime = 0f;
            Hit(decHealthAmount, 0f, this, true);
        }
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    public override void Death()
    {
        isDeath = true;
        gameObject.layer = 0;
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        uiGaugeBar.SetActive(false);
        tag = "Untagged";

        if (isRevive) {
            GameManager.instance.SetScore((int)rank + 1, true);

            StaticManager.itemStat.Add(itemStat);
            RespawnManager.instance.RespawnUnit(unitID, dealMetricsIdx, CurrentMana, steroid, StaticManager.itemStat[^1], transform.parent.GetComponent<RectTransform>());
        }

        if (enemies.Contains(this))
            enemies.Remove(this);

        StartCoroutine(OnDeathAnim());
    }

    protected override IEnumerator OnDeathAnim()
    {
        if (!isRevive) {
            if (isDeathAnim)
                yield break;

            if (isStunAnim) {
                animator.SetBool("5_Debuff", false);
                isStunAnim = false;
                stunRemainingTime = 0f;
            }
            isDeathAnim = true;
            animator.speed = 1f;
            animator.SetBool("isDeath", true);
            yield return null;
            // 사망 모션이 끝날 때까지 대기
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

            health = maxHealth / 100f * resurrectionHealthPer;
            updateHpBarAction(MaxHealth, Health);
            SetStat(lifeStealPerStat, incLifeStealPer, false, true);
            SetStateBar(State.immortable, -2f);

            isDeath = false;
            tag = "Enemy Unit";
            gameObject.layer = LayerMask.NameToLayer("Enemy Unit");
            boxCollider.enabled = true;
            capsuleCollider.enabled = true;
            uiGaugeBar.SetActive(true);

            animator.SetBool("isDeath", false);
            animator.CrossFade("IDLE", 0f);

            enemies.Add(this);

            isRevive = true;
            isDeathAnim = false;

            SoundPlay(skillSoundClip);
        }
        else
            yield return base.OnDeathAnim();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"사망 시 최대 체력의 <color=green>{resurrectionHealthPer}%</color>로 부활하며 피해 흡혈이 <color=green>{incLifeStealPer}%p</color> 증가합니다.\n" +
                    $"부활 후에는 초당 <color=red>{decHealthAmount}</color>만큼 체력이 감소합니다.";

        ENSkillDesc = $"When this unit dies, it resurrects with <color=green>{resurrectionHealthPer}%</color> of max health and gains <color=green>{incLifeStealPer}%p</color> increased life steal.\n" +
                      $"After resurrection, it loses <color=red>{decHealthAmount}</color> health per second.";

        FRSkillDesc = $"Beim Tod aufersteht diese Einheit mit <color=green>{resurrectionHealthPer}%</color> der maximalen Gesundheit und erhält <color=green>{incLifeStealPer}%p</color> erhöhte Lebensraub.\n" +
                      $"Nach der Wiederauferstehung verliert sie <color=red>{decHealthAmount}</color> Gesundheit pro Sekunde.";

        ITSkillDesc = $"Alla morte, questa unità resuscita con il <color=green>{resurrectionHealthPer}%</color> della vita massima e guadagna <color=green>{incLifeStealPer}%p</color> di furto di vita in più.\n" +
                      $"Dopo la resurrezione, perde <color=red>{decHealthAmount}</color> di salute al secondo.";

        DESkillDesc = $"Beim Tod aufersteht diese Einheit mit <color=green>{resurrectionHealthPer}%</color> der maximalen Gesundheit und erhält <color=green>{incLifeStealPer}%p</color> erhöhte Lebensraub.\n" +
                      $"Nach der Wiederauferstehung verliert sie <color=red>{decHealthAmount}</color> Gesundheit pro Sekunde.";

        ESSkillDesc = $"Al morir, esta unidad resucita con el <color=green>{resurrectionHealthPer}%</color> de la salud máxima y gana <color=green>{incLifeStealPer}%p</color> de robo de vida adicional.\n" +
                      $"Después de la resurrección, pierde <color=red>{decHealthAmount}</color> de salud por segundo.";

        JASkillDesc = $"死亡時に最大生命力の<color=green>{resurrectionHealthPer}%</color>で復活し、ライフスティールが<color=green>{incLifeStealPer}%p</color>増加します。\n" +
                      $"復活後は毎秒<color=red>{decHealthAmount}</color>だけ生命力が減少します。";

        PT_BRSkillDesc = $"Al morir, esta unidad resucita con el <color=green>{resurrectionHealthPer}%</color> de la vida máxima y gana <color=green>{incLifeStealPer}%p</color> de robo de vida adicional.\n" +
                          $"Después de la resurrección, pierde <color=red>{decHealthAmount}</color> de vida por segundo.";

        RUSkillDesc = $"При смерти эта единица воскрешается с <color=green>{resurrectionHealthPer}%</color> от макс. здоровья и получает увеличенное вампирство на <color=green>{incLifeStealPer}%p</color>.\n" +
                      $"После воскрешения она теряет <color=red>{decHealthAmount}</color> здоровья в секунду.";

        ZH_HANSSkillDesc = $"死亡后以最大生命力的<color=green>{resurrectionHealthPer}%</color>复活，获得<color=green>{incLifeStealPer}%p</color>的吸血增加。\n" +
                           $"复活后每秒失去<color=red>{decHealthAmount}</color>生命值。";
    }
}
