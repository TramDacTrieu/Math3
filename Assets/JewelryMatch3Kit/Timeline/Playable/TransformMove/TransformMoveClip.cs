using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TransformMoveClip : PlayableAsset, ITimelineClipAsset
{
    public TransformMoveBehaviour template = new TransformMoveBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TransformMoveBehaviour>.Create(graph, template);
        TransformMoveBehaviour clone = playable.GetBehaviour();
        return playable;
    }
}
