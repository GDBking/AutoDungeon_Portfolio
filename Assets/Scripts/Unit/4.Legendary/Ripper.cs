using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Ripper : UnitDefault
{
    [Header("컬렉션")]
    public float incAttackAmount;
    public float incAttackSpeedAmount;
    public float incLifeStealPerAmount;

    public List<AnimationClip> stackAttackAnimClip;
    public TextMeshProUGUI stackCntTxt;

    float IncAttackAmount { get => incAttackAmount + StaticManager.skillPoint[unitIdx] * 2f; }
    float IncAttackSpeedAmount { get => incAttackSpeedAmount + StaticManager.skillPoint[unitIdx] / 3 * 0.1f; }
    float IncLifeStealPerAmount { get => incLifeStealPerAmount + StaticManager.skillPoint[unitIdx] * 2f; }

    int collection;


    protected override void Start()
    {
        base.Start();

        uiStateBar.GetChild(0).gameObject.SetActive(true);
        attackAnimClip = stackAttackAnimClip[0];
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());

        collection++;
        if (collection % 2 == 0 && collection <= (stackAttackAnimClip.Count - 1) * 2) {
            attackAnimClip = stackAttackAnimClip[collection / 2];
        }

        stackCntTxt.SetText(collection.ToString());
        SetStat(attackPowerStat, IncAttackAmount, false, true);
        SetStat(attackSpeedStat, IncAttackSpeedAmount, false, true);
        SetStat(lifeStealPerStat, IncLifeStealPerAmount, false, true);
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, transform.position);
    }

    public override float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1 : 1;
        }
        // 회전 이펙트를 적용 하지 않은 경우
        else {
            // 방향에 따른 이펙트 좌우 반전
            scale.x *= !curFlipX ? -1f : 1f;
        }
        // 스케일 변경 및 적용
        scale *= attackSize;
        effect.transform.localScale = scale;

        if (attackPower == -1f)
            return 0f;

        // 박스 캐스트 수행 (회전 적용)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0) {
            if (attackStyle == AttackStyle.single) {
                foreach (Collider2D enemy in hitEnemies) {
                    if (enemy.gameObject == target) {
                        UnitDefault unitComp = target.GetComponent<UnitDefault>();
                        damage = unitComp.Hit(attackPower, CriticalPer, this, isPenetration);
                        if (unitComp.isDeath)
                            UseSkill();
                        break;
                    }
                }
            }
            else {
                for (int i = hitEnemies.Length - 1; i >= 0; i--)
                    damage += hitEnemies[i].GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
            }

            // 피흡 계산
            float lifeSteal = damage * LifeStealPer / 100f;
            Healing(lifeSteal);

            DealMetrics.instance.UpdateDealMetrics(dealMetricsIdx, damage);
        }
        return damage;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"적 유닛에게 최후의 일격을 가하면 컬렉션을 1개 획득합니다.\n" +
                    $"컬렉션 1개당 효과: 공격력 <color=green>+{IncAttackAmount}</color>, 공격 속도 <color=green>+{IncAttackSpeedAmount}</color>, 피해 흡혈 <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                    $"(사망 시 컬렉션 초기화)";
        
        ENSkillDesc = $"Gains 1 collection upon delivering the final blow to an enemy unit.\n" +
                      $"Effect per collection: Attack Power <color=green>+{IncAttackAmount}</color>, Attack Speed <color=green>+{IncAttackSpeedAmount}</color>, Life Steal <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(Collections reset upon death)";

        FRSkillDesc = $"Gagne 1 collection en portant le coup de grâce à une unité ennemie.\n" +
                      $"Effet par collection : Puissance d'attaque <color=green>+{IncAttackAmount}</color>, Vitesse d'attaque <color=green>+{IncAttackSpeedAmount}</color>, Vol de vie <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(Les collections sont réinitialisées à la mort)";

        ITSkillDesc = $"Ottiene 1 collezione infliggendo il colpo di grazia a un'unità nemica.\n" +
                      $"Effetto per collezione: Potenza di attacco <color=green>+{IncAttackAmount}</color>, Velocità di attacco <color=green>+{IncAttackSpeedAmount}</color>, Furto di vita <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(Le collezioni si azzerano alla morte)";

        DESkillDesc = $"Erhält 1 Sammlung, wenn es der letzten Schlag gegen eine feindliche Einheit verpasst.\n" +
                      $"Effekt pro Sammlung: Angriffskraft <color=green>+{IncAttackAmount}</color>, Angriffsgeschwindigkeit <color=green>+{IncAttackSpeedAmount}</color>, Lebensraub <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(Sammlungen werden beim Tod zurückgesetzt)";

        ESSkillDesc = $"Gana 1 colección al dar el golpe final a una unidad enemiga.\n" +
                      $"Efecto por colección: Poder de ataque <color=green>+{IncAttackAmount}</color>, Velocidad de ataque <color=green>+{IncAttackSpeedAmount}</color>, Robo de vida <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(Las colecciones se reinician al morir)";

        JASkillDesc = $"敵ユニットにとどめを刺すと、コレクションを1つ獲得します。\n" +
                      $"コレクション1つ当たりの効果: 攻撃力<color=green>+{IncAttackAmount}</color>、攻撃速度<color=green>+{IncAttackSpeedAmount}</color>、被ダメージ吸収<color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(死亡時にコレクションがリセットされます)";

        PT_BRSkillDesc = $"Ganha 1 coleção ao dar o golpe final em uma unidade inimiga.\n" +
                         $"Efeito por coleção: Poder de ataque <color=green>+{IncAttackAmount}</color>, Velocidade de ataque <color=green>+{IncAttackSpeedAmount}</color>, Roubo de vida <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                         $"(As coleções são reiniciadas ao morrer)";

        RUSkillDesc = $"Получает 1 коллекцию при нанесении смертельного удара вражескому юниту.\n" +
                      $"Эффект за коллекцию: Сила атаки <color=green>+{IncAttackAmount}</color>, Скорость атаки <color=green>+{IncAttackSpeedAmount}</color>, Кража жизни <color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                      $"(Коллекции сбрасываются при смерти)";

        ZH_HANSSkillDesc = $"对敌方单位给予致命一击时，获得1个收藏品。\n" +
                           $"每个收藏品的效果：攻击力<color=green>+{IncAttackAmount}</color>，攻击速度<color=green>+{IncAttackSpeedAmount}</color>，生命偷取<color=green>+{IncLifeStealPerAmount}%p</color>\n" +
                           $"（死亡时收藏品重置）";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko"))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "공격력 증가량 <color=green>+2</color>\n" +
                   "피해 흡혈 증가량 <color=green>+2%p</color>\n" +
                   "<color=#FFBF00>3P</color>당 효과\n" +
                   "공격 속도 증가량 <color=green>+0.1</color>";

        if (code.StartsWith("en"))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Attack power increase <color=green>+2</color>\n" +
                   "Life steal increase <color=green>+2 percentage points</color>\n" +
                   "<color=#FFBF00>Effect per 3P</color>\n" +
                   "Attack speed increase <color=green>+0.1</color>";

        if (code.StartsWith("fr"))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Augmentation de l’attaque <color=green>+2</color>\n" +
                   "Vol de vie augmenté <color=green>+2 points de pourcentage</color>\n" +
                   "<color=#FFBF00>Effet par 3P</color>\n" +
                   "Vitesse d’attaque augmentée <color=green>+0.1</color>";

        if (code.StartsWith("it"))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Aumento attacco <color=green>+2</color>\n" +
                   "Aumento furto di vita <color=green>+2 punti percentuali</color>\n" +
                   "<color=#FFBF00>Effetto per 3P</color>\n" +
                   "Velocità d’attacco aumentata <color=green>+0.1</color>";

        if (code.StartsWith("de"))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Angriffserhöhung <color=green>+2</color>\n" +
                   "Lebensraub erhöht <color=green>+2 Prozentpunkte</color>\n" +
                   "<color=#FFBF00>Wirkung pro 3P</color>\n" +
                   "Angriffsgeschwindigkeit erhöht <color=green>+0.1</color>";

        if (code.StartsWith("es"))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Aumento de ataque <color=green>+2</color>\n" +
                   "Aumento de robo de vida <color=green>+2 puntos porcentuales</color>\n" +
                   "<color=#FFBF00>Efecto por 3P</color>\n" +
                   "Velocidad de ataque aumentada <color=green>+0.1</color>";

        if (code.StartsWith("ja"))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "攻撃力増加 <color=green>+2</color>\n" +
                   "ライフスティール増加 <color=green>+2パーセンテージポイント</color>\n" +
                   "<color=#FFBF00>3Pごとの効果</color>\n" +
                   "攻撃速度増加 <color=green>+0.1</color>";

        if (code.StartsWith("pt"))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Aumento de ataque <color=green>+2</color>\n" +
                   "Aumento de roubo de vida <color=green>+2 pontos percentuais</color>\n" +
                   "<color=#FFBF00>Efeito por 3P</color>\n" +
                   "Velocidade de ataque aumentada <color=green>+0.1</color>";

        if (code.StartsWith("ru"))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Увеличение атаки <color=green>+2</color>\n" +
                   "Увеличение вампиризма <color=green>+2 процентных пункта</color>\n" +
                   "<color=#FFBF00>Эффект за 3P</color>\n" +
                   "Увеличение скорости атаки <color=green>+0.1</color>";

        if (code.StartsWith("zh"))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "攻击力提升 <color=green>+2</color>\n" +
                   "生命偷取提升 <color=green>+2 个百分点</color>\n" +
                   "<color=#FFBF00>每3P效果</color>\n" +
                   "攻击速度提升 <color=green>+0.1</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Attack power increase <color=green>+2</color>\n" +
               "Life steal increase <color=green>+2 percentage points</color>\n" +
               "<color=#FFBF00>Effect per 3P</color>\n" +
               "Attack speed increase <color=green>+0.1</color>";
    }
}