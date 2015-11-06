using UnityEngine;
using System.Collections;

public class EquipEnhance_EquipAttBar : MonoBehaviour {

	public UISlider m_sld_bar;

	public UILabel m_lb_att;

	public UISprite m_spt_lock;


	public void SetLock( bool p_lock ){
		if( p_lock ){
			m_spt_lock.spriteName = "spt_lock";
		}
		else{
			m_spt_lock.spriteName = "spt_unlock";
		}
	}

    public void ShowInfo(int tempNum)
    {
        m_sld_bar.value = 0.0f;//tempSliderValue;

        m_lb_att.text = tempNum.ToString();
    }
}
