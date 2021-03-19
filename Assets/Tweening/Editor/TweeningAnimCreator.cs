using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class TweeningAnimCreator : MonoBehaviour
{
    [HideInInspector] public float animationTime;
    [HideInInspector] public AnimationCurve animationCurve;
    [HideInInspector] public AnimationCurve rotAnimationCurve;
    [HideInInspector] public AnimationCurve scaleAnimationCurve;
    [HideInInspector] public bool customRotCurve;
    [HideInInspector] public bool customScaleCurve;
    [HideInInspector] public Gradient colorAnimation;
    [HideInInspector] public bool isImage;
    [HideInInspector] public Vector2 animationStartPos;
    [HideInInspector] public Vector2 animationEndPos;
    [HideInInspector] public Vector3 animationStartScale;
    [HideInInspector] public Vector3 animationEndScale;
    [HideInInspector] public float animationStartRot;
    [HideInInspector] public float animationEndRot;
    [HideInInspector] public bool movementRelativeToOriginalPos;

    [Header("Test Animation > Press \"T\" at runtime to preview")]
    public TweeningAnimator testTweenAnimator;

    private RectTransform rectTransform;

    private void Start()
    {
        if(testTweenAnimator.rectTransform != null)
        {
            testTweenAnimator.canvasGroup = testTweenAnimator.rectTransform.GetComponent<CanvasGroup>();
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

    public void CreateAnimation()
    {
        TweeningAnim anim = ScriptableObject.CreateInstance<TweeningAnim>();
        anim.animationCurve = animationCurve;
        anim.animationTime = animationTime;
        anim.animationStartPos = animationStartPos;
        anim.animationEndPos = animationEndPos;
        anim.colorAnimation = colorAnimation;
        anim.isImage = isImage;
        anim.animationEndRot = animationEndRot;
        anim.animationStartRot = animationStartRot;
        anim.animationStartScale = animationStartScale;
        anim.animationEndScale = animationEndScale;
        anim.customRotCurve = customRotCurve;
        anim.customScaleCurve = customScaleCurve;
        anim.scaleAnimationCurve = customScaleCurve ? scaleAnimationCurve : animationCurve;
        anim.rotAnimationCurve = customRotCurve ? rotAnimationCurve : animationCurve;
        anim.movementRelativeToOriginalPos = movementRelativeToOriginalPos;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeContext)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(TweeningAnim).ToString() + ".asset");

        AssetDatabase.CreateAsset(anim, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = anim;
    }

    private void Update()
    {
        if (testTweenAnimator.anim != null && testTweenAnimator.rectTransform != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(testTweenAnimator.anim.Play(testTweenAnimator.rectTransform, testTweenAnimator.canvasGroup));
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                StartCoroutine(testTweenAnimator.anim.PlayBackward(testTweenAnimator.rectTransform, testTweenAnimator.canvasGroup, true));
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
