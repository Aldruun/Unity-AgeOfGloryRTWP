using UnityEngine;

public class PlayerCamera
{
    [Header("Settings")]
    public float panningSpeed = 10;
    public float panningFriction = 15;
    public float rotationSpeedX = 150;
    public float rotationSpeedY = 150;
    public float zoomSpeed = 10;
    public float zoomFriction = 20;

    public Transform camTarget;
    public LayerMask camCollisionLayers;

    public float distance = 10.0f;
    public float yMin = -360f;
    public float yMax = 360f;
    public float XRotMin = 45f;
    public float XRotMax = 45f;
    public float distMin = 15f;
    public float distMax = 35f;
    public Camera camera;
    private readonly float rotDampX = 24f;
    private readonly float rotDampY = 24f;

    private Vector3 moveInput;
    private float mouseX;
    private float mouseY;
    private float zoomInput;

    private Quaternion rotation;
    private Vector3 desiredVelocity;
  
    private readonly CameraShake cameraShaker;
    private readonly Transform camTransform;
    private readonly Transform camRigRoot;
    private readonly Transform camRigRotator;
    private float resetCamRotationTimer = 2f;
    private float mult = 1f;
    private bool interruptFollowTarget;

    public Vector3 finalPosition { get; private set; }

    ///////////////////////////////////
    // Edge Scrolling
    ///////////////////////////////////
    private int ScreenEdgeSize = 40;
    private bool MoveUp;
    private bool MoveDown;
    private bool MoveRight;
    private bool MoveLeft;
    private Rect RigthRect;
    private Rect UpRect;
    private Rect DownRect;
    private Rect LeftRect;

    public PlayerCamera(Camera camera)
    {
        Debug.Assert(camera != null, "Camera null");
        this.camera = camera;
        camTransform = this.camera.transform;
        camTransform.localRotation = Quaternion.identity;
        camRigRotator = camTransform.parent;
        camRigRotator.eulerAngles = new Vector3(45f, 45f, 0);
        camRigRoot = camRigRotator.parent;
        camRigRoot.rotation = Quaternion.identity;
        zoomInput = 30;
        mouseX = camRigRotator.eulerAngles.y;
        mouseY = camRigRotator.eulerAngles.x;
        mouseY = ClampAngle(mouseY, XRotMin, XRotMax);
        cameraShaker = new CameraShake(this, camera);
        GameEventSystem.RequestCameraJumpToPosition = JumpTo;
        GameEventSystem.RequestCameraFollowPC = SetTargetCharacter;
        GameEventSystem.RequestCameraShake = Shake;
    }

    public void UpdateControls(bool blockInput)
    {

        if(blockInput)
        {
            Debug.Log("Blocking input");

            zoomInput = 0;
            desiredVelocity = Vector3.zero;
            return;
        }

        HandlePanning();

        if(GameEventSystem.IsPointerOverUIObject() == false)
        {
            HandleScreenEdgeScrolling();
            HandleRotation();
            HandleZooming();
        }

        //HandleResetRotation();

        if(camTarget != null)
        {
            if(interruptFollowTarget)
            {
                interruptFollowTarget = false;
                camTarget = null;
                return;
            }

            HandleFollowTarget();
        }
    }

    private void Shake(Vector3 triggerPosition, float amount, float duration)
    {
        cameraShaker.Shake(triggerPosition, amount, duration);
    }

    private void HandleFollowTarget()
    {
        Vector3 desiredPos = camTarget.position;
        desiredPos.y = 0;
        camRigRoot.position = Vector3.Lerp(camRigRoot.position, desiredPos, Time.deltaTime * 5f);
    }

    private void HandlePanning()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 forward = GetFlatCamDirNormalized();
        if(moveInput != Vector3.zero)
        {
            interruptFollowTarget = true;
        }
        desiredVelocity = camRigRoot.position + (forward * moveInput.z) + (camTransform.right * moveInput.x);

        camRigRoot.position = Vector3.Lerp(camRigRoot.position, desiredVelocity, Time.unscaledDeltaTime * panningSpeed);
    }

    private void HandleScreenEdgeScrolling()
    {
        UpRect = new Rect(1f, Screen.height - ScreenEdgeSize, Screen.width, ScreenEdgeSize);
        DownRect = new Rect(1f, 1f, Screen.width, ScreenEdgeSize);

        LeftRect = new Rect(1f, 1f, ScreenEdgeSize, Screen.height);
        RigthRect = new Rect(Screen.width - ScreenEdgeSize, 1f, ScreenEdgeSize, Screen.height);

        MoveUp = (UpRect.Contains(Input.mousePosition));
        MoveDown = (DownRect.Contains(Input.mousePosition));

        MoveLeft = (LeftRect.Contains(Input.mousePosition));
        MoveRight = (RigthRect.Contains(Input.mousePosition));

        Vector3 dir = Vector3.zero;
        dir.z = MoveUp ? 1 : MoveDown ? -1 : 0;
        dir.x = MoveLeft ? -1 : MoveRight ? 1 : 0;
        dir = camTransform.TransformDirection(dir);
        dir.y = 0;

        if(dir != Vector3.zero)
        {
            interruptFollowTarget = true;
        }

        camRigRoot.position = Vector3.Lerp(camRigRoot.position, camRigRoot.position + dir.normalized * panningSpeed, Time.unscaledDeltaTime);
    }

    private void HandleRotation()
    {
        if(Input.GetMouseButton(2))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            float inputX = Input.GetAxisRaw("Mouse X") * 0.02f * rotationSpeedX;
            float inputY = Input.GetAxisRaw("Mouse Y") * 0.02f * rotationSpeedY;
            mouseX += inputX;
            mouseY -= inputY;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        mouseY = ClampAngle(mouseY, XRotMin, XRotMax);

        rotation = Quaternion.Euler(mouseY, mouseX, 0);

        float rotX = camRigRotator.eulerAngles.x;
        float rotY = camRigRotator.eulerAngles.y;

        rotX = Mathf.LerpAngle(rotX, rotation.eulerAngles.x, Time.deltaTime * rotDampX * mult);
        rotY = Mathf.LerpAngle(rotY, rotation.eulerAngles.y, Time.deltaTime * rotDampY * mult);

        camRigRotator.rotation = Quaternion.Euler(rotX, rotY, 0f);
    }

    private void HandleResetRotation()
    {
        if(Input.GetMouseButton(2))
        {
            mult = 1f;
            resetCamRotationTimer = 2f;
            return;
        }

        resetCamRotationTimer -= Time.unscaledDeltaTime;

        if(resetCamRotationTimer > 0)
        {
            return;
        }

        mult = 0.1f;

        mouseY = mouseX = 45f;
        //mouseY = Mathf.MoveTowards(mouseY, 45f, Time.unscaledDeltaTime * 35f);
        //mouseX = Mathf.MoveTowards(mouseX, 45f, Time.unscaledDeltaTime * 35f);
    }

    private void HandleZooming()
    {
        float zoomThisFrame = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
       
        zoomInput -= zoomThisFrame;
        zoomInput = Mathf.Clamp(zoomInput, distMin, distMax);
        distance = Mathf.Lerp(distance, zoomInput, Time.unscaledDeltaTime * zoomFriction);
        camTransform.localPosition = new Vector3(0.0f, 0.0f, -distance);
    }

    private Vector3 GetFlatCamDirNormalized()
    {
        Vector3 forward = camTransform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private float ClampAngle(float angle, float min, float max)
    {

        if(angle < -360F)
        {
            angle += 360F;
        }

        if(angle > 360F)
        {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, min, max);
    }

    private void SetTargetCharacter(Transform target)
    {
        camTarget = target;
        camRigRoot.position = camTarget.position;
        //JumpTo(targetCharacter.transform.position);
    }

    private void ClearTargetCharacter()
    {
        if(camTarget != null)
        {
            camTarget = null;

        }
        //JumpTo(targetCharacter.transform.position);
    }

    private void JumpTo(Vector3 position)
    {

        camTarget = null;
        camRigRoot.position = position;
    }
}
