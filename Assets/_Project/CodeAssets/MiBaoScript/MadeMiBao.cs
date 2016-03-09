using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class MadeMiBao : MonoBehaviour ,SocketProcessor{

    [HideInInspector]public MibaoInfo  myZuHe_mibao ;  
	public UISprite Pinzhi;
	public UISprite MibaoIcon;
	public UILabel SuipianNum;
	public UISprite SuipianIcon;
	public UILabel HechengMoney;

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
		
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		
	}

	public void  Init (){

		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(myZuHe_mibao.miBaoId);

		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (mMiBaoXmlTemp.suipianId);

		//Debug.Log ("myZuHe_mibao.suiPianNum"+myZuHe_mibao.suiPianNum);

		SuipianNum.text = myZuHe_mibao.suiPianNum.ToString () + "/" + mMiBaoSuipianXMltemp.hechengNum.ToString ();

		HechengMoney.text = mMiBaoSuipianXMltemp.money.ToString();

		MibaoIcon.spriteName = mMiBaoXmlTemp.icon.ToString ();

		SuipianIcon.spriteName = mMiBaoSuipianXMltemp.icon.ToString ();
	}

	public void MadeFun()
	{
		int money = int.Parse (HechengMoney.text);
		if(money > JunZhuData.Instance().m_junzhuInfo.jinBi)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			return;
		}
		MibaoActivate MiBaoinfo = new MibaoActivate ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		//Debug.Log ("myZuHe_mibao.tempId"+myZuHe_mibao.tempId);
		MiBaoinfo.tempId = myZuHe_mibao.tempId;
		MiBaoinfoer.Serialize (MiBaoinfoStream,MiBaoinfo);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_ACTIVATE_REQ,ref t_protof,ProtoIndexes.S_MIBAO_ACTIVATE_RESP.ToString());//秘宝激活
	
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
	public void Closed()
	{
		if (FreshGuide.Instance().IsActive(300210) && TaskData.Instance.m_TaskInfoDic[300210].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300210];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}
		Destroy (this.gameObject);
		
	}
	public bool OnProcessSocketMessage(QXBuffer p_message){
		//Debug.Log("jieshouxinxi" );
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_ACTIVATE_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoActivateResp MiBaoActiveInfo = new MibaoActivateResp();
				
				t_qx.Deserialize(t_stream, MiBaoActiveInfo, MiBaoActiveInfo.GetType());
			
				if(MiBaoActiveInfo.mibaoInfo != null)
				{
					for(int i = 0; i < SecretPageManeger.localSkill.Count; i++)
					{
						MiBaoXmlTemp mMiBao = MiBaoXmlTemp.getMiBaoXmlTempById(MiBaoActiveInfo.mibaoInfo.miBaoId);
						if(mMiBao.zuheId == SecretPageManeger.localSkill[i].MiBaoZuHeId)
						{
							//Debug.Log("ssssssssssssss"+SecretPageManeger.localSkill[i].Activenums);
							SecretPageManeger.localSkill[i].Activenums += 1;
							SecretPageManeger.localSkill[i].InitSkill();
						}
					}

					m_temp_info = MiBaoActiveInfo.mibaoInfo;
					
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_CARD_TEMP ),
					                        MibaoActivateResLoaded );
					if(UIYindao.m_UIYindao.m_isOpenYindao)
					{
						UIYindao.m_UIYindao.CloseUI();
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
	private MibaoInfo m_temp_info;

	public void MibaoActivateResLoaded( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		GameObject cardtemp = ( GameObject )Instantiate( p_object );

		cardtemp.transform.parent = this.transform.parent;
		
		cardtemp.transform.localPosition = new Vector3(0,-46,0);
		
		cardtemp.transform.localScale = new Vector3(1,1,1);
		
		mbCardTemp mmbCardTemp = cardtemp.GetComponent<mbCardTemp>();
		
		mmbCardTemp.mibaoTemp = m_temp_info;
		
		mmbCardTemp.init();
		
		Closed();
	}
}
