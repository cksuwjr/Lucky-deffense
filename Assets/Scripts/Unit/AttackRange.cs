using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mathf = UnityEngine.Mathf;

public class AttackRange : MonoBehaviour
{
    private List<UnitBase> enemys = new List<UnitBase>();

    public void Init(float range)
    {
        //if (TryGetComponent<CircleCollider2D>(out CircleCollider2D col))
        //    col.radius = range;
        var radius = Mathf.Sqrt(Mathf.Pow(range, 2) + Mathf.Pow(range, 2));

        transform.localScale = Vector3.one * radius;
    }

    public UnitBase[] GetEnemys(int count)
    {
        // 가까움 정렬
        enemys.Sort(delegate (UnitBase a, UnitBase b)
        {
            var distanceA = Vector2.Distance(transform.position, a.transform.position);
            var distanceB = Vector2.Distance(transform.position, b.transform.position);

            return distanceA.CompareTo(distanceB);
        });

        int maxCount = count > enemys.Count ? enemys.Count : count;
        int nowCount = 0;
        UnitBase[] units = new UnitBase[maxCount];

        for (int i = 0; i < maxCount; i++)
        {
            if (enemys[i].dead) continue;
            units[nowCount++] = enemys[i];
        }

        return units;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<UnitBase>(out var unit))
        {
            if (unit.dead) return;
            
            if (collision.CompareTag("Player")) return;
            enemys.Add(unit);
            unit.OnDespawned += RemoveFromList;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<UnitBase>(out var unit))
        {
            enemys.Remove(unit);
            unit.OnDespawned -= RemoveFromList;
        }
    }

    private void RemoveFromList(UnitBase unit)
    {
        enemys.Remove(unit);
    }

}
