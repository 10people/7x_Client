using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MoneyManager : MonoBehaviour {

	private ExploreInfoResp m_moneyInfo;

	public UILabel yb_label;//元宝label

	public UILabel gx_label;//贡献label
	
	public UILabel jt_label;//精铁label
	
	public UILabel qt_label;//青铜label

	private int ybNum;
	private int gxNum;
	private int jtNum;
	private int qtNum;

	private enum CostType
	{
		YB,
		GX,
	}
	private CostType costType = CostType.YB;

	private int tanBaoType;

	public List<NGUILongPress> labelPressList = new List<NGUILongPress>();

	void Start ()
	{
		ShowLabelTips ();
	}

	void ShowLabelTips ()
	{
		for (int i = 0;i < labelPressList.Count;i ++)
		{
			labelPressList[i].name = i.ToString ();
			labelPressList[i].OnLongPress += ActiveTips;
//			labelPressList[i].OnLongPressFinish += DoActiveTips;
		}
	}

	void ActiveTips (GameObject go)
	{
		int commonId = 0;
		switch (go.name)
		{
		case "0":
			commonId = 900002;
			break;
		case "1":
			commonId = 900015;
			break;
		case "2":
			commonId = 920001;
			break;
		case "3":
			commonId = 920002;
			break;
		default:
			break;
		}
		ShowTip.showTip(commonId);
	}

	void DoActiveTips (GameObject go)
	{
		ShowTip.close ();
	}

	//获得money信息
	public void GetMoneyInfo (ExploreInfoResp tempMoneyInfo)
	{
//		Debug.Log ("yuanBao:" + tempMoneyInfo.yuanBao);
		ybNum = tempMoneyInfo.yuanBao;
		gxNum = tempMoneyInfo.gongXian;
		jtNum = tempMoneyInfo.tie;
		qtNum = tempMoneyInfo.tong;

		yb_label.text = tempMoneyInfo.yuanBao.ToString ();
		
		gx_label.text = tempMoneyInfo.gongXian.ToString ();
	}

	public void RefreshMoneyInfo (int type,ExploreInfoResp tempMoneyInfo)
	{
		tanBaoType = type;
//		Debug.Log ("yuanBao:" + tempMoneyInfo.yuanBao);
		m_moneyInfo = tempMoneyInfo;
	}
	
	//数量变化
	public void GetJtOrQt (int type,int num)
	{
		if (type == 0)
		{
			jtNum += num;
			jt_label.gameObject.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
		}

		else if (type == 1 || type == 10 || type == 11 || type == 12)
		{
			qtNum += num;
			qt_label.gameObject.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
		}
	}

	void Update ()
	{
		if (tanBaoType == 1 || tanBaoType == 10)
		{
			NumCount (yb_label,CostType.YB,m_moneyInfo.yuanBao);
		}
		
		else if (tanBaoType == 11 || tanBaoType == 12)
		{
			NumCount (gx_label,CostType.GX,m_moneyInfo.gongXian);
		}

		if (jt_label.gameObject.transform.localScale != Vector3.one)
		{
			Vector3 tempScal = jt_label.gameObject.transform.localScale;

			tempScal.x -= 0.02f;
			tempScal.y -= 0.02f;
			tempScal.z -= 0.02f;
			
			if(tempScal.x < 1)
			{
				tempScal.x = 1f;
				tempScal.y = 1f;
				tempScal.z = 1f;
			}

			jt_label.gameObject.transform.localScale = tempScal;
		}

		if (qt_label.gameObject.transform.localScale != Vector3.one)
		{
			Vector3 tempScal = qt_label.gameObject.transform.localScale;
			
			tempScal.x -= 0.02f;
			tempScal.y -= 0.02f;
			tempScal.z -= 0.02f;
			
			if(tempScal.x < 1)
			{
				tempScal.x = 1f;
				tempScal.y = 1f;
				tempScal.z = 1f;
			}
			
			qt_label.gameObject.transform.localScale = tempScal;
		}

		jt_label.text = jtNum.ToString ();

		qt_label.text = qtNum.ToString ();
	}

	void NumCount (UILabel tempLabel,CostType type,int endNum)
	{
		int startNum = -1;
		switch (type)
		{
		case CostType.YB:
			startNum = ybNum;
			break;
		case CostType.GX:
			startNum = gxNum;
			break;
		default:
			break;
		}
		if (startNum <= endNum)
		{
			if(tempLabel.gameObject.transform.localScale.x != 1)
			{
				Vector3 tempScal = tempLabel.gameObject.transform.localScale;

				tempScal.x -= 0.02f;
				tempScal.y -= 0.02f;
				tempScal.z -= 0.02f;

				if(tempScal.x < 1)
				{
					tempScal.x = 1f;
					tempScal.y = 1f;
					tempScal.z = 1f;
				}

				tempLabel.gameObject.transform.localScale = tempScal;
			}
		}
		else
		{
			if (tempLabel.gameObject.transform.localScale.x < 1.1f)
			{
				Vector3 tempScal = tempLabel.gameObject.transform.localScale;

				tempScal.x += 0.1f;
				tempScal.y += 0.1f;
				tempScal.z += 0.1f;

				if(tempScal.x > 1.1)
				{
					tempScal.x = 1.1f;
					tempScal.y = 1.1f;
					tempScal.z = 1.1f;
				}

				tempLabel.gameObject.transform.localScale = tempScal;
			}
			else
			{
				float tempAddNum = (startNum - endNum) / 10;

				if (Mathf.Abs(tempAddNum) < 1)
				{
					startNum = endNum;
				}

				else
				{
					startNum = (int)(startNum - tempAddNum);
				}

				tempLabel.text = startNum.ToString ();
			}
		}

		switch (type)
		{
		case CostType.YB:
			ybNum = startNum;
			break;
		case CostType.GX:
			gxNum = startNum;
			break;
		default:
			break;
		}
	}
}
