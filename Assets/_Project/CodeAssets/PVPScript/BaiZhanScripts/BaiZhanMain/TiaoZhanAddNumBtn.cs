using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TiaoZhanAddNumBtn : MonoBehaviour,SocketProcessor {

	private AddChanceResp addNumResp;

	private string titleStr;
	private string str1;

	private string confirmStr;
	private string cancelStr;
	
	void Awake () 
	{	
		SocketTool.RegisterMessageProcessor (this);
	}
	
	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}
	
	void OnClick ()
	{
		if (!BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent)
		{
			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = true;
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.ADD_CHANCE_REQ,"27008");
			Debug.Log ("baiZhanAddNumReq:" + ProtoIndexes.ADD_CHANCE_REQ);
		}
	}

	public bool OnProcessSocketMessage (QXBuffer p_message) 
	{	
		if (p_message != null) {
			
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.ADD_CHANCE_RESP:
				Debug.Log ("baiZhanDisCDRes:" + ProtoIndexes.ADD_CHANCE_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				AddChanceResp addNumInfo = new AddChanceResp();
				
				t_qx.Deserialize(t_stream, addNumInfo, addNumInfo.GetType());
				
				if (addNumInfo != null)
				{	
					addNumResp = addNumInfo;

					Debug.Log ("购买需要的元宝数:" + addNumInfo.yuanbao);
					Debug.Log ("该回可购买的次数:" + addNumInfo.count);
					Debug.Log ("剩余购买回数:" + addNumInfo.left);
					
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        AddNumCallback );
				}
				return true;
			}
		}
		
		return false;
	}
	
	void AddNumCallback( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		if (addNumResp.can)
		{
			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_ADDNUM_TITLE);

			string askStr1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_ADDNUM_ASKSTR1);
			string yuanBaoBuyStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_YUANBAO_BUY);
			string askStr2 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_ADDNUM_ASKSTR2);
			string askStr3 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_ADDNUM_ASKSTR3);

			Debug.Log ("11确定增加次数");
			str1 = askStr1 + addNumResp.yuanbao + yuanBaoBuyStr + addNumResp.count + askStr2 + addNumResp.left + askStr3;
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, cancelStr, confirmStr,ClickBtn);
		}
		
		else
		{
			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_ADDNUM_TITLE);

			str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_BUY_ADDNUM_VIP_NOT_ENOUGH);

			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, confirmStr, null,CantAddNumBack);
		}
	}

	void ClickBtn (int i)
	{
		BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
		if (i == 2)
		{	
			Debug.Log ("确定增加次数");
			ConfirmManager.confirm.ConfirmReq (0,null,0);
		}
	}
	void CantAddNumBack (int i)
	{
		BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
	}

	void OnDestroy () 
	{	
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
