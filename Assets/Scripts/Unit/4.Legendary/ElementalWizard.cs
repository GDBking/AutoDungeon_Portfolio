using UnityEngine;
using UnityEngine.Localization.Settings;

public class ElementalWizard : UnitDefault
{
    [Header("불")]
    public float attackCoeffFire;
    public float burnDamage;
    public int count;
    public AnimationClip fireBulletAnimClip;
    public AnimationClip fireSkillAnimClip;
    public AudioClip fireSkillSoundClip;
    public AudioClip fireSkillBulletHitClip;
    [Header("물")]
    public float attackCoeffWater;
    public float decAttackSpeedPer;
    public float decSpeedPer;
    public float durationWater;
    public AnimationClip waterBulletAnimClip;
    public AnimationClip waterSkillAnimClip;
    public AudioClip waterSkillSoundClip;
    public AudioClip waterSkillBulletHitClip;
    [Header("땅")]
    public float attackCoeffLand;
    public float durationLand;
    public AnimationClip landSkillAnimClip;
    public AudioClip landSkillSoundClip;
    [Header("바람")]
    public float incAttackSpeedPer;
    public float incAttackPer;
    public float durationWind;
    public AnimationClip windSkillAnimClip;
    public AudioClip windSkillSoundClip;

    float AttackCoeffFire { get => attackCoeffFire + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float BurnDamage { get => burnDamage + StaticManager.skillPoint[unitIdx] * 5f; }
    int Count { get => count + StaticManager.skillPoint[unitIdx] / 4; }
    float AttackCoeffWater { get => attackCoeffWater + StaticManager.skillPoint[unitIdx] * 0.15f; }
    float DecAttackSpeedPer { get => Mathf.Min(decAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 4f, 99f); }
    float DecSpeedPer { get => Mathf.Min(decSpeedPer + StaticManager.skillPoint[unitIdx] * 4f, 99f); }
    float DurationWater { get => durationWater + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }
    float AttackCoeffLand { get => attackCoeffLand + StaticManager.skillPoint[unitIdx] * 0.1f; }
    float DurationLand { get => durationLand + StaticManager.skillPoint[unitIdx] / 4 * 0.2f; }
    float IncAttackSpeedPer { get => incAttackSpeedPer + StaticManager.skillPoint[unitIdx] * 4f; }
    float IncAttackPer { get => incAttackPer + StaticManager.skillPoint[unitIdx] * 4f; }
    float DurationWind { get => durationWind + StaticManager.skillPoint[unitIdx] / 4 * 0.5f; }

    int randNum;
    int randNum2;

    protected override void Awake()
    {
        base.Awake();

        onSkillFireAction = OnSkillFire;
        isKiting = true;
    }

    protected override void UseSkill()
    {
        if (enemies.Count == 0)
            return;

        if (randNum == 0) {
            randNum = Random.Range(1, 5);
            randNum2 = randNum;

            if (randNum == 2) {
                if (Target == null)
                    return;

                SkillTarget = Target;
                skillTargetPos = SkillTarget.transform.position;
            }
        }

        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        switch (randNum) {
            case 1:
                skillAnimClip = fireSkillAnimClip;
                skillBulletAnimClip = fireBulletAnimClip;
                skillSoundClip = fireSkillSoundClip;
                skillBulletHitClip = fireSkillBulletHitClip;
                foreach (UnitDefault enemy in enemies)
                    StartCoroutine(OnFire(enemy.gameObject, enemy.transform.position));
                break;
            case 2:
                skillAnimClip = waterSkillAnimClip;
                skillBulletAnimClip = waterBulletAnimClip;
                skillSoundClip = waterSkillSoundClip;
                skillBulletHitClip = waterSkillBulletHitClip;
                Vector2 pos = SkillTarget != null ? SkillTarget.transform.position : skillTargetPos;
                StartCoroutine(OnFire(SkillTarget, pos, true, true));
                break;
            case 3:
                skillAnimClip = landSkillAnimClip;
                SoundPlay(landSkillSoundClip);
                CameraShake.instance.Shake(2f, 2f);
                for (int i = enemies.Count - 1; i >= 0; i--) {
                    enemies[i].OnStunAnim(DurationLand);
                    CreateAttackBox(AttackPower * AttackCoeffLand, enemies[i].gameObject, enemies[i].transform.position);
                }
                break;
            case 4:
                skillAnimClip = windSkillAnimClip;
                SoundPlay(windSkillSoundClip);
                foreach (UnitDefault friendly in friends) {
                    GameObject effect = Instantiate(attackObj, friendly.transform.position, Quaternion.identity, friendly.transform);
                    effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(skillAnimClip);

                    friendly.SetStat(friendly.attackSpeedStat, IncAttackSpeedPer, true, true, DurationWind);
                    friendly.SetStat(friendly.attackPowerStat, IncAttackPer, true, true, DurationWind);
                    friendly.SetStateBar(State.incAttackSpeed, DurationWind);
                    friendly.SetStateBar(State.incAttack, DurationWind);
                }
                break;
        }
        randNum = 0;
    }

    void OnSkillFire(GameObject target)
    {
        UnitDefault unitComp = target.GetComponent<UnitDefault>();
        switch (randNum2) {
            case 1:
                unitComp.Burn(BurnDamage, Count, this, dealMetricsIdx);
                CreateAttackBox(AttackPower * AttackCoeffFire, target, target.transform.position);
                break;
            case 2:
                unitComp.SetStat(unitComp.attackSpeedStat, DecAttackSpeedPer, true, false, DurationWater);
                unitComp.SetStat(unitComp.speedStat, DecSpeedPer, true, false, DurationWater);
                unitComp.SetStateBar(State.decAttackSpeed, DurationWater);
                unitComp.SetStateBar(State.decSpeed, DurationWater);
                CreateAttackBox(AttackPower * AttackCoeffWater, target, target.transform.position);
                break;
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"4가지 원소 중 1가지를 무작위로 발사합니다.\n" +
                    $"불: 모든 적에게 공격력 <color=red>{AttackCoeffFire}배</color>의 피해를 입히는 화염구를 발사합니다.\n" +
                    $"피격된 적들은 <color=yellow>{Count}초</color> 동안 화상을 입습니다.\n" +
                    $"초당 화상 데미지: <color=red>{BurnDamage}</color>\n\n" +

                    $"물: 공격 중인 방향으로 유닛을 관통하는 워터볼을 발사합니다.\n" +
                    $"피격된 적들은 공격력 <color=red>{AttackCoeffWater}배</color>의 피해를 입고, <color=yellow>{DurationWater}초</color> 동안 공격 속도가 <color=red>{DecAttackSpeedPer}%</color>, 이동 속도가 <color=red>{DecSpeedPer}%</color> 감소합니다.\n\n" +

                    $"땅: 지진을 일으켜 모든 적에게 공격력 <color=red>{AttackCoeffLand}배</color>의 피해를 입히고, <color=yellow>{DurationLand}초</color> 동안 기절시킵니다.\n\n" +

                    $"바람: 모든 아군유닛은 <color=yellow>{DurationWind}초</color> 동안 공격 속도가 <color=green>{IncAttackSpeedPer}%</color>, 공격력이 <color=green>{IncAttackPer}%</color> 증가합니다.";
        
        ENSkillDesc = $"Fires one of four elements at random.\n" +
                       $"Fire: Launches a fireball that deals damage equal to <color=red>{AttackCoeffFire} times</color> the attack power to all enemies.\n" +
                       $"Hit enemies are burned for <color=yellow>{Count} seconds</color>.\n" +
                       $"Burn damage per second: <color=red>{BurnDamage}</color>\n\n" +
                       $"Water: Fires a water ball that penetrates units in the attacking direction.\n" +
                       $"Hit enemies take damage equal to <color=red>{AttackCoeffWater} times</color> the attack power, and their attack speed is reduced by <color=red>{DecAttackSpeedPer}%</color> and movement speed by <color=red>{DecSpeedPer}%</color> for <color=yellow>{DurationWater} seconds</color>.\n\n" +
                       $"Earth: Causes an earthquake that deals damage equal to <color=red>{AttackCoeffLand} times</color> the attack power to all enemies and stuns them for <color=yellow>{DurationLand} seconds</color>.\n\n" +
                       $"Wind: All allied units have their attack speed increased by <color=green>{IncAttackSpeedPer}%</color> and attack power increased by <color=green>{IncAttackPer}%</color> for <color=yellow>{DurationWind} seconds.</color>";

        FRSkillDesc = $"Lance un des quatre éléments au hasard.\n" +
                       $"Feu : Lance une boule de feu qui inflige des dégâts égaux à <color=red>{AttackCoeffFire} fois</color> la puissance d'attaque à tous les ennemis.\n" +
                       $"Les ennemis touchés sont brûlés pendant <color=yellow>{Count} secondes</color>.\n" +
                       $"Dégâts de brûlure par seconde : <color=red>{BurnDamage}</color>\n\n" +
                       $"Eau : Tire une boule d'eau qui pénètre les unités dans la direction d'attaque.\n" +
                       $"Les ennemis touchés subissent des dégâts égaux à <color=red>{AttackCoeffWater} fois</color> la puissance d'attaque, et leur vitesse d'attaque est réduite de <color=red>{DecAttackSpeedPer}%</color> et leur vitesse de déplacement de <color=red>{DecSpeedPer}%</color> pendant <color=yellow>{DurationWater} secondes</color>.\n\n" +
                       $"Terre : Provoque un tremblement de terre qui inflige des dégâts égaux à <color=red>{AttackCoeffLand} fois</color> la puissance d'attaque à tous les ennemis et les étourdit pendant <color=yellow>{DurationLand} secondes</color>.\n\n" +
                       $"Vent : Toutes les unités alliées voient leur vitesse d'attaque augmentée de <color=green>{IncAttackSpeedPer}%</color> et leur puissance d'attaque augmentée de <color=green>{IncAttackPer}%</color> pendant <color=yellow>{DurationWind} secondes.</color>";

        ITSkillDesc = $"Lancia uno dei quattro elementi a caso.\n" +
                       $"Fuoco: Lancia una palla di fuoco che infligge danni pari a <color=red>{AttackCoeffFire} volte</color> la potenza d'attacco a tutti i nemici.\n" +
                       $"I nemici colpiti vengono bruciati per <color=yellow>{Count} secondi</color>.\n" +
                       $"Danno da bruciatura al secondo: <color=red>{BurnDamage}</color>\n\n" +
                       $"Acqua: Spara una palla d'acqua che penetra le unità nella direzione di attacco.\n" +
                       $"I nemici colpiti subiscono danni pari a <color=red>{AttackCoeffWater} volte</color> la potenza d'attacco, e la loro velocità d'attacco viene ridotta del <color=red>{DecAttackSpeedPer}%</color> e la velocità di movimento del <color=red>{DecSpeedPer}%</color> per <color=yellow>{DurationWater} secondi</color>.\n\n" +
                       $"Terra: Causa un terremoto che infligge danni pari a <color=red>{AttackCoeffLand} volte</color> la potenza d'attacco a tutti i nemici e li stordisce per <color=yellow>{DurationLand} secondi</color>.\n\n" +
                       $"Vento: Tutte le unità alleate vedono la loro velocità d'attacco aumentata del <color=green>{IncAttackSpeedPer}%</color> e la potenza d'attacco aumentata del <color=green>{IncAttackPer}%</color> per <color=yellow>{DurationWind} secondi.</color>";

        DESkillDesc = $"Feuert eines von vier Elementen zufällig ab.\n" +
                       $"Feuer: Startet einen Feuerball, der allen Feinden Schaden in Höhe von <color=red>{AttackCoeffFire} mal</color> der Angriffskraft zufügt.\n" +
                       $"Getroffene Feinde werden für <color=yellow>{Count} Sekunden</color> verbrannt.\n" +
                       $"Brandschaden pro Sekunde: <color=red>{BurnDamage}</color>\n\n" +
                       $"Wasser: Feuert eine Wasserball, die Einheiten in Angriffsrichtung durchdringt.\n" +
                       $"Getroffene Feinde erleiden Schaden in Höhe von <color=red>{AttackCoeffWater} mal</color> der Angriffskraft, und ihre Angriffsgeschwindigkeit wird um <color=red>{DecAttackSpeedPer}%</color> und ihre Bewegungsgeschwindigkeit um <color=red>{DecSpeedPer}%</color> für <color=yellow>{DurationWater} Sekunden</color> reduziert.\n\n" +
                       $"Erde: Verursacht ein Erdbeben, das allen Feinden Schaden in Höhe von <color=red>{AttackCoeffLand} mal</color> der Angriffskraft zufügt und sie für <color=yellow>{DurationLand} Sekunden</color> betäubt.\n\n" +
                       $"Wind: Alle verbündeten Einheiten haben ihre Angriffsgeschwindigkeit um <color=green>{IncAttackSpeedPer}%</color> und ihre Angriffskraft um <color=green>{IncAttackPer}%</color> für <color=yellow>{DurationWind} Sekunden.</color>";

        ESSkillDesc = $"Lanza uno de los cuatro elementos al azar.\n" +
                       $"Fuego: Lanza una bola de fuego que inflige daño igual a <color=red>{AttackCoeffFire} veces</color> el poder de ataque a todos los enemigos.\n" +
                       $"Los enemigos alcanzados son quemados durante <color=yellow>{Count} segundos</color>.\n" +
                       $"Daño por quemadura por segundo: <color=red>{BurnDamage}</color>\n\n" +
                       $"Agua: Dispara una bola de agua que penetra las unidades en la dirección de ataque.\n" +
                       $"Los enemigos alcanzados reciben daño igual a <color=red>{AttackCoeffWater} veces</color> el poder de ataque, y su velocidad de ataque se reduce en <color=red>{DecAttackSpeedPer}%</color> y la velocidad de movimiento en <color=red>{DecSpeedPer}%</color> durante <color=yellow>{DurationWater} segundos</color>.\n\n" +
                       $"Tierra: Causa un terremoto que inflige daño igual a <color=red>{AttackCoeffLand} veces</color> el poder de ataque a todos los enemigos y los aturde durante <color=yellow>{DurationLand} segundos</color>.\n\n" +
                       $"Viento: Todas las unidades aliadas tienen su velocidad de ataque aumentada en <color=green>{IncAttackSpeedPer}%</color> y su poder de ataque aumentado en <color=green>{IncAttackPer}%</color> durante <color=yellow>{DurationWind} segundos.</color>";

        JASkillDesc = $"4つの元素のうち1つをランダムに発射します。\n" +
                       $"火：ファイアボールを発射し、すべての敵に攻撃力の<color=red>{AttackCoeffFire}倍</color>のダメージを与えます。\n" +
                       $"命中した敵は<color=yellow>{Count}秒</color>間、火傷状態になります。\n" +
                       $"火傷ダメージ（毎秒）：<color=red>{BurnDamage}</color>\n\n" +
                       $"水：攻撃方向にユニットを貫通するウォーターボールを発射します。\n" +
                       $"命中した敵は攻撃力の<color=red>{AttackCoeffWater}倍</color>のダメージを受け、<color=yellow>{DurationWater}秒</color>間、攻撃速度が<color=red>{DecAttackSpeedPer}%</color>、移動速度が<color=red>{DecSpeedPer}%</color>減少します。\n\n" +
                       $"地：地震を引き起こし、すべての敵に攻撃力の<color=red>{AttackCoeffLand}倍</color>のダメージを与え、<color=yellow>{DurationLand}秒</color>間気絶させます。\n\n" +
                       $"風：すべての味方ユニットは<color=yellow>{DurationWind}秒</color>間、攻撃速度が<color=green>{IncAttackSpeedPer}%</color>、攻撃力が<color=green>{IncAttackPer}%</color>増加します。";

        PT_BRSkillDesc = $"Dispara um dos quatro elementos aleatoriamente.\n" +
                        $"Fogo: Lança uma bola de fogo que causa dano igual a <color=red>{AttackCoeffFire} vezes</color> o poder de ataque a todos os inimigos.\n" +
                        $"Inimigos atingidos são queimados por <color=yellow>{Count} segundos</color>.\n" +
                        $"Dano de queimadura por segundo: <color=red>{BurnDamage}</color>\n\n" +
                        $"Água: Dispara uma bola de água que penetra unidades na direção do ataque.\n" +
                        $"Inimigos atingidos recebem dano igual a <color=red>{AttackCoeffWater} vezes</color> o poder de ataque, e sua velocidade de ataque é reduzida em <color=red>{DecAttackSpeedPer}%</color> e a velocidade de movimento em <color=red>{DecSpeedPer}%</color> por <color=yellow>{DurationWater} segundos</color>.\n\n" +
                        $"Terra: Causa um terremoto que causa dano igual a <color=red>{AttackCoeffLand} vezes</color> o poder de ataque a todos os inimigos e os atordoa por <color=yellow>{DurationLand} segundos</color>.\n\n" +
                        $"Vento: Todas as unidades aliadas têm sua velocidade de ataque aumentada em <color=green>{IncAttackSpeedPer}%</color> e o poder de ataque aumentado em <color=green>{IncAttackPer}%</color> por <color=yellow>{DurationWind} segundos.</color>";

        RUSkillDesc = $"Выпускает один из четырех элементов случайным образом.\n" +
                       $"Огонь: Выпускает огненный шар, который наносит урон, равный <color=red>{AttackCoeffFire} раза</color> силы атаки всем врагам.\n" +
                       $"Попавшие враги горят в течение <color=yellow>{Count} секунд</color>.\n" +
                       $"Урон от ожога в секунду: <color=red>{BurnDamage}</color>\n\n" +
                       $"Вода: Выпускает водяной шар, который проникает через юнитов в направлении атаки.\n" +
                       $"Попавшие враги получают урон, равный <color=red>{AttackCoeffWater} раза</color> силы атаки, и их скорость атаки уменьшается на <color=red>{DecAttackSpeedPer}%</color>, а скорость передвижения на <color=red>{DecSpeedPer}%</color> в течение <color=yellow>{DurationWater} секунд</color>.\n\n" +
                       $"Земля: Вызывает землетрясение, которое наносит урон, равный <color=red>{AttackCoeffLand} раза</color> силы атаки всем врагам и оглушает их на <color=yellow>{DurationLand} секунд</color>.\n\n" +
                       $"Ветер: Все союзные юниты увеличивают свою скорость атаки на <color=green>{IncAttackSpeedPer}%</color> и силу атаки на <color=green>{IncAttackPer}%</color> в течение <color=yellow>{DurationWind} секунд.</color>";

        ZH_HANSSkillDesc = $"随机发射四种元素之一。\n" +
                          $"火：发射一个火球，对所有敌人造成相当于攻击力<color=red>{AttackCoeffFire}倍</color>的伤害。\n" +
                          $"命中的敌人在<color=yellow>{Count}秒</color>内会被灼烧。\n" +
                          $"每秒灼烧伤害：<color=red>{BurnDamage}</color>\n\n" +
                          $"水：发射一个水球，穿透攻击方向上的单位。\n" +
                          $"命中的敌人受到相当于攻击力<color=red>{AttackCoeffWater}倍</color>的伤害，并且在<color=yellow>{DurationWater}秒</color>内攻击速度降低<color=red>{DecAttackSpeedPer}%</color>，移动速度降低<color=red>{DecSpeedPer}%</color>。\n\n" +
                          $"地：引发地震，对所有敌人造成相当于攻击力<color=red>{AttackCoeffLand}倍</color>的伤害，并使其眩晕<color=yellow>{DurationLand}秒</color>。\n\n" +
                          $"风：所有友军单位在<color=yellow>{DurationWind}秒</color>内攻击速度提高<color=green>{IncAttackSpeedPer}%</color>，攻击力提高<color=green>{IncAttackPer}%</color>。";
    }

    public override string GetSkillUpDesc()
    {
        string code = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (code.StartsWith("ko", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1P</color>당 효과\n" +
                   "불 공격력 계수 <color=red>+0.1</color>\n" +
                   "초당 화상 데미지 <color=red>+5</color>\n" +
                   "물 공격력 계수 <color=red>+0.15</color>\n" +
                   "물 공격 속도 감소량 <color=red>+4%p</color>\n" +
                   "물 이동 속도 감소량 <color=red>+4%p</color>\n" +
                   "땅 공격력 계수 <color=red>+0.1</color>\n" +
                   "바람 공격 속도 증가량 <color=green>+4%p</color>\n" +
                   "바람 공격력 증가량 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>4P</color>당 효과\n" +
                   "화상 지속 시간 <color=yellow>+1</color>\n" +
                   "물 지속 시간 <color=yellow>+0.5</color>\n" +
                   "땅 지속 시간 <color=yellow>+0.2</color>\n" +
                   "바람 지속 시간 <color=yellow>+0.5</color>";

        if (code.StartsWith("en", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effect per 1P</color>\n" +
                   "Fire attack multiplier <color=red>+0.1</color>\n" +
                   "Burn damage per second <color=red>+5</color>\n" +
                   "Water attack multiplier <color=red>+0.15</color>\n" +
                   "Water attack speed decrease <color=red>+4%p</color>\n" +
                   "Water movement speed decrease <color=red>+4%p</color>\n" +
                   "Earth attack multiplier <color=red>+0.1</color>\n" +
                   "Wind attack speed increase <color=green>+4%p</color>\n" +
                   "Wind attack increase <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effect per 4P</color>\n" +
                   "Burn duration <color=yellow>+1</color>\n" +
                   "Water duration <color=yellow>+0.5</color>\n               " +
                   "Earth duration <color=yellow>+0.2</color>\n" +
                   "Wind duration <color=yellow>+0.5</color>";

        if (code.StartsWith("fr", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effet par 1P</color>\n" +
                   "Multiplicateur d'attaque feu <color=red>+0.1</color>\n" +
                   "Dégâts de brûlure par seconde <color=red>+5</color>\n" +
                   "Multiplicateur d'attaque eau <color=red>+0.15</color>\n" +
                   "Réduction vitesse d'attaque eau <color=red>+4%p</color>\n" +
                   "Réduction vitesse de déplacement eau <color=red>+4%p</color>\n" +
                   "Multiplicateur d'attaque terre <color=red>+0.1</color>\n" +
                   "Augmentation vitesse d'attaque vent <color=green>+4%p</color>\n" +
                   "Augmentation d'attaque vent <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effet par 4P</color>\n" +
                   "Durée brûlure <color=yellow>+1</color>\n" +
                   "Durée eau <color=yellow>+0.5</color>\n" +
                   "Durée terre <color=yellow>+0.2</color>\n" +
                   "Durée vent <color=yellow>+0.5</color>";

        if (code.StartsWith("it", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Effetto per 1P</color>\n" +
                   "Moltiplicatore attacco fuoco <color=red>+0.1</color>\n" +
                   "Danno da bruciatura al secondo <color=red>+5</color>\n" +
                   "Moltiplicatore attacco acqua <color=red>+0.15</color>\n" +
                   "Riduzione velocità attacco acqua <color=red>+4%p</color>\n" +
                   "Riduzione velocità movimento acqua <color=red>+4%p</color>\n" +
                   "Moltiplicatore attacco terra <color=red>+0.1</color>\n" +
                   "Aumento velocità attacco vento <color=green>+4%p</color>\n" +
                   "Aumento attacco vento <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Effetto per 4P</color>\n" +
                   "Durata bruciatura <color=yellow>+1</color>\n" +
                   "Durata acqua <color=yellow>+0.5</color>\n" +
                   "Durata terra <color=yellow>+0.2</color>\n" +
                   "Durata vento <color=yellow>+0.5</color>";

        if (code.StartsWith("de", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Wirkung pro 1P</color>\n" +
                   "Feuer-Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Brand-Schaden pro Sekunde <color=red>+5</color>\n" +
                   "Wasser-Angriffs-Multiplikator <color=red>+0.15</color>\n" +
                   "Wasser-Angriffsgeschwindigkeit Reduktion <color=red>+4%p</color>\n" +
                   "Wasser-Bewegungsgeschwindigkeit Reduktion <color=red>+4%p</color>\n" +
                   "Erde-Angriffs-Multiplikator <color=red>+0.1</color>\n" +
                   "Wind-Angriffsgeschwindigkeit Erhöhung <color=green>+4%p</color>\n" +
                   "Wind-Angriffserhöhung <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Wirkung pro 4P</color>\n" +
                   "Branddauer <color=yellow>+1</color>\n" +
                   "Wasserdauer <color=yellow>+0.5</color>\n" +
                   "Erddauer <color=yellow>+0.2</color>\n" +
                   "Winddauer <color=yellow>+0.5</color>";

        if (code.StartsWith("es", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efecto por 1P</color>\n" +
                   "Multiplicador de ataque fuego <color=red>+0.1</color>\n" +
                   "Daño por quemadura por segundo <color=red>+5</color>\n" +
                   "Multiplicador de ataque agua <color=red>+0.15</color>\n" +
                   "Reducción velocidad de ataque agua <color=red>+4%p</color>\n" +
                   "Reducción velocidad de movimiento agua <color=red>+4%p</color>\n" +
                   "Multiplicador de ataque tierra <color=red>+0.1</color>\n" +
                   "Aumento velocidad de ataque viento <color=green>+4%p</color>\n" +
                   "Aumento de ataque viento <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Efecto por 4P</color>\n" +
                   "Duración quemadura <color=yellow>+1</color>\n" +
                   "Duración agua <color=yellow>+0.5</color>\n" +
                   "Duración tierra <color=yellow>+0.2</color>\n" +
                   "Duración viento <color=yellow>+0.5</color>";

        if (code.StartsWith("ja", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>1Pごとの効果</color>\n" +
                   "火属性攻撃係数 <color=red>+0.1</color>\n" +
                   "秒間火傷ダメージ <color=red>+5</color>\n" +
                   "水属性攻撃係数 <color=red>+0.15</color>\n" +
                   "水攻撃速度減少量 <color=red>+4%p</color>\n" +
                   "水移動速度減少量 <color=red>+4%p</color>\n" +
                   "地属性攻撃係数 <color=red>+0.1</color>\n" +
                   "風攻撃速度増加量 <color=green>+4%p</color>\n" +
                   "風攻撃力増加量 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>4Pごとの効果</color>\n" +
                   "火傷持続時間 <color=yellow>+1</color>\n" +
                   "水持続時間 <color=yellow>+0.5</color>\n" +
                   "地持続時間 <color=yellow>+0.2</color>\n" +
                   "風持続時間 <color=yellow>+0.5</color>";

        if (code.StartsWith("pt", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Efeito por 1P</color>\n" +
                   "Multiplicador de ataque fogo <color=red>+0.1</color>\n" +
                   "Dano de queimadura por segundo <color=red>+5</color>\n" +
                   "Multiplicador de ataque água <color=red>+0.15</color>\n" +
                   "Redução de velocidade de ataque água <color=red>+4%p</color>\n" +
                   "Redução de velocidade de movimento água <color=red>+4%p</color>\n" +
                   "Multiplicador de ataque terra <color=red>+0.1</color>\n" +
                   "Aumento de velocidade de ataque vento <color=green>+4%p</color>\n" +
                   "Aumento de ataque vento <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Efeito por 4P</color>\n" +
                   "Duração da queimadura <color=yellow>+1</color>\n" +
                   "Duração da água <color=yellow>+0.5</color>\n" +
                   "Duração da terra <color=yellow>+0.2</color>\n" +
                   "Duração do vento <color=yellow>+0.5</color>";

        if (code.StartsWith("ru", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>Эффект за 1P</color>\n" +
                   "Множитель урона огнем <color=red>+0.1</color>\n" +
                   "Урон от ожога в секунду <color=red>+5</color>\n" +
                   "Множитель атаки водой <color=red>+0.15</color>\n" +
                   "Снижение скорости атаки водой <color=red>+4%p</color>\n" +
                   "Снижение скорости передвижения водой <color=red>+4%p</color>\n" +
                   "Множитель атаки земли <color=red>+0.1</color>\n" +
                   "Увеличение скорости атаки ветром <color=green>+4%p</color>\n" +
                   "Увеличение атаки ветром <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>Эффект за 4P</color>\n" +
                   "Длительность ожога <color=yellow>+1</color>\n" +
                   "Длительность воды <color=yellow>+0.5</color>\n" +
                   "Длительность земли <color=yellow>+0.2</color>\n" +
                   "Длительность ветра <color=yellow>+0.5</color>";

        if (code.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
            return "<color=#FFBF00>每1P效果</color>\n" +
                   "火属性攻击系数 <color=red>+0.1</color>\n" +
                   "每秒灼烧伤害 <color=red>+5</color>\n" +
                   "水属性攻击系数 <color=red>+0.15</color>\n" +
                   "水攻击速度减少量 <color=red>+4%p</color>\n" +
                   "水移动速度减少量 <color=red>+4%p</color>\n" +
                   "地属性攻击系数 <color=red>+0.1</color>\n" +
                   "风攻击速度增加量 <color=green>+4%p</color>\n" +
                   "风攻击力增加量 <color=green>+4%p</color>\n" +
                   "<color=#FFBF00>每4P效果</color>\n" +
                   "灼烧持续时间 <color=yellow>+1</color>\n" +
                   "水持续时间 <color=yellow>+0.5</color>\n" +
                   "地持续时间 <color=yellow>+0.2</color>\n" +
                   "风持续时间 <color=yellow>+0.5</color>";

        return "<color=#FFBF00>Effect per 1P</color>\n" +
               "Fire attack multiplier <color=red>+0.1</color>\n" +
               "Burn damage per second <color=red>+5</color>\n" +
               "Water attack multiplier <color=red>+0.15</color>\n" +
               "Water attack speed decrease <color=red>+4%p</color>\n" +
               "Water movement speed decrease <color=red>+4%p</color>\n" +
               "Earth attack multiplier <color=red>+0.1</color>\n" +
               "Wind attack speed increase <color=green>+4%p</color>\n" +
               "Wind attack increase <color=green>+4%p</color>\n" +
               "<color=#FFBF00>Effect per 4P</color>\n" +
               "Burn duration <color=yellow>+1</color>\n" +
               "Water duration <color=yellow>+0.5</color>\n" +
               "Earth duration <color=yellow>+0.2</color>\n" +
               "Wind duration <color=yellow>+0.5</color>";
    }
}