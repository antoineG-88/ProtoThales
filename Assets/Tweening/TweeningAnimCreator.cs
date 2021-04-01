using UnityEngine;

public class TweeningAnimCreator : MonoBehaviour
{
    [HideInInspector] public float animationTime;
    [HideInInspector] public AnimationCurve animationCurve;
    [HideInInspector] public AnimationCurve rotAnimationCurve;
    [HideInInspector] public AnimationCurve scaleAnimationCurve;
    [HideInInspector] public bool customRotCurve;
    [HideInInspector] public bool customScaleCurve;
    [HideInInspector] public Gradient colorAnimation;
    [HideInInspector] public bool useColorChange;
    [HideInInspector] public Vector2 animationStartPos;
    [HideInInspector] public Vector2 animationEndPos;
    [HideInInspector] public Vector3 animationStartScale;
    [HideInInspector] public Vector3 animationEndScale;
    [HideInInspector] public float animationStartRot;
    [HideInInspector] public float animationEndRot;
    [HideInInspector] public bool movementRelativeToOriginalPos;

    [Header("Test Animation > Press \"T\" and \"U\" at runtime to preview")]
    public TweeningAnimator testTweenAnimator;

    private RectTransform rectTransform;

    private void Start()
    {
        if(testTweenAnimator.rectTransform != null)
        {
            testTweenAnimator.GetCanvasGroup();
        }
    }

    public void SaveStart()
    {
        rectTransform = GetComponent<RectTransform>();
        animationStartPos = rectTransform.anchoredPosition;
        animationStartRot = rectTransform.localRotation.eulerAngles.z;
        animationStartScale = rectTransform.localScale;
    }

    public void SaveEnd()
    {
        rectTransform = GetComponent<RectTransform>();
        animationEndPos = rectTransform.anchoredPosition;
        animationEndRot = rectTransform.localRotation.eulerAngles.z;
        animationEndScale = rectTransform.localScale;
    }

    public TweeningAnim GetAnim()
    {
        TweeningAnim anim = ScriptableObject.CreateInstance<TweeningAnim>();
        anim.animationCurve = animationCurve;
        anim.animationTime = animationTime;
        anim.animationStartPos = animationStartPos;
        anim.animationEndPos = animationEndPos;
        anim.colorAnimation = colorAnimation;
        anim.useColorChange = useColorChange;
        anim.animationEndRot = animationEndRot;
        anim.animationStartRot = animationStartRot;
        anim.animationStartScale = animationStartScale;
        anim.animationEndScale = animationEndScale;
        anim.customRotCurve = customRotCurve;
        anim.customScaleCurve = customScaleCurve;
        anim.scaleAnimationCurve = customScaleCurve ? scaleAnimationCurve : animationCurve;
        anim.rotAnimationCurve = customRotCurve ? rotAnimationCurve : animationCurve;
        anim.movementRelativeToOriginalPos = movementRelativeToOriginalPos;

        return anim;
    }

    private void Update()
    {
        if (testTweenAnimator.anim != null && testTweenAnimator.rectTransform != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(testTweenAnimator.anim.Play(testTweenAnimator));
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                StartCoroutine(testTweenAnimator.anim.PlayBackward(testTweenAnimator, true));
            }
        }

    }

    public void UpdateRelativePos()
    {
        if (movementRelativeToOriginalPos == true && animationStartPos != Vector2.zero)
        {
            animationEndPos -= animationStartPos;
            animationStartPos = Vector2.zero;
        }
    }
}
