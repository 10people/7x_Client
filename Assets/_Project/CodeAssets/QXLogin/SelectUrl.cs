using UnityEngine;
using System.Collections;

public class SelectUrl : MonoBehaviour {
	
	public enum UrlSeclect 
	{
		TiYan,

		CeShi,

		NeiWang,
	}

	private UrlSeclect select;

	public UIPopupList url_select_list;

	private static NetworkHelper.ServerType selectType;

	private bool isOnValue;

	void Awake () 
	{
		ServeSelect ();
		isOnValue = false;
		if(UIYindao.m_UIYindao != null)
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
		}
	}

	void ServeSelect ()
	{
		if (!string.IsNullOrEmpty (PlayerPrefs.GetString ("选服")))
		{
			string serveName = PlayerPrefs.GetString ("选服");

//			Debug.Log ("选服：" + serveName);

			switch (serveName)
			{
			case "体验":

				url_select_list.value = "体验服";
				select = UrlSeclect.TiYan;

				break;

			case "测试":
			
				url_select_list.value = "测试服";
				select = UrlSeclect.CeShi;
				
				break;

			case "内网":

				url_select_list.value = "内网服";
				select = UrlSeclect.NeiWang;

				break;
			}
		}
		else{
			url_select_list.value = NetworkHelper.GetDefaultServerName();
			select = NetworkHelper.GetDefaultLoginServerType();
		}

		SetUrl ();
	}

	/// <summary>
	/// Raises the click pup event.
	/// </summary>
	public void OnClickPup ()
	{
		isOnValue = true;
	}

	/// <summary>
	/// Pops the list value change.
	/// </summary>
	public void PopListValueChange () 
	{
//		Debug.Log( "PopListUrlChange: " + url_select_list.value );

		switch (url_select_list.value) 
		{
		case "体验服":

			select = UrlSeclect.TiYan;
			PlayerPrefs.SetString ("选服","体验");


			break;

		case "测试服":
			
			select = UrlSeclect.CeShi;
			PlayerPrefs.SetString ("选服","测试");
			
			break;

		case "内网服":

			select = UrlSeclect.NeiWang;
			PlayerPrefs.SetString ("选服","内网");

			break;

		default:break;
		}

//		Debug.Log ("ServePart:" + PlayerPrefs.GetString ("选服"));

		if (isOnValue)
		{
			SetUrl ();
		}
	}

	/// <summary>
	/// Sets the URL.
	/// </summary>
	void SetUrl () {
//		Debug.Log( "SelectUrl: " + select );
		
		switch (select) 
		{
		case UrlSeclect.TiYan:
			NetworkHelper.SetServerType( NetworkHelper.ServerType.TiYan );
			
			//			CityGlobalData.RigisterURL = HttpRequest.GetTengXunPrefix() + HttpRequest.REGISTER_URL;
			//			
			//			CityGlobalData.LoginURL = HttpRequest.GetTengXunPrefix() + HttpRequest.LOGIN_URL;
			break;
			
		case UrlSeclect.CeShi:
			NetworkHelper.SetServerType( NetworkHelper.ServerType.CeShi );
			
			//			CityGlobalData.RigisterURL = HttpRequest.GetTengXunPrefix() + HttpRequest.REGISTER_URL;
			//			
			//			CityGlobalData.LoginURL = HttpRequest.GetTengXunPrefix() + HttpRequest.LOGIN_URL;
			break;
			
		case UrlSeclect.NeiWang:
			NetworkHelper.SetServerType( NetworkHelper.ServerType.NeiWang );
			
			//			CityGlobalData.RigisterURL = HttpRequest.GetNeiWangPrefix() + HttpRequest.REGISTER_URL;
			//			
			//			CityGlobalData.LoginURL = HttpRequest.GetNeiWangPrefix() + HttpRequest.LOGIN_URL;
			break;
		}
		
		// overwrite from config
		{
			CityGlobalData.RigisterURL = NetworkHelper.GetPrefix() + NetworkHelper.REGISTER_URL;
			
			CityGlobalData.LoginURL = NetworkHelper.GetPrefix() + NetworkHelper.LOGIN_URL;
		}
	}

	/// <summary>
	/// Gets the type of the server.
	/// </summary>
	/// <returns>The server type.</returns>
	public static NetworkHelper.ServerType GetServerType ()
	{
		if (!string.IsNullOrEmpty (PlayerPrefs.GetString ("选服")))
		{
			string serveName = PlayerPrefs.GetString ("选服");
			
			switch (serveName){
			case "体验":
				selectType = NetworkHelper.ServerType.TiYan;
				
				break;
				
			case "测试":
				selectType = NetworkHelper.ServerType.CeShi;
				
				break;
				
			case "内网":
				selectType = NetworkHelper.ServerType.NeiWang;
				
				break;
			}
		}
		else
		{
			selectType = NetworkHelper.GetDefaultServerType();
		}

		Debug.Log ("SelectUrl.GetServerType:" + selectType);

		return selectType;
	}

	/// <summary>
	/// Sets the type of the server.
	/// </summary>
	/// <param name="type">Type.</param>
	public static void SetUrlServeType ( NetworkHelper.ServerType type ){
		switch (type)
		{
		case NetworkHelper.ServerType.CeShi:
			PlayerPrefs.SetString ("选服","测试");

			break;

		case NetworkHelper.ServerType.NeiWang:
			PlayerPrefs.SetString ("选服","内网");

			break;

		case NetworkHelper.ServerType.TiYan:
			PlayerPrefs.SetString ("选服","体验");

			break;

		default:
			break;
		}
	}
}
