using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Monster : UnitBase
{
    public override void Attack() { return; }

    private float betweenRatio = 0f;
    private Animator animator;

    private void Update()
    {
        if(movable)
            Moves();
    }

    private void Moves()
    {
        betweenRatio += Time.deltaTime * CurrentUnitData.moveSpeed;

        var curPos = CurrentPoint.position;
        var nextPos = GetNextPath(CurrentPoint).position;

        Move(curPos, nextPos, betweenRatio);

        if (curPos.x < nextPos.x) SetDirection(Direction.Right);
        if (curPos.x > nextPos.x) SetDirection(Direction.Left);

        if (betweenRatio >= 1f)
        {
            CurrentPoint = GetNextPath(CurrentPoint);
            betweenRatio = 0f;
        }
    }

    private Path GetNextPath(Path currentPath)
    {
        var currentIndex = movePath.FindIndex(x => x.Equals(currentPath));
        var nextIndex = currentIndex + 1 < movePath.Count ? currentIndex + 1 : 0;
        
        return movePath[nextIndex];
    }

    public override void InitUnit(List<Path> movePath, Path startPos, UnitData unitData)
    {
        base.InitUnit(movePath, startPos, unitData);
        
        if (animator == null) animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Live");
    }

    public override void Die()
    {
        if(animator == null) animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Die");
        base.Die();
        Invoke("DieAfter", 1f);
    }
}
