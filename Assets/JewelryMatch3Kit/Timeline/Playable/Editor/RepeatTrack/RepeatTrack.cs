using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0, 1, 0)]
[TrackClipType(typeof(RepeatControlClip))]
[TrackBindingType(typeof(PlayableDirector))]
public class RepeatTrack : TrackAsset
{

}