using AoG.UI;
using UnityEngine;
using UnityEngine.UI;

/* Popups will
 * - fade out over time
 * - only be spawned by the UIHandler if close to the camera
 * - be invisible (RectTransform disabled) if out of screen
 */
public class UIPopup : MonoBehaviour
{
    private Camera _camera;
    private float _durationFade;
    private float _durVisibleTimer;
    private float _height;
    private Image _image;
    private float _scrollSpeed;
    private TMPro.TextMeshProUGUI _text;
    private float _xOffset;
    public Transform anchor;
    public Vector3 anchorPosition;
    private RectTransform rectTransform;
    private float t;
    private float y;

    //public bool InRange {

    //    get { return Vector3.Distance(Camera.main.transform.position, this.transform.position) > UIHandler.current.uiFadeOutDistance; }
    //}

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _xOffset = Random.Range(-10, 10);
    }

    public void Initialize(Transform anchor, float height, string context, float duration, float scrollSpeed,
        float fadeDuration, Color? color)
    {
        rectTransform = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _text.text = context;
        var cachedColor = color == null ? Color.white : color.Value;
        _text.color = cachedColor;
        this.anchor = anchor;
        _height = height;
        y = 0;
        _scrollSpeed = scrollSpeed;
        _durVisibleTimer = duration;
        _durationFade = fadeDuration;
        transform.position = _camera.WorldToScreenPoint(anchor.position + Vector3.up * height);
    }

    public void Initialize(Vector3 anchorPosition, string context, float duration, float scrollSpeed,
        float fadeDuration, Color? color)
    {
        rectTransform = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _text.text = context;
        var cachedColor = color == null ? Color.white : color.Value;
        _text.color = cachedColor;
        this.anchorPosition = anchorPosition;
        y = 0;
        _scrollSpeed = scrollSpeed;
        _durVisibleTimer = duration;
        _durationFade = fadeDuration;
        transform.position = _camera.WorldToScreenPoint(anchorPosition + Vector3.up * 1.8f);
    }

    public void Initialize(Transform anchor, float height, Sprite icon, float duration, float scrollSpeed,
        float fadeDuration)
    {
        rectTransform = GetComponent<RectTransform>();
        this.anchor = anchor;
        _height = height;
        _image = GetComponent<Image>();
        _image.sprite = icon;
        y = 0;
        _scrollSpeed = scrollSpeed;
        _durVisibleTimer = duration;
        _durationFade = fadeDuration;
        transform.position = _camera.WorldToScreenPoint(anchor.position + Vector3.up * height);
    }

    private void Update()
    {
        // If this popup is out of range, we discard it completely, without the chance to make it visible again
        //if(InRange == false) {

        //    this.gameObject.SetActive(false);
        //}

        // If this popup is out of screen, we temporarily disable the RectTransform and re-activate it if on screen again
        if (_text != null)
            _text.enabled = UIHandler.UIOnScreen(transform.position);
        else
            _image.enabled = UIHandler.UIOnScreen(transform.position);

        if (anchor == null)
        {
            //return;
        }
        else
        {
            anchorPosition = anchor.position;
        }

        _durVisibleTimer -= Time.unscaledDeltaTime;
        y += Time.unscaledDeltaTime * _scrollSpeed;
        var screenPos = _camera.WorldToScreenPoint(anchorPosition + new Vector3(0, _height, 0) + _camera.transform.up * y);
        //_scrollSpeed = Mathf.Lerp(_scrollSpeed, 0, SceneManagment.deltaTime / (_durationFade + _durVisibleTimer));

        rectTransform.position = screenPos;

        if (_durVisibleTimer <= 0)
            if (DoneFading(_durationFade))
                gameObject.SetActive(false);
    }

    private bool DoneFading(float duration)
    {
        if(t < duration)
        {
            t += Time.unscaledDeltaTime;
            // Turn the time into an interpolation factor between 0 and 1.
            var blend = Mathf.Clamp01(t / duration);

            // Blend to the corresponding opacity between start & target.
            if(_image != null)
            {
                var color = _image.color;
                color.a = Mathf.Lerp(1, 0, blend);
                _image.color = color;
            }
            else
            {
                var color = _text.color;
                color.a = Mathf.Lerp(1, 0, blend);
                _text.color = color;
            }

            return false;
        }

        t = 0;
        return true;
    }
}