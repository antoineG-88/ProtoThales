using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarPing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float disappearTimer;
    public float disappearTimerMax;
    public Color color;
    public float identificationDistance;
    public SpriteRenderer identitySpriteRenderer;
    public MadBehavior madBehavior;

    private bool isMadAbove;

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
        identitySpriteRenderer.color = new Color(1,1,1, color.a);
        if (disappearTimer >= disappearTimerMax)
        {
            Destroy(gameObject);
        }

        UpdateMadDetection();
    }


    float distance;

    private void UpdateMadDetection()
    {
        distance = Vector2.Distance(SeaCoord.Planify(transform.position), SeaCoord.Planify(madBehavior.transform.position));
        if(distance <identificationDistance)
        {
            identitySpriteRenderer.gameObject.SetActive(true);
        }
        else
        {
            identitySpriteRenderer.gameObject.SetActive(false);
        }
    }
}
