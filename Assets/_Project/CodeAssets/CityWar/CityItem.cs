using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityItem : MonoBehaviour {

	[HideInInspector]public CityInfo M_CityInfo;

	public UISprite m_cityIcon;
	public UISprite m_allianceFlag;
	public UISprite m_allianceIcon;
	public UISprite m_cityState;

	public UILabel m_cityName;
	public UILabel m_cityLevel;

	public UILabel m_targetLabel;
	public UISprite m_target;
	
	public void InItCityItem (CityInfo tempInfo)
	{
		M_CityInfo = tempInfo;

		JCZCityTemplate cityTemp = JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId);
		m_cityIcon.spriteName = cityTemp.icon == 2 ? "8" : "9";
		m_cityLevel.text = "Lv" + cityTemp.allianceLv.ToString ();
		m_cityState.spriteName = CityWarPage.m_instance.M_TimeLabelDic [M_CityInfo.cityState][3];
		m_cityName.text = NameIdTemplate.GetName_By_NameId (cityTemp.name);

		m_allianceFlag.spriteName = tempInfo.cityState == 0 ? "" : "flag_" + tempInfo.guojiaId;
		m_allianceIcon.spriteName = tempInfo.cityState == 0 ? "" : tempInfo.lmIconId.ToString ();

		m_target.gameObject.SetActive (tempInfo.cityState2 == 0 ? false : true);
//		Debug.Log ("m_cityName:" + m_cityName.text);
//		Debug.Log ("interval:" + CityWarPage.m_instance.CityResp.interval);
//		Debug.Log ("cityState2:" + tempInfo.cityState2);
		if (tempInfo.cityState2 == 1)
		{
			if (CityWarPage.m_instance.CityResp.interval == 0)
			{
				m_target.spriteName = "BlueArrow";
				m_targetLabel.text = "[0dbce8]宣战[-]";
			}
			else if (CityWarPage.m_instance.CityResp.interval == 1)
			{
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,"进攻");
			}
			else
			{
				m_target.spriteName = "";
				m_targetLabel.text = "";
			}

			CityTween ();
		}
		else if (tempInfo.cityState2 == 2 || tempInfo.cityState2 == 3 || tempInfo.cityState2 == 4)
		{
			switch (tempInfo.cityState2)
			{
			case 2:
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,"防守");
				break;
			case 3:
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,"进攻");
				break;
			case 4:
//				Debug.Log ("interval:" + CityWarPage.m_instance.CityResp.interval);
				m_target.spriteName = CityWarPage.m_instance.CityResp.interval == 1 ? "RedArrow" : "";
				m_targetLabel.text =  CityWarPage.m_instance.CityResp.interval == 1 ? MyColorData.getColorString (5,"宣战失败") : "";
				break;
			default:
				break;
			}

			CityTween ();
		}
		else
		{
			m_target.spriteName = "";
			m_targetLabel.text = "";
		}
	}

	private Vector3 m_pos;
	private bool m_up;

	void CityTween ()
	{
		m_target.transform.localPosition = new Vector3 (0,55,0);
		m_pos = m_target.transform.localPosition;
		m_up = true;

		JianTouTween ();
	}

	void JianTouTween ()
	{
		Hashtable move = new Hashtable ();
		move.Add ("position",m_up ? m_pos + new Vector3(0,15,0) : m_pos);
		move.Add ("time",1);
		move.Add ("easetype",iTween.EaseType.easeInOutQuad);
		move.Add ("oncomplete","JianTouTweenBack");
		move.Add ("oncompletetarget",gameObject);
		move.Add ("islocal",true);
		iTween.MoveTo (m_target.gameObject,move);
	}

	void JianTouTweenBack ()
	{
		if (m_up)
		{
			m_up = false;
		}
		else
		{
			m_up = true;
		}
		JianTouTween ();
	}
}
