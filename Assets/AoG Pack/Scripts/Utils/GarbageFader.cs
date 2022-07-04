using System.Collections;
using UnityEngine;

public class GarbageFader : MonoBehaviour
{
    private void OnEnable()
    {
        foreach (var meshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        foreach (var mat in meshRenderer.materials)
        {
            mat.shader = Shader.Find("Custom/PreZ Standard Fade");
            //Debug.Log("Found mat");
            StartCoroutine(CR_FadeOut(mat, 5, 2));
        }

        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        foreach (var mat in meshRenderer.materials)
        {
            mat.shader = Shader.Find("Custom/PreZ Standard Fade");
            //Debug.Log("Found mat");
            StartCoroutine(CR_FadeOut(mat, 5, 2));
        }
    }

    private IEnumerator CR_FadeOut(Material mat, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float alpha = 1;

        for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            //Debug.Log("Fading out corpse");

            var newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
            mat.color = newColor;
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator FadeOutAlpha(Material mat, float delay, float fadePerSecond)
    {
        yield return new WaitForSeconds(delay);

        var color = mat.color;

        while (mat.color.a > 0)
        {
            mat.color = new Color(color.r, color.g, color.b, color.a - fadePerSecond * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}