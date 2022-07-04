using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

/**
 * AnimationEventInfo: a Unity tool that allows the user to search their scene for details on functions called by Animations
 * author: Steve Hart - steven.hart282@gmail.com
 * */

public class AnimationEventFinder : ScriptableWizard
{
	//optional function name to search for
	[Tooltip("The name of the function you are searching for.  Leave this blank if you wish to list all Animation Events")]
	public string functionToSearchFor;

	[MenuItem("Tools/Animation Event Finder")]
	private static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("Animation Event Finder", typeof(AnimationEventFinder), "Search");
	}

	private void OnWizardUpdate()
	{
		helpString = 
			"\nSearch your Heirarchy (or a subselection) for Animation Events" +
			"\n\nSearches all GameObjects in Heirarchy by default.  Select GameObjects in the heirarchy to narrow the search to those GameObjects and their children (recursively)." +
			"\n\n\nEnter a function name in 'Function To Search For', or leave it blank if you would like to list details of all Animation Events\n" +
			"\n\nauthor: Steve Hart";
	}

	//Runs when the 'Search' button is pressed
	private void OnWizardCreate()
	{
		if (String.IsNullOrEmpty (functionToSearchFor))
		{
			Debug.Log ("Searching Animation Events...");
		}
		else
		{
			Debug.Log ("Searching for Animation Event: " + functionToSearchFor + "...");
		}
		List<Animator> animatorList = GetAnimators();
		List<Hashtable> data = this.GetDetailsFromAnimatorList (animatorList);
		this.PrintAnimEventList (data);
		Debug.Log("Animation Info Search Complete.  Searched " + animatorList.Count + " Animators, Found: " + data.Count + " Animation Events matching search criteria");
	}

	//retrieves a list of all animators the user wants to search through, depending on if they have selected any objects in the Heirarchy
	private List<Animator> GetAnimators()
	{
		//user didn't select any objects in the Heirarchy
		if (Selection.gameObjects.Length == 0)
		{
			Debug.Log ("No objects selected in Heirarchy, searching all GameObjects");
			return this.GetAllAnimatorsInHeirarchy ();
		}
		//user selected objects to search in
		else
		{
			Debug.Log (Selection.gameObjects.Length + " Objects selected to for search");
			return this.GetAllAnimatorsInSelection();
		}
	}

	//returns a list of all the Animators in the current Heirarchy
	private List<Animator> GetAllAnimatorsInHeirarchy()
	{
		List<Animator> animatorList = new List<Animator> ();
		foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
		{
			Animator anim = obj.GetComponent<Animator> ();
			if (anim != null)
			{
				animatorList.Add (anim);
			}
		}
		return animatorList;
	}

	//returns a list of all the Animators on the selected GameObjects and their decendents
	private List<Animator> GetAllAnimatorsInSelection()
	{
		List<Animator> animatorList = new List<Animator> ();
		foreach (GameObject obj in Selection.gameObjects)
		{
			Component[] selectedAnimatorList = obj.GetComponentsInChildren<Animator>();
			foreach (Animator anim in selectedAnimatorList)
			{
				animatorList.Add (anim);
			}
		}
		return animatorList;
	}

	//cycles through all animators in a list and requests details from each of its clips
	private List<Hashtable> GetDetailsFromAnimatorList (List<Animator> animatorList)
	{
		List<Hashtable> detailsList = new List<Hashtable> ();
		foreach (Animator animator in animatorList)
		{
			if (animator.runtimeAnimatorController == null)
			{
				Debug.LogWarning ("GameObject " + animator.gameObject.name + " has an Animator with no Controller assigned to the component.  We cannot search clips inside this Animator until one is assigned!");
				continue;
			}
			AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
			if (clips.Length > 0)
			{
				List<Hashtable> detailsFromAnimators = this.GetDetailsFromClips (clips);
				this.AddItemToAllHashtablesInList (detailsFromAnimators, "animatorName", animator.name);
				this.AddItemToAllHashtablesInList (detailsFromAnimators, "gameObjectName", animator.gameObject.name);
				detailsList.AddRange (detailsFromAnimators);
			}
		}
		return detailsList;
	}

	//cycles through all clips in a list and requests details from each of its events
	private List<Hashtable> GetDetailsFromClips (AnimationClip[] clips)
	{
		List<Hashtable> detailList = new List<Hashtable> ();
		foreach (AnimationClip clip in clips)
		{
			AnimationEvent[] events = clip.events;
			if (events.Length > 0)
			{
				List<Hashtable> detailsFromEvents = this.GetDetailsFromEvents (events);
				this.AddItemToAllHashtablesInList (detailsFromEvents, "clip", clip.name);
				detailList.AddRange (detailsFromEvents);
			}
		}
		return detailList;
	}

	//cycles through all events in a list and returns details 
	private List<Hashtable> GetDetailsFromEvents (AnimationEvent[] events)
	{
		List<Hashtable> detailsFromEvents = new List<Hashtable> ();
		foreach (AnimationEvent animEvent in events)
		{
			if (String.IsNullOrEmpty(functionToSearchFor) || functionToSearchFor.Equals(animEvent.functionName, StringComparison.InvariantCultureIgnoreCase))
			{
				Hashtable details = new Hashtable ();
				details.Add ("time", animEvent.time);
				details.Add ("functionName", animEvent.functionName);
				detailsFromEvents.Add (details);
			}
		}
		return detailsFromEvents;
	}


	private void PrintAnimEventList (List<Hashtable> dataList)
	{
		foreach (Hashtable data in dataList)
		{
			this.PrintAnimEventHashtable (data);
		}
	}

	private void PrintAnimEventHashtable (Hashtable data)
	{
		Debug.Log ("\"" + data["functionName"] + "\" is played by GameObject: " + data ["gameObjectName"] + " > Animator: " + data ["animatorName"] + " > Clip: " + data ["clip"] + " @ " + data ["time"] + " seconds");

	}
		
	private void AddItemToAllHashtablesInList (List<Hashtable> hashtableList, string key, string value)
	{
		foreach (Hashtable hash in hashtableList)
		{
			hash.Add (key, value);
		}
	}

	/*private string FormatParameters(AnimationEvent animEvent)
	{
		string parameters = "";
		if (animEvent.stringParameter != "")
		{
			parameters += animEvent.stringParameter;
		}
		if (animEvent.intParameter != 0)
		{
			parameters += animEvent.intParameter + "";
		}
		if (animEvent.floatParameter != 0f)
		{
			parameters += animEvent.floatParameter + "";
		}
		if (animEvent.objectReferenceParameter != null)
		{
			parameters += animEvent.objectReferenceParameter.ToString();
		}
		if (parameters != "")
		{
			return parameters;
		}
		else
		{
			return null;
		}

	}*/
}
