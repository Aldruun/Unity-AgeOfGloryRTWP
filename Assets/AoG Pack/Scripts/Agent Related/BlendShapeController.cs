using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeController : MonoBehaviour
{
    [Header("Settings")]
    public float m_rndLipsMoveFreq = 0.05f;

    private Animator m_animator;

    public bool blink = true;
    public Vector2 blinkInterval = new Vector2(1, 3);

    public float blinkCloseSpeed = 20f;
    public float blinkOpenSpeed = 4f;
    public float exprBlendSpeed = 1.04f;
    private bool blinkCloseFinished;
    public float initialEyeClosedBlendValue { get; set; }
    //LookAt _lookCtrl;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh skinnedMesh;
    private float _blend_blink = 0f;
    private float _blinkTimer;

    public bool m_randomExpressions = true;
    public Vector2 smileChangeInterval = new Vector2(1, 3);
    private float _blend_smile = 0f;
    private float _smileChangeTimer;
    private float _currSmileStrength;

    private void Awake()
    {

        //_lookCtrl = GetComponentInParent<LookAt>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        initialEyeClosedBlendValue = skinnedMeshRenderer.GetBlendShapeWeight(0);
        _blend_blink = initialEyeClosedBlendValue;
        _blinkTimer = Random.Range(blinkInterval.x, blinkInterval.y);

        if(Application.isPlaying && m_randomExpressions)
        {
            StartCoroutine(CR_SmileControl());
        }

        Blink();
    }

    private IEnumerator CR_SmileControl()
    {

        while(m_randomExpressions)
        {

            _smileChangeTimer = Random.Range(smileChangeInterval.x, smileChangeInterval.y);
            float rndSmileStrength = Random.Range(0, 100);
            yield return new WaitForSeconds(_smileChangeTimer);

            //while(_blend_smile != rndSmileStrength) {

            //    _blend_smile = Mathf.Lerp(_blend_smile, rndSmileStrength, Time.deltaTime / exprBlendSpeed);
            //    skinnedMeshRenderer.SetBlendShapeWeight(2, _blend_smile);

            //    yield return null;
            //}

            if(_blend_smile > rndSmileStrength)
            {

                while(_blend_smile > rndSmileStrength)
                {

                    _blend_smile -= exprBlendSpeed;
                    skinnedMeshRenderer.SetBlendShapeWeight(2, _blend_smile);

                    yield return null;
                }
            }
            else
            {

                while(_blend_smile < rndSmileStrength)
                {

                    _blend_smile += exprBlendSpeed;
                    skinnedMeshRenderer.SetBlendShapeWeight(2, _blend_smile);

                    yield return null;
                }
            }
        }
    }

    private IEnumerator CR_Blink()
    {

        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0.2f, 2));
            while(_blend_blink < 100)
            {
                _blend_blink += blinkCloseSpeed;
                //_blend_blink = Mathf.MoveTowards(_blend_blink, 100, blinkCloseSpeed);
                //if(_blend_blink > 100) {

                //    _blend_blink = 100;
                //}
                skinnedMeshRenderer.SetBlendShapeWeight(0, _blend_blink);

                yield return null;
            }

            while(_blend_blink > initialEyeClosedBlendValue)
            {
                //_blend_blink = Mathf.MoveTowards(_blend_blink, initialEyeClosedBlendValue, blinkOpenSpeed);
                _blend_blink -= blinkOpenSpeed;
                //if(_blend_blink < initialEyeClosedBlendValue) {

                //    _blend_blink = initialEyeClosedBlendValue;
                //}
                skinnedMeshRenderer.SetBlendShapeWeight(0, _blend_blink);

                yield return null;
            }
        }
    }

    //IEnumerator CR_Blink() {

    //    for(float t = 0.0f; t < 1.0f; t += Time.deltaTime / blinkCloseSpeed) {

    //        Debug.Log("closing");
    //        _blend_blink = Mathf.Lerp(_blend_blink, 1, t);
    //        skinnedMeshRenderer.SetBlendShapeWeight(0, _blend_blink * 100);
    //        //transform.renderer.material.color = newColor;
    //        yield return null;
    //    }

    //    for(float t = 0.0f; t < 1.0f; t += Time.deltaTime / blinkOpenSpeed) {

    //        Debug.Log("opening");
    //        _blend_blink = Mathf.Lerp(_blend_blink, initialEyeClosedBlendValue * 0.1f, t);
    //        skinnedMeshRenderer.SetBlendShapeWeight(0, _blend_blink * 100);
    //        //transform.renderer.material.color = newColor;
    //        yield return null;
    //    }
    //}

    public void Blink()
    {

        StartCoroutine(CR_Blink());
    }

    public void SetBlendShapeWeight(int index, float weight)
    {
        if(skinnedMeshRenderer == null)
        {

            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }
        skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
    }

    private Transform eye;
    public void HandleEyeLids(Transform eye)
    {
        if(this.eye == null)
        {

            this.eye = eye;
        }

        // Handle "Look Up" expression
        SetBlendShapeWeight(1, Mathf.Lerp(0, 100, Mathf.InverseLerp(35, 20, (360 - eye.localEulerAngles.x))));

        // Handle eyebrows
        float currEyebrowStrength = skinnedMeshRenderer.GetBlendShapeWeight(3);
        float desiredEyebrowStrength = Mathf.Lerp(0, 100, Mathf.InverseLerp(35, 20, (360 - eye.localEulerAngles.x)));
        SetBlendShapeWeight(3, Mathf.MoveTowards(currEyebrowStrength, desiredEyebrowStrength, Time.deltaTime * 200));

        // Handle upper eyelids
        SetBlendShapeWeight(0, Mathf.Lerp(0, 40, Mathf.InverseLerp(35, 50, (360 - eye.localEulerAngles.x))));
        initialEyeClosedBlendValue = skinnedMeshRenderer.GetBlendShapeWeight(0);
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void OnGUI()
    {

        if(eye != null)
            GUI.Label(new Rect(5, 5, 100, 30), (360 - eye.localEulerAngles.x).ToString());
    }
}
