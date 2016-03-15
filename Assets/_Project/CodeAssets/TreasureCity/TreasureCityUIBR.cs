using UnityEngine;
using System.Collections;

public class TreasureCityUIBR : MonoBehaviour {

	public EventHandler openBoxHandler;

	private bool isActive = false;

	void Start ()
	{
		openBoxHandler.m_click_handler += OpenBoxHandlerClickBack;
	}

	void OpenBoxHandlerClickBack (GameObject obj)
	{
		if (isActive)
		{
			if (TreasureCityPlayer.m_instance.TargetBoxUID < 0)
			{
				ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"正在开启宝箱！"));
				return;
			}
			TCityPlayerManager.m_instance.OpenTreasureBox (TreasureCityPlayer.m_instance.TargetBoxUID);
		}
		else
		{
			//没有可开启的箱子
			ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"没有可开启的宝箱！"));
		}
	}

	/// <summary>
	/// Sets the state of the open box button.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	public void SetOpenBoxBtnState (bool active)
	{
		isActive = active;

		NGUIButtonEffectController nguiBtn = openBoxHandler.GetComponent<NGUIButtonEffectController> ();
		nguiBtn.IsOpenColorEffect = isActive;

		UIWidget widget = openBoxHandler.GetComponent<UIWidget> ();
		widget.color = active ? Color.white : Color.grey;
	}
}
