﻿// -------------------------------------------------------------------------------
// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.
// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.
// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.
// -------------------------------------------------------------------------------

using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.Entity;
using GameFW.Entity.Driver;
using UnityEngine;
/// <summary>
/// 防御塔Agent
/// </summary>
public class DefenseTowerAgent : BuildingAgent
{
    #region 当前目标
    private FightAgent curTarget = null;

    public void _set_curTarget(FightAgent value)
    {
        curTarget = value;
    }
    public FightAgent _get_curTarget()
    {
        return curTarget;
    }

    #endregion

    #region 攻击当前敌人

    private bool isAttacking;
    /// <summary>
    /// 攻击当前的目标,每次攻击由这个AI发出攻击请求,确保攻击在各个终端的一致性
    /// </summary>
    /// <returns></returns>
    public behaviac.EBTStatus AttackCurEnemy()
    {
        if (curTarget == null)
        {
            isAttacking = false;
            MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)BuildingFightEvent.DefenseTowerIdle, Id));
            return behaviac.EBTStatus.BT_FAILURE;
        }
        if (curTarget.IsDead())
        {
            isAttacking = false;
            curTarget = null;
            MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)BuildingFightEvent.DefenseTowerIdle, Id));
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        if (!isAttacking)
        {
            isAttacking = true;
            Attack(curTarget);
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }

    /// <summary>
    /// 发送攻击消息
    /// </summary>
    /// <param name="target"></param>
    private void Attack(FightAgent target)
    {
        MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)BuildingFightEvent.DefenseTowerAtkCreq, curTarget.Id));
    }

    #endregion

    #region 寻找敌人
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float AtkRange { get; set; }
    /// <summary>
    /// 攻击速度
    /// </summary>
    public float AtkSpeed { get; set; }
    /// <summary>
    /// 找到视野里的敌人
    /// </summary>
    /// <returns></returns>
	public FightAgent FindEnemyInRange()
    {
        GameObject nearestEnemy = MgrCenter.EntityMgr.GetNearestEnemy(Id, transform.position, AtkRange, gameObject.layer);

        Debug.Log("enemy is null ? " + (nearestEnemy == null).ToString());

        if (nearestEnemy != null)
            Debug.Log(nearestEnemy.transform.position);

        return nearestEnemy == null ? null : nearestEnemy.GetComponent<FightAgent>();
    }
    #endregion

    #region 重载的初始化BT、初始化Agent

    /// <summary>
    /// 初始化BT
    /// </summary>
    /// <returns></returns>
    protected override bool InitPlayer()
    {
        bool bRet = this.btload("DefenseTowerBT");
        if (bRet)
        {
            this.btsetcurrent("DefenseTowerBT");
        }

        return bRet;
    }

    /// <summary>
    /// 初始化Agent
    /// </summary>
    public override void Initial()
    {
        base.Initial();
        DefenseTowerDriver defenseTowerDriver = gameObject.GetComponent<DefenseTowerDriver>();
        if (defenseTowerDriver == null)
            Debug.LogError("defense tower driver not exited.");
        else
        {
            this.Id = defenseTowerDriver.Id;
            this.AtkRange = defenseTowerDriver.AtkRange;
            this.AtkSpeed = defenseTowerDriver.AtkSpeed;
        }
    }
    #endregion
}

