using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FregateAction : BatimentAction
{
    public float captasMaxRange;
    public float captasDetectionSpeed;
    public float captasCooldown;
    public float landBetweenStepsLength;
    public SonarPing pingPrefab;
    public Material captasMat;
    public float captasEffectProgressionStart;
    public float captasEffectProgressionEnd;
    public GameObject captasEffectDisplay;
    public Image captasCooldownFill;

    public UICard captasCard;
    public UICard hullSonarCard;
    public TweeningAnimator hullSonarDescriptionAnim;

    private FregateMovement fregateMovement;
    private bool hullSonarDescriptionOpened;
    private float captasCooldownRemaining;

    public override void Start()
    {
        base.Start();
        fregateMovement = (FregateMovement)batimentMovement;
        hullSonarDescriptionAnim.canvasGroup = hullSonarDescriptionAnim.rectTransform.GetComponent<CanvasGroup>();
        captasEffectDisplay.transform.localScale = new Vector3(captasMaxRange, captasMaxRange, captasMaxRange);
        captasMat.SetFloat("Progression", 900);
        captasEffectDisplay.SetActive(false);
    }

    public override void Update()
    {
        HullSonarCardUpdate();
        CaptasUpdate();
    }

    public void HullSonarCardUpdate()
    {
        if (hullSonarCard.isHovered && !hullSonarDescriptionOpened)
        {
            hullSonarDescriptionOpened = true;
            StartCoroutine(hullSonarDescriptionAnim.anim.Play(hullSonarDescriptionAnim));
        }
        else if (!hullSonarCard.isHovered && hullSonarDescriptionOpened)
        {
            hullSonarDescriptionOpened = false;
            StartCoroutine(hullSonarDescriptionAnim.anim.PlayBackward(hullSonarDescriptionAnim, true));
        }
    }

    public void CaptasUpdate()
    {
        if((captasCard.isClicked || captasCard.isDropped) && captasCooldownRemaining <= 0)
        {
            StartCoroutine(UseCaptas());
        }

        if(captasCooldownRemaining > 0)
        {
            captasCooldownRemaining -= Time.deltaTime;
            captasCooldownFill.fillAmount = 1 - (captasCooldownRemaining / captasCooldown);
        }
        else
        {
            captasCooldownFill.fillAmount = 1;
        }

    }

    private IEnumerator UseCaptas()
    {
        captasCooldownRemaining = captasCooldown;
        float currentRangeReached = 0;
        float previousRangeReached = 0;
        Vector2 fregateOriginalPos = fregateMovement.currentPosition;
        List<GameObject> allMapImmergedObjects = new List<GameObject>();
        float distance;
        float ratio = 0;
        captasEffectDisplay.SetActive(true);
        captasEffectDisplay.transform.position = SeaCoord.GetFlatCoord(fregateOriginalPos) + Vector3.up * 0.1f;
        Collider[] colliders = Physics.OverlapSphere(SeaCoord.GetFlatCoord(fregateOriginalPos), 100);

        for (int i = 0; i < colliders.Length; i++)
        {
            allMapImmergedObjects.Add(colliders[i].gameObject);
        }
        while (currentRangeReached <= captasMaxRange)
        {
            previousRangeReached = currentRangeReached;
            currentRangeReached += captasDetectionSpeed * Time.fixedDeltaTime;
            ratio = currentRangeReached / captasMaxRange;
            captasMat.SetFloat("Progression", Mathf.Lerp(captasEffectProgressionStart, captasEffectProgressionEnd, ratio));

            for (int i = 0; i < allMapImmergedObjects.Count; i++)
            {
                distance = Vector2.Distance(fregateOriginalPos, SeaCoord.Planify(allMapImmergedObjects[i].transform.position));
                if (distance > previousRangeReached && distance <= currentRangeReached)
                {
                    if(!IsLandBetween(fregateOriginalPos, SeaCoord.Planify(allMapImmergedObjects[i].transform.position)))
                    {
                        Instantiate(pingPrefab, SeaCoord.GetFlatCoord(allMapImmergedObjects[i].transform.position) + Vector3.up * 0.1f, pingPrefab.transform.rotation);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        captasMat.SetFloat("Progression", 900);
        captasEffectDisplay.SetActive(false);

    }

    private bool IsLandBetween(Vector2 originPos, Vector2 targetPos)
    {
        bool landBetween = false;
        Vector2 targetDirection = targetPos - originPos;
        targetDirection.Normalize();
        float targetDistance = Vector2.Distance(originPos, targetPos);
        int steps = (int)(targetDistance / landBetweenStepsLength);
        for (int i = 0; i < steps; i++)
        {
            if (TerrainZoneHandler.GetCurrentZone(originPos + targetDirection * i * landBetweenStepsLength, null).relief == TerrainZone.Relief.Land)
            {
                landBetween = true;
                break;
            }
        }

        return landBetween;
    }
}
