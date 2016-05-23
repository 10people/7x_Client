using UnityEngine;
using System.Collections;

public class MuBiaoItem : MonoBehaviour {

	public UILabel mName;
	public UILabel mDesc;
	public GameObject GoButn;

	public GameObject mMenbers;

	public UILabel Menbers;
    bool IsReCret;
	[HideInInspector]public int Id;

	//public GameObject AllUIroot;
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
		LMTargetTemplate mLMTargetTemplate = LMTargetTemplate.getLMTargetTemplate_by_Id (Id);

		mDesc.text = mLMTargetTemplate.condition;
		mName.text = mLMTargetTemplate.Name;
	}
	public void GoToNewUI()
	{
//		<LMTarget id="1" title="联盟图腾" desc="进行联盟膜拜，或者基础捐献" />
//			<LMTarget id="2" title="荒野求生" desc="联盟成员一起努力攻克荒野关卡" />
//				<LMTarget id="3" title="掠夺" desc="成功掠夺其他联盟玩家，并提高联盟积分排行名次" />
//				<LMTarget id="4" title="运镖" desc="每次成功运镖可以增加联盟建设值" />

		switch(Id)
		{
		case 1 :

			Enter_MoBai();

			break;
		case 3 :

			PlunderData.Instance.OpenPlunder();
		   
			break;
		case 2 :

			if (AllianceData.Instance.g_UnionInfo.level >= 2)
			{
				MiBaoGlobleData.Instance().OpenHYMap_UI();
			}
			else
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CantEnterOpenLockLoadBack);
			}
//			if(!FunctionOpenTemp.GetWhetherContainID(106))
//			{
//				ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"每日任务未开启！"));
//				return;
//			}
//
//			TaskData.Instance.m_ShowType = 2;
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TASK),
//			                        TaskLoadCallback);
			break;
		case 4 :
			EnterYABiao();
			break;
		default:
			break;
		}
	}
	void CantEnterOpenLockLoadBack(ref WWW p_www, string p_path, Object p_object)
	{

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str2 = "";
		
		string str1 = "\r\n" + "联盟等级到达2级才能进入荒野求生！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);

		string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr, MyColorData.getColorString(1, str1 + str2), null, null, confirmStr, null, null, null, null);
	}
	void OPenUIFailed(ref WWW p_www,string p_path, Object p_object)
	{

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "提示";
		
		string str1 = "每日任务未开启";
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null,str1,null,confirmStr,null,null,null,null);
	}

	public void EnterYABiao()
	{
		if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(310))
		{
			PlayerSceneSyncManager.Instance.EnterCarriage();
		}
		else
		{
			FunctionWindowsCreateManagerment.ShowUnopen(310);
		}

	}
	public void Enter_MoBai() // 进入膜拜界面  直接复制TutengLev膜拜等级
	{
		WorshipLayerManagerment.m_bulidingLevel = NewAlliancemanager.Instance().TutengLev;
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
		                        WorshipLayerLoadCallback);
		
	}
	private static void WorshipLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObject);
	}
	public void TaskLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject AlianceRoot = GameObject.Find ("New_My_Union(Clone)");
		GameObject AlianceMuBiaoRoot = GameObject.Find ("Leader_Setting(Clone)");
		MainCityUI.TryRemoveFromObjectList (AlianceRoot);
		MainCityUI.TryRemoveFromObjectList (AlianceMuBiaoRoot);
		GameObject tempObject = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(tempObject);
		UIYindao.m_UIYindao.CloseUI();
		Destroy (AlianceRoot);
		Destroy (AlianceMuBiaoRoot);
	}

}
