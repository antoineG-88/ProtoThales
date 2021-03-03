﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TweeningAnim", menuName = "Tweening/Create Tweening Animation", order = 1)]
public class TweeningAnim : ScriptableObject
{
    public float animationTime;
    public AnimationCurve animationCurve;
    public Vector2 animationStartPos;
    public Vector2 animationEndPos;
    public float animationStartRot;
    public float animationEndRot;
    public Gradient colorAnimation;
    public bool isImage;
    public Image colorImage;
    public Text colorText;

    public IEnumerator Play(RectTransform animatedTransform, CanvasGroup canvasGroup)
    {
        float time = 0;
        animatedTransform.anchoredPosition = animationStartPos;

        if(isImage)
        {
            colorImage = animatedTransform.GetComponent<Image>();
        }
        else
        {
            colorText = animatedTransform.GetComponent<Text>();
        }

        while(time < animationTime)
        {
            if(isImage)
            {
                colorImage.color = colorAnimation.Evaluate(time / animationTime);
            }
            else
            {
                colorText.color = colorAnimation.Evaluate(time / animationTime);
            }
            animatedTransform.anchoredPosition = Vector2.Lerp(animationStartPos, animationEndPos, animationCurve.Evaluate(time / animationTime));

            animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationStartRot), Quaternion.Euler(0, 0, animationEndRot), animationCurve.Evaluate(time / animationTime));

            if(canvasGroup != null)
            {
                canvasGroup.alpha = colorAnimation.Evaluate(time / animationTime).a;
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animatedTransform.anchoredPosition = animationEndPos;
        animatedTransform.localRotation = Quaternion.Euler(0, 0, animationEndRot);

        if (isImage)
        {
            colorImage.color = colorAnimation.Evaluate(1);
        }
        else
        {
            colorText.color = colorAnimation.Evaluate(1);
        }

        if(canvasGroup != null)
        {
            canvasGroup.alpha = colorAnimation.Evaluate(1).a;
        }
    }


    public IEnumerator PlayBackward(RectTransform animatedTransform, CanvasGroup canvasGroup, bool onlyInversePos)
    {
        float time = 0;
        animatedTransform.anchoredPosition = animationEndPos;

        if (isImage)
        {
            colorImage = animatedTransform.GetComponent<Image>();
        }
        else
        {
            colorText = animatedTransform.GetComponent<Text>();
        }

        while (time < animationTime)
        {
            if (isImage)
            {
                colorImage.color = colorAnimation.Evaluate(1 - (time / animationTime));
            }
            else
            {
                colorText.color = colorAnimation.Evaluate(1 - (time / animationTime));
            }

            if(onlyInversePos)
            {
                animatedTransform.anchoredPosition = Vector2.Lerp(animationEndPos, animationStartPos, animationCurve.Evaluate(time / animationTime));
                animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationEndRot), Quaternion.Euler(0, 0, animationStartRot), animationCurve.Evaluate(time / animationTime));
            }
            else
            {
                animatedTransform.anchoredPosition = Vector2.Lerp(animationStartPos, animationEndPos, animationCurve.Evaluate(1 - (time / animationTime)));
                animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationStartRot), Quaternion.Euler(0, 0, animationEndRot), animationCurve.Evaluate(1 - (time / animationTime)));
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = colorAnimation.Evaluate(1 - (time / animationTime)).a;
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animatedTransform.anchoredPosition = animationStartPos;
        animatedTransform.localRotation = Quaternion.Euler(0, 0, animationStartRot);

        if (isImage)
        {
            colorImage.color = colorAnimation.Evaluate(0);
        }
        else
        {
            colorText.color = colorAnimation.Evaluate(0);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = colorAnimation.Evaluate(0).a;
        }
    }
}
