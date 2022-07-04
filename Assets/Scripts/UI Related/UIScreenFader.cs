using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AoG.SceneManagement
{
    public class UIScreenFader : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        public void FadeIn(bool fadeIn, float duration)
        {
            if(canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            StartCoroutine(CR_FadeInScreen(fadeIn, duration));
        }

        IEnumerator CR_FadeInScreen(bool fadeIn, float duration)
        {
            float targetValue = 0f;
            canvasGroup.alpha = 1f;
            if(fadeIn == false)
            {
                targetValue = 1f;
                canvasGroup.alpha = 0f;
            }

            while(canvasGroup.alpha != targetValue)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetValue, Time.deltaTime / duration);
                yield return null;
            }

            Debug.Log("Fading done");
        }
    } 
}
