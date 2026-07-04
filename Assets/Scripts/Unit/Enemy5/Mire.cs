using UnityEngine;

public class Mire : UnitDefault
{
    [Header("수렁")]
    public MireObj mireObj;
    public float decSpeedAmount;
    public float decDefenseAmount;
    public int count;
    public float size;

    protected override void UseSkill()
    {
        if (friends.Count == 0)
            return;

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        foreach (UnitDefault enemy in friends) {
            MireObj mire = Instantiate(mireObj, enemy.transform.position, Quaternion.identity, GameManager.instance.skillEffect);
            mire.mireComp = this;
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"모든 적 유닛 아래에 <color=yellow>{count}초</color> 동안 유지되는 <color=#AEEAFF>{size}X{size}</color>크기의 늪을 생성합니다.\n" +
                    $"늪 위에 있는 적 유닛들은 이동 속도가 <color=red>{decSpeedAmount}</color>, 방어력이 <color=red>{decDefenseAmount}</color>만큼 감소합니다.";
        
        ESSkillDesc = $"Creates a <color=#AEEAFF>{size}X{size}</color> mire that lasts for <color=yellow>{count} seconds</color> under all enemy units.\n" +
                       $"Enemy units on the mire have their movement speed reduced by <color=red>{decSpeedAmount}</color> and their defense reduced by <color=red>{decDefenseAmount}</color>.";

        FRSkillDesc = $"Crée un marécage de <color=#AEEAFF>{size}X{size}</color> qui dure <color=yellow>{count} secondes</color> sous toutes les unités ennemies.\n" +
                       $"Les unités ennemies sur le marécage voient leur vitesse de déplacement réduite de <color=red>{decSpeedAmount}</color> et leur défense réduite de <color=red>{decDefenseAmount}</color>.";

        ITSkillDesc = $"Crea un pantano di <color=#AEEAFF>{size}X{size}</color> che dura per <color=yellow>{count} secondi</color> sotto tutte le unità nemiche.\n" +
                          $"Le unità nemiche sul pantano hanno la loro velocità di movimento ridotta di <color=red>{decSpeedAmount}</color> e la loro difesa ridotta di <color=red>{decDefenseAmount}</color>.";

        DESkillDesc = $"Erstellt einen <color=#AEEAFF>{size}X{size}</color> Sumpf, der <color=yellow>{count} Sekunden</color> unter allen feindlichen Einheiten anhält.\n" +
                          $"Feindliche Einheiten im Sumpf haben ihre Bewegungsgeschwindigkeit um <color=red>{decSpeedAmount}</color> und ihre Verteidigung um <color=red>{decDefenseAmount}</color> reduziert.";

        ESSkillDesc = $"Crea un pantano de <color=#AEEAFF>{size}X{size}</color> que dura <color=yellow>{count} segundos</color> debajo de todas las unidades enemigas.\n" +
                          $"Las unidades enemigas en el pantano tienen su velocidad de movimiento reducida en <color=red>{decSpeedAmount}</color> y su defensa reducida en <color=red>{decDefenseAmount}</color>.";

        JASkillDesc = $"すべての敵ユニットの下に<color=yellow>{count}秒</color>間維持される<color=#AEEAFF>{size}X{size}</color>サイズの沼を生成します。\n" +
                            $"沼の上にいる敵ユニットは移動速度が<color=red>{decSpeedAmount}</color>、防御力が<color=red>{decDefenseAmount}</color>だけ減少します。";

        PT_BRSkillDesc = $"Cria um pântano de <color=#AEEAFF>{size}X{size}</color> que dura por <color=yellow>{count} segundos</color> sob todas as unidades inimigas.\n" +
                            $"As unidades inimigas no pântano têm sua velocidade de movimento reduzida em <color=red>{decSpeedAmount}</color> e sua defesa reduzida em <color=red>{decDefenseAmount}</color>.";

        RUSkillDesc = $"Создает болото размером <color=#AEEAFF>{size}X{size}</color>, которое длится <color=yellow>{count} секунд</color> под всеми вражескими юнитами.\n" +
                          $"Вражеские юниты на болоте имеют уменьшенную скорость передвижения на <color=red>{decSpeedAmount}</color> и уменьшенную защиту на <color=red>{decDefenseAmount}</color>.";

        ZH_HANSSkillDesc = $"在所有敌方单位下方创建一个持续<color=yellow>{count}秒</color>的<color=#AEEAFF>{size}X{size}</color>大小的沼泽。\n" +
                          $"沼泽上的敌方单位的移动速度降低<color=red>{decSpeedAmount}</color>，防御力降低<color=red>{decDefenseAmount}</color>。";
    }
}
