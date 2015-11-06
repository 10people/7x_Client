using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


using qxmobile;
using qxmobile.protobuf;

public class DebugPhoneData : MonoBehaviour, SocketProcessor {

	public UIInput m_account_input;

	public string m_idle_tips;

	public string m_registering_tips;

	public string m_login_tips;

	private string m_error_tips;

	private string m_succes_tips;

	private enum Login_User_Status{
		idle,
		register,
		login,
		error,
		Success
	}

	private UILabel m_login_status_label;

	private Login_User_Status m_cur_status = Login_User_Status.idle;

	private int m_tips_last_update_time = 0;


	#region Mono
	// Use this for initialization
	void Start () {
		{
			m_login_status_label = GetComponent<UILabel>();

			Time.timeScale = 1.0f;
		}

		UpdateLoginStatusLabel();

		{
			SocketTool.RegisterMessageProcessor( this );
		}
	}

	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor( this );
	}
	
	// Update is called once per frame
	void Update () {
		if( Time.realtimeSinceStartup - m_tips_last_update_time > 1.0f ){
			m_tips_last_update_time = (int)Time.realtimeSinceStartup;

			UpdateLoginStatusLabel();
		}
	}

	#endregion

	#region Utility
	private void SetCurStatus( Login_User_Status p_status ){
		m_cur_status = p_status;

		UpdateLoginStatusLabel();
	}

	private string GetUserName(){
		return m_account_input.label.text;
	}

	private void UpdateLoginStatusLabel(){
		switch( m_cur_status ){
		case Login_User_Status.idle:
			m_login_status_label.text = m_idle_tips;
			return;

		case Login_User_Status.register:
			m_login_status_label.text = m_registering_tips;
			break;
		
		case Login_User_Status.login:
			m_login_status_label.text = m_login_tips;
			break;

		case Login_User_Status.error:
			m_login_status_label.text = m_error_tips;
			return;

		case Login_User_Status.Success:
			m_login_status_label.text = m_succes_tips;
			return;
		}

		m_login_status_label.text += m_tips_last_update_time % 2 == 0 ? ".." : "...";
	}
	#endregion

	#region UI Call

	public void Register(){
		Debug.LogError( "AutoLogin.Register() " + m_account_input.label.text );

//		SetCurStatus( Login_User_Status.idle );
//
//		if( !SocketTool.Instance().IsConnected() ){
//			SocketTool.Instance().Connect();
//		}
//
//		if( SocketTool.Instance().IsConnected() ){
//			RegReq t_register_request = new RegReq();
//			
//			t_register_request.name = m_account_input.label.text;
//
//
//			MemoryStream t_tream = new MemoryStream();
//
//			QiXiongSerializer t_serializer = new QiXiongSerializer();
//
//			t_serializer.Serialize( t_tream, t_register_request );
//
//
//			byte[] t_bytes = t_tream.ToArray();
//
//			SocketTool.Instance().SendSocketMessage( ProtoIndexes.CREATE_ACCOUNT, ref t_bytes );
//		}
	}

	public void Login(){
		Debug.LogError( "AutoLogin.Login() " + m_account_input.label.text );

//		SetCurStatus( Login_User_Status.idle );
//
//		if( !SocketTool.Instance().IsConnected() ){
//			SocketTool.Instance().Connect();
//		}
//		
//		if( SocketTool.Instance().IsConnected() ){
//			LoginReq t_login_request = new LoginReq();
//			
//			t_login_request.name = m_account_input.label.text;
//			
//			
//			MemoryStream t_tream = new MemoryStream();
//			
//			QiXiongSerializer t_serializer = new QiXiongSerializer();
//			
//			t_serializer.Serialize( t_tream, t_login_request );
//
//
//			byte[] t_bytes = t_tream.ToArray();
//			
//			SocketTool.Instance().SendSocketMessage( ProtoIndexes.LOGIN_ACCOUNT, ref t_bytes );
//		}
	}

	#endregion


	#region Network Process

	public bool OnProcessSocketMessage( QXBuffer p_message ){
		Debug.Log( "AutoLogin.OnProcessSocketMessage: " + p_message.m_protocol_index );

		// ACC_REG_RET
		if( p_message.m_protocol_index == 23102 ){
			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			
			RegRet t_register_response = new RegRet();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Deserialize( t_stream, t_register_response, t_register_response.GetType() );

			Debug.Log( "Regster response:    uid: " + t_register_response.uid + "   name: " + t_register_response.name );

			// < 0: success.
			if( t_register_response.uid < 0 ){
				m_error_tips = "Error Code: " + t_register_response.uid + "   message: " + t_register_response.name;
				
				SetCurStatus( Login_User_Status.error );
			}
			else{
				m_succes_tips = "注册成功.";

				SetCurStatus( Login_User_Status.Success );
			}

			return true;
		}

		// ACC_LOGIN_RET
		if( p_message.m_protocol_index == ProtoIndexes.LOGIN_ACCOUNT_RET ){
			Debug.Log( " DebugPhoneData Processing Login Return: " );

			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			
			LoginRet t_login_response = new LoginRet();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Deserialize( t_stream, t_login_response, t_login_response.GetType() );

			Debug.Log( "Login response:    code: " + t_login_response.code + "   message: " + t_login_response.msg );

			// 0: success.
			if( t_login_response.code != 0 ){
				m_error_tips = "Error Code: " + t_login_response.code + "   message: " + t_login_response.msg;

				SetCurStatus( Login_User_Status.error );
			}
			else{
				m_succes_tips = "登陆成功.";

				SetCurStatus( Login_User_Status.Success );

				// Added By YuGu, 2015.3.4
				Debug.LogError( "Error! Never Call This Directly" );

				Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_MAIN_CITY );
			}

			return true;
		}

		return false;
	}

	#endregion

    public void ConnectByHand()
    {
        Debug.LogError( "ConnectByHand()" );

//        SocketTool.Instance().Connect();
    }


    public void SendPhoneData()
    {
        Debug.Log("SendPhoneData()");

        PlayerSound tempSound = new PlayerSound();
        int tempCount = 60000;
        List<float> tempSam = new List<float>();
        for (int i = 0; i < tempCount; i++)
        {
            tempSam.Add(Random.value);
        }

        tempSound.s_soundData.AddRange(tempSam.GetRange(0, tempSam.Count));
        NGUIDebug.Log("sam: " + tempSam.Count * 5);

        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        t_qx.Serialize(t_tream, tempSound);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
        NGUIDebug.Log("t_protof: " + t_protof.Length);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.PLAYER_SOUND_REPORT, ref t_protof);
    }
}
