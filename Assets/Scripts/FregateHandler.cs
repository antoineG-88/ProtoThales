using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FregateHandler : MonoBehaviour
{
    public float timeBeforeSonar;
    public float timeBeforeBigSonar;
    public int maxAvailable;
    public int maxOnSonar;
    public int maxOnVitesse;
    public float[] distanceSonarSteps = new float[4];
    public float chancesOfWrongInfo;
    [Space]
    public Text numberSonarText;
    public Text numberVitesseText;
    public List<GameObject> availablePictos;
    public Image sonarCharge;
    public GameObject littleSonarEffect;
    public GameObject bigSonarEffect;
    public DocumentHandler documentHandler;
    public SubmarineIA submarine;

    private FregateController fregateController;
    [HideInInspector] public int numberOnSonar;
    [HideInInspector] public int numberOnVitesse;
    [HideInInspector] public int numberAvailable;

    private float currentSonarCharge;

    void Start()
    {
        numberAvailable = 0;
        numberOnSonar = 0;
        numberOnVitesse = 2;
        fregateController = GetComponent<FregateController>();
    }

    void Update()
    {
        if (Statics.inMenu)
        {
            numberSonarText.text = numberOnSonar.ToString();
            numberVitesseText.text = numberOnVitesse.ToString();

            for (int i = 0; i < availablePictos.Count; i++)
            {
                if (i < numberAvailable)
                {
                    availablePictos[i].SetActive(true);
                }
                else
                {
                    availablePictos[i].SetActive(false);
                }
            }

            if(numberOnSonar == 0)
            {
                sonarCharge.fillAmount = 0;
            }
            else if(numberOnSonar == 1)
            {
                sonarCharge.fillAmount = currentSonarCharge / timeBeforeSonar;
            }
            else if (numberOnSonar == 2)
            {
                sonarCharge.fillAmount = currentSonarCharge / timeBeforeBigSonar;
            }
        }

        if(numberOnSonar > 0)
        {
            currentSonarCharge += Time.deltaTime;
            if(currentSonarCharge > (numberOnSonar > 1 ? timeBeforeBigSonar : timeBeforeSonar))
            {
                UseSonar(numberOnSonar > 1);
            }
        }

        if((Input.touchCount >= 5 && Input.GetTouch(4).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.L))
        {
            Instantiate(littleSonarEffect, submarine.transform.position, Quaternion.identity);
        }
    }


    private void UseSonar(bool isBig)
    {

        int distanceStep = 1;
        float distance = Vector3.Distance(transform.position, submarine.transform.position);
        bool isSubmarine = Random.Range(0f, 1f) > chancesOfWrongInfo && distance > 1;
        string direction = "unknown";
        if(isSubmarine)
        {
            for (int i = 0; i < distanceSonarSteps.Length; i++)
            {
                if (distance > distanceSonarSteps[i])
                {
                    distanceStep++;
                }
            }

            if (isBig)
            {
                float angle = Vector2.SignedAngle(fregateController.currentDirection, new Vector2(submarine.transform.position.x - transform.position.x, submarine.transform.position.z - transform.position.z));
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
                }

                documentHandler.GenerateSonarReport(distanceStep, 0, direction);
            }
            else
            {
                documentHandler.GenerateSonarReport(distanceStep, 0);
            }
        }
        else
        {
            if (isBig)
            {
                documentHandler.GenerateSonarReport(Random.Range(1,5), 1, Random.Range(0f, 1f) > 0.5f ? Random.Range(0f, 1f) > 0.5f ? "derrière" : "devant" : Random.Range(0f, 1f) > 0.5f ? "tribord" : "bâbord" );
            }
            else
            {
                documentHandler.GenerateSonarReport(Random.Range(1, 5), 1);
            }
        }

        submarine.WarnSubmarine(distance, isBig);
        Instantiate(isBig ? bigSonarEffect : littleSonarEffect, transform.position, Quaternion.identity);
        ChangeSonarNumber(-numberOnSonar);
    }

    public void ChangeSonarNumber(int numberChange)
    {
        if (numberAvailable - numberChange >= 0 && numberAvailable - numberChange <= maxAvailable)
        {
            numberAvailable -= numberChange;
            numberOnSonar += numberChange;
            currentSonarCharge = 0;
        }
    }


    public void ChangeVitesseNumber(int numberChange)
    {
        if (numberAvailable - numberChange >= 0 && numberAvailable - numberChange <= maxAvailable)
        {
            numberAvailable -= numberChange;
            numberOnVitesse += numberChange;
        }
    }


    #region usefull
    private Vector3 Planify(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private Vector3 Planify(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }
    #endregion
}
