using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManager
{
    private CapsuleCollider defCol;
    private void Start()
    {
        defCol=GetComponent<CapsuleCollider>();
        defCol.center = Vector3.up;
        defCol.height = 2.0f;
        defCol.radius = 0.5f;
        defCol.isTrigger= true;
    }
    private void OnTriggerEnter(Collider other)
    {
        var targetWc = other.GetComponentInParent<WeaponController>();

        if (targetWc == null)
        {
            return;
        }

        //拿到模型去比较面向 而不是摄像机
        var attacker = targetWc.wm.am.ac.playerPrefab;
        var receiver = am.ac.playerPrefab;
       

        //var attackDir = receiver.transform.position - attacker.transform.position;
        ////攻击者和被攻击者之间的夹角
        //var attackAngle = Vector3.Angle(attackDir,attacker.transform.forward);

        //var counterDir=attacker.transform.position-receiver.transform.position;
        ////被攻击者和攻击者之间的夹角
        //var counterAngle = Vector3.Angle(counterDir,receiver.transform.forward);
        ////二者是否是面面相对的 这个值应该接近180度
        //var conterAngle2 = Vector3.Angle(attacker.transform.forward, receiver.transform.forward);

        //var attackValid = attackAngle <= 45;
        //var counterValid = counterAngle < 30 && Mathf.Abs(conterAngle2 - 180) < 30;

        //只有标签是武器的 物体碰撞才会造成damage
        //if (other.CompareTag("Weapon"))
        //{
            am.TryDoDmage(targetWc, CheckAngleTarget(receiver,attacker,45), CheckAnglePlayer(receiver,attacker,30));
        //}
    }
    public static bool CheckAnglePlayer(GameObject player,GameObject target,float playerAngleLimit)
    {
        var counterDir = target.transform.position - player.transform.position;
        //被攻击者和攻击者之间的夹角
        var counterAngle = Vector3.Angle(counterDir, player.transform.forward);
        //二者是否是面面相对的 这个值应该接近180度
        var conterAngle2 = Vector3.Angle(target.transform.forward, player.transform.forward);

        var counterValid = counterAngle < playerAngleLimit && Mathf.Abs(conterAngle2 - 180) < playerAngleLimit;

        return counterValid;
    }

    public static bool CheckAngleTarget(GameObject player, GameObject target,float targetAngleLimit)
    {
        var attackDir = player.transform.position - target.transform.position;
        //攻击者和被攻击者之间的夹角
        var attackAngle = Vector3.Angle(attackDir, target.transform.forward);
        var attackValid = attackAngle <= targetAngleLimit;
        return attackValid;
    }
}
