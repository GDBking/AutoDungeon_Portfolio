using System.Collections.Generic;
using UnityEngine;

public class ChronicPoisoning : UnitDefault
{
    [Header("연독")]
    public float poisonDamage;
    public float extraPoisonDamage;
    public int count;

    readonly List<(float dist, UnitDefault enemy)> unitDist = new();

    protected override void UseSkill()
    {
        if (!CheckForTargetRange(AttackRange, Target))
            return;

        SkillTarget = Target;
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        if (SkillTarget == null)
            return;

        SoundPlay(skillSoundClip);
        
        UnitDefault unitComp = SkillTarget.GetComponent<UnitDefault>();
        if (unitComp.poisonCount > 0) {
            unitDist.Clear();

            foreach (UnitDefault enemy in friends) {
                if (enemy == unitComp) continue;

                float dist = Vector2.Distance(SkillTarget.transform.position, enemy.transform.position);
                unitDist.Add((dist, enemy));
            }

            // 거리 기준 오름차순 정렬
            unitDist.Sort((a, b) => a.dist.CompareTo(b.dist));

            foreach ((float, UnitDefault) enemy in unitDist) {
                if (enemy.Item2.poisonCount == 0) {
                    CreateAttackBox(-1f, enemy.Item2.gameObject, enemy.Item2.transform.position);
                    enemy.Item2.Poison(poisonDamage, count, this);
                    return;
                }
            }

            foreach (UnitDefault enemy in friends) {
                CreateAttackBox(-1f, null, enemy.transform.position);
                enemy.Poison(extraPoisonDamage, enemy.poisonCount, this);
            }
        }
        else {
            CreateAttackBox(-1f, null, SkillTarget.transform.position);
            unitComp.Poison(poisonDamage, count, this);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"공격 중인 대상에게 <color=yellow>{count}초</color> 동안 독 효과를 입힙니다.\n" +
                    $"상대가 이미 독에 걸린 상태라면 대상으로부터 가장 가까운 독에 걸리지 않은 적 유닛에게 전염시킵니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>\n" +
                    $"모든 적 유닛이 독에 걸린 상태라면 모든 적 유닛에게 독 데미지를 <color=red>{extraPoisonDamage}</color>만큼 추가합니다.";
        
        ENSkillDesc = $"Inflicts poison on the attacking target for <color=yellow>{count} seconds</color>.\n" +
                      $"If the target is already poisoned, it spreads to the nearest unpoisoned enemy unit from the target.\n" +
                      $"Poison damage per second: <color=red>{poisonDamage}</color>\n" +
                      $"If all enemy units are poisoned, it adds an additional <color=red>{extraPoisonDamage}</color> poison damage to all enemy units.";

        FRSkillDesc = $"Inflige du poison à la cible attaquante pendant <color=yellow>{count} secondes</color>.\n" +
                        $"Si la cible est déjà empoisonnée, elle se propage à l'unité ennemie non empoisonnée la plus proche de la cible.\n" +
                        $"Dégâts de poison par seconde : <color=red>{poisonDamage}</color>\n" +
                        $"Si toutes les unités ennemies sont empoisonnées, elle ajoute <color=red>{extraPoisonDamage}</color> de dégâts de poison supplémentaires à toutes les unités ennemies.";

        ITSkillDesc = $"Infligge veleno al bersaglio attaccante per <color=yellow>{count} secondi</color>.\n" +
                      $"Se il bersaglio è già avvelenato, si diffonde all'unità nemica non avvelenata più vicina al bersaglio.\n" +
                      $"Danni da veleno al secondo: <color=red>{poisonDamage}</color>\n" +
                      $"Se tutte le unità nemiche sono avvelenate, aggiunge un ulteriore <color=red>{extraPoisonDamage}</color> di danno da veleno a tutte le unità nemiche.";

        DESkillDesc = $"Fügt dem angegriffenen Ziel für <color=yellow>{count} Sekunden</color> Gift zu.\n" +
                        $"Wenn das Ziel bereits vergiftet ist, breitet es sich auf die nächstgelegene ungiftige feindliche Einheit vom Ziel aus.\n" +
                        $"Gift-Schaden pro Sekunde: <color=red>{poisonDamage}</color>\n" +
                        $"Wenn alle feindlichen Einheiten vergiftet sind, fügt es allen feindlichen Einheiten zusätzlichen Gift-Schaden in Höhe von <color=red>{extraPoisonDamage}</color> zu.";

        ESSkillDesc = $"Inflige veneno al objetivo atacante durante <color=yellow>{count} segundos</color>.\n" +
                      $"Si el objetivo ya está envenenado, se extiende a la unidad enemiga no envenenada más cercana desde el objetivo.\n" +
                      $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>\n" +
                      $"Si todas las unidades enemigas están envenenadas, agrega un daño por veneno adicional de <color=red>{extraPoisonDamage}</color> a todas las unidades enemigas.";

        JASkillDesc = $"攻撃中の対象に<color=yellow>{count}秒</color>の毒効果を与えます。\n" +
                      $"対象がすでに毒にかかっている場合、対象から最も近い毒にかかっていない敵ユニットに感染します。\n" +
                      $"毎秒の毒ダメージ: <color=red>{poisonDamage}</color>\n" +
                      $"すべての敵ユニットが毒にかかっている場合、すべての敵ユニットに毒ダメージを<color=red>{extraPoisonDamage}</color>だけ追加します。";

        PT_BRSkillDesc = $"Inflige veneno no alvo atacado por <color=yellow>{count} segundos</color>.\n" +
                         $"Se o alvo já estiver envenenado, ele se espalha para a unidade inimiga não envenenada mais próxima do alvo.\n" +
                         $"Dano por veneno por segundo: <color=red>{poisonDamage}</color>\n" +
                         $"Se todas as unidades inimigas estiverem envenenadas, adiciona um dano por veneno adicional de <color=red>{extraPoisonDamage}</color> a todas as unidades inimigas.";

        RUSkillDesc = $"Накладывает яд на атакуемую цель на <color=yellow>{count} секунд</color>.\n" +
                       $"Если цель уже отравлена, он распространяется на ближайший нетоксичный вражеский юнит от цели.\n" +
                       $"Урон от яда в секунду: <color=red>{poisonDamage}</color>\n" +
                       $"Если все вражеские юниты отравлены, он добавляет всем вражеским юнитам дополнительный урон от яда в размере <color=red>{extraPoisonDamage}</color>.";

        ZH_HANSSkillDesc = $"对被攻击目标造成<color=yellow>{count}秒</color>的中毒效果。\n" +
                           $"如果目标已经中毒，它会传播到目标最近的未中毒敌方单位。\n" +
                           $"每秒中毒伤害：<color=red>{poisonDamage}</color>\n" +
                           $"如果所有敌方单位都中毒，则对所有敌方单位额外增加<color=red>{extraPoisonDamage}</color>点中毒伤害。";
    }
}