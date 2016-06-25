using UnityEngine;
using System.Collections;

public class AlliancePlatesScaleManagerment : MonoBehaviour
{
 
    public int m_designed_w = 960;

    public int m_designed_h = 640;
 
    void Start()
    {
        ScaleBg();
    }


    private void ScaleBg()
    {
        float t_s = ScreenHelper.GetBGScale(m_designed_w, m_designed_h);
  
         transform.localScale = new Vector3(((m_designed_w * t_s) + 6)/ m_designed_w, ((m_designed_h * t_s) + 4)/ m_designed_h, 1);
    }

}
