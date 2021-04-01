using UnityEngine;
using UnityEditor;
using System.IO;

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
        tweeningAnimCreator.useColorChange = EditorGUILayout.Toggle("Use Color Change", tweeningAnimCreator.useColorChange);

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
            CreateAnimInAssets(tweeningAnimCreator.GetAnim());
        }

        GUILayout.Space(25);

        base.OnInspectorGUI();
    }

    public static void CreateAnimInAssets(TweeningAnim newAnim)
    {
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

        AssetDatabase.CreateAsset(newAnim, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newAnim;
    }
}
