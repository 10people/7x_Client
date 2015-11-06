using UnityEngine;
using System.Collections;

public class UISelectSevcer : MonoBehaviour {

    public enum ServerSelect{
        NeiWang,
		TX,
        JianHu,
        DaLi,
		ZhaoWen,
    }

   	public ServerSelect select = ServerSelect.NeiWang;


	public UIPopupList m_server_list;

    void Awake()
    {
        setSevcer();
    }


    void setSevcer()
    {
//		Debug.Log( "UISelectSevcer.SetServer( " + select + " )" );

        switch(select){
            case ServerSelect.NeiWang:
                SocketTool.SetServerPrefix(
					SocketTool.SERVER_PREFIX_INNER_INDIE,
					SocketTool.SERVER_PORT_INNER_INDIE );
                break;

			case ServerSelect.TX:
				SocketTool.SetServerPrefix(
					SocketTool.SERVER_PREFIX_TX,
					SocketTool.SERVER_PORT_INNER_INDIE );
                break;

            case ServerSelect.JianHu:
                SocketTool.SetServerPrefix(
					SocketTool.SERVER_PREFIX_JIAN_HU,
					SocketTool.SERVER_PORT_INNER_INDIE);
                break;

            case ServerSelect.DaLi:
                SocketTool.SetServerPrefix(
					SocketTool.SERVER_PREFIX_DA_LI,
					SocketTool.SERVER_PORT_INNER_INDIE);
                break;

			case ServerSelect.ZhaoWen:
			SocketTool.SetServerPrefix(
				SocketTool.SERVER_LIZHAOWEN,
				SocketTool.SERVER_PORT_INNER_INDIE);
			break;

            default:
                Debug.LogError("选择服务器错误");
                break;
        }
    }

	public void PopListValueChange(){
		Debug.Log( "PopListValueChange: " + m_server_list.value );

		switch( m_server_list.value ){
		case "内网":
			select = ServerSelect.NeiWang;
			break;

		case "TX":
			select = ServerSelect.TX;
			break;

		case "建虎":
			select = ServerSelect.JianHu;
			break;

		case "大力":
			select = ServerSelect.DaLi;
			break;

		case "照文":
			select = ServerSelect.ZhaoWen;
			break;
		
		default:break;
		}
		setSevcer();
	}
}
