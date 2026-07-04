using UnityEngine;

public class Asmodeus : UnitDefault
{
    [Header("패시브")]
    public float decAttackAmount;
    public float decDefenseAmount;
    public GameObject asmodeusState;
    [Header("광란")]
    public float duration;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        foreach (UnitDefault unitComp in friends) {
            GameObject effect = Instantiate(attackObj, unitComp.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

            unitComp.Madness(duration);
        }
    }

    public override void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        SoundPlay(attackSoundClip);

        AttackTarget.GetComponent<UnitDefault>().AsmodeusDebuff(asmodeusState, decAttackAmount, decDefenseAmount);
        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, false);
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"패시브: 기본 공격에 피격된 적은 공격력이 <color=red>{decAttackAmount}</color>, 방어력이 <color=red>{decDefenseAmount}</color>만큼 감소합니다.\n" +
                    $"(최대 <color=yellow>5번</color>까지 중첩)\n\n" +
                    $"광란: 모든 적 유닛들이 <color=yellow>{duration}초</color> 동안 광란 상태에 빠집니다.";
        
        ENSkillDesc = $"Passive: Enemies hit by basic attacks have their attack power reduced by <color=red>{decAttackAmount}</color> and defense reduced by <color=red>{decDefenseAmount}</color>.\n" +
                      $"(Stacks up to <color=yellow>5 times</color>)\n\n" +
                      $"Madness: All enemy units enter a state of madness for <color=yellow>{duration} seconds</color>.";

        FRSkillDesc = $"Passif : Les ennemis touchés par des attaques de base voient leur puissance d'attaque réduite de <color=red>{decAttackAmount}</color> et leur défense réduite de <color=red>{decDefenseAmount}</color>.\n" +
                      $"(Cumulable jusqu'à <color=yellow>5 fois</color>)\n\n" +
                      $"Folie : Toutes les unités ennemies entrent dans un état de folie pendant <color=yellow>{duration} secondes</color>.";

        ITSkillDesc = $"Passivo: I nemici colpiti dagli attacchi base vedono la loro potenza di attacco ridotta di <color=red>{decAttackAmount}</color> e la loro difesa ridotta di <color=red>{decDefenseAmount}</color>.\n" +
                      $"(Si accumula fino a <color=yellow>5 volte</color>)\n\n" +
                      $"Follia: Tutte le unità nemiche entrano in uno stato di follia per <color=yellow>{duration} secondi</color>.";

        DESkillDesc = $"Passiv: Gegner, die von Grundangriffen getroffen werden, haben ihre Angriffskraft um <color=red>{decAttackAmount}</color> und ihre Verteidigung um <color=red>{decDefenseAmount}</color> reduziert.\n" +
                      $"(Bis zu <color=yellow>5 Mal</color> stapelbar)\n\n" +
                      $"Wahnsinn: Alle feindlichen Einheiten geraten für <color=yellow>{duration} Sekunden</color> in einen Zustand des Wahnsinns.";

        ESSkillDesc = $"Pasivo: Los enemigos golpeados por ataques básicos tienen su poder de ataque reducido en <color=red>{decAttackAmount}</color> y su defensa reducida en <color=red>{decDefenseAmount}</color>.\n" +
                      $"(Se acumula hasta <color=yellow>5 veces</color>)\n\n" +
                      $"Locura: Todas las unidades enemigas entran en un estado de locura durante <color=yellow>{duration} segundos</color>.";

        JASkillDesc = $"パッシブ: 通常攻撃に被弾した敵は、攻撃力が<color=red>{decAttackAmount}</color>、防御力が<color=red>{decDefenseAmount}</color>だけ減少します。\n" +
                      $"(最大<color=yellow>5回</color>まで重複)\n\n" +
                      $"狂乱: すべての敵ユニットが<color=yellow>{duration}秒</color>間、狂乱状態に陥ります。";

        PT_BRSkillDesc = $"Passivo: Inimigos atingidos por ataques básicos têm seu poder de ataque reduzido em <color=red>{decAttackAmount}</color> e defesa reduzida em <color=red>{decDefenseAmount}</color>.\n" +
                         $"(Acumula até <color=yellow>5 vezes</color>)\n\n" +
                         $"Loucura: Todas as unidades inimigas entram em um estado de loucura por <color=yellow>{duration} segundos</color>.";

        RUSkillDesc = $"Пассивное: Урон от базовых атак снижает силу атаки противников на <color=red>{decAttackAmount}</color> и защиту на <color=red>{decDefenseAmount}</color>.\n" +
                      $"(Складывается до <color=yellow>5 раз</color>)\n\n" +
                      $"Безумие: Все вражеские юниты впадают в состояние безумия на <color=yellow>{duration} секунд</color>.";

        ZH_HANSSkillDesc = $"被动：受到普通攻击的敌人，攻击力降低<color=red>{decAttackAmount}</color>，防御力降低<color=red>{decDefenseAmount}</color>。\n" +
                           $"（最多可叠加<color=yellow>5次</color>）\n\n" +
                           $"狂乱：所有敌方单位进入狂乱状态，持续<color=yellow>{duration}秒</color>。";
    }
}