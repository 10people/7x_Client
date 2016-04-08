using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityUITR : MYNGUIPanel,SocketProcessor {

	public static TreasureCityUITR tCityUITR;

	public List<EventHandler> topRightHandlerList = new List<EventHandler>();

	public GameObject tanbaoBtnObj;
	public GameObject backToCityObj;

	public MainCityUITongzhi TongZhi;
	public TPController m_TpController;

	void Awake ()
	{
		tCityUITR = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		MainCityUI.setGlobalBelongings (this.transform.gameObject, -16, 0);

//		if (FunctionOpenTemp.IsHaveID (11))
//		{
//			tanbaoBtnObj.SetActive (true);
//			backToCityObj.transform.localPosition = new Vector3(-50,-290,0);
//		}
//		else
//		{
//			tanbaoBtnObj.SetActive (false);
//			backToCityObj.transform.localPosition = new Vector3(-50,-210,0);
//		}

		foreach (EventHandler handler in topRightHandlerList)
		{
			handler.m_click_handler += TopRightHandlerListClickBack;
		}

		QXComData.SendQxProtoMessage(ProtoIndexes.C_MengYouKuaiBao_Req);
	}

	void TopRightHandlerListClickBack (GameObject obj)
	{
		if (TreasureCityUI.IsWindowsExist ())
		{
			foreach (GameObject m_obj in TreasureCityUI.m_WindowObjectList)
			{
				Debug.Log ("obj.name:" + obj.name);
			}
			return;
		}
		switch (obj.name)
		{
		case "BtnSetting":

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_LAYER),
			                        SettingUpLoadCallback);

			break;
		case "BtnTanBao":

			TanBaoData.Instance.TanBaoInfoReq ();

			break;
		case "BtnBackToMainCity":

			PlayerSceneSyncManager.Instance.ExitTreasureCity ();

			break;
		default:
			break;
		}
	}

	void SettingUpLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = (GameObject)Instantiate(p_object);
		TreasureCityUI.TryAddToObjectList(tempObject);
		tempObject.transform.position = new Vector3(0, 500, 0);
		
		
//		UIYindao.m_UIYindao.CloseUI();
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MengYouKuaiBao_Resq:
			{
				Debug.Log ("ProtoIndexes.S_MengYouKuaiBao_Resq:" + ProtoIndexes.S_MengYouKuaiBao_Resq);
				PromptMSGResp tempInfo = new PromptMSGResp();
				tempInfo = QXComData.ReceiveQxProtoMessage (p_message,tempInfo) as PromptMSGResp;

				if (tempInfo != null)
				{
					List<TongzhiData> tempListTongzhiData = new List<TongzhiData>();
					if (tempInfo.msgList != null)
					{
						for (int i = 0; i < tempInfo.msgList.Count; i++)
						{
							TongzhiData tempTongzhiData = new TongzhiData(tempInfo.msgList[i]);
							tempListTongzhiData.Add(tempTongzhiData);
						}
					}

					Global.upDataTongzhiData(tempListTongzhiData);
					TongZhi.upDataShow();
				}

				return true;
			}
			case ProtoIndexes.S_MengYouKuaiBao_PUSH:
			{
				Debug.Log ("ProtoIndexes.S_MengYouKuaiBao_PUSH:" + ProtoIndexes.S_MengYouKuaiBao_PUSH);
				SuBaoMSG tempInfo1 = new SuBaoMSG();
				tempInfo1 = QXComData.ReceiveQxProtoMessage (p_message,tempInfo1) as SuBaoMSG;
				
				TongzhiData tempTongzhiData1 = new TongzhiData(tempInfo1);
				
				List<TongzhiData> tempListTongzhiData1 = new List<TongzhiData>();
				
				tempListTongzhiData1.Add(tempTongzhiData1);
				
				Global.upDataTongzhiData(tempListTongzhiData1);
				TongZhi.upDataShow();
				break;
			}
			case ProtoIndexes.S_Prompt_Action_Resp:
			{
				Debug.Log ("ProtoIndexes.S_Prompt_Action_Resp:" + ProtoIndexes.S_Prompt_Action_Resp);
				PromptActionResp msg = new PromptActionResp();
				msg = QXComData.ReceiveQxProtoMessage (p_message,msg) as PromptActionResp;
				
				if (msg.result != 10)
				{
					ClientMain.m_UITextManager.createText("通知已过时");
					
					return true;
				}
				
				switch (msg.subaoType)
				{
					//transport to position.
				case 101:
				case 102:
				case 104:
				case 105:
				{
					m_TpController.m_ExecuteAfterTP = EnterCarriageScene;
					m_TpController.TPToPosition(new Vector2(msg.posX, msg.posZ), float.Parse(YunBiaoTemplate.GetValueByKey("TP_duration")));
					break;
				}
				default:
				{
					break;
				}
				}
				if(msg.fujian == null || msg.fujian == "")
				{
					break;
				}
				
				string tempFujian = msg.fujian;
				List<string> funjianlist = new List<string>();
				
				while(tempFujian.IndexOf("#") != -1)
				{
					funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
				}
				funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
				List<RewardData> RewardDataList = new List<RewardData>();
				for(int i = 0; i < funjianlist.Count; i ++)
				{
					tempFujian = funjianlist[i];
					Global.NextCutting(ref tempFujian, ":");
					RewardData Rewarddata = new RewardData ( int.Parse(Global.NextCutting(ref tempFujian, ":")), int.Parse(Global.NextCutting(ref tempFujian, ":"))); 
					RewardDataList.Add(Rewarddata);
				}
				GeneralRewardManager.Instance().CreateReward (RewardDataList);
				break;
			}
			}
		}
		return false;
	}

	private void EnterCarriageScene(Vector2 p_position)
	{
		PlayerSceneSyncManager.Instance.EnterCarriage(p_position.x, p_position.y);
	}

	public override void MYClick(GameObject ui)
	{
		if (ui.name.IndexOf("TongzhiIcon") != -1)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TONGZHI),
			                        AddUIPanel);
		}
	}

	public void AddUIPanel(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = (GameObject)Instantiate(p_object);
		TreasureCityUI.TryAddToObjectList(tempObject);
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

	void OnDestroy ()
	{
		tCityUITR = null;
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
