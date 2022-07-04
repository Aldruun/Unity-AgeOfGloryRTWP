using UnityEngine;

public class Locomotion
{
    private readonly int m_AgularSpeedId;
    public float m_AnguarSpeedDampTime = 0.25f;
    private readonly Animator m_Animator;
    private readonly int m_DirectionId;
    public float m_DirectionResponseTime = 0.2f;

    public float m_SpeedDampTime = 0.1f;

    private readonly int m_SpeedId;

    public Locomotion(Animator animator)
    {
        m_Animator = animator;

        m_SpeedId = Animator.StringToHash("Speed");
        m_AgularSpeedId = Animator.StringToHash("AngularSpeed");
        m_DirectionId = Animator.StringToHash("Direction");
    }

    public void Do(float speed, float direction)
    {
        var state = m_Animator.GetCurrentAnimatorStateInfo(0);

        var inTransition = m_Animator.IsInTransition(0);
        var inIdle = state.IsName("Locomotion.Idle");
        var inTurn = state.IsName("Locomotion.TurnOnSpot") || state.IsName("Locomotion.PlantNTurnLeft") ||
                     state.IsName("Locomotion.PlantNTurnRight");
        var inWalkRun = state.IsName("Locomotion.WalkRun");

        var speedDampTime = inIdle ? 0 : m_SpeedDampTime;
        var angularSpeedDampTime = inWalkRun || inTransition ? m_AnguarSpeedDampTime : 0;
        float directionDampTime = inTurn || inTransition ? 1000000 : 0;

        var angularSpeed = direction / m_DirectionResponseTime;

        m_Animator.SetFloat(m_SpeedId, speed, speedDampTime, Time.deltaTime);
        m_Animator.SetFloat(m_AgularSpeedId, angularSpeed, angularSpeedDampTime, Time.deltaTime);
        m_Animator.SetFloat(m_DirectionId, direction, directionDampTime, Time.deltaTime);
    }
}