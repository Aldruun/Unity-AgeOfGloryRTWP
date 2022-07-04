using Unity.Mathematics;
using UnityEngine;

public abstract class QuestStage : ScriptableObject
{
    public abstract void Init();
    public abstract bool Complete();
    public abstract float3 GetLocation();
}