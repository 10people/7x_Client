using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Joystick : MYNGUIPanel
{
    public Camera m_currentCamera;

    public Transform m_joystickBackGround;

    public Transform m_joystickTransform;

    private UICamera.MouseOrTouch m_MouseOrTouch;

	public BoxCollider m_Box;

    public Vector3 m_uiOffset;

	public UISprite m_spriteBG;
	public UISprite m_spriteButton;
    public bool m_isLockBack;
	private bool m_isMove;
	private Vector3 m_Vector3MousePos;
    void Awake()
    {

    }

    void Start()
    {
		m_Box.center = new Vector3((480 + ClientMain.m_iMoveX) / 2 - 76, (320 + ClientMain.m_iMoveY) / 2 - 76, 0);
		m_Box.size = new Vector3(480 + ClientMain.m_iMoveX, 320 + ClientMain.m_iMoveY, 0);
    }

    public void setPos(int x, int y)
    {
        gameObject.transform.localPosition = new Vector3(x, y, 0);
    }

    public const float MaxToggleDistance = 60;

    private bool isWidgetShowed = true;

    void Update()
    {
        if (m_MouseOrTouch != null)
        {
            if (!m_MouseOrTouch.pressStarted)
            {
                m_MouseOrTouch = null;
                m_joystickBackGround.localPosition = Vector3.zero;
                m_joystickTransform.localPosition = Vector3.zero;
                m_uiOffset = Vector3.zero;

                return;
            }
            m_joystickTransform.position = m_currentCamera.ScreenToWorldPoint(m_MouseOrTouch.pos);

            float Dis = Vector3.Distance(Vector3.zero, m_joystickTransform.localPosition);
            if (Dis > MaxToggleDistance)
            {
                m_joystickTransform.localPosition = m_joystickTransform.localPosition.normalized * MaxToggleDistance;
            }
            m_uiOffset = new Vector3(m_joystickTransform.localPosition.x, 0, m_joystickTransform.localPosition.y);
			if(Mathf.Abs(m_joystickTransform.localPosition.x) >= 10 || Mathf.Abs(m_joystickTransform.localPosition.y) >= 10)
			{
				m_spriteBG.color = new Color(1f, 1f, 1f, 1f);
				m_spriteButton.color = new Color(1f, 1f, 1f, 1f);
				m_isMove = true;
			}
			else
			{
				m_uiOffset = Vector3.zero;
			}
        }
    }

    private void SetChildrenWidgetA(Transform parent, float a)
    {
        var childs = parent.GetComponentsInChildren<UIWidget>();
        for (var i = 0; i < childs.Count(); i++)
        {
            childs[i].color = new Color(childs[i].color.r, childs[i].color.g, childs[i].color.b, a);
        }
    }

    public override void MYClick(GameObject ui)
    {

    }

    public override void MYMouseOver(GameObject ui)
    {

    }

    public override void MYMouseOut(GameObject ui)
    {

    }

    public override void MYPress(bool isPress, GameObject ui)
    {
        if (isPress)
        {
			m_isMove = false;
			if (Input.GetMouseButton(0))
			{
				m_Vector3MousePos = Input.mousePosition;
			}

            CityGlobalData.m_joystickControl = true;

            m_MouseOrTouch = UICamera.GetTouch(UICamera.currentTouchID);
            if (!m_isLockBack)
            {
                m_joystickBackGround.position = m_currentCamera.ScreenToWorldPoint(m_MouseOrTouch.pos);
                //Check
                m_joystickBackGround.localPosition = new Vector3(
                    m_joystickBackGround.localPosition.x < 76 ? 76 : m_joystickBackGround.localPosition.x,
                    m_joystickBackGround.localPosition.y < 76 ? 76 : m_joystickBackGround.localPosition.y,
                    0);
            }
        }
        else
        {
            CityGlobalData.m_joystickControl = false;

            m_MouseOrTouch = null;
            m_joystickBackGround.localPosition = new Vector3(76, 76, 0);
            m_joystickTransform.localPosition = Vector3.zero;
            m_uiOffset = Vector3.zero;
			m_spriteBG.color = new Color(1f, 1f, 1f, 0.3f);
			m_spriteButton.color = new Color(1f, 1f, 1f, 0.3f);
			if(!m_isMove)
			{
				if (MainCityUI.IsWindowsExist() || UIYindao.m_UIYindao.m_isOpenYindao)
				{
					return;
				}
				
				Vector3 tempMousePosition = Input.mousePosition;

//				Ray m_ray = NpcManager.m_NpcManager.m_nguiCamera.ScreenPointToRay(tempMousePosition);// 从屏幕发射线
//				RaycastHit nguiHit;
				
//				if (Physics.Raycast(m_ray, out nguiHit)) return;//碰到主界面UI按钮
				
				Ray m_ray = Camera.main.ScreenPointToRay(tempMousePosition);
				
				RaycastHit hit;
				int t_index = LayerMask.NameToLayer("CityRoles");

				if (Physics.Raycast(m_ray, out hit, 1000.0f, 1 << t_index))
				{
					if (hit.collider.transform.name.IndexOf("PlayerObject") > -1)
					{
                        EquipSuoData.CreateChaKan(hit.collider.transform.name, tempMousePosition);
						return;
					}
				}
				int  t_index2 = LayerMask.NameToLayer("Default");
				
				int  depth2 = 1 << t_index2;
				
				if (Physics.Raycast(m_ray,out hit, 10000.0f, depth2)) //碰到3d世界中的npc
				{
					NpcCityTemplate m_currentNpcTemplate = null;
					if (hit.collider.transform.name == "NpcInCity")
					{
						PlayerModelController.m_playerModelController.m_agent.enabled = true;
						m_currentNpcTemplate = hit.collider.transform.GetComponent<NpcObjectItem>().m_template;
						
						PlayerModelController.m_playerModelController.m_iMoveToNpcID = m_currentNpcTemplate.m_npcId;
						if (Vector3.Distance(hit.collider.transform.position, PlayerModelController.m_playerModelController.m_ObjHero.transform.position) > 2)
						{
							PlayerModelController.m_playerModelController.SelfNavigation(hit.collider.gameObject.transform.position);
						}
						else
						{
							//  Debug.Log("m_currentNpcTemplate.m_npcId m_currentNpcTemplate.m_npcId  ::" + m_currentNpcTemplate.m_npcId);
							if (m_currentNpcTemplate.m_npcId == 801)
							{
								NewEmailData.Instance().OpenEmail(0);
							}
							else
							{
								PlayerModelController.m_playerModelController.TidyNpcInfo();
							}
						}
						
					}
				}
			}
			m_isMove = false;
        }
    }

    public override void MYelease(GameObject ui)
    {

    }

    public override void MYondrag(Vector2 delta)
    {

    }

    public override void MYoubleClick(GameObject ui)
    {

    }

    public override void MYonInput(GameObject ui, string c)
    {

    }
}
