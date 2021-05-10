using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(TransformMoveClip))]
[TrackBindingType(typeof(Transform))]
public class TransformMoveTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TransformMoveMixerBehaviour>.Create (graph, inputCount);
    }
}
