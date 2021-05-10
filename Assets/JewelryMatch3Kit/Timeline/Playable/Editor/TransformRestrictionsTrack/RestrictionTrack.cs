using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0, 0, 1)]
[TrackClipType(typeof(RestrictionControlClip))]
[TrackBindingType(typeof(PlayableDirector))]
public class RestrictionTrack : TrackAsset
{

}