using UnityEngine;
using System.Collections;

public class BattlePauseControllor : MonoBehaviour 
{
	public UILabel label;

	public UILabel labelDesc;


	public void refreshData()
	{
		label.text = LanguageTemplate.GetText ((LanguageTemplate.Text)543);

//		int descLanguageId = 0;
//
//		string strNum = "";
//
//		BattleWinTemplate winDescTemplate = BattleUIControlor.Instance().winDescTemplate;
//
//		if(winDescTemplate == null)
//		{
//			winDescTemplate = BattleWinTemplate.templates[0];
//		}
//
//		if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_All)
//		{
//			descLanguageId = 1082;
//			
//			strNum = "";
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Boss)
//		{
//			descLanguageId = 1083;
//			
//			strNum = BattleControlor.Instance().battleCheck.bossKilled + "/" + winDescTemplate.killNum;
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Hero)
//		{
//			descLanguageId = 1092;
//			
//			strNum = BattleControlor.Instance().battleCheck.heroKilled + "/" + winDescTemplate.killNum;
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Soldier)
//		{
//			descLanguageId = 1090;
//			
//			strNum = BattleControlor.Instance().battleCheck.soldierKilled + "/" + winDescTemplate.killNum;
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Gear)
//		{
//			descLanguageId = 1088;
//			
//			strNum = BattleControlor.Instance().battleCheck.gearKilled + "/" + winDescTemplate.killNum;
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Reach_Destination)
//		{
//			descLanguageId = 1085;
//			
//			strNum = ((int)Vector3.Distance(BattleControlor.Instance().getKing().transform.position, winDescTemplate.destination) - winDescTemplate.destinationRadius) + "m";
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Reach_Time)
//		{
//			descLanguageId = 1086;
//			
//			strNum = BattleControlor.Instance().timeLast + "s";
//		}
//		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Wave)
//		{
//			descLanguageId = 1088;
//			
//			strNum = BattleControlor.Instance().battleCheck.waveKilled + "/" + winDescTemplate.killNum;
//		}
//		
//		labelDesc.text = LanguageTemplate.GetText (descLanguageId) + " " + strNum;

		BattleConfigTemplate configTemplate = BattleConfigTemplate.getBattleConfigTemplateByConfigId (CityGlobalData.m_configId);

		labelDesc.text = DescIdTemplate.GetDescriptionById (configTemplate.preDesc);
	}

	public void close()
	{
		TimeHelper.SetTimeScale(1f);

		gameObject.SetActive(false);
	}

	public void Lose()
	{
		close ();

		BattleUIControlor.Instance().devolopmentLose ();
	}

	public void runaway()
	{
		TimeHelper.SetTimeScale(1f);

		GameObject root3d = GameObject.Find ("BattleField_V4_3D");
		
		GameObject root2d = GameObject.Find ("BattleField_V4_2D");

		Destroy (root3d);

		Destroy (root2d);

//		BattleNet bn = (BattleNet)BattleControlor.Instance().gameObject.GetComponent ("BattleNet");

		//SceneManager.EnterMainCity();

        //if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
        //{
            SceneManager.EnterMainCity();
        //}
        //else
        //{
        //    SceneManager.EnterAllianceCity();
        //}

//		Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );
	}

}
