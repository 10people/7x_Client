using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityUITL : MonoBehaviour ,SocketListener {

	public UISprite m_UISpriteHeroIcon;
	
	public UILabel m_playerName;
	public UILabel m_playrLevel;
	public UISprite m_playrVip;
	
	public UILabel m_leagueName;

	public UISprite NationSprite;
	public UISprite m_SpriteExp;
	
	public UILabel m_ZhanLiLabel;

	private bool m_isLianmeng = true;

	void Awake ()
	{
		SocketTool.RegisterSocketListener (this);
	}

	void Start ()
	{
		RefreshJunZhuInfo ();
		RefreshAllianceInfo ();
	}

	/// <summary>
	/// Refreshs the jun zhu info.
	/// </summary>
	public void RefreshJunZhuInfo ()
	{
		//player info
		m_UISpriteHeroIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
		m_playerName.text = JunZhuData.Instance().m_junzhuInfo.name;
		m_playrLevel.text = JunZhuData.Instance().m_junzhuInfo.level.ToString();
		m_playrVip.spriteName = "v" + JunZhuData.Instance().m_junzhuInfo.vipLv.ToString();
		NationSprite.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
		m_SpriteExp.fillAmount = (float)JunZhuData.Instance().m_junzhuInfo.exp / (float)JunZhuData.Instance().m_junzhuInfo.expMax;
		//m_SpriteExp
		m_ZhanLiLabel.text = JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString();
	}

	public void RefreshAllianceInfo()
	{
//		Debug.Log ("AllianceData.Instance.IsAllianceNotExist:" + AllianceData.Instance.IsAllianceNotExist);
		//alliance exist
		if (!AllianceData.Instance.IsAllianceNotExist)
		{
			m_isLianmeng = true;

//			Debug.Log ("AllianceData.Instance.g_UnionInfo.name:" + AllianceData.Instance.g_UnionInfo.name);

			//try set alliance name
			if (AllianceData.Instance.g_UnionInfo != null && AllianceData.Instance.g_UnionInfo.name != null)
			{
				m_leagueName.text = "<" + AllianceData.Instance.g_UnionInfo.name + ">";
			}
		}
		else
		{
//			Debug.Log ("无联盟");
			m_isLianmeng = false;
			
			m_leagueName.text = "无联盟";
		}
	}

	void TopLeftHandlerClickBack (GameObject obj)
	{
		if (TreasureCityUI.IsWindowsExist ())
		{
			return;
		}
		switch (obj.name)
		{
		case "LT_Button_HeroHead":

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_LAYER_AMEND),
			                        JunzhuLayerLoadCallback);

			break;
		case "LT_VIPButton":

			EquipSuoData.TopUpLayerTip (null,false,1,null,true);

			break;
		default:
			break;
		}
	}

	private void JunzhuLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		TreasureCityUI.TryAddToObjectList(tempObject);
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message == null)
		{
			return false;
		}
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.JunZhuInfoRet:
		{
			Debug.Log ("RefreshJunZhuData:" + ProtoIndexes.JunZhuInfoRet);		
			JunZhuInfoRet tempInfo = new JunZhuInfoRet();
			tempInfo = QXComData.ReceiveQxProtoMessage (p_message,tempInfo) as JunZhuInfoRet;

		//	JunZhuData.Instance().m_junzhuInfo = tempInfo;
			Debug.Log ("tempInfo:" + tempInfo.lianMengId);
            JunZhuData.Instance().SetInfo(tempInfo);
            RefreshJunZhuInfo();

			return true;
		}
		default:
			return false;
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterSocketListener (this);
	}
}
