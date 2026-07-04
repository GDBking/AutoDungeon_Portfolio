using UnityEngine;

public class Mammon : UnitDefault
{
    [Header("탐욕")]
    public float incAttackAmount;
    public float incAttackDuration;
    public float stunDuration;
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

        CameraShake.instance.Shake(2f, 2f);

        SetStat(attackPowerStat, (StaticManager.gold / 100f) * incAttackAmount, false, true, incAttackDuration);
        SetStateBar(State.incAttack, incAttackDuration);

        GameObject effect = Instantiate(attackObj, transform.position, Quaternion.identity);
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);
        effect.GetComponent<SpriteRenderer>().sortingOrder = -499;
        effect.transform.localScale *= size;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));
        foreach (Collider2D enemy in hitEnemies) {
            enemy.GetComponent<UnitDefault>().OnStunAnim(stunDuration);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"<color=yellow>{incAttackDuration}초</color> 동안 본인의 공격력이 플레이어의 G량에 비례해 증가합니다.\n" +
                    $"또한 본인 반경 <color=#AEEAFF>{size}X{size}</color>범위의 적들을 <color=yellow>{stunDuration}초</color> 동안 기절시킵니다.\n" +
                    $"공격력 상승 비율 (<color=#FFE600>100G</color> : <color=green>+{incAttackAmount}</color>)";

        ENSkillDesc = $"Increases own attack power for <color=yellow>{incAttackDuration} seconds</color> based on the player's gold amount.\n" +
                      $"Also stuns enemies within a <color=#AEEAFF>{size}X{size}</color> area around this unit for <color=yellow>{stunDuration} seconds</color>.\n" +
                      $"Attack increase ratio (<color=#FFE600>100G</color>: <color=green>+{incAttackAmount}</color>)";

        FRSkillDesc = $"Augmente la puissance d'attaque de cette unité pendant <color=yellow>{incAttackDuration} secondes</color> en fonction de la quantité d'or du joueur.\n" +
                      $"Elle étourdit également les ennemis dans une zone de <color=#AEEAFF>{size}X{size}</color> autour de cette unité pendant <color=yellow>{stunDuration} secondes</color>.\n" +
                      $"Rapport d'augmentation de l'attaque (<color=#FFE600>100G</color> : <color=green>+{incAttackAmount}</color>)";

        ITSkillDesc = $"Aumenta la potenza d'attacco di questa unità per <color=yellow>{incAttackDuration} secondi</color> in base alla quantità di oro del giocatore.\n" +
                      $"Stordisce inoltre i nemici nell'area di <color=#AEEAFF>{size}X{size}</color> intorno a questa unità per <color=yellow>{stunDuration} secondi</color>.\n" +
                      $"Rapporto di aumento dell'attacco (<color=#FFE600>100G</color> : <color=green>+{incAttackAmount}</color>)";

        DESkillDesc = $"Erhöht die Angriffskraft dieser Einheit für <color=yellow>{incAttackDuration} Sekunden</color> basierend auf der Goldmenge des Spielers.\n" +
                      $"Betäubt außerdem Gegner in einem <color=#AEEAFF>{size}X{size}</color>-Bereich um diese Einheit für <color=yellow>{stunDuration} Sekunden</color>.\n" +
                      $"Angriffsanstiegsverhältnis (<color=#FFE600>100G</color>: <color=green>+{incAttackAmount}</color>)";

        ESSkillDesc = $"Aumenta el poder de ataque de esta unidad durante <color=yellow>{incAttackDuration} segundos</color> en función de la cantidad de oro del jugador.\n" +
                      $"También aturde a los enemigos dentro de un área de <color=#AEEAFF>{size}X{size}</color> alrededor de esta unidad durante <color=yellow>{stunDuration} segundos</color>.\n" +
                      $"Relación de aumento de ataque (<color=#FFE600>100G</color>: <color=green>+{incAttackAmount}</color>)";

        JASkillDesc = $"<color=yellow>{incAttackDuration}秒</color>の間、プレイヤーの所持金に応じて自身の攻撃力が上昇します。\n" +
                      $"また、このユニットの周囲<color=#AEEAFF>{size}X{size}</color>範囲の敵を<color=yellow>{stunDuration}秒</color>間スタンさせます。\n" +
                      $"攻撃力上昇比率（<color=#FFE600>100G</color>：<color=green>+{incAttackAmount}</color>）";

        PT_BRSkillDesc = $"Aumenta o poder de ataque desta unidade por <color=yellow>{incAttackDuration} segundos</color> com base na quantidade de ouro do jogador.\n" +
                          $"Também atordoa os inimigos dentro de uma área de <color=#AEEAFF>{size}X{size}</color> ao redor desta unidade por <color=yellow>{stunDuration} segundos</color>.\n" +
                          $"Taxa de aumento de ataque (<color=#FFE600>100G</color>: <color=green>+{incAttackAmount}</color>)";

        RUSkillDesc = $"Увеличивает атаку этой единицы на <color=yellow>{incAttackDuration} секунд</color> в зависимости от количества золота у игрока.\n" +
                      $"Также оглушает врагов в области <color=#AEEAFF>{size}X{size}</color> вокруг этой единицы на <color=yellow>{stunDuration} секунд</color>.\n" +
                      $"Коэффициент увеличения атаки (<color=#FFE600>100G</color>: <color=green>+{incAttackAmount}</color>)";

        ZH_HANSSkillDesc = $"在<color=yellow>{incAttackDuration}秒</color>内，基于玩家的金钱数量提高此单位的攻击力。\n" +
                           $"同时使此单位周围<color=#AEEAFF>{size}X{size}</color>范围的敌人在<color=yellow>{stunDuration}秒</color>内眩晕。\n" +
                           $"攻击力提升比例（<color=#FFE600>100G</color>：<color=green>+{incAttackAmount}</color>）";
    }
}
