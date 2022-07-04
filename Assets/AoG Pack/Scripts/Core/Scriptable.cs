using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ScriptableType
{
	ACTOR = 0,
	PROXIMITY = 1,
	TRIGGER = 2,
	TRAVEL = 3,
	DOOR = 4,
	CONTAINER = 5,
	AREA = 6,
	GLOBAL = 7
}

public abstract class Scriptable
{
	public static int globalScriptableCounter = 10000;
	private int _globalID;

	public bool debug;
	public bool debugActions;
	public bool debugRoundSystem;

	public ScriptableType type;
	public ScriptableMonoObject scrMono;
	public Transform transform;

	protected int lastDamage;
	private int CurrentActionTarget = 0;
	private bool CurrentActionInterruptable = true;
	private int CurrentActionTicks = 0;
	private int CurrentActionState = 0;
	protected InternalFlags internalFlags;
	protected float waitTimer;

	private Map _area;

	/// <summary>
	/// Initializes scripts array and actionQueue and sets GlobalID
	/// </summary>
	public virtual void InitScriptable(ScriptableType type)
    {
		this.type = type;
		_globalID = ++globalScriptableCounter;

	}

 //   void Awake()
 //   {
	//	GameMaster.OnScriptableInit += Init;
	//}

 //   void OnDisable()
	//{
	//	GameMaster.OnScriptableInit -= Init;

	//	//behaviours.Clear();
	//	//behaviours = null;
	//}

	//hack Set by the mono object itself, inside SpawnPoint.cs for now
	public virtual void SetScriptable(ScriptableMonoObject newScrMono)
    {
		scrMono = newScrMono.SetScriptable(this);
		transform = scrMono.transform;
	}

	public virtual void UpdateScriptableLoop()
    {
		//Ticks++;
		//AdjustedTicks++;
		//AuraTicks++;

		//if(UnselectableTimer)
		//{
		//	UnselectableTimer--;
		//	if(!UnselectableTimer && Type == ST_ACTOR)
		//	{
		//		ActorInput* actor = (ActorInput*)this;
		//		actor->SetCircleSize();
		//		if(actor->InParty)
		//		{
		//			core->GetGame()->SelectActor(actor, true, SELECT_QUIET);
		//			core->SetEventFlag(EF_PORTRAIT);
		//		}
		//	}
		//}
		
		UpdateScriptTicks();
		//InterruptCasting = false;
    }

	//public void AddScript(AICombatScript script)
 //   {
 //       if(_scripts.Contains(script))
 //       {
	//		Debug.LogError("Script already in list");
 //       }

	//	_scripts.Add(script);
 //   }

	public void Wait(float waitTime)
    {
		waitTimer = waitTime;
    }

	public virtual void UpdateScriptTicks()
	{
		//int scriptDepth = 1;

  //      if(GameStateManager.Instance.GetCurrentGame().controlStatus.partyAIEnabled == 0)
  //      {
		//	scriptDepth = 0;
		//}

		//ExecuteScript(scriptDepth);
	}

	internal Map GetCurrentArea()
	{
		return _area;
	}

	internal virtual void SetMap(Map map)
    {
		_area = map;
	}

	//bool InMove()
	//{
	//	if (this.GetType() != typeof(ActorInput)) {
	//		return false;
	//	}

	//	ActorInput me = (ActorInput)this;
	//	return me.navAgent.hasPath;
	//}

	public void SetWait(float waitDuration)
    {
		waitTimer = waitDuration;
    }

	public float GetWait()
	{
		return waitTimer;
	}

	#region Utility Functions
	public int GetGlobalID() { return _globalID; }
	#endregion Utility Functions
}
