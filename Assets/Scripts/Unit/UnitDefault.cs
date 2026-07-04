using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StatWrapper
{
    public float extra;
    public float coeff = 1.0f;
}

public abstract class UnitDefault : MonoBehaviour
{
    #region Field
    public enum State { decDefense, stun, wantedList, incCriticalPer, incAttackRange,
                        bondage, burn, incAttack, incAttackSpeed, poison, invincible,
                        penetrationShoot, incDefense, phantom, incLifeSteal, decAttackSpeed,
                        decSpeed, comeback, stealth, berserk, silence, decAttack,
                        taunt, antiHeal, smash, immortable, incSpeed, decayArrow, enthrallment,
                        servant, decAttackRange, reign, bleeding, madness, luciferBuff,
                        HPPotion, ATKPotion, DEFPotion, DPSPotion, LSPotion, CRTPotion, steroid }
    public enum AttackStyle { single, range }
    public enum Rank { common, uncommon, rare, legendary, boss }

    [HideInInspector] public AttackStyle attackStyle;
    public Rank rank;

    [Header("유닛 ID")]
    public string unitID;
    public int unitIdx = -1;

    [Header("스탯")]
    public int level;
    public int unitMoney;
    public int cost;

    public float maxHealth;
    public StatWrapper maxHealthStat = new();

    public float health;
    public StatWrapper healthStat = new();

    public int maxMana;
    public StatWrapper maxManaStat = new();

    public int currentMana;

    public float attackPower;
    public StatWrapper attackPowerStat = new();

    public float defense;
    public StatWrapper defenseStat = new();

    public float attackSpeed;
    public StatWrapper attackSpeedStat = new();

    public float criticalPer;
    public StatWrapper criticalPerStat = new();

    [System.NonSerialized] public float criticalCoeff = 1.75f;

    public float lifeStealPer;
    public StatWrapper lifeStealPerStat = new();

    public float speed;
    public StatWrapper speedStat = new();

    public float attackRange;
    public StatWrapper attackRangeStat = new();

    public float shotSpeed;
    public StatWrapper shotSpeedStat = new();

    public float attackSize = 0.5f;

    [System.NonSerialized]public float steroid = 1.0f;

    [Header("한국어")]
    public string unitName;
    public string skillName;
    [TextArea] public string skillDesc;

    [Header("영어")]
    public string ENUnitName;
    public string ENSkillName;
    [TextArea] public string ENSkillDesc;

    [Header("프랑스어")]
    public string FRUnitName;
    public string FRSkillName;
    [TextArea] public string FRSkillDesc;

    [Header("이탈리아어")]
    public string ITUnitName;
    public string ITSkillName;
    [TextArea] public string ITSkillDesc;

    [Header("독일어")]
    public string DEUnitName;
    public string DESkillName;
    [TextArea] public string DESkillDesc;

    [Header("스페인어")]
    public string ESUnitName;
    public string ESSkillName;
    [TextArea] public string ESSkillDesc;

    [Header("일본어")]
    public string JAUnitName;
    public string JASkillName;
    [TextArea] public string JASkillDesc;

    [Header("포르투갈어 (브라질)")]
    public string PT_BRUnitName;
    public string PT_BRSkillName;
    [TextArea] public string PT_BRSkillDesc;

    [Header("러시아어")]
    public string RUUnitName;
    public string RUSkillName;
    [TextArea] public string RUSkillDesc;

    [Header("중국어 (간체)")]
    public string ZH_HANSUnitName;
    public string ZH_HANSSkillName;
    [TextArea] public string ZH_HANSSkillDesc;

    protected GameObject unitInfo;
    protected GameObject itemInfo;
    protected GameObject eventInfo;
    protected GameObject relicsInfo;
    List<TextMeshProUGUI> statTxt;
    List<TextMeshProUGUI> statAmountTxt;
    protected Image curHealthImg;
    protected Image curManaImg;
    protected ScrollRect descScrollRect;

    [Header("공격 애니메이션 클립")]
    public AnimationClip attackAnimClip;
    public AnimationClip skillAnimClip;
    public static GameObject attackObj;
    
    [Header("총알 애니메이션 클립")]
    public AnimationClip bulletAnimClip;
    public AnimationClip skillBulletAnimClip;
    public static GameObject bulletObj;

    [Header("사운드")]
    public AudioClip attackSoundClip;
    public AudioClip skillSoundClip;
    public AudioClip bulletHitClip;
    public AudioClip skillBulletHitClip;

    [Header("피격 이펙트")]
    public bool isAttackEffectRotation = true;
    public bool isSkillEffectRotation = true;
    
    // 총알 프리팹
    protected List<GameObject> bulletList = new();

    // 타겟
    [HideInInspector] public string enemyTag;
    private GameObject target;
    private GameObject friendlyTarget;
    [HideInInspector] public Vector2 friendlyTargetPos;
    protected GameObject attackTarget;
    [HideInInspector] public Vector2 attackTargetPos;
    private GameObject skillTarget;
    [HideInInspector] public Vector2 skillTargetPos;
    public static Transform unitField;
    public static Transform enemyField;
    public static List<UnitDefault> friends = new();
    public static List<UnitDefault> enemies = new();

    // 콜라이더
    protected BoxCollider2D boxCollider;
    protected CapsuleCollider2D capsuleCollider;
    protected Rigidbody2D rigid2D;

    // 레이어
    protected SortingGroup sortingGroup;

    // 게이지바
    protected GameObject uiGaugeBar;
    [HideInInspector] public Transform uiStateBar;
    public System.Action<float, float> updateHpBarAction;
    public System.Action<float, float> updateMpBarAction;
    [HideInInspector] public Transform relicsState;
    public static bool isReclicsActive;

    // 레벨 이펙트
    [HideInInspector] public Animator levelEffect;

    // 스킬 액션
    public System.Action onSkillAction;
    public System.Action<GameObject> onSkillFireAction;

    [System.NonSerialized]
    public int dealMetricsIdx = -1;
    #endregion

    #region State
    [HideInInspector] public bool isDeath;
    [HideInInspector] public bool isHealer;
    protected Animator animator;
    [HideInInspector] public bool isKiting;
    [HideInInspector] public bool isKitingUnit;
    protected bool isKitingAnim;
    protected bool isMovingAnim;
    protected bool isAttackAnim;
    [HideInInspector] public bool curFlipX;
    protected bool nextFlipX;
    protected bool isSkillAvaliable;
    protected bool isSkillAnim;
    protected bool isStunAnim;
    protected bool isSilence;
    protected bool isInvincible;
    [HideInInspector] public bool isTaunt;
    [HideInInspector] public GameObject tauntUnit;
    bool isTempest;
    protected bool isDeathAnim;
    protected bool isBondage;
    bool isAntiHeal;
    protected bool isEnthrallment;
    bool isVicious;
    bool isLeviathan;
    bool isMadness;
    protected float stunRemainingTime;
    protected float bondageRemainingTime;
    protected float silenceRemainingTime;
    float invincibleRemainingTime;
    float tempestRemainingTime;
    float antiHealRemainingTime;
    float enthrallmentRemainingTime;
    float tauntRemainingTime;
    float viciousRemainingTime;
    float leviathanRemainingTime;
    float madnessRemainingTime;
    int burnCount;
    [HideInInspector] public int poisonCount;
    int bleedingCount;
    [HideInInspector] public int asmodeusCount;
    [HideInInspector] public bool isEquipmentUnit;

    [HideInInspector] public UnitUpgradeStat upgradeStat;
    [HideInInspector] public UnitUpgradeStat itemStat = new();

    [HideInInspector] public Drag dragComp;
    #endregion

    #region Property
    public GameObject Target { get => !isTaunt && (target == null || target.CompareTag("Untagged")) ? null : (isTaunt ? tauntUnit : target); set => target = value; }
    protected GameObject FriendlyTarget { get => friendlyTarget; set => friendlyTarget = value; }
    public GameObject AttackTarget { get => !isTaunt && (attackTarget == null || attackTarget.CompareTag("Untagged")) ? null : (isTaunt ? tauntUnit : attackTarget); set => attackTarget = value; }
    public GameObject SkillTarget { get => !isTaunt && (skillTarget == null || skillTarget.CompareTag("Untagged")) ? null : (isTaunt ? tauntUnit : skillTarget); set => skillTarget = value; }
    public float MaxHealth { get => Mathf.Max((maxHealth + (itemStat.maxHealth + maxHealthStat.extra) * steroid + upgradeStat.maxHealth) * ((maxHealthStat.coeff - 1f) * steroid + 1f), 0f); }
    public int MaxMana { get => Mathf.Max(maxMana, 0); }
    public float Speed { get => Mathf.Min(Mathf.Max((speed + speedStat.extra * steroid + upgradeStat.speed) * ((speedStat.coeff - 1f) * steroid + 1f), 0.1f), 8f); }
    public float AttackPower { get => Mathf.Max((attackPower + (itemStat.attackPower + attackPowerStat.extra) * steroid + upgradeStat.attackPower) * ((attackPowerStat.coeff - 1f) * steroid + 1f), 1f); }
    public float Defense { get => Mathf.Max((defense + (itemStat.defense + defenseStat.extra) * steroid + upgradeStat.defense) * ((defenseStat.coeff - 1f) * steroid + 1f), 0f); }
    public float AttackSpeed { get => Mathf.Max((attackSpeed + (itemStat.attackSpeed + attackSpeedStat.extra) * steroid + upgradeStat.attackSpeed) * ((attackSpeedStat.coeff - 1f) * steroid + 1f), 0.1f); }
    public float ShotSpeed { get => Mathf.Max((shotSpeed + shotSpeedStat.extra * steroid + upgradeStat.shotSpeed) * ((shotSpeedStat.coeff - 1f) * steroid + 1f), 1f); }
    public float AttackRange { get => Mathf.Max((attackRange + attackRangeStat.extra * steroid + upgradeStat.attackRange) * ((attackRangeStat.coeff - 1f) * steroid + 1f), 0.5f); }
    public float CriticalPer { get => Mathf.Min(Mathf.Max((criticalPer + (itemStat.criticalPer + criticalPerStat.extra) * steroid + upgradeStat.criticalPer) * ((criticalPerStat.coeff - 1f) * steroid + 1f), 0f)); }
    public float LifeStealPer { get => Mathf.Max((lifeStealPer + (itemStat.lifeStealPer + lifeStealPerStat.extra) * steroid + upgradeStat.lifeStealPer) * ((lifeStealPerStat.coeff - 1f) * steroid + 1f), 0f); }
    public float Health { get => Mathf.Min(Mathf.Max((health + (itemStat.maxHealth + maxHealthStat.extra) * steroid + upgradeStat.maxHealth) * ((maxHealthStat.coeff - 1f) * steroid + 1f), 0f), MaxHealth); }
    public int CurrentMana { get => Mathf.Min(Mathf.Max(currentMana, 0), MaxMana); set => currentMana = Mathf.Max(Mathf.Min(value, MaxMana), 0); }
    public int Level { get => level + upgradeStat.level; set => level = value; }
    #endregion
    //------------------------------------------------------------------------------
    protected virtual void Awake()
    {
        if (unitIdx != -1 && unitIdx != -2 && unitIdx != 48) {
            if (unitIdx < -2)
                upgradeStat = StaticManager.stat[-unitIdx];
            else {
                upgradeStat = StaticManager.stat[unitIdx];
                StaticManager.totalScore[unitIdx].unitName = unitName;
            }

            upgradeStat.unitName = unitName;
        }

        animator = GetComponent<SPUM_Prefabs>()._anim;
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider2D>();
        rigid2D = GetComponent<Rigidbody2D>();
        if (unitIdx >= 0)
            dragComp = GetComponent<Drag>();

        sortingGroup = transform.GetChild(0).GetComponent<SortingGroup>();

        uiGaugeBar = transform.GetChild(1).gameObject;
        uiStateBar = uiGaugeBar.transform.GetChild(1);
        updateHpBarAction = uiGaugeBar.GetComponent<UiGaugeBar>().UpdateHpBar;
        updateMpBarAction = uiGaugeBar.GetComponent<UiGaugeBar>().UpdateMpBar;

        // 아군이면 적군 태그를, 적군이면 아군 태그를 enemyTag 변수에 저장
        enemyTag = CompareTag("Friendly Unit") ? "Enemy Unit" : "Friendly Unit";

        onSkillAction = OnSkillAttack;

        health = maxHealth;

        if (unitIdx >= 0 && unitIdx != 48) {
            levelEffect = transform.GetChild(0).GetChild(2).GetComponent<Animator>();

            if (!StaticManager.unitSpecificComp.TryGetValue(unitName, out var list)) {
                list = new List<UnitDefault>();
                StaticManager.unitSpecificComp[unitName] = list;
            }
            list.Add(this);

            levelEffect.SetInteger("Level", Level);
        }

        unitInfo = GameManager.instance.unitInfo;
        itemInfo = GameManager.instance.itemInfo;
        eventInfo = GameManager.instance.eventInfo;
        relicsInfo = GameManager.instance.relicsInfo;
        statTxt = GameManager.instance.statTxt;
        statAmountTxt = GameManager.instance.statAmountTxt;

        curHealthImg = GameManager.instance.curHealthImg;
        curManaImg = GameManager.instance.curManaImg;
        descScrollRect = GameManager.instance.descScrollRect;

        if (unitIdx >= 0 && unitIdx != 48) {
            relicsState = transform.GetChild(2);
            SetRelicsState();
        }
    }
    Canvas uiGaugeBarCan;
    protected virtual void Start()
    {
        // isKiting은 게임 도중 껐다 켜질 수 있으므로 isKitingUnit에 기존의 상태를 저장
        isKitingUnit = isKiting;

        capsuleCollider.enabled = true;

        uiGaugeBarCan = uiGaugeBar.GetComponent<Canvas>();
        uiGaugeBarCan.overrideSorting = true;

        StartCoroutine(ChargingMana());

        LuciferDebuff(true);

        SetPotionStateBar();
    }
    protected virtual void FixedUpdate()
    {
        OnMove();
        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.localPosition.y);
        uiGaugeBarCan.sortingOrder = sortingGroup.sortingOrder;

        if (isDeath || isKitingAnim || isMovingAnim)
            return;

        FindNearestEnemy(unitIdx >= 0 ? (isMadness ? friends : enemies) : friends);
        IsFlipX();

        if (isSkillAvaliable && !isSkillAnim && !isStunAnim && !isSilence && !isKitingAnim && !isMovingAnim && !isAttackAnim && !isEnthrallment &&!isMadness)
            UseSkill();

        StartCoroutine(OnAttackAnim());
    }
    //------------------------------------------------------------------------------
    /// <summary>
    /// 타겟의 위치와 비교하여 좌우반전
    /// </summary>
    protected void IsFlipX()
    {
        if (Target == null || isMovingAnim || isKitingAnim)
            return;

        // 변경하지 않아도 되면 무시
        nextFlipX = transform.position.x - Target.transform.position.x < 0;
        if (nextFlipX == curFlipX)
            return;
        // 변경해야 하면 localScale.x값의 부호를 변경, UiGaugeBar도 변경
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        uiGaugeBar.transform.localScale = new Vector2(-uiGaugeBar.transform.localScale.x, uiGaugeBar.transform.localScale.y);
        if (unitIdx >= 0 && unitIdx != 48)
            relicsState.localScale = new Vector2(-relicsState.localScale.x, relicsState.localScale.y);

        curFlipX = nextFlipX; // 현재 상태를 최신화
    }
    /// <summary>
    /// 타겟의 방향으로 공격 사거리까지 이동
    /// </summary>
    protected Coroutine moveSoundCoroutine;
    protected virtual void OnMove()
    {
        if (CheckForTargetRange(AttackRange, Target) || isStunAnim || isSkillAnim || isDeath || isKitingAnim || isMovingAnim || isBondage || isAttackAnim)
            return;

        // 타겟이 없으면 Idle 상태로 전환
        if (Target == null) {
            animator.speed = 1f;
            animator.SetBool("1_Move", false);
            return;
        }

        // 타겟 방향으로 이동속도의 속도로 이동
        animator.speed = Speed;
        animator.SetBool("1_Move", true);
        moveSoundCoroutine ??= StartCoroutine(MoveSound());
        transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, Speed * Time.fixedDeltaTime);
    }
    readonly WaitForSeconds wait = new(0.5f);
    protected IEnumerator MoveSound()
    {
        AudioManager.instance.PlaySfx(GameManager.instance.moveClip);
        yield return wait;

        moveSoundCoroutine = null;
    }
    /// <summary>
    ///  스킬 타겟의 방향으로 스킬 사거리 까지 이동
    /// </summary>
    /// <param name="range">스킬 사거리</param>
    /// <param name="skillTarget">스킬 타겟</param>
    /*protected IEnumerator OnMoveToSkillTarget(float range, GameObject skillTarget)
    {
        if (!isSkillAvaliable || isSkillAnim || isStunAnim || skillTarget == null)
            yield break;
        
        isSkillAnim = true;
        float dist = Vector2.Distance(transform.position, skillTarget.transform.position); // 스킬 타겟과의 거리
        WaitForFixedUpdate wait = new();
        animator.speed = Speed;
        animator.SetBool("1_Move", true);

        // 스킬 타겟이 스킬 사거리 안에 들어올 때 까지 이동
        while (skillTarget != null && dist > range) {
            transform.position = Vector2.MoveTowards(transform.position, skillTarget.transform.position, Speed * Time.fixedDeltaTime);
            dist = Vector2.Distance(transform.position, skillTarget.transform.position); // 스킬 타겟과의 거리 최신화
            yield return wait;
        }
        isSkillAnim = false;

        // 이동이 완료 됐고 스킬 타겟이 null이 아니면 스킬 애니메이션 실행
        if (skillTarget != null)
            StartCoroutine(OnSkillAnim());
    }*/
    /// <summary>
    /// 공격 애니메이션 실행
    /// </summary>
    protected virtual IEnumerator OnAttackAnim()
    {
        if (Target == null || !CheckForTargetRange(AttackRange, Target) || isAttackAnim || isSkillAnim || isStunAnim || isEnthrallment)
            yield break;

        AttackTarget = Target;
        attackTargetPos = Target.transform.position;
        isAttackAnim = true;
        animator.speed = AttackSpeed;
        animator.SetTrigger("2_Attack");
        yield return null;
        // 공격 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed);

        if (!isStunAnim && !isDeath && !isBondage && !isTaunt) {
            if (isKitingUnit && Random.Range(1, 101) <= 40) {
                isKitingAnim = true;
                StartCoroutine(OnKitingAnim()); // 카이팅 실행
            }
            else if (!isKitingUnit && Random.Range(1, 101) <= 40) {
                isMovingAnim = true;
                StartCoroutine(OnMovingAnim()); // 무빙 애니메이션 실행
            }
        }
        isAttackAnim = false;
    }
    /// <summary>
    /// 스킬 애니메이션 실행
    /// </summary>
    protected virtual IEnumerator OnSkillAnim()
    {
        isSkillAnim = true;
        SkillUseAfterState(); // 마나, isSkillAvailable 등 초기화
        animator.speed = AttackSpeed;
        animator.SetTrigger("2_Skill");
        yield return null;
        // 스킬 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / animator.speed);
        isSkillAnim = false;
        if (!isStunAnim && !isDeath && !isBondage && isKiting && Random.Range(1, 101) <= 40) {
            isKitingAnim = true;
            StartCoroutine(OnKitingAnim()); // 카이팅 실행
        }
    }
    /// <summary>
    /// 일정 시간 기절
    /// </summary>
    /// <param name="stunTime">지속 시간</param>
    public virtual void OnStunAnim(float stunTime)
    {
        SetStateBar(State.stun, stunTime);
        StartCoroutine(OnStunAnimRoutine(stunTime));
    }
    IEnumerator OnStunAnimRoutine(float stunTime)
    {
        // 남은 기절 시간과 추가로 들어온 기절 시간을 비교하여 더 큰 값을 저장
        stunRemainingTime = Mathf.Max(stunRemainingTime, stunTime);
        
        if (isStunAnim || isDeath)
            yield break;
        
        isStunAnim = true;
        animator.speed = 1f;
        animator.SetBool("5_Debuff", true);
        // 남은 기절 시간만큼 기절 상태 유지
        while (stunRemainingTime > 0f) {
            stunRemainingTime -= Time.deltaTime;
            yield return null;
        }
        stunRemainingTime = 0f; // 정확한 값 보정
        if (!isDeath)
            animator.SetBool("5_Debuff", false);
        isStunAnim = false;
    }
    /// <summary>
    /// 카이팅 기능
    /// </summary>
    protected IEnumerator OnKitingAnim()
    {
        if (AttackTarget == null) {
            isKitingAnim = false;
            yield break;
        }

        rigid2D.mass = 3f;

        float dist = Vector2.Distance(transform.position, AttackTarget.transform.position); // 타겟과의 거리
        // 타겟과의 거리가 사정거리의 80% 이하이면
        if (dist <= AttackRange * 0.8f && isKiting) {
            animator.speed = Speed;
            animator.SetBool("1_Move", true);
            // 타겟의 반대 방향으로 이동하므로 flipX를 반대로 설정
            curFlipX = !curFlipX;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            uiGaugeBar.transform.localScale = new Vector2(-uiGaugeBar.transform.localScale.x, uiGaugeBar.transform.localScale.y);
            if (unitIdx >= 0 && unitIdx != 48)
                relicsState.localScale = new Vector2(-relicsState.localScale.x, relicsState.localScale.y);
            // 최대 0.3~0.5초 사이의 랜덤한 시간만큼 이동
            float randTime = Random.Range(0.3f, 0.5f);
            float duration = 0f; // 이동 경과 시간
            WaitForFixedUpdate wait = new();
            // 이동 경과시간 초과 또는 최대 사거리만큼 멀어졌거나 isKiting이 false로 변경 될 때까지 이동
            while (duration < randTime && dist < AttackRange && AttackTarget != null && isKiting && !isDeath) {
                duration += Time.fixedDeltaTime;

                transform.Translate(Speed * Time.fixedDeltaTime * ((Vector2)transform.position - (Vector2)AttackTarget.transform.position).normalized);
                dist = Vector2.Distance(transform.position, AttackTarget.transform.position); // 타겟과의 거리 최신화
                yield return wait;
            }
        }
        else {
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
                if (unitIdx >= 0 && unitIdx != 48)
                    relicsState.localScale = new Vector2(-relicsState.localScale.x, relicsState.localScale.y);
            }

            while (duration < randTime && !isDeath && !isBondage && !isStunAnim) {
                duration += Time.fixedDeltaTime;

                transform.Translate(Speed * Time.fixedDeltaTime * dir);
                yield return wait;
            }
        }

        rigid2D.mass = 1f;

        isKitingAnim = false;
    }
    protected virtual IEnumerator OnMovingAnim()
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
            if (unitIdx >= 0 && unitIdx != 48)
                relicsState.localScale = new Vector2(-relicsState.localScale.x, relicsState.localScale.y);
        }

        while (duration < randTime && !isDeath && !isBondage && !isStunAnim) {
            duration += Time.fixedDeltaTime;

            transform.Translate(Speed * Time.fixedDeltaTime * dir);
            yield return wait;
        }

        rigid2D.mass = 1f;

        isMovingAnim = false;
    }
    /// <summary>
    /// 사망 애니메이션 실행
    /// </summary>
    protected virtual IEnumerator OnDeathAnim()
    {
        if (isDeathAnim)
            yield break;

        if (isStunAnim)
            animator.SetBool("5_Debuff", false);
        isDeathAnim = true;
        animator.speed = 1f;
        animator.SetBool("isDeath", true);
        yield return null;
        // 사망 모션이 끝날 때까지 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        Destroy(gameObject);
    }
    //------------------------------------------------------------------------------
    void SetRelicsState()
    {
        relicsState.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.relicsSprList[(int)StaticManager.title[unitIdx][0]];
        relicsState.GetChild(1).GetComponent<Image>().sprite = GameManager.instance.relicsSprList[(int)StaticManager.title[unitIdx][1]];
        relicsState.GetChild(2).GetComponent<Image>().sprite = GameManager.instance.relicsSprList[(int)StaticManager.title[unitIdx][2]];

        relicsState.gameObject.SetActive(isReclicsActive);
    }
    /// <summary>
    /// 사정거리 내에 타겟이 있는지 확인하는 메서드
    /// </summary>
    /// <param name="range">체크할 사정거리</param>
    /// <param name="targetObj">체크할 타겟</param>
    /// <returns>있으면 true, 없으면 false</returns>
    protected bool CheckForTargetRange(float range, GameObject targetObj)
    {
        if (targetObj == null)
            return false;

        // 타겟과의 거리와 사정거리를 비교하여 사정거리가 더 크거나 같으면 true, 작으면 false return
        return range >= Vector2.Distance(transform.position, targetObj.transform.position);
    }
    protected void FindNearestEnemy(List<UnitDefault> unitComps)
    {
        float nearest = float.MaxValue; // 현재 가장 가까운 거리
        GameObject nearestEnemy = null; // 현재 가장 가까운 유닛
        // 모든 적 유닛을 검사하며 가장 가까운 적 유닛을 판별
        foreach (UnitDefault unitComp in unitComps) {
            if (unitComp.CompareTag("Untagged") || unitComp == this)
                continue;

            float dist = Vector2.Distance(transform.position, unitComp.transform.position);
            // 현재 거리보다 가까우면 최신화
            if (dist < nearest) {
                nearest = dist;
                nearestEnemy = unitComp.gameObject;
            }
        }

        // 가장 가까운 적 유닛 target에 저장
        Target = nearestEnemy;
    }
    /// <summary>
    /// 가장 먼 적 유닛을 SkillTarget에 저장
    /// </summary>
    protected void FindFarthestEnemy(List<UnitDefault> unitComps)
    {
        float farthest = 0f; // 현재 가장 먼 거리
        GameObject farthestEnemy = null; // 현재 가장 먼 유닛
        // 모든 적 유닛을 검사하며 가장 먼 적 유닛을 판별
        foreach (UnitDefault unitComp in unitComps) {
            float dist = Vector2.Distance(transform.position, unitComp.transform.position);
            // 현재 거리보다 멀면 최신화
            if (dist > farthest) {
                farthest = dist;
                farthestEnemy = unitComp.gameObject;
            }
        }

        // 가장 먼 적 유닛 SkillTarget에 저장
        SkillTarget = farthestEnemy;
        if (SkillTarget != null)
            skillTargetPos = SkillTarget.transform.position;
    }
    /// <summary>
    ///  가장 체력비가 낮은 아군 유닛을 FriendlyTarget에 저장
    /// </summary>
    /// <param name="isMyself">본인을 포함할 것인지</param>
    protected void FindLesserHpGaugePerFriendly()
    {
        float lesserHpGaugePer = 100f; // 현재 가장 낮은 체력비
        GameObject lesserHpFriend = null; // 현재 가장 체력비가 낮은 아군 유닛
        // 모든 아군을 검사하며 가장 체력비가 낮은 아군을 판별
        foreach (UnitDefault friend in friends) {
            float hpGaugePer = friend.Health / friend.MaxHealth; // 아군 유닛의 체력비 계산
            // 현재보다 낮으면 최신화
            if (lesserHpGaugePer > hpGaugePer) {
                lesserHpGaugePer = hpGaugePer;
                lesserHpFriend = friend.gameObject;
            }
        }
        // 가장 체력비가 낮은 아군 friendlyTarget에 저장
        FriendlyTarget = lesserHpFriend;
        if (FriendlyTarget != null)
            friendlyTargetPos = FriendlyTarget.transform.position;
    }
    /// <summary>
    /// 랜덤한 아군 유닛을 FriendlyTarget에 저장
    /// </summary>
    /// <param name="isMyself">본인을 포함할 것인지</param>
    protected void RandomFriendlyUnit(List<UnitDefault> unitComps)
    {
        var filteredFriends = unitComps.Where(f => f != this).ToList();
        if (filteredFriends.Count == 0) {
            FriendlyTarget = null;
            return;
        }
        FriendlyTarget = filteredFriends[Random.Range(0, filteredFriends.Count)].gameObject;
        friendlyTargetPos = FriendlyTarget.transform.position;
    }
    /// <summary>
    /// 랜덤한 적 유닛을 SkillTarget에 저장
    /// </summary>
    protected void RandomEnemyUnit(List<UnitDefault> unitComps, UnitDefault exceptComp = null)
    {
        if (unitComps.Count == 0) {
            SkillTarget = null;
            return;
        }

        if (unitComps.Count == 1 && unitComps[0] == exceptComp) {
            SkillTarget = null;
            return;
        }

        int randNum = Random.Range(0, unitComps.Count);

        if (unitComps[randNum] == exceptComp) {
            RandomEnemyUnit(unitComps, exceptComp);
            return;
        }

        SkillTarget = unitComps[randNum].gameObject;
        skillTargetPos = SkillTarget.transform.position;
    }
    //------------------------------------------------------------------------------
    /// <summary>
    /// 피격시 호출
    /// </summary>
    /// <param name="attack">피격 데미지</param>
    /// <param name="criticalPer">치명타 확률</param>
    /// <param name="criticalCoeff">치명타 계수</param>
    /// <param name="isPenetration">관통 여부</param>
    /// <returns>총 입은 피해량</returns>
    public virtual float Hit(float attack, float criticalPer, UnitDefault hitUnit, bool isPenetration = false)
    {
        if (isDeath)
            return 0;

        if (isInvincible) {
            GameManager.instance.ShowDamage(0f, transform, Color.white);
            return 0;
        }

        // 치명타 확률 계산
        bool isCritical = Random.Range(1f, 100f) <= criticalPer;
        float damage = attack;
        if (!isPenetration) {
            damage = attack * (100f / (Defense + 100f));
        }
        // 데미지 텍스트 색상 설정
        Color color = Color.red;
        // 치명타 발동 시 치명타 계수 곱셈
        if (isCritical) {
            damage *= criticalCoeff;
            color = Color.yellow;
        }
        // 데미지는 1보다 작을 수 없음
        if (damage <= 1f)
            damage = 1f;

        // 사망시
        if (Health - damage <= 0) {
            // 현재 체력보다 데미지가 높을 경우 데미지는 현재 체력
            damage = Health;
            health -= damage;
            Death();
        }
        // 피격시
        else {
            health -= damage;
        }
        // 체력바 최신화
        updateHpBarAction(MaxHealth, Health);
        UpdateStatInfo();
        // 데미지 표시
        GameManager.instance.ShowDamage(damage, transform, color); // 현재 유닛의 위치에서 데미지 표시

        if (hitUnit.unitIdx >= 0 && hitUnit.unitIdx != 48) {
            StaticManager.totalScore[hitUnit.unitIdx].values[0] += damage;
        }
        if (unitIdx >= 0 && unitIdx != 48) {
            StaticManager.totalScore[unitIdx].values[1] += damage;
        }

        // 총 데미지 return
        return damage;
    }
    /// <summary>
    /// 사망 처리
    /// </summary>
    public virtual void Death()
    {
        isDeath = true;
        gameObject.layer = 0;
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        uiGaugeBar.SetActive(false);
        tag = "Untagged";

        if (unitIdx != -2) {
            GameManager.instance.SetScore((int)rank + 1, unitIdx < -2);

            StaticManager.itemStat.Add(itemStat);
            RespawnManager.instance.RespawnUnit(unitID, dealMetricsIdx, CurrentMana, steroid, StaticManager.itemStat[^1], transform.parent.GetComponent<RectTransform>());
        }

        if (unitIdx >= 0 && friends.Contains(this))
            friends.Remove(this);
        else if (enemies.Contains(this))
            enemies.Remove(this);

        StartCoroutine(OnDeathAnim());
    }
    /// <summary>
    /// 회복
    /// </summary>
    /// <param name="healingAmount">회복량</param>
    public virtual void Healing(float healingAmount, int idx = -1)
    {
        if (healingAmount == 0)
            return;

        if (isAntiHeal)
            healingAmount = 0f; // 회복 불가
        else if (burnCount > 0)
            healingAmount *= 0.6f;

        // 회복 시 최대 체력을 넘어가면 잃은 체력만큼 회복
        healingAmount = healingAmount + Health <= MaxHealth ? healingAmount : MaxHealth - Health;
        if (healingAmount == 0)
            return;

        health += healingAmount;

        Color color = healingAmount >= 0 ? Color.green : Color.red;
        GameManager.instance.ShowDamage(healingAmount, transform, color); // 회복량 표시

        if (idx != -1) {
            StaticManager.totalScore[idx].values[2] += healingAmount;
        }

        updateHpBarAction(MaxHealth, Health);
        UpdateStatInfo();
    }
    /// <summary>
    /// 마나 회복량 만큼 마나 회복
    /// </summary>
    protected virtual IEnumerator ChargingMana()
    {
        if (MaxMana == 0)
            yield break;

        WaitForSeconds wait = new(1f);

        while (true) {
            yield return wait;

            while (isSilence)
                yield return new WaitForSeconds(silenceRemainingTime);

            updateMpBarAction(MaxMana, ++CurrentMana); // 마나바 최신화
            UpdateStatInfo();

            float manaRatio = (float)CurrentMana / MaxMana;  // 0~1
            float t = Mathf.InverseLerp(0.5f, 1f, manaRatio); // 0~1로 정규화
            float result = Mathf.Lerp(20f, 100f, t);           // 20~100으로 변환
            if (CurrentMana >= MaxMana / 2 && Random.Range(1, 101) <= result) {
                isSkillAvaliable = true;
            }
        }
    }
    public virtual void SetMana(int amount)
    {
        if (MaxMana == 0)
            return;

        CurrentMana += amount;
        updateMpBarAction(MaxMana, CurrentMana); // 마나바 최신화
        UpdateStatInfo();

        if (CurrentMana < MaxMana / 2 && isSkillAvaliable)
            isSkillAvaliable = false;
    }
    /// <summary>
    /// dir방향으로 strength의 힘만큼 밀어냄
    /// </summary>
    /// <param name="dir">밀어낼 방향</param>
    /// <param name="strength">밀어낼 힘</param>
    public IEnumerator Pushing(Vector2 dir, float strength)
    {
        float duration = strength / 10f; // 밀리는 시간(멀리 밀쳐질 수록 오래 걸림)
        float elapsedTime = 0f; // 경과 시간
        Vector2 curPos = transform.position; // 현재 위치
        Vector2 targetPos = curPos + (dir * strength); // 밀쳐진 후 위치
        OnStunAnim(duration); // 밀쳐지는 시간 동안 기절 상태 유지
        WaitForFixedUpdate wait = new();

        while (elapsedTime < duration) {
            elapsedTime += Time.fixedDeltaTime;
            // 총 밀쳐질 시간 대비 현재 경과 시간이 얼마나 지났는지 비율을 계산
            float t = elapsedTime / duration;
            // Lerp메서드를 이용해서 위에서 구한 비율만큼 기존 위치에서 밀쳐질 위치로 이동
            transform.position = Vector2.Lerp(curPos, targetPos, t);
            yield return wait;
        }
        transform.position = targetPos; // 정확한 위치 보간
    }
    /// <summary>
    /// 일정 시간 침묵
    /// </summary>
    /// <param name="time">지속 시간</param>
    /// <returns></returns>
    public void Silence(float time)
    {
        SetStateBar(State.silence, time);
        StartCoroutine(SilenceRoutine(time));
    }
    IEnumerator SilenceRoutine(float time)
    {
        if (silenceRemainingTime == -1f)
            yield break;

        if (time == -1f) {
            isSilence = true;
            yield break;
        }

        silenceRemainingTime = Mathf.Max(silenceRemainingTime, time);
        if (isSilence)
            yield break;

        isSilence = true;
        // 남은 침묵 시간만큼 침묵 상태 유지
        while (silenceRemainingTime > 0f) {
            silenceRemainingTime -= Time.deltaTime;
            yield return null;
        }
        silenceRemainingTime = 0f; // 정확한 값 보정
        isSilence = false;
    }
    /// <summary>
    /// 일정 시간 속박
    /// </summary>
    /// <param name="time">지속 시간</param>
    public virtual void Bondage(float time, AnimationClip animClip = null)
    {
        SetStateBar(State.bondage, time);
        StartCoroutine(BondageRoutine(time, animClip));
    }
    IEnumerator BondageRoutine(float time, AnimationClip animClip)
    {
        bondageRemainingTime = Mathf.Max(bondageRemainingTime, time);
        if (isBondage)
            yield break;

        isBondage = true;

        GameObject effect = null;
        if (animClip != null) {
            // 속박 애니메이션 재생
            effect = Instantiate(attackObj, transform);
            effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(animClip, false);
        }

        // 남은 속박 시간만큼 속박 상태 유지
        while (bondageRemainingTime > 0f) {
            bondageRemainingTime -= Time.deltaTime;
            yield return null;
        }
        bondageRemainingTime = 0f; // 정확한 값 보정
        if (animClip != null)
            Destroy(effect); // 속박 애니메이션 제거

        isBondage = false;
    }
    /// <summary>
    /// 일정 시간 무적
    /// </summary>
    /// <param name="time">지속 시간</param>
    public void Invincible(float time)
    {
        SetStateBar(State.invincible, time);
        StartCoroutine(InvincibleRoutine(time));
    }
    IEnumerator InvincibleRoutine(float time)
    {
        invincibleRemainingTime = Mathf.Max(invincibleRemainingTime, time);
        if (isInvincible)
            yield break;

        isInvincible = true;
        while (invincibleRemainingTime > 0f) {
            invincibleRemainingTime -= Time.deltaTime;
            yield return null;
        }
        invincibleRemainingTime = 0f;
        isInvincible = false;
    }
    public void AntiHeal(float time)
    {
        SetStateBar(State.antiHeal, time);
        StartCoroutine(AntiHealRoutine(time));
    }
    IEnumerator AntiHealRoutine(float time)
    {
        antiHealRemainingTime = Mathf.Max(antiHealRemainingTime, time);
        if (isAntiHeal)
            yield break;

        isAntiHeal = true;
        while (antiHealRemainingTime > 0f) {
            antiHealRemainingTime -= Time.deltaTime;
            yield return null;
        }
        antiHealRemainingTime = 0f;
        isAntiHeal = false;
    }
    /// <summary>
    /// 독 공격(방어력 30% 감소)
    /// </summary>
    /// <param name="damage">초당 데미지</param>
    /// <param name="count">횟수</param>
    public void Poison(float damage, int count, UnitDefault hitUnit, int dealIdx = -1)
    {
        SetStateBar(State.poison, count);
        StartCoroutine(PoisonRoutine(damage, count, hitUnit, dealIdx));
        SetStat(attackSpeedStat, 25f, true, false, count);
    }
    IEnumerator PoisonRoutine(float damage, int count, UnitDefault hitUnit, int dealIdx)
    {
        WaitForSeconds wait = new(1f);

        poisonCount++;
        for (int i = 0; i < count; i++) {
            if (isDeath)
                yield break;

            yield return wait;
            float deal = Hit(damage, 0f, hitUnit);
            DealMetrics.instance.UpdateDealMetrics(dealIdx, deal);
        }
        poisonCount--;
    }
    /// <summary>
    /// 화상 공격(치유량 30% 감소)
    /// </summary>
    /// <param name="damage">초당 데미지</param>
    /// <param name="count">횟수</param>
    /// <returns></returns>
    public void Burn(float damage, int count, UnitDefault hitUnit, int dealIdx = -1)
    {
        SetStateBar(State.burn, count);
        StartCoroutine(BurnRoutine(damage, count, hitUnit, dealIdx));
        SetStat(attackPowerStat, 10f, true, false, count);
    }
    IEnumerator BurnRoutine(float damage, int count, UnitDefault hitUnit, int dealIdx)
    {
        WaitForSeconds wait = new(1f);

        burnCount++;
        for (int i = 0; i < count; i++) {
            if (isDeath)
                yield break;

            yield return wait;
            float deal = Hit(damage, 0f, hitUnit);
            DealMetrics.instance.UpdateDealMetrics(dealIdx, deal);
        }
        burnCount--;
    }
    public void Bleeding(float damage, int count, UnitDefault hitUnit, int dealIdx = -1)
    {
        SetStateBar(State.bleeding, count);
        StartCoroutine(BleedingRoutine(damage, count, hitUnit, dealIdx));
        SetStat(attackPowerStat, 15f, true, false, count);
        SetStat(defenseStat, 15f, true, false, count);
    }
    IEnumerator BleedingRoutine(float damage, int count, UnitDefault hitUnit, int dealIdx)
    {
        WaitForSeconds wait = new(1f);

        bleedingCount++;
        for (int i = 0; i < count; i++) {
            if (isDeath)
                yield break;

            yield return wait;
            float deal = Hit(damage, 0f, hitUnit);
            DealMetrics.instance.UpdateDealMetrics(dealIdx, deal);
        }
        bleedingCount--;
    }
    public void DecreaseTempest(float attackSpeed, float speed, float defense, float duration)
    {
        SetStateBar(State.decAttackSpeed, duration);
        SetStateBar(State.decSpeed, duration);
        SetStateBar(State.decDefense, duration);
        StartCoroutine(DecreaseTempestRoutine(attackSpeed, speed, defense, duration));
    }
    IEnumerator DecreaseTempestRoutine(float attackSpeed, float speed, float defense, float time)
    {
        tempestRemainingTime = Mathf.Max(tempestRemainingTime, time);
        if (isTempest)
            yield break;

        isTempest = true;
        SetStat(attackSpeedStat, attackSpeed, false, false);
        SetStat(speedStat, speed, false, false);
        SetStat(defenseStat, defense, false, false);
        while (tempestRemainingTime > 0f) {
            tempestRemainingTime -= Time.deltaTime;
            yield return null;
        }
        tempestRemainingTime = 0f;
        SetStat(attackSpeedStat, attackSpeed, false, true);
        SetStat(speedStat, speed, false, true);
        SetStat(defenseStat, defense, false, true);
        isTempest = false;
    }
    Coroutine co;
    public void Taunt(GameObject tauntUnit, float time)
    {
        if (time < tauntRemainingTime)
            return;

        if (co != null) {
            StopCoroutine(co);
            co = null;
        }

        if (isAttackAnim) {
            isAttackAnim = false;
            animator.CrossFade("IDLE", 0f);
        }

        this.tauntUnit = tauntUnit;
        co = StartCoroutine(TauntRoutine(tauntUnit.GetComponent<UnitDefault>(), time));

        SetStateBar(State.taunt, time);
    }
    IEnumerator TauntRoutine(UnitDefault tauntUnit, float time)
    {
        isTaunt = true;

        WaitForFixedUpdate wait = new();
        tauntRemainingTime = time;
        while (tauntRemainingTime > 0f) {
            if (tauntUnit == null || tauntUnit.isDeath) {
                isTaunt = false;
                tauntRemainingTime = 0f;
                Transform tauntState = uiStateBar.Find("taunt");
                if (tauntState != null)
                    Destroy(tauntState.gameObject);

                if (isAttackAnim || isSkillAnim) {
                    isAttackAnim = false;
                    isSkillAnim = false;
                    animator.CrossFade("IDLE", 0f);
                }

                yield break;
            }
            yield return wait;
            tauntRemainingTime -= Time.fixedDeltaTime;
        }

        tauntRemainingTime = 0f;
        isTaunt = false;
    }
    public void Enthrallment(float duration)
    {
        SetStateBar(State.enthrallment, duration);
        StartCoroutine(EnthrallmentRoutine(duration));
    }
    IEnumerator EnthrallmentRoutine(float duration)
    {
        enthrallmentRemainingTime = Mathf.Max(enthrallmentRemainingTime, duration);
        if (isEnthrallment)
            yield break;

        isEnthrallment = true;
        while (enthrallmentRemainingTime > 0f) {
            enthrallmentRemainingTime -= Time.deltaTime;
            yield return null;
        }
        enthrallmentRemainingTime = 0f;
        isEnthrallment = false;
    }
    public void AddDoomList()
    {
        Doom[] dooms = enemyField.GetComponentsInChildren<Doom>();
        
        foreach (Doom doom in dooms) {
            doom.doomList.Add(this);
        }
    }
    public void ViciousDebuff(float decAttackPer, float duration)
    {
        SetStateBar(State.decAttack, duration);
        StartCoroutine(ViciousDebuffRoutine(decAttackPer, duration));
    }
    IEnumerator ViciousDebuffRoutine(float decAttackPer, float duration)
    {
        viciousRemainingTime = duration;
        if (isVicious)
            yield break;

        isVicious = true;
        attackPowerStat.coeff *= (100f - decAttackPer) / 100f;
        UpdateStatInfo();

        while (viciousRemainingTime > 0f) {
            viciousRemainingTime -= Time.deltaTime;
            yield return null;
        }

        attackPowerStat.coeff /= (100f - decAttackPer) / 100f;
        UpdateStatInfo();
        viciousRemainingTime = 0f;
        isVicious = false;
    }
    public void LeviathanDebuff(float decSpeedPer, float decDefenseAmount, float duration)
    {
        SetStateBar(State.decSpeed, duration);
        SetStateBar(State.decDefense, duration);
        StartCoroutine(LeviathanDebuffRoutine(decSpeedPer, decDefenseAmount, duration));
    }
    IEnumerator LeviathanDebuffRoutine(float decSpeedPer, float decDefenseAmount, float duration)
    {
        leviathanRemainingTime = duration;
        if (isLeviathan)
            yield break;

        isLeviathan = true;
        speedStat.coeff *= (100f - decSpeedPer) / 100f;
        defenseStat.extra -= decDefenseAmount;
        UpdateStatInfo();

        while (leviathanRemainingTime > 0f) {
            leviathanRemainingTime -= Time.deltaTime;
            yield return null;
        }

        speedStat.coeff /= (100f - decSpeedPer) / 100f;
        defenseStat.extra += decDefenseAmount;
        UpdateStatInfo();
        leviathanRemainingTime = 0f;
        isLeviathan = false;
    }
    public void Madness(float duration)
    {
        SetStateBar(State.madness, duration);
        StartCoroutine(MadnessRoutine(duration));
    }
    IEnumerator MadnessRoutine(float duration)
    {
        madnessRemainingTime = Mathf.Max(madnessRemainingTime, duration);
        if (isMadness)
            yield break;

        isMadness = true;
        enemyTag = tag;

        while (madnessRemainingTime > 0f) {
            madnessRemainingTime -= Time.deltaTime;
            yield return null;
        }

        enemyTag = "Enemy Unit";
        madnessRemainingTime = 0f;
        isMadness = false;
    }
    TextMeshProUGUI asmodeusCntTxt;
    public void AsmodeusDebuff(GameObject asmodeusState, float decAttackAmount, float decDefenseAmount)
    {
        if (asmodeusCount == 5)
            return;

        if (asmodeusCount == 0) {
            GameObject state = Instantiate(asmodeusState, uiStateBar);
            asmodeusCntTxt = state.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        asmodeusCntTxt.SetText((++asmodeusCount).ToString());
        SetStat(attackPowerStat, decAttackAmount, false, false);
        SetStat(defenseStat, decDefenseAmount, false, false);
    }
    TextMeshProUGUI luciferTxt;

    public void LuciferDebuff(bool isInit)
    {
        if (itemStat.level == 0) {
            if (!isInit) {
                Destroy(luciferTxt.transform.parent.gameObject);

                SetStat(attackPowerStat, Lucifer.DecAttackAmount * 3f, false, true);
                SetStat(defenseStat, Lucifer.DecDefenseAmount * 3f, false, true);
            }
            return;
        }

        if (luciferTxt == null) {
            GameObject state = Instantiate(GameManager.instance.luciferState, uiStateBar);
            luciferTxt = state.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        luciferTxt.SetText(itemStat.level.ToString());

        if (isInit) {
            SetStat(attackPowerStat, Lucifer.DecAttackAmount * itemStat.level, false, false);
            SetStat(defenseStat, Lucifer.DecDefenseAmount * itemStat.level, false, false);
        }
        else {
            SetStat(attackPowerStat, Lucifer.DecAttackAmount, false, false);
            SetStat(defenseStat, Lucifer.DecDefenseAmount, false, false);
        }
    }
    /// <summary>
    /// 스탯 버프/디버프
    /// </summary>
    /// <param name="amount">증감 양</param>
    /// <param name="isPer">퍼센테이지 여부</param>
    /// <param name="isIncrease">증가 여부</param>
    /// <param name="time">지속 시간</param>
    public void SetStat(StatWrapper stat, float amount, bool isPer, bool isIncrease, float time = 0f)
    {
        StartCoroutine(SetStatRoutine(stat, amount, isPer, isIncrease, time));
    }
    IEnumerator SetStatRoutine(StatWrapper stat, float amount, bool isPer, bool isIncrease, float time)
    {
        float increasePer = (100f + amount) / 100f;
        float decreasePer = (100f - amount) / 100f;
        if (decreasePer <= 0f)
            decreasePer = 0.01f;
        if (time > 0f) {
            if (isPer) {
                stat.coeff *= isIncrease ? increasePer : decreasePer;
                UpdateStatInfo();
                yield return new WaitForSeconds(time);
                stat.coeff /= isIncrease ? increasePer : decreasePer;
            }
            else {
                stat.extra += isIncrease ? amount : -amount;
                UpdateStatInfo();
                yield return new WaitForSeconds(time);
                stat.extra -= isIncrease ? amount : -amount;
            }
        }
        else {
            if (isPer) {
                stat.coeff *= isIncrease ? increasePer : decreasePer;
            }
            else {
                stat.extra += isIncrease ? amount : -amount;
            }
        }
        UpdateStatInfo();
    }
    /// <summary>
    /// 상태이상 표시
    /// </summary>
    /// <param name="state">상태이상 효과</param>
    /// <param name="duration">지속 시간 / -2 = INF</param>
    public void SetStateBar(State state, float duration)
    {
        Sprite img = GameManager.instance.stateSprites[(int)state];
        for (int i = 0; i < uiStateBar.childCount; i++) {
            if (uiStateBar.GetChild(i).name == state.ToString() && uiStateBar.GetChild(i).name != "wantedList" && uiStateBar.GetChild(i).name != "steroid") {
                RemainingTime remainingTime = uiStateBar.GetChild(i).GetChild(0).GetComponent<RemainingTime>();
                if (remainingTime.totalTime != -2f) {
                    remainingTime.totalTime = Mathf.Max(remainingTime.time, duration);
                    remainingTime.time = 0f;
                }
                return;
            }
        }
        GameObject stateImg = Instantiate(GameManager.instance.stateImgPrefab, uiStateBar);
        stateImg.name = state.ToString();
        stateImg.GetComponent<Image>().sprite = img;
        RemainingTime remainingComp = stateImg.transform.GetChild(0).GetComponent<RemainingTime>();
        remainingComp.totalTime = duration;
    }
    public void SetPotionStateBar()
    {
        if (itemStat.maxHealth != 0)
            SetStateBar(State.HPPotion, -2f);
        else if (itemStat.attackPower != 0)
            SetStateBar(State.ATKPotion, -2f);
        else if (itemStat.defense != 0)
            SetStateBar(State.DEFPotion, -2f);
        else if (itemStat.attackSpeed != 0)
            SetStateBar(State.DPSPotion, -2f);
        else if (itemStat.lifeStealPer != 0)
            SetStateBar(State.LSPotion, -2f);
        else if (itemStat.criticalPer != 0)
            SetStateBar(State.CRTPotion, -2f);
    }
    //------------------------------------------------------------------------------
    /// <summary>
    /// 원거리 기본 공격(애니메이션 이벤트용)
    /// </summary>
    /// <param name="isSkill">스킬 공격 여부</param>
    /// <param name="isPenetration">관통 여부</param>
    public virtual IEnumerator OnFire(GameObject target, Vector2 targetPos, bool isSkill = true, bool isPenetration = false)
    {
        Vector2 startPos = transform.position; // 시작 위치
        if (isPenetration)
            targetPos = startPos + (targetPos - startPos).normalized * 20f;

        float distance = Vector2.Distance(startPos, targetPos); // 이동할 총 거리
        
        float duration = distance / ShotSpeed; // 총 이동 시간 계산
        float elapsedTime = 0f; // 경과 시간
        // 총알 회전각 계산
        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        // 프리팹 인스턴스화
        GameObject bullet = Instantiate(bulletObj, startPos, rotation);
        bulletList.Add(bullet);
        if (isSkill)
            bullet.tag = "Skill Bullet";
        if (isPenetration)
            bullet.tag = "Penetration Bullet";

        // 인스턴스의 Bullet 컴포넌트 필드 초기화
        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.SetAnimatorOverrideController(isSkill ? skillBulletAnimClip : bulletAnimClip, attackSize);
        bulletComp.unitDefault = this;
        bulletComp.target = target;
        if (isPenetration)
            bulletComp.enemyTag = enemyTag;

        if (!isSkill)
            SoundPlay(attackSoundClip);
        else
            SoundPlay(skillSoundClip);

        WaitForFixedUpdate wait = new();
        // 발사체가 타겟으로 이동하는 로직
        while (elapsedTime < duration) {
            if (bullet == null)
                yield break;

            // 경과 시간 업데이트
            elapsedTime += Time.fixedDeltaTime;

            // 현재 시간에 비례하여 이동할 거리 계산
            float t = elapsedTime / duration;

            // 발사체의 위치 계산 (Lerp 사용)
            bullet.transform.position = Vector2.Lerp(startPos, targetPos, t);

            // 다음 프레임 대기
            yield return wait;
        }

        if (bullet != null) {
            // 마지막 위치를 타겟의 위치로 설정 (정확한 위치 보정)
            bullet.transform.position = targetPos;

            // 발사체 제거
            Destroy(bullet);
        }
    }
    /// <summary>
    /// 근거리 기본 공격(애니메이션 이벤트 용)
    /// </summary>
    public virtual void OnShortAttack()
    {
        if (AttackTarget == null)
            return;

        SoundPlay(attackSoundClip);

        CreateAttackBox(AttackPower, AttackTarget, AttackTarget.transform.position, attackStyle, false);
    }
    /// <summary>
    /// 피격 판정 범위를 생성, 공격 이펙트 생성
    /// </summary>
    /// <param name="attackPower">공격력</param>
    /// <param name="attackStyle">공격 스타일</param>
    /// <param name="isSkill">스킬 공격 여부</param>
    /// <param name="isPenetration">관통 여부</param>
    public virtual float CreateAttackBox(float attackPower, GameObject target, Vector2 targetPos, AttackStyle attackStyle = AttackStyle.single, bool isSkill = true, bool isPenetration = false)
    {
        Quaternion rotation = Quaternion.identity;

        // 회전 이펙트가 필요한 경우
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            Vector2 thisPos = transform.position;

            // 타겟 방향 계산
            Vector2 dir = (targetPos - thisPos).normalized;

            // 회전 방향 계산
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 공격 이펙트 오브젝트 생성
        GameObject effect = Instantiate(attackObj, targetPos, rotation);

        // 공격 타입에 맞는 클립 할당
        effect.GetComponent<DestroyEffect>().SetAnimatorOverrideController(isSkill ? skillAnimClip : attackAnimClip);
        Vector2 scale = effect.transform.localScale;
        // 회전 이펙트를 적용한 경우만
        if ((!isSkill && isAttackEffectRotation) || (isSkill && isSkillEffectRotation)) {
            // 오른쪽에서 왼쪽으로 공격 시 이펙트가 상하 반전되는 현상을 제거
            scale.y *= !curFlipX ? -1f : 1f;
        }
        // 회전 이펙트를 적용 하지 않은 경우
        else {
            // 방향에 따른 이펙트 좌우 반전
            scale.x *= !curFlipX ? -1f : 1f;
        }
        // 스케일 변경 및 적용
        scale *= attackSize;
        effect.transform.localScale = scale;

        if (attackPower == -1f)
            return 0f;

        // 박스 캐스트 수행
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPos, attackSize / 2f, LayerMask.GetMask(enemyTag));

        float damage = 0f;

        // 범위 내 적이 있을 경우 처리
        if (hitEnemies.Length > 0) {
            if (attackStyle == AttackStyle.single) {
                foreach (Collider2D enemy in hitEnemies) {
                    if (enemy.gameObject == target) {
                        damage = target.GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
                        break;
                    }
                }
            }
            else {
                for (int i = hitEnemies.Length - 1; i >= 0; i--)
                    damage += hitEnemies[i].GetComponent<UnitDefault>().Hit(attackPower, CriticalPer, this, isPenetration);
            }

            // 피흡 계산
            float lifeSteal = damage * LifeStealPer / 100f;
            Healing(lifeSteal);

            DealMetrics.instance.UpdateDealMetrics(dealMetricsIdx, damage);
        }
        return damage;
    }
    /// <summary>
    /// 스킬 사용 후 상태 설정
    /// </summary>
    protected virtual void SkillUseAfterState() {
        isSkillAvaliable = false;
        CurrentMana -= MaxMana / 2;
        updateMpBarAction(MaxMana, CurrentMana);
        UpdateStatInfo();
    }
    public void SoundPlay(AudioClip clip)
    {
        AudioManager.instance.PlaySfx(clip);
    }
    public void SetLevelEffect(int level)
    {
        List<UnitDefault> unitSpecificList = StaticManager.unitSpecificComp[unitName];
        for (int i = 0; i < unitSpecificList.Count; i++) {
            unitSpecificList[i].levelEffect.SetInteger("Level", level);

            if (level == 0) {
                unitSpecificList[i].upgradeStat = StaticManager.stat[unitIdx];
            }
        }
    }
    //------------------------------------------------------------------------------
    /// <summary>
    /// 씬에서 유닛 선택 시 사정거리 표시(원)
    /// </summary>
    /*void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
    /// <summary>
    ///  유닛의 공격 방향, 사거리 시각화(직선)
    /// </summary>
    void OnDrawGizmos()
    {
        if (Target == null)
            return;

        // 타겟 방향 계산
        Vector2 direction = (Target.transform.position - transform.position).normalized;

        // 공격 방향 화살표 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * AttackRange);
    }*/
    //------------------------------------------------------------------------------
    protected virtual void OnEnable()
    {
        if (unitIdx >= 0) {
            dragComp.enabled = false;

            DealMetrics.instance.CreateDealMetrics(this);

            friends.Add(this);
        }
        else
            enemies.Add(this);
    }
    /// <summary>
    /// 벽에 닿으면 카이팅 중지
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.enabled)
            return;

        if (collision.CompareTag("Wall"))
            isKiting = false;
    }
    /// <summary>
    /// 벽에서 떨어지면 카이팅 재시작
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!this.enabled)
            return;

        if (collision.CompareTag("Wall"))
            isKiting = isKitingUnit;
    }
    protected virtual void OnDestroy()
    {
        foreach (GameObject bullet in bulletList) {
            if (bullet != null)
                Destroy(bullet);
        }

        if (StaticManager.unitSpecificComp.TryGetValue(unitName, out var list)) {
            list.Remove(this);
        }

        if (!GameManager.instance.isEnd) {
            if (dealMetricsIdx != -1) {
                DealMetrics.instance.transform.GetChild(dealMetricsIdx).GetComponent<Button>().interactable = false;
            }
            if (unitIdx >= 0 && friends.Contains(this))
                friends.Remove(this);
            else if (enemies.Contains(this))
                enemies.Remove(this);
        }
    }
    //------------------------------------------------------------------------------
    protected abstract void UseSkill();
    protected abstract void OnSkillAttack();
    public abstract void UpdateSkillDesc();

    protected virtual void SetUpgradeAction() { }
    public virtual string GetSkillUpDesc() { return null; }
    //------------------------------------------------------------------------------
    public virtual void UpgradeApply()
    {
        upgradeStat.level++;
        upgradeStat.maxHealth = MaxHealth - maxHealth;
        upgradeStat.speed = Speed - speed;
        upgradeStat.attackPower = AttackPower - attackPower;
        upgradeStat.defense = Defense - defense;
        upgradeStat.attackSpeed = AttackSpeed - attackSpeed;
        upgradeStat.shotSpeed = ShotSpeed - shotSpeed;
        upgradeStat.attackRange = AttackRange - attackRange;
        upgradeStat.criticalPer = CriticalPer - criticalPer;
        upgradeStat.lifeStealPer = LifeStealPer - lifeStealPer;

        SetLevelEffect(Level);

        if (Level == 9)
            SteamAchievement.Unlock("극한의 단련");

        AnalyticsInitializer.instance.Upgrade(unitName, Level);
    }

    public void UpdateStatInfo(bool isPointerEnter = false, bool isUpgradeUnit = false)
    {
        if ((!unitInfo.activeSelf || StaticManager.activeUnit != this) && !isPointerEnter)
            return;

        if (isDeath) {
            StaticManager.activeUnit = null;
            unitInfo.SetActive(false);
            return;
        }

        if (isUpgradeUnit) {
            unitInfo = GameManager.instance.upUnitInfo;
            statTxt = GameManager.instance.upStatTxt;
            statAmountTxt = GameManager.instance.upStatAmountTxt;
            descScrollRect = GameManager.instance.upDescScrollRect;
            itemInfo = GameManager.instance.upItemInfo;
        }
        else
            StaticManager.activeUnit = this;

        itemInfo.SetActive(false);
        eventInfo.SetActive(false);
        relicsInfo.SetActive(false);

        statTxt[0].SetText(GetLocalizedUnitName());
        statTxt[1].SetText(!isUpgradeUnit ? Level.ToString() : (Level + 1).ToString());
        if (unitIdx >= 0 && unitIdx != 48) {
            statTxt[1].text += $"/{StaticManager.maxLevel[unitIdx]}";
        }
        statTxt[2].SetText(cost.ToString());

        curHealthImg.fillAmount = Health / MaxHealth;
        if (MaxMana != 0f)
            curManaImg.fillAmount = (float)CurrentMana / MaxMana;

        var stats = new (int index, float baseVal, float actualVal, StatWrapper stat, float itemStat, string format, string suffix, string prefix)[]
        {
            (3, MaxHealth, maxHealth + upgradeStat.maxHealth, maxHealthStat, itemStat.maxHealth, "F0", "", ""),
            (4, Health, health, null, 0f, "F0", "", ""),
            (5, MaxMana, maxMana + upgradeStat.maxMana, maxManaStat, itemStat.maxMana, "F0", "", ""),
            (6, CurrentMana, currentMana, null, 0f, "F0", "", ""),
            (7, AttackPower, attackPower + upgradeStat.attackPower, attackPowerStat, itemStat.attackPower, "F0", "", ""),
            (8, Defense, defense + upgradeStat.defense, defenseStat, itemStat.defense, "F1", "", ""),
            (9, AttackSpeed, attackSpeed + upgradeStat.attackSpeed, attackSpeedStat, itemStat.attackSpeed, "F2", "", ""),
            (10, Speed, speed + upgradeStat.speed, speedStat, itemStat.speed, "F1", "", ""),
            (11, AttackRange, attackRange + upgradeStat.attackRange, attackRangeStat, itemStat.attackRange, "F1", "", ""),
            (12, CriticalPer, criticalPer + upgradeStat.criticalPer, criticalPerStat, itemStat.criticalPer, "F1", "%", ""),
            (13, LifeStealPer, lifeStealPer + upgradeStat.lifeStealPer, lifeStealPerStat, itemStat.lifeStealPer, "F1", "%", "")
        };

        foreach (var (index, baseVal, actualVal, stat, itemStat, format, suffix, prefix) in stats)
            SetStatText(index, baseVal, actualVal, stat, itemStat, format, suffix, prefix);

        UpdateSkillDesc();
        statTxt[14].SetText($"<color=#FFD700>[{GetLocalizedUnitSkillName()}]</color> {(unitIdx >= 0 && unitIdx != 48 ? $" <color=#FFBF00>[{StaticManager.skillPoint[unitIdx]}P]</color>" : "")}\n{GetLocalizedUnitSkillDesc()}");
        if (unitIdx >= 0 && unitIdx != 48)
        {
            statTxt[15].SetText(StaticManager.GetLocalizedTitleString(unitIdx));
            statTxt[15].gameObject.SetActive(true);
        }
        else
            statTxt[15].gameObject.SetActive(false);


        if (isPointerEnter) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(descScrollRect.content);
            StartCoroutine(SetScrollTopNextFrame());
        }

        if (!isUpgradeUnit) {
            statAmountTxt[0].SetText(upgradeStat.maxHealth.ToString("F0"));
            statAmountTxt[1].SetText(upgradeStat.attackPower.ToString("F0"));
            statAmountTxt[2].SetText(upgradeStat.defense.ToString("F0"));
            statAmountTxt[3].SetText(upgradeStat.attackSpeed.ToString("F1"));
            statAmountTxt[4].SetText(upgradeStat.attackRange.ToString("F1"));
            statAmountTxt[5].SetText(upgradeStat.criticalPer.ToString("F0"));
            statAmountTxt[6].SetText(upgradeStat.lifeStealPer.ToString("F0"));
        }
        else {
            statAmountTxt[0].SetText((MaxHealth - maxHealth).ToString("F0"));
            statAmountTxt[1].SetText((AttackPower - attackPower).ToString("F0"));
            statAmountTxt[2].SetText((Defense - defense).ToString("F0"));
            statAmountTxt[3].SetText((AttackSpeed - attackSpeed).ToString("F1"));
            statAmountTxt[4].SetText((AttackRange - attackRange).ToString("F1"));
            statAmountTxt[5].SetText((CriticalPer - criticalPer).ToString("F0"));
            statAmountTxt[6].SetText((LifeStealPer - lifeStealPer).ToString("F0"));
        }

        unitInfo.SetActive(true);
    }

    void SetStatText(int index, float baseValue, float actualValue = -1f, StatWrapper stat = null, float itemStat = 0f, string format = "F0", string suffix = "", string prefix = "")
    {
        if (actualValue < 0) actualValue = baseValue;
        string text = $"{prefix}{baseValue.ToString(format)}{suffix}";

        if (stat != null && ((stat.extra + itemStat) != 0f || stat.coeff != 1f)) {
            float extra = (stat.extra + itemStat) * steroid;
            float coeff = (stat.coeff - 1f) * steroid + 1f;

            text += $" ({prefix}{actualValue.ToString(format)}";

            if (extra != 0f) {
                string color = extra > 0 ? "#00FF00" : "#FF5050"; // 초록 or 빨강
                string sign = extra > 0 ? "+" : ""; // 음수면 자동으로 '-' 붙음
                text += $"{sign}<color={color}>{extra.ToString(format)}</color>";
            }

            if (coeff != 1f) {
                string color = coeff > 1f ? "#00FF00" : "#FF5050"; // 초록 or 빨강
                text += $"x<color={color}>{coeff:F2}</color>";
            }

            text += ")";
        }

        statTxt[index].SetText(text);
    }

    IEnumerator SetScrollTopNextFrame()
    {
        yield return null;
        descScrollRect.verticalNormalizedPosition = 1f;
    }

    public void ResetRelicsEffect()
    {
        maxHealthStat.coeff = 1f;
        attackPowerStat.coeff = 1f;
        defenseStat.extra = 0f;
        attackSpeedStat.coeff = 1f;
        criticalPerStat.extra = 0f;
        lifeStealPerStat.extra = 0f;
    }

    public string GetLocalizedUnitName()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        if (code.StartsWith("ko")) return unitName;
        if (code.StartsWith("en")) return ENUnitName;
        if (code.StartsWith("fr")) return FRUnitName;
        if (code.StartsWith("it")) return ITUnitName;
        if (code.StartsWith("de")) return DEUnitName;
        if (code.StartsWith("es")) return ESUnitName;
        if (code.StartsWith("ja")) return JAUnitName;
        if (code.StartsWith("pt")) return PT_BRUnitName;
        if (code.StartsWith("ru")) return RUUnitName;
        if (code.StartsWith("zh")) return ZH_HANSUnitName;

        // fallback
        return ENUnitName;
    }

    public string GetLocalizedUnitSkillName()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        if (code.StartsWith("ko")) return skillName;
        if (code.StartsWith("en")) return ENSkillName;
        if (code.StartsWith("fr")) return FRSkillName;
        if (code.StartsWith("it")) return ITSkillName;
        if (code.StartsWith("de")) return DESkillName;
        if (code.StartsWith("es")) return ESSkillName;
        if (code.StartsWith("ja")) return JASkillName;
        if (code.StartsWith("pt")) return PT_BRSkillName;
        if (code.StartsWith("ru")) return RUSkillName;
        if (code.StartsWith("zh")) return ZH_HANSSkillName;

        // fallback
        return ENSkillName;
    }
    public string GetLocalizedUnitSkillDesc()
    {
        string code = LocalizationSettings.SelectedLocale.Identifier.Code.ToLower();

        if (code.StartsWith("ko")) return skillDesc;
        if (code.StartsWith("en")) return ENSkillDesc;
        if (code.StartsWith("fr")) return FRSkillDesc;
        if (code.StartsWith("it")) return ITSkillDesc;
        if (code.StartsWith("de")) return DESkillDesc;
        if (code.StartsWith("es")) return ESSkillDesc;
        if (code.StartsWith("ja")) return JASkillDesc;
        if (code.StartsWith("pt")) return PT_BRSkillDesc;
        if (code.StartsWith("ru")) return RUSkillDesc;
        if (code.StartsWith("zh")) return ZH_HANSSkillDesc;

        // fallback
        return ENSkillDesc;
    }
}