using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

// [CustomEditor(typeof(GameObject))]
// [CanEditMultipleObjects]
public class SceneViewAnimationEditor : Editor
{
    private GameObject myTarget;
    int selection;

    public void OnSceneGUI()
    {
        DrawDefaultInspector();
        Handles.BeginGUI();
        myTarget = (GameObject)Selection.activeObject;
        Animator animator = myTarget.GetComponentInParent<Animator>();
        if (animator != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            selection = EditorGUILayout.Popup(selection, clips.Select(j => j.name).ToArray(), GUILayout.Width(50));
            if (GUILayout.Button("Add position", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                foreach (RectTransform item in animator.GetComponentInChildren<RectTransform>())
                {
                    SetAniamtionForCurrentObject(clips[selection], item.gameObject);
                }
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }
        Handles.EndGUI();
    }

    private void SetAniamtionForCurrentObject(AnimationClip clip, GameObject myTarget)
    {
        string relativePath = "";
        if (myTarget.transform.parent.GetComponent<Animator>() != null) relativePath = myTarget.name;
        else if (myTarget.transform.GetComponent<Animator>() == null) relativePath = myTarget.transform.parent.name + "/" + myTarget.name;
        SetCurve(clip, myTarget.transform.localPosition, relativePath, "m_AnchoredPosition");
        SetCurve(clip, myTarget.transform.localRotation.eulerAngles, relativePath, "m_AnchoredRotation");
    }

    private void SetCurve(AnimationClip clip, Vector2 v, string relativePath, string curveName)
    {
        AnimationCurve curve = AnimationCurve.Linear(0.0F, v.x, 2.0F, v.x);
        clip.SetCurve(relativePath, typeof(RectTransform), curveName + ".x", curve);
        curve = AnimationCurve.Linear(0.0F, v.y, 2.0F, v.y);
        clip.SetCurve(relativePath, typeof(RectTransform), curveName + ".y", curve);
    }

    // public override void OnInspectorGUI()
    // {
    //     DrawDefaultInspector();
    //     if (GUILayout.Button("Press Me", new GUILayoutOption[] { GUILayout.Width(50) }))
    //         Debug.Log("Got it to work.");
    // }
}