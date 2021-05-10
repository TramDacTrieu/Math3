using UnityEngine;
using UnityEngine.Playables;
using System;
[Serializable]
public class RepeatControlBehaviour : PlayableBehaviour
{
    [SerializeField]
    int repeatTimes;
    int counter;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        PlayableDirector director = playerData as PlayableDirector;
        if (director == null)
        {
            Debug.LogError("Director reference is missing. Please appont director to the track");
            return;
        }
        if (playable.GetTime() >= playable.GetDuration() - 0.1f)
        {
            if (repeatTimes > 0)
            {
                counter++;
                if (repeatTimes > counter)
                    director.time -= playable.GetDuration();
            }
            else
            {
                director.time -= playable.GetDuration();
            }
        }
    }

}