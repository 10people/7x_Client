using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShowMiBaoReward : MonoBehaviour {

	public Award miBaoRewardInfo;//奖励信息
	public int tanBaoType;//探宝类型
	
	public UITexture miBaoTex;//秘宝图标
	
	public UISprite border;//品质边框
	
	public GameObject star;//星级
	
	public UILabel miBaoName;//秘宝名字

	public GameObject miBaoObj;//秘宝卡obj

	public GameObject pieceWindow;//转变碎片的obj

//	public GameObject singleObj;
//	public GameObject multipObj;

	private float time = 0.5f;//缩放时间
	
	public void ShowMiBao ()
	{
//		MiBaoXmlTemp mbXml = MiBaoXmlTemp.getMiBaoXmlTempById(miBaoRewardInfo.itemId);
		
//		int iconId = mbXml.icon;
//		
//		int nameId = mbXml.nameId;
//		
//		int pinZhiId = mbXml.pinzhi;
		//		Debug.Log ("pinzhi:" + pinZhiId);

		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (miBaoRewardInfo.itemId);
		int iconId = commonTemp.icon;
		int nameId = commonTemp.nameId;
		int pinZhiId = commonTemp.color;

		miBaoTex.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON)
		                                                + iconId.ToString ());

		ShowBorder (pinZhiId,border);
		
		//		float x = star.transform.localPosition.x;
		
		float y = star.transform.localPosition.y;
		
		float z = star.transform.localPosition.z;
		
		//		Vector3 pos = new Vector3 (x,y,z);
		
		for (int i = 0;i < miBaoRewardInfo.itemStar;i ++){
			
			GameObject m_star = (GameObject)Instantiate (star);
			
			m_star.SetActive (true);
			
			m_star.transform.parent = star.transform.parent;
			
			m_star.transform.localPosition = new Vector3(i * 35 - (miBaoRewardInfo.itemStar - 1) * 17.5f,y,z);
			
			m_star.transform.localScale = star.transform.localScale;
		}
		
		miBaoName.text = NameIdTemplate.GetName_By_NameId (nameId);

		CardAnim ();
	}
	
	//显示边框品质
	void ShowBorder (int pinZhi,UISprite border)
	{
		border.spriteName = "pinzhi" + (pinZhi - 1);
	}

	//秘宝卡片动画
	void CardAnim ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",time);
		scale.Add ("scale",Vector3.one);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("oncomplete","ScaleEnd");
		scale.Add ("oncompletetarget",gameObject);
		iTween.ScaleTo(miBaoObj,scale);
	}
	
	//缩放结束
	void ScaleEnd ()
	{
		UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,miBaoObj,EffectIdTemplate.GetPathByeffectId(100157));
		StartCoroutine (EffectWait ());
		miBaoObj.GetComponent<BoxCollider> ().enabled = true;

		if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress < 0 && FirstTanBao.Instance.GetTanBaoState2 ())
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100040];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			Debug.Log ("探宝引导:" + tempTaskData.m_iCurIndex);
		}
	}

	IEnumerator EffectWait ()
	{
		yield return new WaitForSeconds (0f);
		UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,miBaoObj,EffectIdTemplate.GetPathByeffectId(100148));
	}

	//如果拥有此秘宝则转为秘宝碎片,如果没有则点击销毁奖励窗口
	public void ClickMiBao ()
	{
		//		Debug.Log ("clickit");
		miBaoObj.GetComponent<BoxCollider> ().enabled = false;
		
		if (miBaoRewardInfo.pieceNumber > 0)
		{
			Turn (miBaoRewardInfo);

			Destroy (miBaoObj);
		}

		else
		{
			if (tanBaoType == 10 || tanBaoType == 12)
			{
				MultipleReward miltipReward = GameObject.Find ("MultipleReward(Clone)").GetComponent<MultipleReward> ();
				miltipReward.MiBaoIndex ++;
				miltipReward.CheckExitMiBao ();
				miltipReward.SetClickOver = false;
			}
			else
			{
				if (tanBaoType == 1)
				{
					if(UIYindao.m_UIYindao.m_isOpenYindao)
					{
//						CityGlobalData.m_isRightGuide = true;
						if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress < 0 && FirstTanBao.Instance.GetTanBaoState2 ())
						{
							ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100040];
							UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
							Debug.Log ("探宝引导:" + tempTaskData.m_iCurIndex);
						}
					}
				}
				GameObject singleObj = GameObject.Find ("SingleReward(Clone)");
				SingleReward singleReward = singleObj.GetComponent<SingleReward> ();
				singleReward.ZheZhaoControl (false);

				List<RewardData> dataList = new List<RewardData>();
				RewardData data1;
				data1 = new RewardData(miBaoRewardInfo.itemId,miBaoRewardInfo.itemNumber);
				dataList.Add (data1);
				
				RewardData data2;
				data2 = new RewardData(tanBaoType == 0 ? 920001 : 920002,1);
				dataList.Add (data2);
				
				GeneralRewardManager.Instance ().CreateReward (dataList);

				Destroy (singleObj);
			}

			Destroy (this.gameObject);
		}
	}



	//弹出碎片窗口
	void Turn (Award tempAward)
	{
		miBaoObj.SetActive (false);
		
		GameObject pWindow = (GameObject)Instantiate (pieceWindow);
		pWindow.SetActive (true);
		pWindow.transform.parent = pieceWindow.transform.parent;
		pWindow.transform.localPosition = pieceWindow.transform.localPosition;
		pWindow.transform.localScale = Vector3.one;
		
		PieceWindow piece = pWindow.GetComponent<PieceWindow> ();
		piece.GetAwardInfo (tempAward);
		piece.tanBaoType = tanBaoType;
	}
}
