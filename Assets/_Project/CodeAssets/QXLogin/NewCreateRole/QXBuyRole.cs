using UnityEngine;
using System.Collections;

public class QXBuyRole : GeneralInstance<QXBuyRole> {

	public delegate void BuyRoleDelegate ();
	public BuyRoleDelegate M_BuyRoleDelegate;

	public UISprite m_roleIcon;
	public UISprite m_roleName;

	public UILabel m_cost;

	private int m_roleId;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItBuyRole (int roleId)
	{
		m_roleId = roleId;
		m_roleIcon.spriteName = QXSelectRolePage.m_instance.M_RoleDic [roleId] [2];
		m_roleName.spriteName = "roleName" + (roleId + 1);

		m_cost.text = CanshuTemplate.GetValueByKey (CanshuTemplate.UNLOCK_ROLE_PIRCE).ToString ();
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "BuyBtn":
			if (QXComData.JunZhuInfo ().vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey (25))
			{
				int m_costNum = (int)CanshuTemplate.GetValueByKey (CanshuTemplate.UNLOCK_ROLE_PIRCE);
				if (QXComData.JunZhuInfo ().yuanBao >= m_costNum)
				{
					QXSelectRole.Instance ().UnLockRoleOperate (m_roleId,true);
				}
				else
				{
					//元宝不足
//					ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"元宝不足！"));
					Global.CreateFunctionIcon (101);
				}
			}
			else
			{
				//vip等级不足
//				ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"V特权等级不足！"));
				Global.CreateFunctionIcon (1901);
			}
			break;
		default:
			break;
		}

		if (M_BuyRoleDelegate != null)
		{
			M_BuyRoleDelegate ();
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
