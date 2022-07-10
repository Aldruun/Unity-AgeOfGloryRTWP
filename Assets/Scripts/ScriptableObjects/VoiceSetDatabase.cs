using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "VoiceSetDatabase", menuName = "ScriptableObjects/VoiceSetDatabase")]
public class VoiceSetDatabase : ScriptableObject
{
    //public List<VoiceSetDrawer> voiceSets = new List<VoiceSetDrawer>();

    //internal CharacterVoiceSet GetRandomVoiceSet(Gender gender)
    //{
    //    //CharacterVoiceSet> voiceSets = new ;
    //    //if(gender == Gender.Female)
    //    //{
    //    //    oiceSets = 
    //    //}
    //}

    //public List<CharacterVoiceSetData> femaleVoiceSets = new List<CharacterVoiceSetData>();
    public List<ActorVoiceSetData> characterVoiceSets = new List<ActorVoiceSetData>();
    public List<ActorVoiceSetData> creatureVoiceSets = new List<ActorVoiceSetData>();

    internal string GetVoiceSetID(VoiceSet voiceSet)
    {
        for(int i = 0; i < creatureVoiceSets.Count; i++)
        {
            if(creatureVoiceSets[i].voiceSet.Name == voiceSet.ToString())
            {
                return voiceSet.ToString();
            }
        }

        return "GetVoiceSetFailedException";
    }

    //internal int GetRandomVoiceSetIndex(ActorRace race, Gender gender)
    //{
    //    int rndIndex;


        //if(gender == Gender.Female)
        //{
        //    //List<CharacterVoiceSetData> availableVoiceSets = new List<CharacterVoiceSetData>();
        //    rndIndex = UnityEngine.Random.Range(0, femaleVoiceSets.Count);
        //}
        //else if(gender == Gender.Male)
        //{
        //    rndIndex = UnityEngine.Random.Range(0, maleVoiceSets.Count);
        //}
        //else
        //{
        //    rndIndex = UnityEngine.Random.Range(0, voiceSets.Count);
        //}

    //    return rndIndex;
    //}

    public void AddCharacterVoiceDataSet(ActorVoiceSetData voiceSetData)
    {
        characterVoiceSets.Add(voiceSetData);
    }

    public void AddCreatureVoiceDataSet(ActorVoiceSetData voiceSetData)
    {
        creatureVoiceSets.Add(voiceSetData);
    }

    public CharacterVoiceSet GetVoiceSetByID(string ID)
    {
        for(int i = 0; i < characterVoiceSets.Count; i++)
        {
            if(characterVoiceSets[i].voiceSet.Name == ID)
            {
                return characterVoiceSets[i].voiceSet;
            }
        }

        for(int i = 0; i < creatureVoiceSets.Count; i++)
        {
            if(creatureVoiceSets[i].voiceSet.Name == ID)
            {
                return creatureVoiceSets[i].voiceSet;
            }
        }

        //Debug.LogError("Voiceset with ID '" + ID + "' not found");
        return null;
    }

    public CharacterVoiceSet AccessVoiceSetByIndex(int index, bool isCreature)
    {
        if(isCreature)
        {
            return creatureVoiceSets[index].voiceSet;
        }
        else
        {
            return characterVoiceSets[index].voiceSet;
        }
    }

    public List<ActorVoiceSetData> AccessVoiceSetCategory(int index)
    {
        switch(index)
        {
            case 0:
                return characterVoiceSets;

            case 1:
                return creatureVoiceSets;

        }

        return null;
    }
}

[Serializable]
public class CharacterVoiceSet
{
    public AudioClip[] areaDangerous;
    public AudioClip[] areaDiscovered;
    public AudioClip[] attack;
    public AudioClip[] attackLong;
    public AudioClip[] battleStart;
    public AudioClip[] battleWon;
    public AudioClip[] cannotCarry;
    public AudioClip[] cannotOpen;
    public AudioClip[] cannotUse;
    public AudioClip[] chargeUp;
    public AudioClip[] criticalHit;
    public AudioClip[] death;
    public AudioClip[] distraught;
    public AudioClip[] dungeonCleared;
    public AudioClip[] foundLoot;
    public AudioClip[] hurt;
    public AudioClip[] idle;
    public AudioClip[] joinParty;
    public AudioClip[] jump;
    public AudioClip[] levelGained;
    public AudioClip[] moveOrderNo;
    public AudioClip[] moveOrderYes;
    public AudioClip[] selected;
    public AudioClip[] taunt;
    public AudioClip[] useItem;

    public string ID;
    public string Name;

    //public AudioClip[] randomChatter;

    public CharacterVoiceSet(string name)
    {
        Name = name;
    }

    public CharacterVoiceSet()
    {
    }
}

//[Serializable]
//public class VoiceSetDrawer
//{
//    public Gender gender;

//    [HideInInspector] public string name;

//    public CharacterVoiceSet voiceSet;

//    public VoiceSetDrawer(CharacterVoiceSet voiceSet, Gender gender, string name)
//    {
//        this.voiceSet = voiceSet;
//        this.gender = gender;
//        this.name = name;
//    }
//}
[Serializable]
public class ActorVoiceSetData
{
    public Gender Gender;
    public ActorRace Race;
    public string folder;
    public CharacterVoiceSet voiceSet;

    public ActorVoiceSetData(CharacterVoiceSet voiceSet, /*Gender gender, */string folder)
    {
        this.voiceSet = voiceSet;
        //this.Gender = gender;
        this.folder = folder;
    }
    public ActorVoiceSetData()
    {
    }
}