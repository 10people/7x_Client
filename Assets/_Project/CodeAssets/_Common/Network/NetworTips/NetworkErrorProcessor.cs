using UnityEngine;
using System.Collections;

public class NetworkErrorProcessor : MonoBehaviour {

	public GameObject m_pop_window = null;

	public void ReConnectSocket(){
		Debug.Log( "ReConnectSocket()" );

		if( SocketTool.WillReconnect() ){
			if( UIYindao.m_UIYindao != null && UIYindao.m_UIYindao.m_isOpenYindao ){
				UIYindao.m_UIYindao.CloseUI();
			}

			StartCoroutine( DelayedEnterLogin() );
		}
	}

	IEnumerator DelayedEnterLogin(){
		yield return new WaitForEndOfFrame();

		yield return new WaitForEndOfFrame();
		
		SceneManager.RequestEnterLogin();
	}

	void Update(){
//		if( SocketTool.IsConnected() ){
//			SocketTool.DestroyReConnectWindow();
//		}
	}
}
