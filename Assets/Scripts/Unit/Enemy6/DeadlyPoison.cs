using System.Collections;
using UnityEngine;

public class DeadlyPoison : UnitDefault
{
    [Header("맹독")]
    public float decSpeedPer;
    public float poisonDamage;
    public int count;
    public float size;
    public DeadlyPoisonPool deadlyPoisonPoolPrefab;

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        DeadlyPoisonPool deadlyPoisonComp = Instantiate(deadlyPoisonPoolPrefab, transform.position, Quaternion.identity, GameManager.instance.skillEffect);
        deadlyPoisonComp.unitComp = this;
    }

    protected override IEnumerator OnDeathAnim()
    {
        OnSkillAttack();

        return base.OnDeathAnim();
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"사망 시 <color=#AEEAFF>{size}X{size}</color>크기의 맹독을 퍼트립니다.\n" +
                    $"맹독은 <color=yellow>{count}초</color> 동안 유지되며 맹독에 닿은 적들은 독 효과를 입고, 이동 속도가 <color=red>{decSpeedPer}%</color> 감소합니다.\n" +
                    $"초당 독 데미지: <color=red>{poisonDamage}</color>";
        
        ENSkillDesc = $"Upon death, releases deadly poison with a size of <color=#AEEAFF>{size}X{size}</color>.\n" +
                       $"The deadly poison lasts for <color=yellow>{count} seconds</color>, and enemies that come into contact with it are poisoned and have their movement speed reduced by <color=red>{decSpeedPer}%</color>.\n" +
                       $"Poison damage per second: <color=red>{poisonDamage}</color>";

        FRSkillDesc = $"À la mort, libère un poison mortel d'une taille de <color=#AEEAFF>{size}X{size}</color>.\n" +
                          $"Le poison mortel dure <color=yellow>{count} secondes</color>, et les ennemis qui entrent en contact avec lui sont empoisonnés et voient leur vitesse de déplacement réduite de <color=red>{decSpeedPer}%</color>.\n" +
                          $"Dégâts de poison par seconde : <color=red>{poisonDamage}</color>";

        ITSkillDesc = $"Upon death, releases deadly poison with a size of <color=#AEEAFF>{size}X{size}</color>.\n" +
                       $"The deadly poison lasts for <color=yellow>{count} seconds</color>, and enemies that come into contact with it are poisoned and have their movement speed reduced by <color=red>{decSpeedPer}%</color>.\n" +
                       $"Poison damage per second: <color=red>{poisonDamage}</color>";

        DESkillDesc = $"Beim Tod wird ein tödliches Gift mit einer Größe von <color=#AEEAFF>{size}X{size}</color> freigesetzt.\n" +
                          $"Das tödliche Gift hält <color=yellow>{count} Sekunden</color> an, und Feinde, die damit in Kontakt kommen, werden vergiftet und ihre Bewegungsgeschwindigkeit um <color=red>{decSpeedPer}%</color> reduziert.\n" +
                          $"Gift-Schaden pro Sekunde: <color=red>{poisonDamage}</color>";

        ESSkillDesc = $"Al morir, libera un veneno mortal con un tamaño de <color=#AEEAFF>{size}X{size}</color>.\n" +
                       $"El veneno mortal dura <color=yellow>{count} segundos</color>, y los enemigos que entran en contacto con él son envenenados y tienen su velocidad de movimiento reducida en <color=red>{decSpeedPer}%</color>.\n" +
                       $"Daño por veneno por segundo: <color=red>{poisonDamage}</color>";

        JASkillDesc = $"死亡時に<color=#AEEAFF>{size}X{size}</color>の大きさの猛毒を放出します。\n" +
                          $"猛毒は<color=yellow>{count}秒</color>間持続し、触れた敵は毒状態になり、移動速度が<color=red>{decSpeedPer}%</color>減少します。\n" +
                          $"毎秒の毒ダメージ: <color=red>{poisonDamage}</color>";

        PT_BRSkillDesc = $"Ao morrer, libera um veneno mortal com tamanho de <color=#AEEAFF>{size}X{size}</color>.\n" +
                            $"O veneno mortal dura <color=yellow>{count} segundos</color>, e os inimigos que entram em contato com ele são envenenados e têm sua velocidade de movimento reduzida em <color=red>{decSpeedPer}%</color>.\n" +
                            $"Dano por veneno por segundo: <color=red>{poisonDamage}</color>";

        RUSkillDesc = $"После смерти выпускает смертельный яд размером <color=#AEEAFF>{size}X{size}</color>.\n" +
                          $"Смертельный яд сохраняется в течение <color=yellow>{count} секунд</color>, и враги, которые с ним соприкасаются, отравляются, а их скорость передвижения уменьшается на <color=red>{decSpeedPer}%</color>.\n" +
                          $"Урон от яда в секунду: <color=red>{poisonDamage}</color>";

        ZH_HANSSkillDesc = $"死亡时释放出大小为<color=#AEEAFF>{size}X{size}</color>的致命毒药。\n" +
                           $"致命毒药持续<color=yellow>{count}秒</color>，与之接触的敌人会中毒，移动速度降低<color=red>{decSpeedPer}%</color>。\n" +
                           $"每秒毒药伤害：<color=red>{poisonDamage}</color>";
    }
}