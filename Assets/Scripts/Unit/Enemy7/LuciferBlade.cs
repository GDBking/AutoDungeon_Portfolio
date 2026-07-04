using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LuciferBlade : MonoBehaviour
{
    [HideInInspector] public Lucifer unitComp;

    public RectTransform rectComp;
    public BoxCollider2D col;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public Image imgComp;
    public Sprite attackSpr;
    public AnimationClip hitAnimClip;

    public AudioClip baldeStartClip;
    public AudioClip baldeEndClip;

    float skillAttackPower;
    float criticalPer;

    private void Awake()
    {
        canvas.overrideSorting = true;
    }

    private void Start()
    {
        AudioManager.instance.PlaySfx(baldeStartClip);

        skillAttackPower = unitComp.AttackPower * unitComp.skillAttackCoeff;
        criticalPer = unitComp.CriticalPer;

        StartCoroutine(BladeShoot());
    }

    IEnumerator BladeShoot()
    {
        WaitForFixedUpdate wait = new();

        while (rectComp.sizeDelta.x < 1600f) {
            rectComp.sizeDelta += 1600f * Time.fixedDeltaTime * Vector2.right;
            yield return wait;
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.instance.PlaySfx(baldeEndClip);

        imgComp.sprite = attackSpr;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            col.bounds.center,
            col.size / 100f,
            transform.eulerAngles.z,
            LayerMask.GetMask("Friendly Unit")
        );

        for (int i = hitEnemies.Length - 1; i >= 0; i--) {
            UnitDefault enemyComp = hitEnemies[i].GetComponent<UnitDefault>();

            GameObject effect = Instantiate(GameManager.instance.effectObj, enemyComp.transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(hitAnimClip);

            enemyComp.Hit(skillAttackPower, criticalPer, unitComp);
        }

        while (canvasGroup.alpha > 0f) {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}