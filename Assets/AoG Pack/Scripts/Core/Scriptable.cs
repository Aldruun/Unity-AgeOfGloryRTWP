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

public abstract class Scriptable : MonoBehaviour
{
	public static int globalScriptableCounter = 10000;
	private int _globalID;

	public bool debug;
	public bool debugActions;
	public bool debugRoundSystem;

	public ScriptableType type;

	protected int lastDamage;
	private int CurrentActionTarget = 0;
	private bool CurrentActionInterruptable = true;
	private int CurrentActionTicks = 0;
	private int CurrentActionState = 0;
	protected InternalFlags internalFlags;
	protected float waitTimer;

	internal LinkedList<GameAction> actionQueue;
	public GameAction CurrentAction;
	public GameAction OverrideAction;

	internal GameScript[] scripts;

	private Map _area;

	/// <summary>
	/// Initializes scripts array and actionQueue and sets GlobalID
	/// </summary>
	public virtual void InitScriptable(ScriptableType type)
    {
		this.type = type;
		_globalID = ++globalScriptableCounter;

		scripts = new GameScript[8];
		actionQueue = new LinkedList<GameAction>();
	}

	public virtual void UpdateScriptTicks()
	{
		//int scriptDepth = 1;

		//      if(Interface.GetCurrentGame().controlStatus.partyAIEnabled == 0)
		//      {
		//	scriptDepth = 0;
		//}

		//ExecuteScript(scriptDepth);
	}

	public void ExecuteScript(int scriptCount)
	{

		for(int i = 0; i < scriptCount; i++)
		{
			if(scripts[i] != null)
			{
				GameScript script = scripts[i];

				script.OnUpdate();

				if(script.dead)
					script = null;

			}
		}
	}

	public void SetScript(GameScript script, int index)
	{
		if(index >= 8)
		{
			Debug.LogError("Scriptable: Invalid script index!");
		}

		if(scripts[index] != null && scripts[index].running)
		{
			scripts[index].dead = true;
		}

		scripts[index] = script;
	}

	public void ReleaseCurrentAction()
	{
		if(debugActions)
			Debug.Log(name + ": <color=cyan>Attempting to release current action</color>");
		if(CurrentAction != null)
		{
			//CurrentAction.OnDone?.Invoke();

			if(debugActions)
				Debug.Log(name + ": <color=cyan>Releasing action '" + CurrentAction.ToString() + "'</color>");
			CurrentAction.Release();
			CurrentAction = null;
		}

		CurrentActionTarget = 0;
		CurrentActionInterruptable = true;
		CurrentActionTicks = 0;
		CurrentActionState = 0;
	}

	/// <summary>
	/// Simply add this action to the end of the queue.
	/// </summary>
	/// <param name="newAction"></param>
	/// <param name="instant"></param>
	public void AddAction(GameAction newAction, bool instant)
	{
		if(newAction == null)
		{
			Debug.LogError("Scriptable: NULL action encountered!");
			return;
		}

		if(instant /*&& CurrentAction == null && GetNextAction() == null*/)
		{
			if(debugActions)
				Debug.Log("<color=cyan>Instant action '" + newAction.ToString() + "' started</color>");
			ReleaseCurrentAction();
			//CurrentAction = newAction;
			//ExecuteAction(this, CurrentAction); //Why? Current action will be performed instantly either way

		}

		if(debugActions)
			Debug.Log("<color=cyan>Enqueueing action '" + newAction.ToString() + "'</color>");

		actionQueue.AddLast(newAction);
	}

	/// <summary>
	/// Set the next action to be processed after the CurrentAction has finnished.
	/// </summary>
	/// <param name="newAction"></param>
	public void AddActionInFront(GameAction newAction)
	{
		if(newAction == null)
		{
			Debug.LogError("Scriptable: NULL action encountered!");
			return;
		}

		if(debugActions)
			Debug.Log("<color=cyan>Adding action " + newAction.ToString() + " in front</color>");

		actionQueue.AddFirst(newAction);
	}

	public GameAction GetCurrentAction()
	{
		return CurrentAction;
	}

	public GameAction GetNextAction()
	{
		if(actionQueue.Count == 0)
		{
			return null;
		}

		return actionQueue.Last.Value;
	}

	protected GameAction PopNextAction()
	{
		if(actionQueue == null)
		{
			if(debugActions)
				Debug.Log(name + ": <color=cyan>Can't pop -> action queue null</color>");
			return null;
		}

		if(actionQueue.Count == 0)
		{
			if(debugActions)
				Debug.Log(name + ": <color=cyan>Can't pop -> action queue empty</color>");
			return null;
		}

		GameAction last = actionQueue.Last.Value;
		actionQueue.RemoveLast();
		return last;
	}

	internal virtual void Stop()
	{
		ClearActions();
	}

	protected void ClearActions()
	{
		if(debugActions)
			Debug.Log(name + ": <color=cyan>Clearing action queue</color>");
		ReleaseCurrentAction();
		foreach(var a in actionQueue.ToArray())
		{
			if(debugActions)
				Debug.Log(name + ": <color=cyan>Clearing action queue: Releasing action '" + a.ToString() + "'</color>");
			actionQueue.Remove(a);
			a.Release();
		}

		waitTimer = 0;
	}

	protected virtual void ProcessActions()
	{
		if(waitTimer > 0)
		{
			waitTimer -= Time.deltaTime;
			if(waitTimer > 0)
				return;
		}

		if(CurrentAction == null)
		{
			CurrentAction = PopNextAction();
			return;
		}

		if(debugActions)
			Debug.Log(name + ": <color=orange>PA: Exec CurrAction '" + CurrentAction.ToString() + "'</color>");

		ExecuteAction(this, CurrentAction);

		////! Displaces movable scriptable to the next path node.
		////! Returns true if one exists
		//if(InMove())
		//{
		//	return;
		//}
	}

	public void ExecuteAction(Scriptable sender, GameAction action)
	{
		if(sender is Actor a)
		{
			if(action != null)
			{
				if(action.Done(a))
				{
					ReleaseCurrentAction();
				}
			}
		}
	}

	public void Wait(float waitTime)
    {
		waitTimer = waitTime;
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
