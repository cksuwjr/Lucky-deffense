using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageObject : PoolObject
{
    private TextMeshProUGUI text;
    public float speed;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(float value)
    {
        text.text = $"{value:F0}";
        StartCoroutine("MoveUp");
    }

    IEnumerator MoveUp()
    {
        float time = 0;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            transform.position += Time.deltaTime * speed * Vector3.up;
            yield return null;
        }
        ReturnToPool();
    }
}
