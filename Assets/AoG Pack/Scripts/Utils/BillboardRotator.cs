using UnityEngine;

public class BillboardRotator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Actor _agent;

    private Camera _camera;
    //NPCController _npcCTRL;
    //SpriteRenderer _planStateSpriteRenderer;
    public Vector3 dir;

    private void Start()
    {
        _camera = Camera.main;
        _agent = GetComponentInParent<Actor>();
        //_agent.OnCombatStateChanged += ChangeStateSpriteColor;

        //_npcCTRL.behaviours.actionSelector.OnActionChanged += ChangeActionSpriteColor;
        //_npcCTRL.behaviours.actionSelector.OnActionProcessingStateChanged += ChangeActionSpriteColor;

        //var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        //_spriteRenderer = spriteRenderers[0];
        //_planStateSpriteRenderer = spriteRenderers[1];

        //_agent.Combat.OnDeath += (s, a) =>
        //{
        //    Destroy(this.gameObject);
        //};
    }

    private void OnDisable()
    {
        //if(this._agent != null)
        //    this._agent.OnCombatStateChanged -= ChangeStateSpriteColor;
        //_npcCTRL.behaviours.actionSelector.OnActionChanged -= ChangeActionSpriteColor;
        //_npcCTRL.behaviours.actionSelector.OnActionProcessingStateChanged -= ChangeStateSpriteColor;
    }

    private void LateUpdate()
    {

        transform.LookAt(
            transform.position + _camera.transform.rotation * Vector3.forward, /*Camera.main.transform.rotation * */
            _camera.transform.up);
    }

    //void ChangeStateSpriteColor(bool inCombat)
    //{
    //    if(inCombat)
    //    {
    //        _planStateSpriteRenderer.color = Color.red;
    //    }
    //    else
    //    {
    //        _planStateSpriteRenderer.color = Color.green;
    //    }
    //}
}