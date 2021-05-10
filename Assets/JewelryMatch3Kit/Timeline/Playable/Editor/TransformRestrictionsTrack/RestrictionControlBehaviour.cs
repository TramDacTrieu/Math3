using UnityEngine;
using UnityEngine.Playables;
using System;
[Serializable]
public class RestrictionControlBehaviour : PlayableBehaviour
{
    [SerializeField]
    public Vector3Bool position;
    public Vector3Bool rotation;
    public Vector3Bool scale;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        PlayableDirector gameObject = playerData as PlayableDirector;
        if (rotation.z) gameObject.transform.rotation = Quaternion.identity;
    }

}

[Serializable]
public class Vector3Bool
{
    public bool x;
    public bool y;
    public bool z;
}