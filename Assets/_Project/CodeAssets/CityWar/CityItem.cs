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

	public UILabel m_alliance;

	public UILabel m_targetLabel;
	public UISprite m_target;
	
	public void InItCityItem (CityInfo tempInfo)
	{
		M_CityInfo = tempInfo;

		JCZCityTemplate cityTemp = JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId);
		m_cityIcon.spriteName = cityTemp.icon == 2 ? "9" : "8";
		m_cityLevel.text = "Lv" + cityTemp.allianceLv.ToString ();
		m_cityState.spriteName = CityWarPage.m_instance.M_TimeLabelDic [M_CityInfo.cityState][2];
		m_cityName.text = NameIdTemplate.GetName_By_NameId (cityTemp.name);

		m_allianceFlag.spriteName = tempInfo.cityState == 0 ? "" : "flag_" + tempInfo.guojiaId;
		m_allianceIcon.spriteName = tempInfo.cityState == 0 ? "" : tempInfo.lmIconId.ToString ();

		m_alliance.text = tempInfo.cityState == 0 ? "" : MyColorData.getColorString (tempInfo.cityState == 1 ? 4 : 5,"<" + tempInfo.ocLmName + ">");

		m_target.gameObject.SetActive (tempInfo.cityState2 == 0 ? false : true);
//		Debug.Log ("m_cityName:" + m_cityName.text);
//		Debug.Log ("interval:" + CityWarPage.m_instance.CityResp.interval);
//		Debug.Log ("cityState2:" + tempInfo.cityState2);
//		Debug.Log (m_cityName.text + ":tempInfo.cityState2:" + tempInfo.cityState2 + "||CityWarPage.m_instance.CityResp.interval:" + CityWarPage.m_instance.CityResp.interval);
		switch (tempInfo.cityState2)
		{
		case 0://无状态
			m_target.spriteName = "";
			m_targetLabel.text = "";
			break;
		case 1://宣战
			switch (CityWarPage.m_instance.CityResp.interval)
			{
			case 0://宣战时段
				m_target.spriteName = "BlueArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,"[0dbce8]" + CityWarPage.m_instance.m_xuanZhan + "[-]");//宣战
				break;
//			case 1://揭晓时段
//				m_target.spriteName = "RedArrow";
//				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_attackBefore);//进攻备战
//				break;
//			case 2://战斗时段
//				m_target.spriteName = "RedArrow";
//				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_attack);//进攻
//				break;
			default://其它时段
				m_target.spriteName = "";
				m_targetLabel.text = "";
				break;
			}
			break;
		case 2://防守
			switch (CityWarPage.m_instance.CityResp.interval)
			{
			case 0://宣战时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_fangShouBefore);//防守备战
				break;
			case 1://揭晓时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_fangShouBefore);//防守备战
				break;
			case 2://战斗时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_fangShou);//防守
				break;
			case 3://其它时段
				m_target.spriteName = "";
				m_targetLabel.text = "";
				break;
			}
			break;
		case 3://进攻
			switch (CityWarPage.m_instance.CityResp.interval)
			{
			case 0://宣战时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_attackBefore);//进攻备战
				break;
			case 1://揭晓时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_attackBefore);//进攻备战
				break;
			case 2://战斗时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_attack);//进攻
				break;
			default://其它时段
				m_target.spriteName = "";
				m_targetLabel.text = "";
				break;
			}
			break;
		case 4://宣战失败
			switch (CityWarPage.m_instance.CityResp.interval)
			{
//			case 0://宣战时段
//				m_target.spriteName = "RedArrow";
//				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_xuanZhan);//宣战
//				break;
			case 1://揭晓时段
				m_target.spriteName = "RedArrow";
				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_xuanZhanFail);//宣战失败
				break;
//			case 2://战斗时段
//				m_target.spriteName = "RedArrow";
//				m_targetLabel.text =  MyColorData.getColorString (5,CityWarPage.m_instance.m_attack);//进攻
//				break;
			default://其它时段
				m_target.spriteName = "";
				m_targetLabel.text = "";
				break;
			}
			break;
		}

		CityTween ();
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
