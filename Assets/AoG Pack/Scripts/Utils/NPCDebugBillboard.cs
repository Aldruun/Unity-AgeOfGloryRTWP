using UnityEngine;

public class NPCDebugBillboard : MonoBehaviour
{
    private SpriteRenderer _actionSpriteRenderer;
    private ActorInput _agent;

    private Camera _camera;
    //SpriteRenderer _planStateSpriteRenderer;
    public Vector3 dir;

    private void Start()
    {
        _camera = Camera.main;
        _agent = GetComponentInParent<ActorInput>();
        //_agent.SetAttackState += ChangeStateSpriteColor;

        //_npcCTRL.behaviours.actionSelector.OnActionChanged += ChangeActionSpriteColor;
        //_npcCTRL.behaviours.actionSelector.OnActionProcessingStateChanged += ChangeActionSpriteColor;

        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _actionSpriteRenderer = spriteRenderers[0];
        //_planStateSpriteRenderer = spriteRenderers[1];

        //_agent.Combat.OnDeath += (s, a) =>
        //{
        //    for (var i = 0; i < spriteRenderers.Length; i++) spriteRenderers[i].enabled = false;
        //};
    }

    private void OnDisable()
    {
        //if(this._agent != null)
        //    this._agent.SetAttackState -= ChangeStateSpriteColor;
        //_npcCTRL.behaviours.actionSelector.OnActionChanged -= ChangeActionSpriteColor;
        //_npcCTRL.behaviours.actionSelector.OnActionProcessingStateChanged -= ChangeStateSpriteColor;
    }

    //void LateUpdate()
    //{

    //    transform.LookAt(
    //        transform.position + _camera.transform.rotation * Vector3.forward, /*Camera.main.transform.rotation * */
    //        _camera.transform.up);
    //}

    //void ChangeActionSpriteColor(NPCAction action)
    //{
    //    if (action == null)
    //    {
    //        //_action = null;
    //        _actionSpriteRenderer.color = Color.grey;
    //        return;
    //    }

    //    //if (action == _action) return;

    //    //_action = action;

    //    _actionSpriteRenderer.color = action.actionColor;
    //}

    private void ChangeStateSpriteColor(int attackState)
    {
        switch(attackState)
        {
            case 0:
                _actionSpriteRenderer.color = Color.red;
                break;
            case 1:
                _actionSpriteRenderer.color = Color.green;
                break;
            case 2:
                _actionSpriteRenderer.color = Color.yellow;
                break;
            default:
                Debug.LogError("Invalid attackstate");
                break;
        }
    }
}