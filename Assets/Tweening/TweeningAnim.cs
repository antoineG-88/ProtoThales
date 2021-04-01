using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TweeningAnim", menuName = "Tweening/Create Tweening Animation", order = 1)]
public class TweeningAnim : ScriptableObject
{
    public float animationTime;
    public AnimationCurve animationCurve;
    public AnimationCurve rotAnimationCurve;
    public AnimationCurve scaleAnimationCurve;
    public bool customRotCurve;
    public bool customScaleCurve;
    public Vector2 animationStartPos;
    public Vector2 animationEndPos;
    public Vector3 animationStartScale;
    public Vector3 animationEndScale;
    public float animationStartRot;
    public float animationEndRot;
    public bool movementRelativeToOriginalPos;
    public Gradient colorAnimation;
    public bool useColorChange;
    public Image colorImage;

    private RectTransform animatedTransform;
    private CanvasGroup canvasGroup;

    public IEnumerator Play(TweeningAnimator animator, Vector2 originalPos)
    {
        animatedTransform = animator.rectTransform;
        canvasGroup = animator.canvasGroup;

        float time = 0;
        if(movementRelativeToOriginalPos)
        {
            animatedTransform.anchoredPosition = originalPos;
        }
        else
        {
            animatedTransform.anchoredPosition = animationStartPos;
        }


        if (useColorChange)
        {
            colorImage = animatedTransform.GetComponent<Image>();
        }
        while(time < animationTime)
        {
            if(useColorChange && colorImage != null)
            {
                colorImage.color = colorAnimation.Evaluate(time / animationTime);
                if(canvasGroup != null)
                {
                    colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
                }
            }


            animatedTransform.anchoredPosition = Vector2.Lerp(movementRelativeToOriginalPos ? originalPos : animationStartPos, movementRelativeToOriginalPos ? originalPos + animationEndPos : animationEndPos, animationCurve.Evaluate(time / animationTime));

            animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationStartRot), Quaternion.Euler(0, 0, animationEndRot), customRotCurve ? rotAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));

            animatedTransform.localScale = Vector3.Lerp(animationStartScale, animationEndScale, customScaleCurve ? scaleAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));

            if (canvasGroup != null)
            {
                canvasGroup.alpha = colorAnimation.Evaluate(time / animationTime).a;
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animatedTransform.anchoredPosition = movementRelativeToOriginalPos ? originalPos + animationEndPos : animationEndPos;
        animatedTransform.localRotation = Quaternion.Euler(0, 0, animationEndRot);
        animatedTransform.localScale = animationEndScale;

        if (useColorChange && colorImage != null)
        {
            colorImage.color = colorAnimation.Evaluate(1);
            if (canvasGroup != null)
            {
                colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = colorAnimation.Evaluate(1).a;
        }
    }

    public IEnumerator Play(TweeningAnimator animator)
    {
        animatedTransform = animator.rectTransform;
        canvasGroup = animator.canvasGroup;

        float time = 0;
        animatedTransform.anchoredPosition = animationStartPos;


        if (useColorChange)
        {
            colorImage = animatedTransform.GetComponent<Image>();
        }
        while (time < animationTime)
        {
            if (useColorChange && colorImage != null)
            {
                colorImage.color = colorAnimation.Evaluate(time / animationTime);
                if (canvasGroup != null)
                {
                    colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
                }
            }
            animatedTransform.anchoredPosition = Vector2.Lerp(animationStartPos, animationEndPos, animationCurve.Evaluate(time / animationTime));

            animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationStartRot), Quaternion.Euler(0, 0, animationEndRot), customRotCurve ? rotAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));

            animatedTransform.localScale = Vector3.Lerp(animationStartScale, animationEndScale, customScaleCurve ? scaleAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));

            if (canvasGroup != null)
            {
                canvasGroup.alpha = colorAnimation.Evaluate(time / animationTime).a;
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animatedTransform.anchoredPosition = animationEndPos;
        animatedTransform.localRotation = Quaternion.Euler(0, 0, animationEndRot);
        animatedTransform.localScale = animationEndScale;

        if (useColorChange && colorImage != null)
        {
            colorImage.color = colorAnimation.Evaluate(1);
            if (canvasGroup != null)
            {
                colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = colorAnimation.Evaluate(1).a;
        }
    }

    public IEnumerator PlayBackward(TweeningAnimator animator, Vector2 originalPos, bool onlyInversePos)
    {
        animatedTransform = animator.rectTransform;
        canvasGroup = animator.canvasGroup;

        float time = 0;
        if (movementRelativeToOriginalPos)
        {
            animatedTransform.anchoredPosition = originalPos + animationEndPos;
        }
        else
        {
            animatedTransform.anchoredPosition = animationEndPos;
        }

        if (useColorChange)
        {
            colorImage = animatedTransform.GetComponent<Image>();
        }

        while (time < animationTime)
        {
            if (useColorChange && colorImage != null)
            {
                colorImage.color = colorAnimation.Evaluate(1 - (time / animationTime));
                if (canvasGroup != null)
                {
                    colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
                }
            }

            if(onlyInversePos)
            {
                animatedTransform.anchoredPosition = Vector2.Lerp(movementRelativeToOriginalPos ? originalPos + animationEndPos : animationEndPos, movementRelativeToOriginalPos ? originalPos : animationStartPos, animationCurve.Evaluate(time / animationTime));
                animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationEndRot), Quaternion.Euler(0, 0, animationStartRot), customRotCurve ? rotAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));
                animatedTransform.localScale = Vector3.Lerp(animationEndScale, animationStartScale, customScaleCurve ? scaleAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));
            }
            else
            {
                animatedTransform.anchoredPosition = Vector2.Lerp(movementRelativeToOriginalPos ? originalPos : animationStartPos, movementRelativeToOriginalPos ? originalPos + animationEndPos : animationEndPos, animationCurve.Evaluate(1 - (time / animationTime)));
                animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationStartRot), Quaternion.Euler(0, 0, animationEndRot), customRotCurve ? rotAnimationCurve.Evaluate(1 - (time / animationTime)) : animationCurve.Evaluate(1 - (time / animationTime)));
                animatedTransform.localScale = Vector3.Lerp(animationStartScale, animationEndScale, customScaleCurve ? scaleAnimationCurve.Evaluate(1- (time / animationTime)) : animationCurve.Evaluate(1 - (time / animationTime)));
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = colorAnimation.Evaluate(1 - (time / animationTime)).a;
            }

            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        animatedTransform.anchoredPosition = movementRelativeToOriginalPos ? originalPos : animationStartPos;
        animatedTransform.localRotation = Quaternion.Euler(0, 0, animationStartRot);
        animatedTransform.localScale = animationStartScale;

        if (useColorChange && colorImage != null)
        {
            colorImage.color = colorAnimation.Evaluate(0);
            if (canvasGroup != null)
            {
                colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = colorAnimation.Evaluate(0).a;
        }
    }

    public IEnumerator PlayBackward(TweeningAnimator animator, bool onlyInversePos)
    {
        animatedTransform = animator.rectTransform;
        canvasGroup = animator.canvasGroup;
        float time = 0;
        animatedTransform.anchoredPosition = animationEndPos;

        if (useColorChange)
        {
            colorImage = animatedTransform.GetComponent<Image>();
        }

        while (time < animationTime)
        {
            if (useColorChange && colorImage != null)
            {
                colorImage.color = colorAnimation.Evaluate(1 - (time / animationTime));
                if (canvasGroup != null)
                {
                    colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
                }
            }

            if (onlyInversePos)
            {
                animatedTransform.anchoredPosition = Vector2.Lerp(animationEndPos, animationStartPos, animationCurve.Evaluate(time / animationTime));
                animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationEndRot), Quaternion.Euler(0, 0, animationStartRot), customRotCurve ? rotAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));
                animatedTransform.localScale = Vector3.Lerp(animationEndScale, animationStartScale, customScaleCurve ? scaleAnimationCurve.Evaluate(time / animationTime) : animationCurve.Evaluate(time / animationTime));
            }
            else
            {
                animatedTransform.anchoredPosition = Vector2.Lerp(animationStartPos, animationEndPos, animationCurve.Evaluate(1 - (time / animationTime)));
                animatedTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, animationStartRot), Quaternion.Euler(0, 0, animationEndRot), customRotCurve ? rotAnimationCurve.Evaluate(1 - (time / animationTime)) : animationCurve.Evaluate(1 - (time / animationTime)));
                animatedTransform.localScale = Vector3.Lerp(animationStartScale, animationEndScale, customScaleCurve ? scaleAnimationCurve.Evaluate(1 - (time / animationTime)) : animationCurve.Evaluate(1 - (time / animationTime)));
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
        animatedTransform.localScale = animationStartScale;

        if (useColorChange && colorImage != null)
        {
            colorImage.color = colorAnimation.Evaluate(0);
            if (canvasGroup != null)
            {
                colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 1);
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = colorAnimation.Evaluate(0).a;
        }
    }
}
