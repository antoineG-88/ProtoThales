using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public TweeningAnimator darkBackAnim;

    void Start()
    {
        darkBackAnim.canvasGroup = darkBackAnim.rectTransform.GetComponent<CanvasGroup>();
        UICard.darkBackAnim = darkBackAnim;
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
}
