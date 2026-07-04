using System.Collections;
using UnityEngine;

public class ApparitionClone : UnitDefault
{
    readonly WaitForSeconds wait = new(1f);

    protected override void FixedUpdate()
    {
        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);
    }

    protected override void UseSkill()
    {
        
    }

    protected override void OnSkillAttack()
    {
        
    }

    protected override IEnumerator OnMovingAnim()
    {
        rigid2D.mass = 3f;

        animator.speed = Speed;
        animator.SetBool("1_Move", true);

        // 최대 0.3~0.5초 사이의 랜덤한 시간만큼 이동
        float randTime = Random.Range(0.3f, 0.5f);
        float duration = 0f; // 이동 경과 시간
        WaitForFixedUpdate wait = new();
        Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        if (curFlipX && dir.x < 0f || !curFlipX && dir.x > 0f) {
            // 현재 방향과 반대 방향으로 이동하므로 flipX를 반대로 설정
            curFlipX = !curFlipX;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            uiGaugeBar.transform.localScale = new Vector2(-uiGaugeBar.transform.localScale.x, uiGaugeBar.transform.localScale.y);
        }

        while (duration < randTime && !isDeath && !isBondage && !isStunAnim) {
            duration += Time.fixedDeltaTime;

            transform.Translate(Speed * Time.fixedDeltaTime * dir);
            yield return wait;
        }

        rigid2D.mass = 1f;

        animator.speed = 1f;
        animator.SetBool("1_Move", false);

        isMovingAnim = false;
    }

    public void TauntTarget(float time, float size)
    {
        StartCoroutine(TauntTargetRoutine(time, size));
    }

    IEnumerator TauntTargetRoutine(float time, float size)
    {
        for (int i = 0; i < time; i++) {
            SoundPlay(skillSoundClip);

            CreateAttackBox(-1f, null, transform.position);
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, size / 2f, LayerMask.GetMask(enemyTag));

            foreach (Collider2D enemy in hitEnemies) {
                enemy.GetComponent<UnitDefault>().Taunt(gameObject, 1f);
            }

            StartCoroutine(OnMovingAnim());

            yield return wait;
        }
        Destroy(gameObject);
    }

    public override float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        return 0f;
    }

    protected override void OnEnable()
    {
        
    }

    public override void UpdateSkillDesc()
    {
        
    }
}