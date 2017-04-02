using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour {

    public float changeTimeSeconds = 5;
    public float startAlpha = 0;
    public float endAlpha = 1;
    public CanvasGroup canvasGroup;

    float changeRate = 0;
    float timeSoFar = 0;
    bool fading = false;
    public bool isShowing=true;
    //CanvasGroup canvasGroup;


    //void Start()
    //{
    //    canvasGroup = this.GetComponent<CanvasGroup>();
    //    if (canvasGroup == null)
    //    {
    //        Debug.Log("Must have canvas group attached!");
    //        this.enabled = false;
    //    }
    //}

    public void DoFade()
    {
        if (isShowing)
        {
            FadeOut();
            isShowing = !isShowing;
            canvasGroup.interactable = false;
        }
        else
        {
            FadeIn();
            isShowing = !isShowing;
            canvasGroup.interactable = true;
        }
        
    }



    public void FadeIn()
    {
        gameObject.SetActive(true);
        startAlpha = 0;
        endAlpha = 1;
        timeSoFar = 0;
        fading = true;
        StartCoroutine(FadeCoroutine());
    }

    public void FadeOut()
    {
        startAlpha = 1;
        endAlpha = 0;
        timeSoFar = 0;
        fading = true;
        StartCoroutine(FadeCoroutine());
        gameObject.SetActive(false);
    }

    IEnumerator FadeCoroutine()
    {
        changeRate = (endAlpha - startAlpha) / changeTimeSeconds;
        SetAlpha(startAlpha);
        while (fading)
        {
            timeSoFar += Time.deltaTime;

            if (timeSoFar > changeTimeSeconds)
            {
                fading = false;
                SetAlpha(endAlpha);
                yield break;
            }
            else
            {
                SetAlpha(canvasGroup.alpha + (changeRate * Time.deltaTime));
            }

            yield return null;
        }
    }





    public void SetAlpha(float alpha)
    {
        canvasGroup.alpha = Mathf.Clamp(alpha, 0, 1);
    }
}

