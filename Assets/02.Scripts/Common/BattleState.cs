using System.Collections;
using UnityEngine;
using ArrowClash.Common;
public abstract class BattleState
{
    protected TurnManager manager;
    public BattleState(TurnManager manager) => this.manager = manager;
    public virtual void Enter(){}
    public virtual void Update(){}
    public virtual void Exit(){}
}
