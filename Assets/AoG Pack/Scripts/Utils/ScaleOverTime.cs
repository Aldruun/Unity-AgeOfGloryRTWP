using System.Collections;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    [SerializeField] private Vector3 finalScale = Vector3.one;
    [SerializeField] private float scaleDuration = 0.4f;
    [SerializeField] private Vector3 startScale = new Vector3(0, 0, 0);

    private void OnEnable()
    {
        transform.localScale = startScale;

        StartCoroutine(CR_Scale(scaleDuration));
    }

    private IEnumerator CR_Scale(float duration)
    {
        // Track how many seconds we've been fading.
        float t = 0;

        while (t < duration)
        {
            // Step the fade forward one frame.
            t += Time.deltaTime;

            // Turn the time into an interpolation factor between 0 and 1.
            var blend = Mathf.Clamp01(t / duration);

            transform.localScale =
                Vector3.Lerp(startScale, finalScale, blend); // new Vector3(blend, blend, blend) * Time.deltaTime;

            // Wait one frame, and repeat.
            yield return null;
        }
    }
}