using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TransformTweenClip : PlayableAsset, ITimelineClipAsset
{
    public TransformTweenBehaviour template = new TransformTweenBehaviour();
    public ExposedReference<Transform> startLocation;
    public ExposedReference<Transform> endLocation;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TransformTweenBehaviour>.Create(graph, template);
        TransformTweenBehaviour clone = playable.GetBehaviour();
        var parameters = owner.GetComponent<TransformTweenParameters>() == null ? owner.AddComponent<TransformTweenParameters>() : owner.GetComponent<TransformTweenParameters>();
        clone.startLocation = parameters.startTransform;
        clone.endLocation = parameters.endTransform;
        // clone.startLocation = startLocation.Resolve(graph.GetResolver());
        // clone.endLocation = endLocation.Resolve(graph.GetResolver());
        return playable;
    }
}