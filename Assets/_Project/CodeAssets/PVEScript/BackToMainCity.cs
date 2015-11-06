using UnityEngine;
using System.Collections;

public class BackToMainCity : MonoBehaviour {

void OnClick()
	{

//		CityGlobalData.m_nextSceneName = ConstInGame.CONST_SCENE_NAME_MAIN_CITY;
//		
//		Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );

		SceneManager.EnterMainCity();
	}
}
