using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class DuiHuanMind : MonoBehaviour {

	public UILabel Property;

	public UISprite FuwenIcon;

	public UILabel FuwenLevel;

	public UILabel Cost;

	[HideInInspector]public int M_Cost;

	public int awardid;

	public UISprite pinzhi;

	public UILabel FuWenname;

	int mcost;
	void Start () {
	
	}
	

	void Update () {
	
	}

	public void Init()
	{
		FuWenDuiHuanTemplate mFuWenDuiHuanTemplate = FuWenDuiHuanTemplate.GetFuWenDuiHuanTemplate_By_Id (awardid);
		FuWenTemplate mfuwentemp = FuWenTemplate.GetFuWenTemplateByFuWenId (awardid);
		Property.text = GetFuWenProperty (mfuwentemp.shuxing)+ "+" + mfuwentemp.shuxingValue.ToString ();
		Cost.text = mFuWenDuiHuanTemplate.cost.ToString ();
		mcost = mFuWenDuiHuanTemplate.cost;
		FuwenIcon.spriteName = awardid.ToString ();
		FuwenLevel.text = "lv" + mfuwentemp.fuwenLevel.ToString ();
		FuWenname.text = NameIdTemplate.GetName_By_NameId(mfuwentemp.name);
		pinzhi.spriteName = "pinzhi"+(mfuwentemp.color-1).ToString();
	}
	string GetFuWenProperty(int index)
	{
		string mstr = "";
		switch(index)
		{
		case 1:
			mstr = "攻击";
			break;
		case 2:
			mstr = "防御";
			break;
		case 3:
			mstr = "生命";
			break;
		case 4:
			mstr = "武器伤害加深";
			break;
		case 5:
			mstr = "武器伤害抵抗";
			break;
		case 6:
			mstr = "武器暴击加深";
			break;
		case 7:
			mstr = "武器暴击抵抗";
			break;
		case 8:
			mstr = "技能伤害加深";
			break;
		case 9:
			mstr = "技能伤害抵抗";
			break;
		case 10:
			mstr = "技能暴击加深";
			break;
		case 11:
			mstr = "技能暴击抵抗";
			break;
		default:
			break;
		}
		return mstr;
	}
	public void DuiHuanBtn()
	{
		int mSuipianNumber = M_Cost;

		if(mSuipianNumber < mcost)
		{
			string data = "甲片数量不足！";
			
			ClientMain.m_UITextManager.createText(data);
		}
		else
		{
			FuwenDuiHuan  mFuwenDuiHuan  = new FuwenDuiHuan  ();
			MemoryStream MiBaoinfoStream = new MemoryStream ();
			QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
			
			mFuwenDuiHuan.fuwenItemId = awardid;
			
			MiBaoinfoer.Serialize (MiBaoinfoStream,mFuwenDuiHuan);
			
			byte[] t_protof;
			t_protof = MiBaoinfoStream.ToArray();
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_DUI_HUAN,ref t_protof,ProtoIndexes.S_FUWEN_DUI_HUAN_RESP.ToString());

			Destroy (this.gameObject);
		}
	}
	public void Close()
	{
		Destroy (this.gameObject);
	}
}
