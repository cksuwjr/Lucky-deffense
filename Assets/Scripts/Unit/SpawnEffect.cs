using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : PoolObject
{
    private Vector3 targetPos;
    private Vector3 startPos;

    private float speed = 5f;

    public void Init(Vector3 startPosition, Vector3 destination)
    {
        var sPos = startPosition;
        var dPos = destination;
        sPos.z = 0;
        dPos.z = 0;
        transform.position = sPos;
        startPos = sPos;
        targetPos = dPos;
        StartCoroutine("StartEffect");
    }

    private IEnumerator StartEffect()
    {
        float time = 0;

        var y = targetPos.y + Random.Range(1.5f, 3f);
        Vector3 P1 = new Vector3(Random.Range(-2.5f, 2.5f), y, 0);
        Vector3 P2 = P1;

        while (time <= 1f)
        {
            transform.position = Bezier(startPos, P1, P2, targetPos, time);
            time += Time.deltaTime * speed;
            yield return null;
        }
        yield return YieldInstructionCache.WaitForSeconds(0.3f);
        ReturnToPool();
    }

    public Vector3 Bezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        var M0 = Vector3.Lerp(P0, P1, t);
        var M1 = Vector3.Lerp(P1, P2, t);
        var M2 = Vector3.Lerp(P2, P3, t);

        Vector3 B0 = Vector3.Lerp(M0, M1, t);
        Vector3 B1 = Vector3.Lerp(M1, M2, t);

        return Vector3.Lerp(B0, B1, t);
    }
}
