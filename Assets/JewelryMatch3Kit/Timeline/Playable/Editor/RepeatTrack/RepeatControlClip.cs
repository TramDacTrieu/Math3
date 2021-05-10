using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class RepeatControlClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private RepeatControlBehaviour template = new RepeatControlBehaviour();
    private ScriptPlayable<RepeatControlBehaviour> scriptPlayable;

    public ClipCaps clipCaps
    {
        get
        {
            return ClipCaps.None;
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        scriptPlayable = ScriptPlayable<RepeatControlBehaviour>.Create(graph, template);
        return scriptPlayable;

    }

}
