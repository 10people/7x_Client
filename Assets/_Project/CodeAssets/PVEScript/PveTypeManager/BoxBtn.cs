using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BoxBtn : MYNGUIPanel {
	public Level m_Lev;
	public int Star_Id;

	public UISprite icon;

	public int Itemid;
	
	//public Award mAwardinfo;
	
	public string PinZhi = "pinzhi";
	
	public UISprite PinZhikuang;

	public UISprite LingQuAready;

	public NGUILongPress EnergyDetailLongPress1;
	/// <summary>
	/// The is ling qu. 1未完成 2 可以领取 3 已经领取
	/// </summary>
	public int  IsLingQu; 
	public int awardNum;
	public UILabel Num;
	void Awake()
	{
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		
		string[] Awardlist = PveStarTemplate.GetAwardInfo (Star_Id);
//		Debug.Log ("Star_Id = "+Star_Id);
		int awardid = int.Parse(Awardlist[1]);
		ShowTip.showTip (awardid);
	}
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
//		Debug.Log ("IsLingQu = "+IsLingQu);
		if (IsLingQu == 1) 
		{
			LingQuAready.gameObject.SetActive (false);
			//this.gameObject.GetComponent<UIButton>().enabled = false;
		} else if (IsLingQu == 2) 
		{
			//播放特效
//			int effectid = 600154;
//			UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,this.gameObject,EffectIdTemplate.GetPathByeffectId(effectid));
			LingQuAready.gameObject.SetActive (false);
			//this.gameObject.GetComponent<UIButton>().enabled = false;

		}else if (IsLingQu == 3) 
		{
			LingQuAready.gameObject.SetActive (true);
			//this.gameObject.GetComponent<UIButton>().enabled = false;
		}
		string[] Awardlist = PveStarTemplate.GetAwardInfo (Star_Id);
	
		Itemid = int.Parse(Awardlist[1]);
		CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(Itemid);

		icon.spriteName = mItemTemp.icon.ToString();
		PinZhikuang.spriteName = mItemTemp.color !=0 ?PinZhi+(mItemTemp.color-1).ToString():"";

	    awardNum = int.Parse(Awardlist[2]);
		Num.text = awardNum.ToString ();
	}
	public void SendLingQu()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}

		MemoryStream t_tream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		GetPveStarAward award = new GetPveStarAward();
		
		award.s_starNum = Star_Id;
		
		award.guanQiaId = m_Lev.guanQiaId;
		if(CityGlobalData.PT_Or_CQ)
		{
			award.isChuanQi = false;
		}
		else{
			award.isChuanQi = true;
		}
		t_qx.Serialize(t_tream, award);
		
		byte[] t_protof;
		
		t_protof = t_tream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_STAR_REWARD_GET, ref t_protof);
		LingQuAready.gameObject.SetActive (true);
		this.gameObject.GetComponent<UIButton>().enabled = false;
		//UI3DEffectTool.ClearUIFx (this.gameObject);
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	
	#endregion
}
