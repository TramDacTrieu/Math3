using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class RestrictionControlClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private RestrictionControlBehaviour template = new RestrictionControlBehaviour();
    private ScriptPlayable<RestrictionControlBehaviour> scriptPlayable;

    public ClipCaps clipCaps
    {
        get
        {
            return ClipCaps.None;
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        scriptPlayable = ScriptPlayable<RestrictionControlBehaviour>.Create(graph, template);
        return scriptPlayable;

    }

}
