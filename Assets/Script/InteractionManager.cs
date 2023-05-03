using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : IActorManager
{
    private CapsuleCollider interCol;

    public List<EventCasterManager> overlapEcasetms=new List<EventCasterManager>();
    private void Start()
    {
        interCol=GetComponent<CapsuleCollider>(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        //һ��������ܰ������������¼�
        var eventCasterManger=other.GetComponents<EventCasterManager>();
        foreach (var item in eventCasterManger)
        {
            if (!overlapEcasetms.Contains(item))
            {
                overlapEcasetms.Add(item);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var eventCasterManger = other.GetComponents<EventCasterManager>();
        foreach (var item in eventCasterManger)
        {
            if (overlapEcasetms.Contains(item))
            {
                overlapEcasetms.Remove(item);
            }
        }
    }
}
