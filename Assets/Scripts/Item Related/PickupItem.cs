using System.Collections;
using UnityEngine;

public class PickupItem
{
    public int gold;
    public string identifier;
    public Actor occupee { get; private set; }
    public Vector3 spawnPosition { get; private set; }
    private readonly GameObject _obj;

    public PickupItem(Vector3 spawnPosition, string identifier, GameObject go, int gold)
    {
        this.gold = gold;
        this.identifier = identifier;
        this.spawnPosition = spawnPosition;
        _obj = go;
        //_obj = PoolSystem.GetPoolObject(identifier, ObjectPoolingCategory.MISC);
        //_obj.transform.position = spawnPosition;
        //_obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        GameEventSystem.OnPickUpItemSpawned?.Invoke(this);
    }

    public void Collect(ActorStats agent)
    {
        //agent.Execute_AddGold(gold);

        GameEventSystem.OnPickUpItemCollected?.Invoke(this);

        _obj.SetActive(false);
    }

    public bool Reserve(Actor agent)
    {
        if(occupee != null && occupee.dead == false)
        {
            return false;
        }

        occupee = agent;
        _ = agent.StartCoroutine(CR_Unreserve());
        return true;
    }

    private IEnumerator CR_Unreserve()
    {

        float dur = 10;

        yield return new WaitForSeconds(dur);

        occupee = null;
    }
}
