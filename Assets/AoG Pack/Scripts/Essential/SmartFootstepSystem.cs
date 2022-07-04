using AoG.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public enum FootstepSoundCondition
{
    Default,
    IsBareFoot,
    WearsHeavyArmor
}

public enum FootstepSoundType
{
    Rock,
    Grass,
    Dirt,
    Sand,
    Water
}

public class SmartFootstepSystem : MonoBehaviour
{
    public bool debug;
    [Tooltip("Footstep sound types depend rather on the material of equipped boots than on expensive raycasting ground texture checks.")]
    public bool simple = true;
    private AudioClip[] defaultLeftRightFootsteps;

    public float stepCooldown = 0.1f;
    private float _stepTimer;

    public FootstepSoundCondition soundCondition;

    private static List<Footstep> footStepCollection;


    public string groundTextureName;

    public AudioSource audioSource_Footsteps;
    public AudioSource audioSource_Foley;
    private AudioClip currentFootstepAudioClip;
    private AudioClip currentFoleyAudioClip;

    public float rayOriginHeight = 1f;
    public float groundCheckDistance = 2f;
    private Terrain _terrainUnderFoot;
    private TerrainData terrainData;
    private TerrainLayer[] splatPrototypes;
    private RaycastHit hit;
    [HideInInspector] public Texture2D currentTexture;
    [HideInInspector] public bool onTerrain;
    private bool usingTerrain;

    private Animator _animator;
    private float currentFrameFootstepLeft;
    private float lastFrameFootstepLeft;
    private float currentFrameFootstepRight;
    private float lastFrameFootstepRight;

    public bool enableFX;
    public GameObject particles;
    private GameObject _prtclObj;
    public Transform lFootPrtclSpot;
    public Transform rFootPrtclSpot;
    private Vector3 currFXPos;
    private ActorInput _actorMonoObject;
    private MapInfo mapInfo;

    private void Start()
    {
        mapInfo = GameObject.FindWithTag("MapInfo").GetComponent<MapInfo>();

        if(simple)
        {
            defaultLeftRightFootsteps = Resources.LoadAll<AudioClip>("SFX/Actor SFX/Footsteps/Simple");
        }

        _stepTimer = stepCooldown;
        _animator = transform.GetComponent<Animator>();
        _actorMonoObject = GetComponent<ActorInput>();

        if(_animator.isHuman)
        {
            lFootPrtclSpot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            rFootPrtclSpot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }

        if(particles != null)
        {
            _prtclObj = Instantiate(particles, transform.position, transform.rotation);

            if(_animator != null)
            {
            }
        }

        if(particles == null || lFootPrtclSpot == null || rFootPrtclSpot == null)
        {
            enableFX = false;
        }

        if(audioSource_Footsteps == null)
            audioSource_Footsteps = GetComponent<AudioSource>();

        GetTerrainInfo();

        if(footStepCollection == null)
        {
            footStepCollection = new List<Footstep>();

            //string[] stoneTags = { "stone", "rock", "concret", "road", "street", "marmor" };
            //string[] grassTags = { "grass" };
            //string[] dirtTags = { "dirt" };
            //string[] woodTags = { "wood" };
            //string[] waterTags = { "water" };
            //if(usingTerrain)
            //{
            //    foreach(TerrainLayer splatPrototype in _terrainUnderFoot.terrainData.terrainLayers)
            //    {
            //        Footstep footstepCol = new Footstep(ResourceManager.FootstepSoundsStoneFemale, true);
            //        footstepCol.triggerTextures.Add(splatPrototype.diffuseTexture);

            //        if(footstepCol == null)
            //            Debug.LogError("No matches found for texture '" + splatPrototype.diffuseTexture.name + "'");
            //        else
            //            footStepCollection.Add(footstepCol);
            //    }
            //}
            //if(usingTerrain)
            //{
            //    foreach(TerrainLayer splatPrototype in _terrainUnderFoot.terrainData.terrainLayers)
            //    {
            //        if(splatPrototype.diffuseTexture == null)
            //        {
            //            continue;
            //        }

            //        Footstep footstepCol = null;
            //        foreach(string tag in stoneTags)
            //        {
            //            if(splatPrototype.diffuseTexture.name.ToLower().Contains(tag))
            //            {
            //                footstepCol = new Footstep(ResourceManager.FootstepSoundsStoneFemale, true);
            //                footstepCol.triggerTextures.Add(splatPrototype.diffuseTexture);
            //                goto LoopEnd;
            //            }
            //        }
            //        foreach(string tag in grassTags)
            //        {
            //            if(splatPrototype.diffuseTexture.name.ToLower().Contains(tag))
            //            {
            //                footstepCol = new Footstep(ResourceManager.FootstepSoundsGrass, true);
            //                footstepCol.triggerTextures.Add(splatPrototype.diffuseTexture);
            //                goto LoopEnd;
            //            }
            //        }
            //        foreach(string tag in dirtTags)
            //        {
            //            if(splatPrototype.diffuseTexture.name.ToLower().Contains(tag))
            //            {
            //                footstepCol = new Footstep(ResourceManager.FootstepSoundsDirt, true);
            //                footstepCol.triggerTextures.Add(splatPrototype.diffuseTexture);
            //                goto LoopEnd;
            //            }
            //        }
            //        foreach(string tag in woodTags)
            //        {
            //            if(splatPrototype.diffuseTexture.name.ToLower().Contains(tag))
            //            {
            //                footstepCol = new Footstep(ResourceManager.FootstepSoundsWood, true);
            //                footstepCol.triggerTextures.Add(splatPrototype.diffuseTexture);
            //                goto LoopEnd;
            //            }
            //        }
            //        foreach(string tag in waterTags)
            //        {
            //            if(splatPrototype.diffuseTexture.name.ToLower().Contains(tag))
            //            {
            //                footstepCol = new Footstep(ResourceManager.FootstepSoundsWater, true);
            //                footstepCol.triggerTextures.Add(splatPrototype.diffuseTexture);
            //                goto LoopEnd;
            //            }
            //        }

            //    LoopEnd:
            //        if(footstepCol == null)
            //            Debug.LogError("No matches found for texture '" + splatPrototype.diffuseTexture.name + "'");
            //        else
            //            footStepCollection.Add(footstepCol);
            //    }
            //}
            //else
            //{
            FootstepLink[] links = mapInfo.footstepLinks;

            foreach(FootstepLink link in links)
            {
                Footstep footstepCol = null;
                switch(link.surface)
                {
                    case FootstepSoundType.Rock:
                        footstepCol = new Footstep(ResourceManager.FootstepSoundsStone, true);
                        break;
                    case FootstepSoundType.Grass:
                        footstepCol = new Footstep(ResourceManager.FootstepSoundsGrass, false);
                        break;
                    case FootstepSoundType.Water:
                        footstepCol = new Footstep(ResourceManager.FootstepSoundsWater, false);
                        break;
                    case FootstepSoundType.Dirt:
                        footstepCol = new Footstep(ResourceManager.FootstepSoundsDirt, false);
                        break;
                    case FootstepSoundType.Sand:
                        footstepCol = new Footstep(ResourceManager.FootstepSoundsCarpet, false);
                        break;
                }

                footstepCol.triggerTextures.Add(link.texture);
                footStepCollection.Add(footstepCol);
            }
            //}

        }
    }

    private void GetTerrainInfo()
    {
        if(Terrain.activeTerrain == null)
        {
            //Debug.Log("<color=red>Active Terrain = null</color>");
            usingTerrain = false;
        }
        else
        {
            _terrainUnderFoot = Terrain.activeTerrain;
            terrainData = _terrainUnderFoot.terrainData;
            splatPrototypes = terrainData.terrainLayers;
            usingTerrain = true;
        }
    }

    private Texture2D GetGroundTexture()
    {
        Ray ray = new Ray(transform.position + (Vector3.up * rayOriginHeight), Vector3.down);

        //if(debug)
        //    Debug.DrawRay(transform.position + (Vector3.up * rayOriginHeight), Vector3.down, Color.red);

        //check if the character is currently on a terrain and toggle the "onTerrain" bool
        if(Physics.Raycast(ray, out hit, groundCheckDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            if(hit.collider.GetComponent<Terrain>())
            {
                _terrainUnderFoot = hit.collider.GetComponent<Terrain>();
                //if(debug)
                //    Debug.Log("<color=cyan>Footsteps: on terrain</color>");
                onTerrain = true;
            }
            else
            {
                //if(debug)
                //    Debug.Log("<color=yellow>Footsteps: on mesh</color>");
                onTerrain = false;
            }

            //if(debug)
            //    Debug.Log("*Footstep raycast hit '<color=cyan>" + hit.collider.name + "'</color>");
        }
        else
        {
            //if(debug)
            //    Debug.Log("<color=red>Footstep raycast hit nothing</color>");
        }

        //Get the current texture the character is standing on
        if(usingTerrain && onTerrain)
        {
            return splatPrototypes[GetMainTexture(transform.position)].diffuseTexture;
            //Debug.Log("Current terrain texture: " + currentTexture.name);
        }
        else
        {
            return GetRendererTexture();
        }
    }

    //void Update()
    //{
        
    //    currentFrameFootstepLeft = _animator.GetFloat("FootstepLeft");       //get left foot's CURVE FLOAT from the Animator Controller, from the LAST FRAME.
    //    if(currentFrameFootstepLeft > 0 && lastFrameFootstepLeft < 0)
    //    {       //is this frame's curve BIGGER than the last frames?
    //        //RaycastHit surfaceHitLeft;
    //        //Ray aboveLeftFoot = new Ray(leftFoot.transform.position + new Vector3(0, 1.5f, 0), Vector3.down);
    //        //LayerMask layerMask = ~(1 << 18) | (1 << 19);   //Here we ignore layer 18 and 19 (Player and NPCs). We want the raycast to hit the ground, not people.
    //        //if(Physics.Raycast(aboveLeftFoot, out surfaceHitLeft, 2f, layerMask))
    //        //{
    //        //    floor = (surfaceHitLeft.transform.gameObject);
    //        //    currentFoot = "Left";               //This will help us place the Instantiated or Toggled FX at the correct position.
    //        //    if(floor != null)
    //        //    {
    //        //        Invoke("CheckTexture", 0);      //Play LEFT FOOTSTEP
    //        //    }
    //        //}
    //        PlayFootstepSound();
    //    }
    //    lastFrameFootstepLeft = _animator.GetFloat("FootstepLeft");  //get left foot's CURVE FLOAT from the Animator Controller, from the CURRENT FRAME.

    //    //-----------------------------------------------------------------------------------------

    //    //Check THIS FRAME to see if we need to play a sound for the right foot, RIGHT NOW...
    //    currentFrameFootstepRight = _animator.GetFloat("FootstepRight"); //get right foot's CURVE FLOAT from the Animator Controller, from the LAST FRAME.
    //    if(currentFrameFootstepRight < 0 && lastFrameFootstepRight > 0)
    //    {       //is this frame's curve SMALLER than last frames?
    //        //RaycastHit surfaceHitRight;
    //        //Ray aboveRightFoot = new Ray(rFootPrtclSpot.position + new Vector3(0, 1.5f, 0), Vector3.down);
    //        //LayerMask layerMask = ~(1 << 18) | (1 << 19);   //Here we ignore layer 18 and 19 (Player and NPCs). We want the raycast to hit the ground, not people.
    //        //if(Physics.Raycast(aboveRightFoot, out surfaceHitRight, 2f, layerMask))
    //        //{
    //        //    floor = (surfaceHitRight.transform.gameObject);
    //        //    currentFoot = "Right";              //This will help us place the Instantiated or Toggled FX at the correct position.
    //        //    if(floor != null)
    //        //    {
    //        //        Invoke("CheckTexture", 0);      //Play RIGHT FOOTSTEP
    //        //    }
    //        //}

    //        PlayFootstepSound();
    //    }
    //    lastFrameFootstepRight = _animator.GetFloat("FootstepRight");
    //}

    private bool _left;
    public void PlayFootstepSound(AnimationEvent animationEvent)
    {
        Profiler.BeginSample("SMARTFOOTSTEP: ClipInfoCheck");
        if(animationEvent.animatorClipInfo.weight < 0.5)
        {
            return;
        }
        Profiler.EndSample();

        if(_actorMonoObject.inWater)
        {
            SFXPlayer.TriggerSFX(ResourceManager.FootstepSoundsWater[Random.Range(0, ResourceManager.FootstepSoundsWater.Length)], transform.position);
            return;
        }

        if(simple)
        {
            SFXPlayer.TriggerSFX(_left ? defaultLeftRightFootsteps[0] : defaultLeftRightFootsteps[1], transform.position);
            _left = !_left;
            return;
        }

        if(debug)
            Debug.Log("Footstep");

        currentTexture = GetGroundTexture();
        //Debug.Assert(currentTexture != null);
        if(currentTexture != null)
        {
            if(debug)
                Debug.Log("Found texture ['" + currentTexture.name + "']");

            foreach(Footstep footStep in footStepCollection)
            {
                if(debug)
                    Debug.Log("Found corresponding footstep data");

                foreach(Texture2D triggerTexture in footStep.triggerTextures)
                {
                    //Debug.Log("* Footstep - next iteration: " + triggerName + "| currentTexture name: " + currentTexture.name.ToLower());
                    //Debug.Log("* foreach(string s in footStep.TriggerTextureNames)");

                    if(currentTexture.name == triggerTexture.name)
                    {
                        //if(debug)
                        //    Debug.Log("Found footstep sound for '<color=white>" + triggerTexture.name + "</color>'");

                        if(soundCondition == FootstepSoundCondition.IsBareFoot)
                        {
                            currentFootstepAudioClip = ResourceManager.FootstepSoundsBarefoot[Random.Range(0, ResourceManager.FootstepSoundsBarefoot.Length)];
                        }
                        else
                        {
                            currentFootstepAudioClip = footStep.audioClips[Random.Range(0, footStep.audioClips.Length)];
                        }

                        if(soundCondition == FootstepSoundCondition.WearsHeavyArmor)
                        {
                            currentFoleyAudioClip = ResourceManager.FootstepSoundsMetalRustling[Random.Range(0, ResourceManager.FootstepSoundsMetalRustling.Length)];
                            PlayFoleyClip(currentFoleyAudioClip);
                        }

                        new Sound(transform, 1);
                        groundTextureName = currentFootstepAudioClip == null ? "" : currentTexture.name;
                        PlayFootstepClip(currentFootstepAudioClip);
                        break;
                    }
                    else
                    {
                        currentFootstepAudioClip = null;
                    }
                }
            }
        }

        //FX
        if(enableFX)
        {
            ParticleSystem fx = _prtclObj.GetComponent<ParticleSystem>();

            //_prtclObj.transform.position = footIndex == 1 ? rFootPrtclSpot.position : lFootPrtclSpot.position; ;

            fx.Emit(3);
        }
    }

    private void PlayFootstepClip(AudioClip clip)
    {
        if(clip == null)
            return;

        audioSource_Footsteps.clip = clip;
        audioSource_Footsteps.pitch = Random.Range(0.9f, 1.1f);
        audioSource_Footsteps.volume = Random.Range(0.95f, 1.05f);
        audioSource_Footsteps.Play();
    }

    private void PlayFoleyClip(AudioClip clip)
    {
        if(clip == null)
            return;

        audioSource_Foley.clip = clip;
        audioSource_Foley.pitch = Random.Range(0.9f, 1.1f);
        audioSource_Foley.volume = Random.Range(0.47f, 1.53f);
        audioSource_Foley.Play();
    }

    /*returns an array containing the relative mix of textures
       on the main terrain at this world position.*/

    public float[] GetTextureMix(Vector3 worldPos)
    {
        //_terrainUnderFoot = Terrain.activeTerrain;
        terrainData = _terrainUnderFoot.terrainData;
        Vector3 terrainPos = _terrainUnderFoot.transform.position;

        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for(int n = 0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }

        return cellMix;
    }

    /*returns the zero-based index of the most dominant texture
       on the main terrain at this world position.*/

    public int GetMainTexture(Vector3 worldPos)
    {
        float[] mix = GetTextureMix(worldPos);
        float maxMix = 0;
        int maxIndex = 0;

        for(int n = 0; n < mix.Length; ++n)
        {
            if(mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }

        return maxIndex;
    }

    //returns the mainTexture of a renderer's material at this position
    public Texture2D GetRendererTexture()
    {
        Texture2D texture = null;

        if(Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hit, groundCheckDistance))
        {
            if(hit.collider.gameObject.GetComponent<Renderer>())
            {
                MeshFilter meshFilter = (MeshFilter)hit.collider.GetComponent(typeof(MeshFilter));
                Mesh mesh = meshFilter.mesh;
                int totalSubMeshes = mesh.subMeshCount;
                int[] subMeshes = new int[totalSubMeshes];
                for(int i = 0; i < totalSubMeshes; i++)
                {
                    subMeshes[i] = mesh.GetTriangles(i).Length / 3;
                }

                int hitSubMesh = 0;
                int maxVal = 0;

                for(int i = 0; i < totalSubMeshes; i++)
                {
                    maxVal += subMeshes[i];

                    if(hit.triangleIndex <= maxVal - 1)
                    {
                        hitSubMesh = i + 1;
                        break;
                    }
                }
                texture = (Texture2D)hit.collider.gameObject.GetComponent<Renderer>().materials[hitSubMesh - 1].mainTexture;
            }
        }
        return texture;
    }

    private void OnDisable()
    {
        //if(prot_Sound != null)
        //    prot_Sound.OnRequestFootstepSound -= TryPlayFootstep;
    }
}

public class Footstep
{
    public bool barefootOverrides;
    public List<Texture2D> triggerTextures;
    public AudioClip[] audioClips;

    public Footstep(AudioClip[] audioClips, bool barefootOverrides)
    {
        this.triggerTextures = new List<Texture2D>();
        this.barefootOverrides = barefootOverrides;
        this.audioClips = audioClips;
    }
}

public class Sound
{
    public Vector3 originalPosition;
    public Transform source;
    public float volume = 1;
    public float range;

    public Sound(Transform source, float volume, float range = 20)
    {
        Collider[] listeningAgents = Physics.OverlapSphere(source.position, range, (1 << LayerMask.NameToLayer("Actors"))).Where(a => a.transform != source).ToArray();
        //DebugExtension.DebugWireSphere(transform.position, audioSource.maxDistance, 0.2f);

        originalPosition = source.position;

        foreach(Collider col in listeningAgents)
        {
            if(Physics.Linecast(originalPosition, col.transform.position, ~(1 << LayerMask.NameToLayer("Obstacles"))) == false)
            {
                if(Vector3.Distance(originalPosition, col.transform.position) > (range * 0.75f))
                {
                    continue;
                }
            }
            col.SendMessage("Callback_HeardSound", this, SendMessageOptions.DontRequireReceiver);
        }
        this.source = source;
        this.volume = volume;
        this.range = range;
    }
}