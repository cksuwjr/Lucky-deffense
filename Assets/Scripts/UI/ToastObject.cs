using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ToastType
{
    None,
    Bubble,
}

public class ToastObject : PoolObject
{
    private TextMeshProUGUI text;
    private GameObject body;
    private ToastType type;
    public float speed;

    private void Awake()
    {
        body = transform.GetChild(0).gameObject;
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(float value, string additionalString = "", ToastType toastType = ToastType.None)
    {
        text.text = $"{additionalString}{value:F0}";
        type = toastType;
        StartCoroutine("MoveUp");
    }

    IEnumerator MoveUp()
    {
        float time = 0;
        if (type == ToastType.Bubble)
        {
            LeanTween.scale(body, new Vector3(1.3f, 1.3f, 1.3f), 0.1f);
            LeanTween.scale(body, Vector3.one, 0.1f).setDelay(0.1f);
            YieldInstructionCache.WaitForSeconds(0.2f);
        }

        
        var originColor = text.color;
        var color = text.color;

        while (time < 0.4f)
        {
            time += Time.deltaTime;
            transform.position += Time.deltaTime * speed * Vector3.up;
            if (type == ToastType.Bubble)
            {
                color.a = 1 - (time / 0.4f);
                text.color = color;
            }
            yield return null;
        }
        text.color = originColor;
        ReturnToPool();
    }
}
