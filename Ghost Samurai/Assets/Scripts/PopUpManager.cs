using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PopUpManager : MonoBehaviour
{
    [Header("You died Pop-up")] 
    [SerializeField] private GameObject youDiedPopUpGameObject;
    [SerializeField] private GameObject youDiedPopUpBackgroundImage;
    [SerializeField] private CanvasGroup youDiedDeathImagePopUpCanvasGroup; // Allow us to set the alpha to fade over time
    
    
    [Header("Shinobi Execution Pop-up")] 
    [SerializeField] private GameObject bossDefeatedPopUpGameObject;
    [SerializeField] private GameObject bossDefeatedBackgroundImage;
    [SerializeField] private CanvasGroup bossDefeatedDeathImagePopUpCanvasGroup; // Allow us to set the alpha to fade over time

    public void SendYouDiedPopup()
    {
        youDiedPopUpBackgroundImage.SetActive(true);
        youDiedPopUpGameObject.SetActive(true);
        StartCoroutine(FadeInPopUpOverTime(youDiedDeathImagePopUpCanvasGroup ,5f));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedDeathImagePopUpCanvasGroup, 2f, 5f));
        
    }
    
    public void SendBossDefeatedPopup()
    {
        bossDefeatedBackgroundImage.SetActive(true);
        bossDefeatedPopUpGameObject.SetActive(true);
        StartCoroutine(FadeInPopUpOverTime(bossDefeatedDeathImagePopUpCanvasGroup ,5f));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedDeathImagePopUpCanvasGroup, 2f, 5f));
        
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvasGroup, float duration)
    {
        if (duration > 0)
        {
            canvasGroup.alpha = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, duration * Time.deltaTime); 
                yield return null;
            }
            
        }
        canvasGroup.alpha = 1;
        yield return null;
        
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvasGroup, float duration, float delay)
    {
        if (duration > 0)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, duration * Time.deltaTime); 
                yield return null;
            }
            
        }
        canvasGroup.alpha = 0;
        youDiedPopUpBackgroundImage.SetActive(false);
        yield return null;
    }
}
