using UnityEngine;
using System.Collections;

public class LoginTipWindow : MonoBehaviour {

	public GameObject winObj;

	public UILabel desLabel;
	
	public void ScaleAnim ()
	{
		Hashtable scale = new Hashtable ();
		
		scale.Add ("scale",Vector3.one);
		scale.Add ("time",0.3f);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("islocal",true);
		
		iTween.ScaleTo (winObj,scale);
	}

	public void ShowTip (string msg)
	{
		desLabel.text = msg;

//		switch (type)
//		{
//		case 1:
//
//			desLabel.text = "账号或密码不能为空！";
//
//			break;
//
//		case 2:
//
//			desLabel.text = "该账号已注册！";
//
//			break;
//
//		case 3:
//
//			desLabel.text = "账号或密码输入错误\n请重新输入！";
//
//			break;
//
//		case 4:
//
//			desLabel.text = "注册成功\n请登录";
//
//			break;
//
//		case 5:
//
//			desLabel.text = "有奇怪的文字混进来了\n再仔细推敲一下吧...";
//
//			break;
//
//		default:break;
//		}
	}

	public void SureBtn ()
	{
		Destroy (this.gameObject);
	}
}
