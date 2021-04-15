using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MissionSelect : MonoBehaviour
{
    public RectTransform missionIconPanel;
    public List<Mission> missions;
    public float openDist;
    public TweeningAnimator animator;

    public Text missionTitleText;
    public Text missionDescriptionText;
    public GlobeScript globeScript;
    private Camera mainCamera;
    public LayerMask globeMask;
    private bool atLeastOneOpened;
    private bool atLeastOneClicked;
    private void Start()
    {
        mainCamera = Camera.main;
        animator.canvasGroup = animator.rectTransform.GetComponent<CanvasGroup>();
        StartCoroutine(animator.anim.PlayBackward(animator.rectTransform, animator.canvasGroup, true));
    }

    void Update()
    {
        UpdateIcons();

        if(InputDuo.tapDown)
        {
            atLeastOneClicked = false;
            for (int i = 0; i < missions.Count; i++)
            {
                if(missions[i].isSelectable)
                {
                    if (Input.GetButton("LeftClick"))
                    {
                        if (Vector2.Distance(mainCamera.ScreenToViewportPoint(Input.mousePosition), missions[i].iconPos) < openDist)
                        {
                            OpenMission(missions[i]);
                        }
                    }
                    else
                    {
                        Touch touch = Input.GetTouch(0);
                        if (Vector2.Distance(mainCamera.ScreenToViewportPoint(touch.position), missions[i].iconPos) < openDist)
                        {
                            OpenMission(missions[i]);
                        }
                    }
                }
            }

            if (!atLeastOneClicked && atLeastOneOpened)
            {
                StartCoroutine(animator.anim.PlayBackward(animator.rectTransform,animator.canvasGroup, true));
                Invoke("Untercatable", 0.3f);
                atLeastOneOpened = false;
            }
        }
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < missions.Count; i++)
        {
            missions[i].iconPos = mainCamera.WorldToViewportPoint(missions[i].transform.position);

            missions[i].iconRectTransform.anchoredPosition = new Vector2((missions[i].iconPos.x - 0.5f) * missionIconPanel.sizeDelta.x,
                (missions[i].iconPos.y - 0.5f) * missionIconPanel.sizeDelta.y);

            Ray ray = new Ray(mainCamera.transform.position, missions[i].transform.position - mainCamera.transform.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, (mainCamera.transform.position - missions[i].transform.position).magnitude - 0.2f, globeMask);

            if(hit.collider != null)
            {
                missions[i].iconImage.color = new Color(1, 1, 1, 0);
                missions[i].isSelectable = false;
            }
            else
            {
                missions[i].iconImage.color = Color.white;
                missions[i].isSelectable = true;
            }
        }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void OpenMission(Mission mission)
    {
        atLeastOneClicked = true;
        atLeastOneOpened = true;
        missionTitleText.text = mission.title;
        missionDescriptionText.text = mission.description;
        StartCoroutine(animator.anim.Play(animator.rectTransform, animator.canvasGroup));
        animator.canvasGroup.interactable = true;
    }

    private void Untercatable()
    {
        animator.canvasGroup.interactable = false;
    }
}
