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
    public SubmarineCounterMeasures submarineCounterMeasures;
    public enum UnderWaterType { Bio, Submarine, ShipWreck, Lure};
    public UnderWaterType type;
    public bool isIdentifiable;
    private bool identifiedFlag;
    private AudioSource source;
    public AudioClip identificationSound;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
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

        if(isIdentifiable)
        {
            UpdateMadDetection();
        }
    }


    float distance;

    private void UpdateMadDetection()
    {
        distance = Vector2.Distance(SeaCoord.Planify(transform.position), SeaCoord.Planify(madBehavior.transform.position));
        if(distance < identificationDistance)
        {
            if (!identifiedFlag)
            {
                identifiedFlag = true;
                source.PlayOneShot(identificationSound);
            }

            identitySpriteRenderer.gameObject.SetActive(true);
            if(type == UnderWaterType.Submarine)
            {
                GameManager.submarineActionHandler.RefreshIdentified();
            }
        }
        else
        {
            identifiedFlag = false;
            identitySpriteRenderer.gameObject.SetActive(false);
        }
    }
}
