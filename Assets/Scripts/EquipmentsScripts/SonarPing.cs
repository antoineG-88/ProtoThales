using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarPing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float disappearTimer;
    public float disappearTimerMax;
    public Color color;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        disappearTimer = 0;
    }

    private void Update()
    {
        disappearTimer += Time.deltaTime;

        color.a = Mathf.Lerp(disappearTimerMax, 0f, disappearTimer / disappearTimerMax);
        spriteRenderer.color = color;

        if (disappearTimer >= disappearTimerMax)
        {
            Destroy(gameObject);
        }
    }
}
