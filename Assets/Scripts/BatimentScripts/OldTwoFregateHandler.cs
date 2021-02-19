using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OldTwoFregateHandler : MonoBehaviour
{
    public int maxUnitAvailable;
    public float deepSonarChargeTime;
    public float[] deepSonarDistanceSteps;
    public Sprite[] deepSonarDistanceStepImages;
    public GameObject sonarEffectPrefab;
    public Image deepSonarCharge;
    public Text unitsControlNumberText;
    public Text unitsDeepSonarNumberText;
    public Text unitsHullSonarNumberText;
    public Text unitsAvailableNumberText;
    public Image hullSonarImage;
    public Color hullSonarActivatedColor;
    public Transform submarine;
    public PinHandler pinHandler;

    [HideInInspector] public bool isUsingHullSonar;
    private int unitsAvailable;
    private int unitEngagedOnControl;
    private int unitEngagedOnHullSonar;
    private int unitEngagedOnDeepSonar;
    private float currentSonarCharge;
    private BatimentController batimentController;
    private Color hullSonarBaseColor;
    private Fregate fregate;

    [Header("HullSonar")]
    public float maxDetectionDistanceSubmarine;
    public Color colorFar;
    public Color colorClose;
    [Space]
    public GameObject colorFeedback;

    private float submarineDistance;
    

    void Start()
    {
        fregate = GetComponent<Fregate>();
        unitEngagedOnDeepSonar = 0;
        unitEngagedOnControl = 0;
        unitEngagedOnHullSonar = 0;
        unitsAvailable = maxUnitAvailable;
        hullSonarBaseColor = hullSonarImage.color;
        colorFeedback.SetActive(false);
    }

    void Update()
    {
        //fregate.unitsOnControl = unitEngagedOnControl;

        isUsingHullSonar = unitEngagedOnHullSonar >= 1;

        unitsHullSonarNumberText.text = unitEngagedOnHullSonar.ToString();
        unitsControlNumberText.text = unitEngagedOnControl.ToString();
        unitsDeepSonarNumberText.text = unitEngagedOnDeepSonar.ToString();
        unitsAvailableNumberText.text = unitsAvailable.ToString();

        if (unitEngagedOnDeepSonar == 0)
        {
            deepSonarCharge.fillAmount = 0;
        }
        else if (unitEngagedOnDeepSonar == 1)
        {
            currentSonarCharge += Time.deltaTime;
            if (currentSonarCharge > deepSonarChargeTime)
            {
                UseSonar();
            }
            deepSonarCharge.fillAmount = currentSonarCharge / deepSonarChargeTime;
        }

        submarineDistance = Vector2.Distance(fregate.currentPosition, SeaCoord.Planify(submarine.position));

        if (submarineDistance <= maxDetectionDistanceSubmarine && unitEngagedOnHullSonar == 1)
        {
            ChangeColorOverDistance();
        }
        else
        {
            colorFeedback.SetActive(false);
        }
    }

    private void ChangeColorOverDistance()
    {
        colorFeedback.SetActive(true);

        colorFeedback.transform.position = new Vector3(fregate.transform.position.x + 2, fregate.transform.position.y + 2, fregate.transform.position.z + 2);

        colorFeedback.GetComponent<SpriteRenderer>().color = Color.Lerp(colorClose, colorFar, submarineDistance / maxDetectionDistanceSubmarine);
    }

    private void UseSonar()
    {
        int distanceStep = 1;
        float distance = Vector3.Distance(transform.position, submarine.transform.position);
        /*string direction = "unknown";

        for (int i = 0; i < deepSonarDistanceSteps.Length; i++)
        {
            if (distance > deepSonarDistanceSteps[i])
            {
                distanceStep++;
            }
        }

        float angle = Vector2.SignedAngle(fregate.currentDirection, SeaCoord.Planify(submarine.position - fregate.transform.position));
        if (Mathf.Abs(angle) >= 135)
        {
            direction = "derrière";
        }
        if (Mathf.Abs(angle) < 45)
        {
            direction = "devant";
        }
        if (Mathf.Abs(angle) >= 45 && Mathf.Abs(angle) < 135)
        {
            if (angle > 0)
            {
                direction = "bâbord";
            }
            else
            {
                direction = "tribord";
            }
        }*/

        pinHandler.CreateDeepSonarPin(distanceStep, fregate.currentPosition);
        Instantiate(sonarEffectPrefab, fregate.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        unitsAvailable++;
        unitEngagedOnDeepSonar--;
        currentSonarCharge = 0;
    }


    public void AddControl(int value)
    {
        if (unitsAvailable - value >= 0 && unitsAvailable - value <= maxUnitAvailable && unitEngagedOnControl + value >= 0)
        {
            unitsAvailable -= value;
            unitEngagedOnControl += value;
        }
    }

    public void ActivateHullSonar()
    {
        if (unitsAvailable > 0 && unitEngagedOnHullSonar == 0)
        {
            unitsAvailable--;
            unitEngagedOnHullSonar++;
            hullSonarImage.color = hullSonarActivatedColor;
        }
        else if (unitEngagedOnHullSonar == 1)
        {
            unitsAvailable++;
            unitEngagedOnHullSonar--;
            hullSonarImage.color = hullSonarBaseColor;
        }
    }

    public void AddDeepSonar(int value)
    {
        if (unitsAvailable > 0 && unitEngagedOnDeepSonar == 0)
        {
            unitsAvailable -= value;
            unitEngagedOnDeepSonar += value;
        }
    }
}
