using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MissionSelect : MonoBehaviour
{
    public GameObject missionPanel;
    public Text missionTitleText;
    public Text missionDescriptionText;
    public float sphereCastRadius;
    public LayerMask missionMask;
    public TweeningAnimator missionAnim;
    public GlobeScript globeScript;

    private RaycastHit hit;
    private Ray ray;

    private void Start()
    {

        StartCoroutine(missionAnim.anim.PlayBackward(missionAnim.rectTransform, null, true));
    }

    void Update()
    {
        if(InputDuo.tapDown)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.SphereCast(ray, sphereCastRadius, out hit, 1000, missionMask))
            {
                MissionGlobe mission = hit.collider.GetComponent<MissionGlobe>();
                if(mission != null)
                {
                    globeScript.canTurn = false;
                    StartCoroutine(missionAnim.anim.Play(missionAnim.rectTransform, null));
                    missionTitleText.text = mission.title;
                    missionDescriptionText.text = mission.description;
                }
            }
        }
    }

    public void CloseMission()
    {
        globeScript.canTurn = true;
        StartCoroutine(missionAnim.anim.PlayBackward(missionAnim.rectTransform, null, true));
    }

    public void LoasScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
