using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXSelectRole : MonoBehaviour,SocketProcessor {

	private static QXSelectRole m_selectRole;
	public static QXSelectRole Instance ()
	{
		if (m_selectRole == null)
		{
			GameObject t_GameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();
			
			m_selectRole = t_GameObject.AddComponent<QXSelectRole>();
		}
		
		return m_selectRole;
	}

	private GameObject m_selectRoleObj;
	private QXSelectRolePage.SelectType m_selectType;

	private ErrorMessage m_errorMsg;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	public void SelectRolePage (QXSelectRolePage.SelectType tempType)
	{
		m_selectType = tempType;
		
		switch (tempType)
		{
		case QXSelectRolePage.SelectType.CREATE_ROLE:
			m_errorMsg = null;
			LoadSelectRoleObj ();
			break;
		case QXSelectRolePage.SelectType.UNLOCK_ROLE:
			Global.m_isChangeRoleOpen = true;
			QXComData.SendQxProtoMessage (ProtoIndexes.MODEL_INFO,ProtoIndexes.MODEL_INFO.ToString ());
			break;
		default:
			break;
		}
	}

	#region CreateRole

	public void CreatRole (string tempName,int tempRoleId,int tempNationId = 1) 
	{	
		CreateRoleRequest creatReq = new CreateRoleRequest();
		
		creatReq.roleName = tempName; //角色名字
		creatReq.roleId = tempRoleId + 1;  //角色id
		creatReq.guoJiaId = tempNationId; //国家id
		
		QXComData.SendQxProtoMessage (creatReq,ProtoIndexes.CREATE_ROLE_REQUEST,ProtoIndexes.CREATE_ROLE_RESPONSE1.ToString ());

		CityGlobalData.m_king_model_Id = tempRoleId + 1;
	}
	#endregion

	#region UnLockRole
	private int m_unLockId = 0;
	private bool m_haveBuy = false;
	public void UnLockRoleOperate (int tempRoleId,bool isUnLock)
	{
		m_unLockId = tempRoleId;
//		Debug.Log ("tempRoleId：" + tempRoleId);
		m_haveBuy = isUnLock;
		ErrorMessage errorMsg = new ErrorMessage ();
		errorMsg.errorCode = tempRoleId + 1;
		QXComData.SendQxProtoMessage (errorMsg,isUnLock ? ProtoIndexes.UNLOCK_MODEL : ProtoIndexes.CHANGE_MODEL,
		                              isUnLock ? ProtoIndexes.MODEL_INFO.ToString () : ProtoIndexes.BD_CHANGE_MODEL.ToString ());
	}

    #endregion

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
            case ProtoIndexes.CREATE_ROLE_RESPONSE1:
            {
                CreateRoleResponse creatRes = new CreateRoleResponse();
                creatRes = QXComData.ReceiveQxProtoMessage(p_message, creatRes) as CreateRoleResponse;

                if (creatRes != null)
                {
                    switch (creatRes.isSucceed)
                    {
                        case true:

                            //CityGlobalData.m_nextSceneName = ConstInGame.CONST_SCENE_NAME_MAIN_CITY;    //创建成功跳转
                            //Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );

                            CityGlobalData.m_CreateRoleCurrent = true;

                            //EnterBattleField.EnterBattlePve( 1, 1, LevelType.LEVEL_NORMAL );
                            //						if (TimeHelper.Instance.IsTimeCalcKeyExist("RoleAnimate"))
                            //						{
                            //							TimeHelper.Instance.RemoveFromTimeCalc("RoleAnimate");
                            //						}

                            //						Debug.Log("EnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCityEnterMainCity");

                            SceneManager.EnterMainCity();

                            break;

                        case false:
                            //						Debug.Log("creatRes.msg:" + creatRes.msg);

                            //创建失败
                            if (ThirdPlatform.IsMyAppAndroidPlatform())
                            {
                                QXComData.CreateBoxDiy(creatRes.msg, true, null);
                            }
                            else
                            {
                                QXComData.CreateBoxDiy(creatRes.msg, true, RandomAgain, true);
                            }

                            break;
                    }
                }

                return true;
            }
            case ProtoIndexes.MODEL_INFO:
            {
                ErrorMessage errorMes = new ErrorMessage();
                errorMes = QXComData.ReceiveQxProtoMessage(p_message, errorMes) as ErrorMessage;

                if (errorMes != null)
                {
                    Debug.Log("errorDesc:" + errorMes.errorDesc);
                    Debug.Log("cmd:" + errorMes.cmd);
                    Debug.Log("errorCode:" + errorMes.errorCode);
                    m_errorMsg = errorMes;

					if (Global.m_isChangeRoleOpen)
					{
						if (m_haveBuy && int.Parse (errorMes.errorDesc.Split ('#')[0]) <= 0)
						{
							UnLockRoleOperate (m_unLockId,false);
							m_haveBuy = false;
						}
						else
						{
							LoadSelectRoleObj();
						}
					}
                }

                return true;
            }
            case ProtoIndexes.BD_CHANGE_MODEL:
            {
                ErrorMessage errorMes = new ErrorMessage();
                errorMes = QXComData.ReceiveQxProtoMessage(p_message, errorMes) as ErrorMessage;

                if (errorMes != null)
                {
					if (errorMes.cmd == PlayersManager.m_Self_UID)
					{
						CityGlobalData.m_king_model_Id = errorMes.errorCode;
					}
                  
					Debug.Log ("errorMes.errorCode:" + errorMes.errorCode);
					Debug.Log ("Global.m_isChangeRoleOpen:" + Global.m_isChangeRoleOpen);
					if (Global.m_isChangeRoleOpen)
					{
						SelectRolePage(QXSelectRolePage.SelectType.UNLOCK_ROLE);
						
						SettingUpLayerManangerment.m_SettingUp.ShowIcon();
					}

					if (MainCityUI.m_MainCityUI != null)
					{
						Debug.Log ("errorMes.cmd:" + errorMes.cmd);
						Debug.Log ("PlayersManager.m_Self_UID:" + PlayersManager.m_Self_UID);
						if (errorMes.cmd != PlayersManager.m_Self_UID)
						{
							PlayerInCityManager.m_PlayerInCity.Reload_Skeleton(errorMes.cmd, errorMes.errorCode);
						}
						else
						{
							PlayerModelController.m_playerModelController.SwitchModel();
						}
					}
                    
                }

                break;
            }
            }
        }
        return false;
    }

	void LoadSelectRoleObj ()
	{
		if (m_selectRoleObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LOGIN_CREATE_ROLE ),
			                        SelectRoleLoadBack );
		}
		else
		{
			m_selectRoleObj.SetActive (true);
			InItSelectRolePage ();
		}
	}

	private void SelectRoleLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		m_selectRoleObj = (GameObject)Instantiate (p_object);

		InItSelectRolePage ();
	}

	void InItSelectRolePage ()
	{
		MainCityUI.TryAddToObjectList (m_selectRoleObj);
		TreasureCityUI.TryAddToObjectList (m_selectRoleObj);
		QXSelectRolePage.m_instance.InItRolePage (m_selectType,m_errorMsg);
		QXSelectRolePage.m_instance.M_RoleDelegate = RoleDelegateCallBack;
	}

	void RoleDelegateCallBack ()
	{
		MainCityUI.TryRemoveFromObjectList (m_selectRoleObj);
		TreasureCityUI.TryRemoveFromObjectList (m_selectRoleObj);
		Destroy (m_selectRoleObj);
		m_selectRoleObj = null;
//		m_selectRoleObj.SetActive (false);
		m_haveBuy = false;
		Global.m_isChangeRoleOpen = false;
	}

	void RandomAgain (int i)
	{
		QXSelectRolePage.m_instance.RandomName ();
	}

	new void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
