using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GenericFunctions;

public class PortraitRecorder : MonoBehaviour
{

    public System.Action<ActorStats, Sprite> OnCreatedSnapshot;

    //public Transform testTarget;

    public Camera snapshotCamera;

    public LayerMask portraitLayers;

    public int imageSizeX = 50;
    public int imageSizeY = 60;

    public float snapshotDistance = 1;
    public float maxRandomAngle = 30;

    private RawImage image;

    private void OnEnable()
    {

        //GameEventSystem.OnPartyMemberAdded += CreatePortrait;
    }
    private void OnDisable()
    {

        //GameEventSystem.OnPartyMemberAdded -= CreatePortrait;
    }

    private void CreatePortrait(ActorInput agent, RawImage image)
    {

        if(snapshotCamera == null)
        {

            snapshotCamera = GetComponentInChildren<Camera>();
            snapshotCamera.cullingMask = portraitLayers;
        }

        Dictionary<Renderer, int> rendererLayerMap = new Dictionary<Renderer, int>();

        Renderer[] allRenderers = agent.GetComponentsInChildren<Renderer>();

        for(int i = 0; i < allRenderers.Length; i++)
        {

            Renderer renderer = allRenderers[i];
            rendererLayerMap[renderer] = renderer.gameObject.layer;
            renderer.gameObject.layer = LayerMask.NameToLayer("PortraitCreation");
        }



        Transform camTarget = agent.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);

        if(camTarget == null)
        {
            camTarget = agent.transform.Child("head", false, true);
        }
        if(camTarget == null)
        {
            Debug.LogError("camTarget = null");
            return;
        }

        Vector3 camPos = RandomAnglePoint(camTarget, agent.transform.forward, maxRandomAngle, snapshotDistance);
        snapshotCamera.transform.position = camPos;
        snapshotCamera.transform.rotation = Quaternion.LookRotation((camTarget.position - snapshotCamera.transform.position).normalized);

        gameObject.SetActive(true);
        snapshotCamera.enabled = true;

        image.texture = CaptureImage(snapshotCamera, imageSizeX, imageSizeY);

        for(int i = 0; i < allRenderers.Length; i++)
        {

            Renderer renderer = allRenderers[i];
            renderer.gameObject.layer = rendererLayerMap[renderer];
        }

        gameObject.SetActive(true);
        snapshotCamera.enabled = false;
    }

    public Texture2D CaptureImage(Camera camera, int width, int height)
    {
        Texture2D captured = new Texture2D(width, height);

        camera.Render();
        RenderTexture.active = camera.targetTexture;
        captured.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        captured.Apply();
        RenderTexture.active = null;
        return captured;
    }

    public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
    {
        var angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        var PointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V;
    }
    public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
    {
        return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
    }

    private Vector3 RandomAnglePoint(Transform origin, Vector3 forward, float angle, float distance)
    {

        float radius = Mathf.Tan(Mathf.Deg2Rad * angle / 2) * distance;
        Vector2 circle = Random.insideUnitCircle * radius;
        Vector3 targetPosition = origin.position + forward * distance + origin.rotation * new Vector3(circle.x, circle.y);
        return targetPosition;
    }
}
