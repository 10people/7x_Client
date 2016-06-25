using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class Huangyetearce : MonoBehaviour {

	public HuangYeTreasure m_HuangYeTreasure;

	public UISprite Treasure_Icon;
	UIFont mtitleFont;
	UIFont mbtn1Font;

	public UILabel Tre_Name;

	public UILabel Tre_Lv;

	public UILabel Tre_JinDu;

	public void Init()
	{
//		Debug.Log ("初始化藏宝点。。。。。。。。");
		int stas = m_HuangYeTreasure.isOpen; // 判断宝藏是否开启 0为未开启 1 为开启

		HuangyeTemplate mHY = HuangyeTemplate.getHuangyeTemplate_byid (m_HuangYeTreasure.guanQiaId);
		
		string mName = NameIdTemplate.GetName_By_NameId (mHY.nameId);

		Tre_Name.text = mName;

		HuangYePveTemplate mHuangyePve = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHY.id);

		Tre_Lv.text = mHuangyePve.lv.ToString(); ///  后台缺少等级的数据


		Tre_JinDu.text = "("+ m_HuangYeTreasure.jindu.ToString() + "%" + ")";
		if(stas == 0)  
		{
			Treasure_Icon.spriteName = "treasure_notOpen";
		}
		else{

			Treasure_Icon.spriteName = "treasure_open";
		}
	}
	private GameObject ZhengRongUI;

	public void openTreasure()
	{
		int stas = m_HuangYeTreasure.isOpen; // 判断宝藏是否开启 0为未开启 1 为开启
		//int Open_stas = m_HuangYeTreasure.status;// 判断宝藏是否有人在攻打 0为正常 1 为有人在攻打


	//	Debug.Log("stas        "+stas);
		//Debug.Log("Open_stas        "+Open_stas);
		if(stas == 0) // 未开启 如果是盟主则提示是否开启 如果是普通玩家就显示未开启
		{
			int identity = AllianceData.Instance.g_UnionInfo.identity; // 身份
			//Debug.Log("identity        "+identity);
			if(identity == 2||identity == 1)
			{
				//盟主
			//	Debug.Log("盟主        ");
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loodBoxBack);

				//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.HUANGYE_ZHENGROONG ),LoadBack);
			}
			else
			{
				//玩家
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBackNotOpen);
			}

		}
		else{
			// 弹出提示UI  有人正在挑战 请稍候在来攻打
//			if(Open_stas == 1)
//			{
//				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loodBoxBack11);
//			}
//			else{

				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HYTREASURE_UI), LoadBack);

			//}
		}
	}
	void LoadBackNotOpen( ref WWW p_www,string p_path, Object p_object)
	{
		string titleStr =  LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_9);
		
		string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_10);
		
		string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		
		string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		//uibox.setBox(titleStr, null, str1, null, strbtn,  null, null, null, null);

		uibox.setBox(titleStr,null, str1,null,strbtn,null,null,null,null);
	}
	void LoadBack( ref WWW p_www,string p_path, Object p_object )
	{
		if(ZhengRongUI != null)
		{
			return;
		}
		ZhengRongUI = Instantiate(p_object) as GameObject;
		
		ZhengRongUI.transform.parent = GameObject.Find("WildnessSurvival(Clone)").transform;
		
		ZhengRongUI.transform.localPosition = new Vector3(0,0,0);
		
		ZhengRongUI.transform.localScale = new Vector3(1,1,1);
		
		HYRetearceEnemy m_HuangyeZhengRong = ZhengRongUI.GetComponent<HYRetearceEnemy>();
		
		m_HuangyeZhengRong.mHuangYeTreasure = m_HuangYeTreasure;

		m_HuangyeZhengRong.Init();
	}
//	void loodBoxBack11(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//		uibox.setBox("稍后攻打",null, "现在有人正在挑战请稍后再来攻打",null,"确定",null,null,null,null);
//	}

	void loodBoxBack(ref WWW p_www,string p_path, Object p_object)
	{
	//	Debug.Log("盟主22222        ");
		HuangYePveTemplate mhuangyeTemp = HuangYePveTemplate.getHuangYePveTemplatee_byid( m_HuangYeTreasure.guanQiaId);
		string str1 = "是否要花费"+mhuangyeTemp.openCost.ToString()+"建设值开启宝藏?";
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox("宝藏开启",null, str1,null,CancleBtn,confirmStr,IsOpenmHuangYeTreasure,null,null);
	}
	void IsOpenmHuangYeTreasure(int i)
	{
		//Debug.Log (" i = "+i );
		if(i == 2)
		{
			int builddata  = AllianceData.Instance.g_UnionInfo.build;
			HuangYePveTemplate mhuangyeTemp = HuangYePveTemplate.getHuangYePveTemplatee_byid( m_HuangYeTreasure.guanQiaId);
//			Debug.Log("mHuangYeTreasure.fileIdt"+m_HuangYeTreasure.fileId);
//			Debug.Log("AllianceData.Instance.g_UnionInfo.build"+AllianceData.Instance.g_UnionInfo.build);
//			Debug.Log("mhuangyeTemp.openCost"+mhuangyeTemp.openCost);
			if(builddata >= mhuangyeTemp.openCost)
			{
				OpenHuangYeTreasure OpenTrea = new OpenHuangYeTreasure ();
				
				MemoryStream OpenTreaStream = new MemoryStream ();
				
				QiXiongSerializer OpenTreaer = new QiXiongSerializer ();
				
				OpenTrea.id = m_HuangYeTreasure.id;
				
				OpenTreaer.Serialize (OpenTreaStream,OpenTrea);
				
				byte[] t_protof;
				t_protof = OpenTreaStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_OPEN_TREASURE,ref t_protof);
			}
			else{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadMibaoUpBack);
			}
		}
	}

	void LoadMibaoUpBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		uibox.setBox("建设值不足",null, "你的建设值不足了 无法开启",null,"确定",null,null,null,null);
	}
}
