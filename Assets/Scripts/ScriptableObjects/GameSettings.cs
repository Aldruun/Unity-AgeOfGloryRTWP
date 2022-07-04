using UnityEngine;

public enum GameDifficulty
{
    EASY,
    NORMAL,
    HARD
}

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public GameDifficulty gameDifficulty = GameDifficulty.NORMAL;

    [Header("Player Settings")]
    public bool friendlyFire = false;

    [Header("General Settings")]
    public float interactionDistance = 2;
    public float actorMoveDampRun = 3;
    public float actorMoveDampWalk = 5;
    public AnimationCurve expExponentialCurve;

    [Header("AI")]
    public LayerMask agentLayers;
    public float aiUpdateInterval = 0.2f;
    public float baseAggroRange = 0.2f;
    [Range(0, 10)] public float globalExpMult = 1;
    [Range(0, 10)] public float globalDmgMult = 1;

    public float globalManaCostMult = 1;
    public float globalManaRegenMult = 1;
    public AnimationCurve jumpCurve;
    public Vector3 upperBodyAimOffset;

    [Header("Navigation")]
    public LayerMask groundLayers;
    public float m_climbSpeedLadder = 2;

    [Header("Weapons Control")]
    public float m_delay_drawWeapon_lefthip = 0.4f;
    public float m_delay_sheathWeapon_lefthip = 0.4f;

    [Header("UI")]
    public Gradient ActorCircleFlashGradient;
    public float m_floatingInfoHeight = 2;

    [Header("Motion")]
    public float rotationSpeed = 120;

    [Header("Developer Settings")]
    public bool DebugAILineOfSight;
    public bool DebugAITargetScans;
    public bool DebugSkillAOEProjectile;
    public bool DebugSkillBeamProjectile;
    public bool DebugProjectileOnTrigger;
    //private void Awake()
    //{
    //    DebugAILineOfSight = debugAILineOfSight;
    //    DebugAITargetScans = debugAITargetScans;

    //}
}