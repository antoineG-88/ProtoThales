using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonobuoyBehavior : MonoBehaviour
{
    [Header("Effect")]
    public GameObject scanEffectPrefab;
    public GameObject warnScanEffectPrefab;
    public SpriteRenderer identifyImage;
    public float effectFrequency;
    private float timerBeforeEffect;

    [Header("Sonobuoy")]
    public float sonobuoyRange;
    public float sonobuoyIncreasedRange;
    private float distance;
    public float sonobuoyLifeTime;
    private float timeBeforeDestroy;

    [Header("Range")]
    public GameObject rangeDisplay;
    public GameObject effectDisplay;
    //public GameObject sonobuoyCollider;
    public SonobuoyEffectProperties noElementEffect;
    public SonobuoyEffectProperties elementDetectedEffect;
    public MeshRenderer sonoMeshRenderer;
    public Material sonobuoyRefMat;
    private Material ownMat;

    public List<GameObject> objectInsideRange = new List<GameObject>();


    public List<GameObject> objectsCanBeDetected;
    private Sprite[] objectsCanBeDetectedSprite;

    private bool flagObjectInsideRange;
    private bool madIsAboveSonobuoy;
    private float actualSonobuoyRange;

    public MadBehavior madScript;

    private void Start()
    {
        if (TerrainZoneHandler.GetCurrentZone(SeaCoord.Planify(transform.position), null).relief == TerrainZone.Relief.Flat)
        {
            actualSonobuoyRange = sonobuoyIncreasedRange;
        }
        else
        {
            actualSonobuoyRange = sonobuoyRange;
        }

        //madScript.sonobuoys.Add(gameObject);
        objectsCanBeDetected = madScript.objectsCanBeDetected;
        objectsCanBeDetectedSprite = madScript.objectsCanBeDetectedSprite;

        timeBeforeDestroy = sonobuoyLifeTime;
        timerBeforeEffect = effectFrequency;

        rangeDisplay.transform.localScale = new Vector2(rangeDisplay.transform.localScale.x * actualSonobuoyRange, rangeDisplay.transform.localScale.y * actualSonobuoyRange);
        effectDisplay.transform.localScale = new Vector2(effectDisplay.transform.localScale.x * actualSonobuoyRange, effectDisplay.transform.localScale.y * actualSonobuoyRange);
        ownMat = Instantiate(sonobuoyRefMat);
        noElementEffect.ApplyToMat(ownMat);
        sonoMeshRenderer.material = ownMat;
        //sonobuoyCollider.transform.localScale = new Vector2(sonobuoyRange * 2, sonobuoyRange * 2);
        identifyImage.sprite = null;

    }

    private void Update()
    {
        LifeTime();
        Scan();
        Effect();
    }

    private void LifeTime()
    {
        if (timeBeforeDestroy > 0)
        {
            timeBeforeDestroy -= Time.deltaTime;
        }
        else
        {
            madScript.sonobuoys.Remove(this);
            Destroy(gameObject);
        }
    }

    private void Scan()
    {
        bool atLeastOneObjectDetected = false;
        int idendityIndex = 0;

        for (int i = 0; i < objectsCanBeDetected.Count; i++)
        {
            distance = Vector2.Distance(SeaCoord.Planify(objectsCanBeDetected[i].transform.position), SeaCoord.Planify(transform.position));

            if (distance < actualSonobuoyRange)
            {
                if(objectsCanBeDetected[i].tag == "Submarine")
                {
                    if(GameManager.submarineCounterMeasures.submarineIsInvisible)
                    {
                        //Do no detect submarine
                    }
                    else
                    {
                        GameManager.submarineCounterMeasures.submarineDetectSonobuoy = true;

                        float distanceFromMad = Vector2.Distance(SeaCoord.Planify(madScript.transform.position), SeaCoord.Planify(transform.position));

                        if (distanceFromMad < actualSonobuoyRange)
                        {
                            objectsCanBeDetected[i].GetComponent<SubmarineCounterMeasures>().RefreshIdentified();
                        }

                        atLeastOneObjectDetected = true;
                        idendityIndex = i;

                        if (!objectInsideRange.Contains(objectsCanBeDetected[i]))
                        {
                            objectInsideRange.Add(objectsCanBeDetected[i]);
                        }
                    }
                }
                else
                {
                    atLeastOneObjectDetected = true;
                    idendityIndex = i;

                    if (!objectInsideRange.Contains(objectsCanBeDetected[i]))
                    {
                        objectInsideRange.Add(objectsCanBeDetected[i]);
                    }
                }
            }
            else if (objectInsideRange.Contains(objectsCanBeDetected[i]))
            {
                objectInsideRange.Remove(objectsCanBeDetected[i]);
            }

            /*if (objectsCanBeDetected[i].GetComponent<SubmarineCounterMeasures>() != null)
            {
                if (objectsCanBeDetected[i].GetComponent<SubmarineCounterMeasures>().submarineIsInvisible)
                {
                    //Do no detect submarine
                }
                else
                { 
                    if (distance < actualSonobuoyRange)
                    {
                        objectsCanBeDetected[i].GetComponent<SubmarineCounterMeasures>().submarineDetectSonobuoy = true;

                        float distanceFromMad = Vector2.Distance(SeaCoord.Planify(madScript.transform.position), SeaCoord.Planify(transform.position));

                        if (distanceFromMad < actualSonobuoyRange)
                        {
                            objectsCanBeDetected[i].GetComponent<SubmarineCounterMeasures>().RefreshIdentified();
                        }

                        atLeastOneObjectDetected = true;
                        idendityIndex = i;

                        if (!objectInsideRange.Contains(objectsCanBeDetected[i]))
                        {
                            objectInsideRange.Add(objectsCanBeDetected[i]);
                        }
                    }
                    else if (objectInsideRange.Contains(objectsCanBeDetected[i]))
                    {
                        objectInsideRange.Remove(objectsCanBeDetected[i]);
                    }
                    if (distance > actualSonobuoyRange)
                    {
                        objectsCanBeDetected[i].GetComponent<SubmarineCounterMeasures>().submarineDetectSonobuoy = false;
                    }
                }
            }
            else
            {
                if (distance < actualSonobuoyRange)
                {
                    atLeastOneObjectDetected = true;
                    idendityIndex = i;

                    if (!objectInsideRange.Contains(objectsCanBeDetected[i]))
                    {
                        objectInsideRange.Add(objectsCanBeDetected[i]);
                    }
                }
                else if (objectInsideRange.Contains(objectsCanBeDetected[i]))
                {
                    objectInsideRange.Remove(objectsCanBeDetected[i]);
                }
            }   */        
        }

        if (atLeastOneObjectDetected)
        {
            elementDetectedEffect.ApplyToMat(ownMat);
            //rangeSprite.color = sonobuoyElementDetect;

            MadAbove();
            if (madIsAboveSonobuoy)
            {
                //If many objects inside : show only submarine icon
                if (objectInsideRange.Contains(objectsCanBeDetected[0]))
                {
                    identifyImage.sprite = objectsCanBeDetectedSprite[0];
                }
                else
                {
                    if (idendityIndex >= objectsCanBeDetectedSprite.Length)
                    {
                        identifyImage.sprite = objectsCanBeDetectedSprite[0];
                    }
                    else
                    {
                        identifyImage.sprite = objectsCanBeDetectedSprite[idendityIndex];
                    }
                }               
            }
            else
            {
                identifyImage.sprite = null;
            }
        }
        else
        {
            identifyImage.sprite = null;
            noElementEffect.ApplyToMat(ownMat);
            //rangeSprite.color = sonobuoyNoElement;
        }
    }

    private void MadAbove()
    {
        float distanceFromMad = Vector2.Distance(SeaCoord.Planify(madScript.transform.position), SeaCoord.Planify(transform.position));

        if (distanceFromMad < actualSonobuoyRange)
        {
            madIsAboveSonobuoy = true;
        }
        else
        {
            madIsAboveSonobuoy = false;
        }
    }

    private void Effect()
    {
        if (timerBeforeEffect > 0)
        {
            timerBeforeEffect -= Time.deltaTime;
        }
        else
        {
            timerBeforeEffect = effectFrequency;

            if (distance < actualSonobuoyRange)
            {
                Instantiate(warnScanEffectPrefab, SeaCoord.GetFlatCoord(transform.position), Quaternion.identity);
            }
            else
            {
                Instantiate(scanEffectPrefab, SeaCoord.GetFlatCoord(transform.position), Quaternion.identity);
            }
        }
    }

    [System.Serializable]
    public class SonobuoyEffectProperties
    {
        public float largeurOndes;
        public float vitesseOndes;
        public float quantiteOndes;
        public float flouHaloInterieur;
        public float largeurHaloInterieur;
        public Color couleurOndes;

        public void ApplyToMat(Material material)
        {
            material.SetFloat("LargeurOnde", largeurOndes);
            material.SetFloat("VitesseOnde", vitesseOndes);
            material.SetFloat("QuantiteOndes", quantiteOndes);
            material.SetFloat("FlouInterieur", flouHaloInterieur);
            material.SetFloat("LargeurInterieur", largeurHaloInterieur);
            material.SetColor("ColorOnde", couleurOndes);
        }
    }
}
