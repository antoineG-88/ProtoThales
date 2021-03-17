using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweeningAnimCreator))]
public class TweeningEditor : Editor
{
    private TweeningAnimCreator tweeningAnimCreator;

    public override void OnInspectorGUI()
    {
        tweeningAnimCreator = (TweeningAnimCreator)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("         Animation Values");
        GUILayout.Space(10);

        tweeningAnimCreator.animationTime = EditorGUILayout.FloatField("Animation Time", tweeningAnimCreator.animationTime);
        tweeningAnimCreator.animationCurve = EditorGUILayout.CurveField("Global Animation Curve", tweeningAnimCreator.animationCurve);

        if(tweeningAnimCreator.customRotCurve)
        {
            GUILayout.BeginHorizontal();
        }

        tweeningAnimCreator.customRotCurve = EditorGUILayout.Toggle("Custom Rotation anim curve", tweeningAnimCreator.customRotCurve);
        if (tweeningAnimCreator.customRotCurve)
        {
            if (GUILayout.Button("Copy global curve"))
            {
                tweeningAnimCreator.rotAnimationCurve = tweeningAnimCreator.animationCurve;
            }
            GUILayout.EndHorizontal();
            tweeningAnimCreator.rotAnimationCurve = EditorGUILayout.CurveField("Rotation animation curve", tweeningAnimCreator.rotAnimationCurve);
        }


        if (tweeningAnimCreator.customScaleCurve)
        {
            GUILayout.BeginHorizontal();
        }

        tweeningAnimCreator.customScaleCurve = EditorGUILayout.Toggle("Custom Scale anim curve", tweeningAnimCreator.customScaleCurve);
        if (tweeningAnimCreator.customScaleCurve)
        {
            if (GUILayout.Button("Copy global curve"))
            {
                tweeningAnimCreator.scaleAnimationCurve = tweeningAnimCreator.animationCurve;
            }
            GUILayout.EndHorizontal();
            tweeningAnimCreator.scaleAnimationCurve = EditorGUILayout.CurveField("Scale animation curve", tweeningAnimCreator.scaleAnimationCurve);
        }

        tweeningAnimCreator.colorAnimation = EditorGUILayout.GradientField("Color Animation", tweeningAnimCreator.colorAnimation);
        tweeningAnimCreator.isImage = EditorGUILayout.Toggle("Modify color on Image ?", tweeningAnimCreator.isImage);

        GUILayout.Space(15);

        EditorGUILayout.Vector2Field("Animation Start Position", tweeningAnimCreator.animationStartPos);
        EditorGUILayout.FloatField("Animation Start Rotation", tweeningAnimCreator.animationStartRot);
        EditorGUILayout.Vector3Field("Animation Start Scale", tweeningAnimCreator.animationStartScale);

        if (GUILayout.Button("Save Animation Start"))
        {
            tweeningAnimCreator.SaveStart();
        }

        GUILayout.Space(15);

        EditorGUILayout.Vector2Field("Animation End Position", tweeningAnimCreator.animationEndPos);
        EditorGUILayout.FloatField("Animation End Rotation", tweeningAnimCreator.animationEndRot);
        EditorGUILayout.Vector3Field("Animation End Scale", tweeningAnimCreator.animationEndScale);

        if (GUILayout.Button("Save Animation End"))
        {
            tweeningAnimCreator.SaveEnd();
        }

        tweeningAnimCreator.movementRelativeToOriginalPos = EditorGUILayout.Toggle("Movement relative to start pos", tweeningAnimCreator.movementRelativeToOriginalPos);
        if (tweeningAnimCreator.movementRelativeToOriginalPos)
        {
            tweeningAnimCreator.UpdateRelativePos();
        }

        GUILayout.Space(15);


        if (GUILayout.Button("CREATE"))
        {
            tweeningAnimCreator.CreateAnimation();
        }

        GUILayout.Space(25);

        base.OnInspectorGUI();
    }
}
