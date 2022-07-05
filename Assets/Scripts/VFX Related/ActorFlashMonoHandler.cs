using AoG.Core;
using System.Collections;
using UnityEngine;

public class ActorFlashMonoHandler
{
    private SkinnedMeshRenderer[] actorMeshRenderers;
    //private Material[] originalMaterials;
    //private Color[] originalColors;
    //private Material fleshMaterial;

    public ActorFlashMonoHandler(Actor self)
    {
        actorMeshRenderers = self.GetComponentsInChildren<SkinnedMeshRenderer>();
        //originalColors = new Color[actorMeshRenderers.Length];

        //for(int i = 0; i < actorMeshRenderers.Length; i++)
        //{
        //    originalColors[i] = actorMeshRenderers[i].material.color;
        //}
    }

    public void Flash(Color color, float duration)
    {
        CoroutineRunner.Instance.StartCoroutine(CR_Flash(color, duration));
    }

    private IEnumerator CR_Flash(Color color, float duration)
    {
        //SetFlashColors(color);

        float time = 0f;

        while(time < duration)
        {
            //actorMeshRenderers[i].material.color = color;
            for(int i = 0; i < actorMeshRenderers.Length; i++)
            {
                for(int m = 0; m < actorMeshRenderers[i].materials.Length; m++)
                {
                    //float pingPong = Mathf.PingPong(Time.time, time);
                    Color finalColor = Color.Lerp(color, Color.black, time / duration);
                    //Color finalColor = color * Mathf.LinearToGammaSpace(emission);

                    actorMeshRenderers[i].materials[m].SetColor("_EmissionColor", finalColor);
                    actorMeshRenderers[i].materials[m].EnableKeyword("_EMISSION");
                }
            }

            // Scale time
            time += Time.deltaTime;

            yield return null;
        }

        //yield return new WaitForSeconds(duration);
        RevertToOriginalColors();
    }

    //void SetFlashColors(Color color)
    //{
    //    for(int i = 0; i < actorMeshRenderers.Length; i++)
    //    {
    //        actorMeshRenderers[i].material.color = color;
    //    }
    //}

    private void RevertToOriginalColors()
    {
        for(int i = 0; i < actorMeshRenderers.Length; i++)
        {
            for(int m = 0; m < actorMeshRenderers[i].materials.Length; m++)
            {
                actorMeshRenderers[i].materials[m].SetColor("_EmissionColor", Color.black);
                actorMeshRenderers[i].materials[m].EnableKeyword("_EMISSION");
            }
        }
    }
}
