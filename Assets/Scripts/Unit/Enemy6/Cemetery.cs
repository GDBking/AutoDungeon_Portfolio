using System.Collections.Generic;
using UnityEngine;

public class Cemetery : UnitDefault
{
    [Header("묘지")]
    public int tombCount;
    public GameObject tombPrefab;
    public GameObject zombiePrefab;

    readonly static List<GameObject> tombList = new();

    protected override void Awake()
    {
        base.Awake();

        if (GameManager.instance.isEnd)
            tombList.Clear();
    }

    protected override void UseSkill()
    {
        StartCoroutine(OnSkillAnim());
    }

    protected override void OnSkillAttack()
    {
        SoundPlay(skillSoundClip);

        if (tombList.Count >= tombCount) {
            foreach (GameObject obj in tombList) {
                Instantiate(zombiePrefab, obj.transform.position, Quaternion.identity, enemyField);

                Destroy(obj);
            }
            tombList.Clear();
        }
        else {
            Vector2 randPos = RandomPositionManager.instance.GetRandomBattleFieldPos();
            GameObject tomb = Instantiate(tombPrefab, randPos, Quaternion.identity, GameManager.instance.skillEffect);
            tombList.Add(tomb);
        }
    }

    public override void UpdateSkillDesc()
    {
        skillDesc = $"맵의 랜덤한 위치에 묘지를 생성합니다.\n" +
                    $"묘지가 <color=yellow>{tombCount}개</color>가 되면 다음 스킬 사용 시 묘지에서 좀비가 생성됩니다.";

        ENSkillDesc = $"Create a cemetery at a random location on the map.\n" +
                      $"When the number of cemeteries reaches <color=yellow>{tombCount}</color>, a zombie is generated from the cemeteries upon the next skill use.";

        FRSkillDesc = $"Créez un cimetière à un endroit aléatoire de la carte.\n" +
                      $"Lorsque le nombre de cimetières atteint <color=yellow>{tombCount}</color>, un zombie est généré à partir des cimetières lors de la prochaine utilisation de la compétence.";

        ITSkillDesc = $"Crea un cimitero in una posizione casuale sulla mappa.\n" +
                      $"Quando il numero di cimiteri raggiunge <color=yellow>{tombCount}</color>, uno zombie viene generato dai cimiteri al successivo utilizzo dell'abilità.";

        DESkillDesc = $"Erstellt einen Friedhof an einem zufälligen Ort auf der Karte.\n" +
                      $"Wenn die Anzahl der Friedhöfe <color=yellow>{tombCount}</color> erreicht, wird beim nächsten Einsatz der Fähigkeit ein Zombie aus den Friedhöfen generiert.";

        ESSkillDesc = $"Crea un cementerio en una ubicación aleatoria del mapa.\n" +
                      $"Cuando el número de cementerios alcanza <color=yellow>{tombCount}</color>, se genera un zombi a partir de los cementerios en el siguiente uso de la habilidad.";

        JASkillDesc = $"マップのランダムな位置に墓地を生成します。\n" +
                      $"墓地が<color=yellow>{tombCount}個</color>になると、次のスキル使用時に墓地からゾンビが生成されます。";

        PT_BRSkillDesc = $"Crie um cemitério em um local aleatório no mapa.\n" +
                         $"Quando o número de cemitérios atingir <color=yellow>{tombCount}</color>, um zumbi é gerado a partir dos cemitérios na próxima utilização da habilidade.";

        RUSkillDesc = $"Создайте кладбище в случайном месте на карте.\n" +
                      $"Когда количество кладбищ достигнет <color=yellow>{tombCount}</color>, при следующем использовании навыка из кладбищ появится зомби.";

        ZH_HANSSkillDesc = $"在地图的随机位置创建一个墓地。\n" +
                           $"当墓地数量达到<color=yellow>{tombCount}</color>时，下次使用技能时将从墓地生成僵尸。";
    }
}