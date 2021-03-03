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
        tweeningAnimCreator.animationCurve = EditorGUILayout.CurveField("Animation Curve", tweeningAnimCreator.animationCurve);
        tweeningAnimCreator.colorAnimation = EditorGUILayout.GradientField("Color Animation", tweeningAnimCreator.colorAnimation);
        tweeningAnimCreator.isImage = EditorGUILayout.Toggle("Modify color on Image ?", tweeningAnimCreator.isImage);

        GUILayout.Space(15);

        EditorGUILayout.Vector2Field("Animation Start Position", tweeningAnimCreator.animationStartPos);
        EditorGUILayout.FloatField("Animation Start Rotation", tweeningAnimCreator.animationStartRot);

        if (GUILayout.Button("Save Animation Start"))
        {
            tweeningAnimCreator.SaveStart();
        }

        GUILayout.Space(15);

        EditorGUILayout.Vector2Field("Animation End Position", tweeningAnimCreator.animationEndPos);
        EditorGUILayout.FloatField("Animation End Rotation", tweeningAnimCreator.animationEndRot);

        if (GUILayout.Button("Save Animation End"))
        {
            tweeningAnimCreator.SaveEnd();
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
