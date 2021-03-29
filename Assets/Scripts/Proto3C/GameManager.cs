using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public TweeningAnimator darkBackAnim;
    public bool debugOnPc;
    public GameObject _winPanel;
    public static GameObject winPanel;
    public CameraController _cameraController;
    public SubmarineActionHandler _submarineActionHandler;
    public static bool useMouseControl;
    public static CameraController cameraController;
    public static SubmarineActionHandler submarineActionHandler;

    void Start()
    {
        winPanel = _winPanel;
        useMouseControl = debugOnPc;
        darkBackAnim.canvasGroup = darkBackAnim.rectTransform.GetComponent<CanvasGroup>();
        UICard.darkBackAnim = darkBackAnim;
        cameraController = _cameraController;
        submarineActionHandler = _submarineActionHandler;
    }

    void Update()
    {
        UICard.UpdateFocusCard();
        UICard.UpdateSelectedCard();
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public static void Win()
    {
        winPanel.SetActive(true);
    }
}
