using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : IActorManager
{
    public PlayableDirector pd;
    [Header("Timeline Assets")]
    public TimelineAsset frontStab;
    public TimelineAsset openBox;
    public TimelineAsset leverUp;

    [Header("Assets Settings")]
    public ActorManager attacker;
    public ActorManager victim;

    public void PlayFrontStab(string timelineName, ActorManager acttacker, ActorManager victim)
    {
        if (pd.state == PlayState.Playing)
        {
            return;
        }
        if (timelineName == "frontStab")
        {
            pd.playableAsset = Instantiate(frontStab);          
            var timeline = (TimelineAsset)pd.playableAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                if(track.name=="Attacker Script")
                {
                    pd.SetGenericBinding(track,attacker);
                    foreach (var clip in track.GetClips())
                    {
                        var myclip = (MySuperPlayableClip)clip.asset;
                        //ar mybehav = myclip.template;
                        myclip.am.exposedName = Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, acttacker);
                    }
                }
                else if(track.name=="Victim Script")
                {
                    pd.SetGenericBinding(track,victim);
                    foreach (var clip in track.GetClips())
                    {
                        var myclip = (MySuperPlayableClip)clip.asset;
                        //var mybehav = myclip.template;
                        myclip.am.exposedName = Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);                       
                    }
                }
                else if(track.name == "Attack Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.anim);
                }
                else if (track.name == "Victim Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.anim);
                }
            }

            //先调用使得Timeline内MySuperPlayableClip—>CreatePlayable调用 然后让其为exposedName进行初始化
            pd.Evaluate();
            pd.Play();
        }
        else if(timelineName== "openBox")
        {
            pd.playableAsset = Instantiate(openBox);
            var timeline = (TimelineAsset)pd.playableAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        var myclip = (MySuperPlayableClip)clip.asset;
                        //ar mybehav = myclip.template;
                        myclip.am.exposedName = Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, acttacker);
                    }
                }
                else if (track.name == "Box Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        var myclip = (MySuperPlayableClip)clip.asset;
                        //var mybehav = myclip.template;
                        myclip.am.exposedName = Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.anim);
                }
                else if (track.name == "Box Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.anim);
                }
            }

            //先调用使得Timeline内MySuperPlayableClip—>CreatePlayable调用 然后让其为exposedName进行初始化
            pd.Evaluate();
            pd.Play();
        }
        else if (timelineName == "leverUp")
        {
            pd.playableAsset = Instantiate(leverUp);
            var timeline = (TimelineAsset)pd.playableAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        var myclip = (MySuperPlayableClip)clip.asset;
                        //ar mybehav = myclip.template;
                        myclip.am.exposedName = Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, acttacker);
                    }
                }
                else if (track.name == "Lever Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        var myclip = (MySuperPlayableClip)clip.asset;
                        //var mybehav = myclip.template;
                        myclip.am.exposedName = Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.anim);
                }
                else if (track.name == "Lever Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.anim);
                }
            }

            //先调用使得Timeline内MySuperPlayableClip—>CreatePlayable调用 然后让其为exposedName进行初始化
            pd.Evaluate();
            pd.Play();
        }
    }

    private void Start()
    {
        pd = GetComponent<PlayableDirector>();
        pd.playOnAwake = false;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && LayerMask.LayerToName(gameObject.layer) == "Player")
        {
            pd.Play();
        }
    }
}
