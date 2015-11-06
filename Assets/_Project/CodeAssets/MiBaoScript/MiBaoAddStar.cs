using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class MiBaoAddStar : MonoBehaviour,SocketProcessor {
	[HideInInspector]public MibaoInfo showMiBao;
	public UISprite Pinzhi1;
	public UISprite Pinzhi2;
	public UISprite MibaoIcon;
	public UISprite MibaoIcon2;
	public UISprite Stars1;
	public UISprite Stars2;

	public UILabel needMoney;
	
	GameObject cardtemp ;
	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
		
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		
	}
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void init()
	{
		int star = showMiBao.star;
		Debug.Log ("star = "+star);
		MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (star);
		//Debug.Log ("mMiBaoStarTemp.money = "+mMiBaoStarTemp.needMoney);
		needMoney.text = mMiBaoStarTemp.needMoney.ToString ();
		MiBaoCreateStars (star,Stars1);
		MiBaoCreateStars (star+1,Stars2);
	}

	void MiBaoCreateStars(int num, UISprite sp)
	{

		
		for(int i = 0; i < num; i++)
		{
			GameObject spriteObject = (GameObject)Instantiate(sp.gameObject);
			
			spriteObject.SetActive(true);
			
			spriteObject.transform.parent = sp.gameObject.transform.parent;
			
			spriteObject.transform.localScale = sp.gameObject.transform.localScale;
			
			spriteObject.transform.localPosition = sp.gameObject.transform.localPosition + new Vector3(i * 20 - (num - 1) * 10, 0, 0);
		
		}
	}

	public void SendAddStarReq()
	{
		int star = showMiBao.star;
		
		MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (star);
		int jibi = JunZhuData.Instance ().m_junzhuInfo.jinBi;
		//Debug.Log("jinBi        = "+jibi );
		//Debug.Log ("needMoney        = "+mMiBaoStarTemp.needMoney );

		if(JunZhuData.Instance().m_junzhuInfo.jinBi < mMiBaoStarTemp.needMoney)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);

			return;
		}
		//Debug.Log ("7777777777777777777777777777777777777777777");
		MibaoStarUpReq MiBaoinfo = new MibaoStarUpReq ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();

		MiBaoinfo.mibaoId = showMiBao.miBaoId;
		MiBaoinfoer.Serialize (MiBaoinfoStream,MiBaoinfo);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_STARUP_REQ,ref t_protof,ProtoIndexes.s_Mibao_StarUp_Resp.ToString());//秘宝激活
	
	}
	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTongBi);
	}
	void getTongBi(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
		}
	}

	MibaoStarUpResp m_iBaoActiveInfo;
	void LoadBck_2(ref WWW p_www,string p_path, Object p_object)
	{
	    cardtemp = Instantiate(p_object) as GameObject;
		
		cardtemp.transform.parent = this.transform.parent;
		
		cardtemp.transform.localPosition = new Vector3(0,-46,0);
		
		cardtemp.transform.localScale = new Vector3(1,1,1);
		if(cardtemp)
		{
			mbCardTemp mmbCardTemp = cardtemp.GetComponent<mbCardTemp>();
			
			mmbCardTemp.mibaoTemp =  m_iBaoActiveInfo.mibaoInfo;
			
			mmbCardTemp.init();
			Closed();

		}

	}
	public bool OnProcessSocketMessage(QXBuffer p_message){
		//Debug.Log("jieshouxinxi" );
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.s_Mibao_StarUp_Resp:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoStarUpResp MiBaoActiveInfo = new MibaoStarUpResp();
				
				t_qx.Deserialize(t_stream, MiBaoActiveInfo, MiBaoActiveInfo.GetType());
				m_iBaoActiveInfo = MiBaoActiveInfo;
				if(MiBaoActiveInfo.mibaoInfo != null)
				{
					GameObject mMibao = GameObject.Find("MiBao(Clone)");
					if(mMibao)
					{

						MiBaoCard mMiBaoCard = mMibao.GetComponent<MiBaoCard>();
						//Debug.Log("1mMiBaoCard.My_mibaoinfo.gongJi  " +mMiBaoCard.My_mibaoinfo.gongJi);
						mMiBaoCard.My_mibaoinfo = MiBaoActiveInfo.mibaoInfo;

						//Debug.Log("2MiBaoActiveInfo.mibaoInfo.gongJi " +MiBaoActiveInfo.mibaoInfo.gongJi);

						mMiBaoCard.init();
					}
					//Debug.Log("dataBack" +MiBaoActiveInfo.mibaoInfo.);
					for(int i = 0; i < SecretPageManeger.localMiBao.Count; i++)
					{
						if(SecretPageManeger.localMiBao[i].m_mibaoinfo.miBaoId == MiBaoActiveInfo.mibaoInfo.miBaoId)
						{
							SecretPageManeger.localMiBao[i].m_mibaoinfo = MiBaoActiveInfo.mibaoInfo;
							SecretPageManeger.localMiBao[i].Init();

							Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_CARD_TEMP ),LoadBck_2);
						
						}
						
						
					}
					
					
				}
				else{
					
					//Debug.Log("m_MiBaoInfo.mibaiInfo == null" );
					return  false;
				}
				//	InitData();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
				return true;
			}
	
			default: return false;
			}
			
		}else
		{
			//Debug.Log ("p_message == null");
		}
		
		return false;
	}
	BuyTimesInfo m_resp;

	public  void Closed(){

		Destroy (this.gameObject);
	}
}
