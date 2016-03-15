using UnityEngine;
using System.Collections;

public class TreasureBoxManager : TreasureCitySingleton<TreasureBoxManager> {

	private LayerMask m_layerMask = 1 << 8;
	
	private Ray m_ray;

	private UICamera.MouseOrTouch m_MouseOrTouch;

	private int m_depth = 5;

	void Awake ()
	{
		base.Awake ();
	}

	void OnDestroy ()
	{
		base.OnDestroy ();
	}

	void LateUpdate()
	{
		if ( TreasureCityUI.m_instance == null )
		{
			return;
		}
		
		if (QXChatData.Instance.SetOpenChat || CityGlobalData.m_joystickControl || TreasureCityUI.IsWindowsExist() || JunZhuLevelUpManagerment.m_JunZhuLevelUp != null) return; //现在在操纵摇杆 or 有ui界面弹出  npc不响应点击事件
		{
			if (Input.GetMouseButton(0))
			{
				//if (MainCityUI.IsWindowsExist() || UIYindao.m_UIYindao.m_isOpenYindao
				if (TreasureCityUI.IsWindowsExist())
				{
					return;
				}
				Vector3 tempMousePosition = Input.mousePosition;
				Camera nguiCamera = Global.GetObj(ref TreasureCityRoot.m_instance.treasureCityUI, "Camera_UI").GetComponent<Camera>();
				m_ray = nguiCamera.ScreenPointToRay (tempMousePosition);// 从屏幕发射线
				RaycastHit nguiHit;
				if (Physics.Raycast(m_ray, out nguiHit)) return;//碰到主界面UI按钮
				
				m_ray = Camera.main.ScreenPointToRay (tempMousePosition);
				
				RaycastHit hit;
				int t_index = LayerMask.NameToLayer("CityRoles");
				
				m_depth  = 1 << t_index;
				
				if (Physics.Raycast (m_ray, out hit, 1000.0f, m_depth))
				{
					if (hit.collider.transform.name.IndexOf("PlayerObject") > -1)
					{
						m_MouseOrTouch = UICamera.GetTouch(UICamera.currentTouchID);
						EquipSuoData.CreateChaKan(hit.collider.transform.name, m_MouseOrTouch.pos);
						return;
					}
				}

				int t_index2 = LayerMask.NameToLayer("Default");
				
				int depth2 = 1 << t_index2;
				
				if (Physics.Raycast(m_ray,out hit, 10000.0f, depth2)) //碰到3d世界中的npc
				{
					if (hit.collider.transform.name == "TreasureBox")
					{
//						PlayerModelController.m_playerModelController.m_agent.enabled = true;
						TreasureCityPlayer.m_instance.m_agent.enabled = true;

						TreasureBox box = hit.collider.transform.GetComponent<TreasureBox> ();
						TreasureCityPlayer.m_instance.TargetBoxUID = box.enterScene.uid;

//						PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;

//						if (Vector3.Distance (hit.collider.transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 2)
						if (Vector3.Distance (hit.collider.transform.position, TreasureCityPlayer.m_instance.m_playerObj.transform.position) > 2)
						{
//							PlayerModelController.m_playerModelController.SelfNavigation (hit.collider.gameObject.transform.position);
							TreasureCityPlayer.m_instance.SelfNavigation (hit.collider.gameObject.transform.position);
						}
					}
				}
			}
		}
	}
}
