using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

public class BubbleController : MonoBehaviour
{
    public UILabel BubbleLabel;

    public float bubbleGapTime;
    public float bubbleExistTime;
    public float bubbleDistance;

    public List<string> bubbleStrList = new List<string>();

    public void ShowBubble()
    {
        System.Random temp = new Random();
        BubbleLabel.text = bubbleStrList[temp.Next(0, bubbleStrList.Count - 1)];
        BubbleLabel.gameObject.SetActive(true);

        if (TimeHelper.Instance.IsTimeCalcKeyExist("Bubble"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("Bubble");
        }
        TimeHelper.Instance.AddOneDelegateToTimeCalc("Bubble", bubbleExistTime, HideBubble);
    }

    public void HideBubble()
    {
        BubbleLabel.gameObject.SetActive(false);

        TimeHelper.Instance.RemoveFromTimeCalc("Bubble");
    }
}