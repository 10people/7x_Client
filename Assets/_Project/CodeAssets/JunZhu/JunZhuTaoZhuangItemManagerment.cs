using UnityEngine;
using System.Collections;

public class JunZhuTaoZhuangItemManagerment : MonoBehaviour
{
    public UILabel m_labUp;
    public UILabel m_labDown;

    public void ShowInfo(JunZhuTaoZhuangEffect.TaoZHuang _taozhuang)
    {
        m_labUp.text = _taozhuang._Title;
        m_labDown.text = _taozhuang._Des;
    }
}
