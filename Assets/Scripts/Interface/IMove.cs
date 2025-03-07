using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    public void SetMovable(bool tf);
    public void Move(Vector3 positionA, Vector3 positionB, float t);
}
