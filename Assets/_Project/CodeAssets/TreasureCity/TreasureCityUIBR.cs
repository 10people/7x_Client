using UnityEngine;
using System.Collections;

public class TreasureCityUIBR : GeneralInstance<TreasureCityUIBR> {

	public EventHandler openBoxHandler;

	private bool isActive = false;

	void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		SetOpenBoxBtnState (false);
		openBoxHandler.m_click_handler += OpenBoxHandlerClickBack;
	}

	void OpenBoxHandlerClickBack (GameObject obj)
	{
//		TreasureCityUI.m_instance.TopUIMsg ("hahahahhahahahahahah");
		if (isActive)
		{
			if (TreasureCityData.Instance ().CanGetBoxCount () <= 0)
			{
				ClientMain.m_UITextManager.createText (MyColorData.getColorString (4,"今日您可抢的宝箱数已为[d80202]0[-]，明天再来吧。"));
				return;
			}

			TCityPlayerManager.m_instance.OpenBox ();
		}
		else
		{
			//没有可开启的箱子
			ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"请走到周围的宝箱附近！"));
		}
	}

	/// <summary>
	/// Sets the state of the open box button.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	public void SetOpenBoxBtnState (bool active)
	{
		isActive = active;

		SetBtnEffect (active);

		openBoxHandler.GetComponent<UIPlaySound> ().enabled = active;

		UIWidget widget = openBoxHandler.GetComponent<UIWidget> ();
		widget.color = active ? Color.white : Color.grey;
	}

	private bool openWindow = false;
	public bool OpenWindow { set{openWindow = value;} get{return openWindow;} }//是否打开功能界面

	/// <summary>
	/// Sets the button effect.
	/// </summary>
	private void SetBtnEffect (bool isActive)
	{
		if (!OpenWindow)
		{
			if (isActive)
			{
//				Debug.Log (UI3DEffectTool.HaveAnyFx (openBoxHandler.gameObject));
				if (!UI3DEffectTool.HaveAnyFx (openBoxHandler.gameObject))
				{
					QXComData.InstanceEffect (QXComData.EffectPos.BOTTOM,openBoxHandler.gameObject,600154);
				}
			}
			else
			{
				ClearBtnEffect ();
			}
		}
		else
		{
			ClearBtnEffect ();
		}
	}

	/// <summary>
	/// Clears the button effect.
	/// </summary>
	public void ClearBtnEffect ()
	{
		QXComData.ClearEffect (openBoxHandler.gameObject);
	}

	void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
