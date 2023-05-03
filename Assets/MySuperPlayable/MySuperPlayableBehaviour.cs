using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MySuperPlayableBehaviour : PlayableBehaviour
{
    public ActorManager am;
    public float MyFloat;

    PlayableDirector pd;
    public override void OnPlayableCreate (Playable playable)
    { 
        
    }
    public override void OnGraphStart(Playable playable)
    {
        //�Ƶ�������߼�ȥִ�� ��Ϊ����timelineû������������
        //am.LockUnlockActorController(true);
    }
    public override void OnGraphStop(Playable playable)
    {
        //�Ƶ�������߼�ȥִ�� ��Ϊ����timelineû������������
        //am.LockUnlockActorController(false);       
    }
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        am.LockUnlockActorController(true);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        am.LockUnlockActorController(false);
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //base.OnBehaviourPlay();
    }

}
