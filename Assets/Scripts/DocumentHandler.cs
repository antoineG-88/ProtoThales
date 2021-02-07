using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DocumentHandler : MonoBehaviour
{
    public Image signalImage;
    public Text directionText;
    public Sprite[] submarineSignals;
    public Sprite[] whaleSignals;
    public Color foundColor;

    public void GenerateSonarReport(int distance, int type)
    {
        if (type == 0)
        {
            signalImage.sprite = submarineSignals[distance - 1];
        }
        else
        {
            signalImage.sprite = whaleSignals[distance - 1];
        }

        if(distance == 1)
        {
            signalImage.color = foundColor;
        }
        else
        {
            signalImage.color = Color.white;
        }

        directionText.text = "Direction : Unknown";
    }


    public void GenerateSonarReport(int distance, int type, string direction)
    {
        if (type == 0)
        {
            signalImage.sprite = submarineSignals[distance - 1];
        }
        else
        {
            signalImage.sprite = whaleSignals[distance - 1];
        }

        if (distance == 1)
        {
            signalImage.color = foundColor;
        }
        else
        {
            signalImage.color = Color.white;
        }

        directionText.text = "Direction : " + direction;
    }
}
