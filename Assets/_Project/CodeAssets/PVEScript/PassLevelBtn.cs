using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PassLevelBtn : MonoBehaviour  , SocketListener{

	[HideInInspector]public int LingQu; // 0为不可领取 1为可领取 2为已经领取

	[HideInInspector] public int CurrSection;
	[HideInInspector]public List<int > Sec_idlist = new List<int>();

	public static PassLevelBtn mPaaseBtn;
	[HideInInspector]public bool IsOPenEffect;

	[HideInInspector]public bool mDataback;

	public static PassLevelBtn Instance()
	{
		if (!mPaaseBtn)
		{
			mPaaseBtn = (PassLevelBtn)GameObject.FindObjectOfType (typeof(PassLevelBtn));
		}
		
		return mPaaseBtn;
	}
	void Awake()
	{
		mDataback = false;
		SocketTool.RegisterSocketListener(this);
	}
	void OnDisable()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	void Start () 
	{
	//	Invoke ("Sendmassege",0.5f);

	}
	void OnEnable()
	{
		Sendmassege ();
	}
	void Sendmassege()
	{
		//Debug.Log("CNOT_GET_AWART_ZHANGJIE_RESP = 24156");
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_NOT_GET_AWART_ZHANGJIE_REQ);
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_NOT_GET_AWART_ZHANGJIE_RESP:// 请weilingqu章节奖励
			{

				MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				GetNotGetAwardZhangJieResp mGetNotGetAwardZhangJieResp = new GetNotGetAwardZhangJieResp();

				t_qx.Deserialize(t_tream, mGetNotGetAwardZhangJieResp, mGetNotGetAwardZhangJieResp.GetType());

				mDataback = true;

				Sec_idlist = mGetNotGetAwardZhangJieResp.zhangJiaId;

				CurrSection = MapData.mapinstance.CurrChapter;
				if(CurrSection != 0){
					InitData(CurrSection);
				}
				return true;
			}
				break;
			}
		}
		return false;
	}
	public void InitData(int Zhangjieid)
	{
//		Debug.Log("Sec_idlist.Count = "+Sec_idlist.Count);
//		Debug.Log("Zhangjieid = "+Zhangjieid);
		if(Sec_idlist == null || Sec_idlist.Count == 0)
		{
			// 已经全部领取
			PveUImanager.instances.CloseArt();
			CloseEffect();
			StopCoroutine ("BtnShake");
			int hongdian_id = 300001;
			PushAndNotificationHelper.SetRedSpotNotification (hongdian_id, false);
		}
		else
		{
			PveUImanager.instances.ShowArt();
			//Debug.Log("Sec_idlist = "+Sec_idlist[0]);
			foreach(int id in Sec_idlist )
			{
				if(id == Zhangjieid)
				{
					//显示红点
					StopCoroutine ("BtnShake");
					IsOPenEffect = true;
					StartCoroutine ("BtnShake");
					return;
				}
			}
			CloseEffect();
			IsOPenEffect = false;
			StopCoroutine ("BtnShake");
		}
	}
	public void OPenEffect()
	{
		if(IsOPenEffect)
		{
			UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,this.gameObject,EffectIdTemplate.GetPathByeffectId(100168));
		}
	}
	public void CloseEffect()
	{
		UI3DEffectTool.ClearUIFx (this.gameObject);
	}
	IEnumerator BtnShake()
	{
		float m_scale = 2f;
	

		OPenEffect ();
//		Debug.Log("显示红点 = ");

		while(m_scale > 1.1f)
		{
//			Debug.Log("= = = == = ");
			//iTween.ShakePosition(this.gameObject,new Vector3(0.02f,0.001f,0),1);
			iTween.ShakeRotation(this.gameObject,new Vector3(0,0,20f),0.8f);
			yield return new WaitForSeconds (2.5f);
		}
	}
}
