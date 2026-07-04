using System.Collections.Generic;
using UnityEngine;

public class Prison : UnitDefault
{
    [Header("감옥")]
    public PrisonObj prisonObj;

    readonly public static List<GameObject> prisonList = new();

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target) || prisonList.Contains(Target))
            return;

        SkillTarget = Target;
        prisonList.Add(SkillTarget);
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null) {
            Prison.prisonList.RemoveAll(x => x == null);
            return;
        }

        SoundPlay(skillSoundClip);
        CreateAttackBox(-1f, null, SkillTarget.transform.position);
        PrisonObj prison = Instantiate(prisonObj, SkillTarget.transform.position, Quaternion.identity, enemyField);
        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();
        prison.prisonUnit = unitComp;
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = "공격 중인 대상의 위치에 감옥을 생성해 가둡니다.\n" +
                    "감옥의 내구도는 <color=green>3</color>이며, 모든 공격에 1의 고정 피해를 입습니다.";
        
        ENSkillDesc = "Creates a prison at the location of the attacking target to imprison them.\n" +
                      "The prison has a durability of <color=green>3</color> and takes 1 fixed damage from all attacks.";

        FRSkillDesc = "Crée une prison à l'emplacement de la cible attaquante pour l'emprisonner.\n" +
                        "La prison a une durabilité de <color=green>3</color> et subit 1 point de dégâts fixes de toutes les attaques.";

        ITSkillDesc = "Crea una prigione nella posizione del bersaglio attaccante per imprigionarlo.\n" +
                        "La prigione ha una durabilità di <color=green>3</color> e subisce 1 danno fisso da tutti gli attacchi.";

        DESkillDesc = "Erstellt ein Gefängnis am Standort des angreifenden Ziels, um es einzusperren.\n" +
                        "Das Gefängnis hat eine Haltbarkeit von <color=green>3</color> und erleidet von allen Angriffen 1 festen Schaden.";

        ESSkillDesc = "Crea una prisión en la ubicación del objetivo atacante para encarcelarlo.\n" +
                      "La prisión tiene una durabilidad de <color=green>3</color> y recibe 1 punto de daño fijo de todos los ataques.";

        JASkillDesc = "攻撃中の対象の位置に刑務所を作成して閉じ込めます。\n" +
                        "刑務所の耐久度は<color=green>3</color>で、すべての攻撃から1の固定ダメージを受けます。";

        PT_BRSkillDesc = "Cria uma prisão na localização do alvo atacante para aprisioná-lo.\n" +
                         "A prisão tem uma durabilidade de <color=green>3</color> e recebe 1 de dano fixo de todos os ataques.";

        RUSkillDesc = "Создает тюрьму в месте нахождения атакуемой цели, чтобы заключить ее в тюрьму.\n" +
                        "Тюрьма имеет прочность <color=green>3</color> и получает 1 фиксированный урон от всех атак.";

        ZH_HANSSkillDesc = "在攻击目标的位置创建一个监狱以囚禁他们。\n" +
                         "监狱的耐久度为<color=green>3</color>，并且所有攻击都会造成1点固定伤害。";
    }
}
