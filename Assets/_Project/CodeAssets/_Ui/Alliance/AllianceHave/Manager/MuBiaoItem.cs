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
		if (mLMTargetTemplate.Name == "招募") {
			IsReCret = true;
		} else {
			IsReCret = false;
		}
		if(IsReCret)
		{
			mMenbers.SetActive(true);

			if(NewAlliancemanager.Instance().m_allianceHaveRes.identity == 2)
			{
				GoButn.SetActive(true);
			}
			else
			{
				GoButn.SetActive(false);
			}
			Menbers.text = NewAlliancemanager.Instance().m_allianceHaveRes.members.ToString()+"/"+NewAlliancemanager.Instance().m_allianceHaveRes.memberMax.ToString();
		}
		else
		{
			GoButn.SetActive(true);
			mMenbers.SetActive(false);
		}
		mDesc.text = mLMTargetTemplate.condition;
		mName.text = mLMTargetTemplate.Name;
	}
	public void GoToNewUI()
	{
//		<LMTarget id="1" title="联盟膜拜" desc="进行联盟膜拜，增加联盟建设值和联盟经验" />
//			<LMTarget id="2" title="每日任务" desc="达成联盟封禅所需活跃度，封禅增加联盟建设值和联盟经验" />
//				<LMTarget id="3" title="掠夺" desc="成功掠夺敌对国玩家，增加联盟建设值和联盟经验" />
//				<LMTarget id="4" title="招募" desc="尽快提升联盟人数，为联盟招募伙伴" />

		switch(Id)
		{
		case 1 :

			Enter_MoBai();

			break;
		case 3 :

			PlunderData.Instance.OpenPlunder();
		   
			break;
		case 2 :

			TaskData.Instance.m_ShowType = 2;
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TASK),
			                        TaskLoadCallback);
			break;
		case 5 :
			ReCruitSetting();
			break;
		case 4 :
			EnterYABiao();
			break;
		default:
			break;
		}
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
		GameObject tempObject = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(tempObject);
		UIYindao.m_UIYindao.CloseUI();
	}

	GameObject OpenRecruit;
	public void ReCruitSetting()
	{
		if(OpenRecruit)
		{
			return;
		}
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_RECRUIT ),
		                        RecruitLoadCallback );
	}
	
	//联盟招募异步加载回调
	public void RecruitLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{	
		OpenRecruit = Instantiate( p_object ) as GameObject;
		OpenRecruit.transform.parent = GameObject.Find("Leader_Setting(Clone)").transform;
		OpenRecruit.transform.localScale = Vector3.one;
		OpenRecruit.transform.localPosition = Vector3.zero;
		ReCruit mReCruit = OpenRecruit.GetComponent<ReCruit>();
		//mReCruit.Z_UnionInfo = m_tempInfo;
		mReCruit.initLevel ();
		mReCruit.ChangeNum ();
		mReCruit.init ();
	}
}
