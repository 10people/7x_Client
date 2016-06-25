using UnityEngine;
using System.Collections;

public class CameraDivisionUI : MonoBehaviour 
{
	public GameObject btnInit;

	public GameObject btnClose;

	public GameObject btnFx;

	public GameObject btnScene;

	public GameObject btnModel;

	public GameObject btnPlayer;

	public GameObject btnUI3D;


	void Awake()
	{
		if( ConfigTool.GetBool( ConfigTool.CONST_SHOW_BATTLE_CAMERA_OPS) == false )
		{
			gameObject.SetActive(ConfigTool.GetBool( ConfigTool.CONST_SHOW_CAMERA_DIVITION_OPS));
		}
	}

	public void init()
	{
		if (BattleControlor.Instance().result != BattleControlor.BattleResult.RESULT_BATTLING) return;

		CameraDivisionControllor.Instance ().init ();

		btnInit.SetActive (false);

		btnClose.SetActive (true);

		btnFx.SetActive (true);

		btnScene.SetActive (true);

		btnModel.SetActive (true);

		btnPlayer.SetActive (true);

		btnUI3D.SetActive (true);

		TimeHelper.SetTimeScale (0f);
	}

	public void close()
	{
		TimeHelper.SetTimeScale (1f);

		btnInit.SetActive (true);
		
		btnClose.SetActive (false);
		
		btnFx.SetActive (false);
		
		btnScene.SetActive (false);
		
		btnModel.SetActive (false);
		
		btnPlayer.SetActive (false);
		
		btnUI3D.SetActive (false);

		CameraDivisionControllor.Instance ().end ();
	}

	public void onFx()
	{
		CameraDivisionControllor.Instance ().changeLayer (CameraDivisionControllor.LayerIndex.Fx);
	}

	public void onScene()
	{
		CameraDivisionControllor.Instance ().changeLayer (CameraDivisionControllor.LayerIndex.Ground);
	}

	public void onModel()
	{
		CameraDivisionControllor.Instance ().changeLayer (CameraDivisionControllor.LayerIndex.Model);
	}

	public void onPlayer()
	{
		CameraDivisionControllor.Instance ().changeLayer (CameraDivisionControllor.LayerIndex.Player);
	}

	public void onUI3D()
	{
		CameraDivisionControllor.Instance ().changeLayer (CameraDivisionControllor.LayerIndex.UI3D);
	}

}
