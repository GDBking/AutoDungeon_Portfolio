using UnityEngine;

public class MapPoisonFog : MonoBehaviour
{
    [HideInInspector] public Calamity unitComp;

    float poisonDamage;
    int cnt;
    float elapsedTime;

    private void Start()
    {
        poisonDamage = unitComp.poisonDamage;
        cnt = unitComp.count;
    }

    private void Update()
    {
        elapsedTime -= Time.deltaTime;

        if (elapsedTime <= 0f) {
            elapsedTime = 1f;
            
            if (cnt-- == 0) {
                Destroy(gameObject);
                return;
            }

            foreach (UnitDefault enemy in UnitDefault.friends) {
                enemy.Poison(poisonDamage, 1, unitComp);
            }
        }
    }
}
