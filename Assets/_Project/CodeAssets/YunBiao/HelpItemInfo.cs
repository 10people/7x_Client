using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HelpItemInfo : MonoBehaviour {

	public UISprite headIcon;

	private XieZhuJunZhu xieZhuJunZhu;

	public GameObject tipsObj;

	public void GetHelpInfo (XieZhuJunZhu tempJunZhu)
	{
		xieZhuJunZhu = tempJunZhu;

		headIcon.spriteName = "PlayerIcon" + tempJunZhu.roleId.ToString ();
	}

	public void RefuseBtn ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        RefuseLoadBack );
	}
	void RefuseLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();
		
		string titleStr = "提示";
		
		string str = "是否拒绝" + xieZhuJunZhu.name + "的协助？";;

		string cancel = "取消";

		string confirm = "确定";

		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, cancel, confirm,
		             RefuseBack);
	}
	void RefuseBack (int i)
	{
		if (i == 2)
		{
			//发送拒绝协助请求
			TiChuYBHelpRsq refuseReq = new TiChuYBHelpRsq();

			refuseReq.jzId = xieZhuJunZhu.jzId;

			SelectHorseMainPage.GetRefuseId = xieZhuJunZhu.jzId;
			SelectHorseMainPage.GetRefuseName = xieZhuJunZhu.name;

			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer t_serializer = new QiXiongSerializer ();
			
			t_serializer.Serialize (t_stream,refuseReq);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_TICHU_YBHELP_RSQ,ref t_protof,"3434");
			Debug.Log ("拒绝协助请求:" + ProtoIndexes.C_TICHU_YBHELP_RSQ);
		}
	}

	void OnPress (bool isPressed)
	{
		tipsObj.SetActive (isPressed);

		if (isPressed)
		{
			YunBiaoHelpTips help = tipsObj.GetComponent <YunBiaoHelpTips> ();
			help.ShowHelpInfo (xieZhuJunZhu.name,xieZhuJunZhu.roleId,xieZhuJunZhu.addHuDun);
		}
	}
}
