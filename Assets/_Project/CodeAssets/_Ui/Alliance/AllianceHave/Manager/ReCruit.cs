using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ReCruit : MonoBehaviour, SocketProcessor {

	//[HideInInspector]public  AllianceHaveResp Z_UnionInfo;

	private OpenApplyResp openRes;

	public GameObject Level_LeftBtn;
	public GameObject Level_RightBtn;
	public GameObject JunXian_LeftBtn;
	public GameObject JunXian_RightBtn;
	public GameObject ShenPi_LeftBtn;
	public GameObject ShenPi_RightBtn;

	public GameObject input_Notice;
	public UILabel Show_Notice;

	public UILabel m_Lv;
	public UILabel m_Junxian;
	public UILabel needShenpi;

	public UILabel inputInfo;
	[HideInInspector]
	public int mlv_min  = 20;
	[HideInInspector]
	public int mlv_Max  = 100;
	[HideInInspector]
	public int mjunxian_min  = 1;
	[HideInInspector]
	public int mjunxian_Max  = 8;
	[HideInInspector]
	public  int  mlv  = 20;
	[HideInInspector]
	public int mjunxian = 1;
	[HideInInspector]
	public int  needSp = 1;

	//public UILabel Notice_Text;
	
	public UILabel inputNotice;
	public int CharNumber;
	
	public GameObject InputNumber;//输入框
	
	public GameObject ComfrimInput;//确认输入按钮

	public UILabel ReCruitBtnLabel;

	public GameObject SaveRecuiBtn;//确认输入按钮

	private string jieSanTitleStr;
	private string closeTitleStr;
	private string confirmStr;
	private string cancelStr;

	public GameObject EditBtn;
	void Awake()
	{ 

		SocketTool.RegisterMessageProcessor(this);
	}
	void Start () {
   
		jieSanTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
		closeTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_TITLE);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}
	
	
	void Update () {
		
		if(input_Notice.activeSelf)
		{
			UIInput minput = InputNumber.GetComponent<UIInput>();
			if(inputNotice.text.Length > 0)
			{
				ComfrimInput.SetActive(true);
			}
			else
			{
				ComfrimInput.SetActive(false);
			}
//			CharNumber = minput.characterLimit - inputNotice.text.Length;
//			if(CharNumber >= 0){
//				Notice_Text.text = CharNumber.ToString();
//			}
			
		}
	}
	public void  initLevel()
	{
		mlv  = 20;
		mlv_min  = 20;
		mlv_Max  = 100;
		mjunxian = 1;
		needSp = 1;
		mjunxian_Max  = 8;
		mjunxian_min  = 1;
	}
	public void init()
	{
	
		Level_LeftBtn.SetActive(true);
		Level_RightBtn.SetActive(true);
		JunXian_LeftBtn.SetActive(true);
		JunXian_RightBtn.SetActive(true);
		ShenPi_LeftBtn.SetActive(true);
		ShenPi_RightBtn.SetActive(true);
		//Debug.Log ("mlv "+mlv);
		//Debug.Log ("mjunxian "+mjunxian);
		//Debug.Log ("needSp "+needSp);
		if(mlv == mlv_min)
		{
			
			Level_LeftBtn.SetActive(false);
		}
		if(mlv == 100)
		{
			
			Level_RightBtn.SetActive(false);
		}
		if(mjunxian == 1)
		{
			
			JunXian_LeftBtn.SetActive(false);
		}
		if(mjunxian == 8)
		{
			
			JunXian_RightBtn.SetActive(false);
		}
		if(needSp == 0)
		{
			
			ShenPi_RightBtn.SetActive(false);
		}
		if(needSp == 1)
		{
			
			ShenPi_LeftBtn.SetActive(false);
		}
		m_Lv.text = mlv.ToString ();
		BaiZhanTemplate mBaiZhanTemplate = BaiZhanTemplate.getBaiZhanTemplateById (mjunxian);
		string mJX = NameIdTemplate.GetName_By_NameId (mBaiZhanTemplate.funDesc);
		m_Junxian.text = mJX;
		if(needSp == 0)
		{
			needShenpi.text = LanguageTemplate.GetText (LanguageTemplate.Text.YES);
		}else{
			needShenpi.text = LanguageTemplate.GetText (LanguageTemplate.Text.NO);
		}

	}
	AllianceHaveResp mAllianceHaveResp;
	public void ChangeNum()
	{
		mAllianceHaveResp = NewAlliancemanager.Instance ().m_allianceHaveRes;
		if(mAllianceHaveResp.attchCndition == null)
		{
			Debug.Log ("mAllianceHaveResp.attchCndition =  null");
		}
		else
		{
			Debug.Log ("mAllianceHaveResp.attchCndition =  "+mAllianceHaveResp.attchCndition);
		}


		if(mAllianceHaveResp.isAllow == 1) //
		{
			mlv = mAllianceHaveResp.applyLevel;
			mjunxian = mAllianceHaveResp.junXian;
			needSp = mAllianceHaveResp.isShenPi;
			inputInfo.text = mAllianceHaveResp.attchCndition;
			Show_Notice.text = inputInfo.text;
			ReCruitBtnLabel.text = "关闭招募";
			
			SaveRecuiBtn.SetActive(true);
		}
		else{
			ReCruitBtnLabel.text = "开启招募";
			
			SaveRecuiBtn.SetActive(false);
		}
		if(mAllianceHaveResp.identity == 2)
		{
			EditBtn.SetActive(true);
		}
		else
		{
			EditBtn.SetActive(false);
		}
	}
	public void ShenPiLeftBtn()//审批的控制左边按钮
	{
		if(needSp != 1)
		{
			needSp = 1;
			init ();
		}

	}
	public void ShenPiRightBtn()//审批的控制右边按钮
	{
		if(needSp != 0)
		{
			needSp = 0;
			init ();
		}

	}
	public void WriteingNotice()//编写公告
	{
		input_Notice.SetActive (true);
	}
	public void CloseinputNotice()//编写公告
	{
		input_Notice.SetActive (false);
	}
	public void WriteingComfrim()
	{
		Show_Notice.text = inputInfo.text;
		input_Notice.SetActive (false);
	}


	public void BackBtn()//back按钮
	{
		Destroy (this.gameObject);
	}
	public void CancleBtn()//取消按钮
	{
		Destroy (this.gameObject);
	}
	public bool IsSava = true;
	public void All_ComfrimBtm()//确定发布按钮
	{
		if (mAllianceHaveResp.isAllow == 1) {

			CloseReCruit ();
		} else {
		
			IsSava = false;

			ComfrimBtm();

		}
	}
	public void SavaComf()//确定发布按钮
	{
		IsSava = true;

		ComfrimBtm ();
	}
	public void ComfrimBtm()//确定发布按钮
	{
		OpenApply mOpenApply = new OpenApply ();
		MemoryStream mOpenApplyStream = new MemoryStream ();
		
		QiXiongSerializer mOpenApplyer = new QiXiongSerializer ();
	
		mOpenApply.id = mAllianceHaveResp.id;
		mOpenApply.levelMin = mlv;
		mOpenApply.junXianMin = mjunxian;
		mOpenApply.isExamine = needSp;
		mOpenApply.attach = Show_Notice.text;
		mOpenApplyer.Serialize (mOpenApplyStream,mOpenApply);
		
		byte[] t_protof;
		
		t_protof = mOpenApplyStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.OPEN_APPLY,ref t_protof,"30134");
	}
	public void CloseReCruit()
	{
		CloseApply closeReq = new CloseApply ();
		
		closeReq.id = mAllianceHaveResp.id;
		
		MemoryStream closeStream = new MemoryStream ();
		
		QiXiongSerializer closeQx = new QiXiongSerializer ();
		
		closeQx.Serialize (closeStream, closeReq);
		
		byte[] t_protof = closeStream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.CLOSE_APPLY, ref t_protof, "30136");
	
	}
	public bool OnProcessSocketMessage(QXBuffer p_message){//接收Union求返回的数据
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				
			case ProtoIndexes.OPEN_APPLY_RESP:// z招募信息发布返回 。。。。。。。。。
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				OpenApplyResp openApplyRes = new OpenApplyResp();
				
				t_qx.Deserialize(t_stream, openApplyRes, openApplyRes.GetType());

				if (openApplyRes != null)
				{
					openRes = openApplyRes;

					if (openApplyRes.code == 0)
					{
						//SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);//刷新页面
						AllianceData.Instance.RequestData ();
						GameObject leaderSetObj = GameObject.Find("AllaniceApply");
						if(leaderSetObj)
						{
							ApplyManager mLeaderSetting = leaderSetObj.GetComponent<ApplyManager>();
							mLeaderSetting.m_tempInfo.isAllow = 1;
						}

					}
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ResourceLoadCallback );

					Destroy(this.gameObject);

				}
				return true;
			}
			case ProtoIndexes.CLOSE_APPLY_OK://关闭招募信息返回
			{
				Debug.Log ("guanbi:" + ProtoIndexes.CLOSE_APPLY_OK);
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        CloseRecuritLoadCallback );
				
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
				
				GameObject leaderSetObj = GameObject.Find("AllaniceApply");
				if(leaderSetObj)
				{
					ApplyManager mLeaderSetting = leaderSetObj.GetComponent<ApplyManager>();
					mLeaderSetting.m_tempInfo.isAllow = 0;
				}

				Destroy(this.gameObject);
				return true;
			}


			default: return false;
			}
		}
		
		return false;
	}
	//关闭招募返回异步加载回调
	public void CloseRecuritLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{	
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_SUCCESS);
		uibox.setBox(closeTitleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null);
	}
	public void ResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		string str = "";
		
		string titleStr = "";
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);;

		switch (openRes.code)
		{
		case 0:

			if(!IsSava)
			{
				str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_RUCRUIT_TIPS_SUCCESS);
				
				titleStr = titleStr = "发布成功";
			}else
			{
				str = "联盟招募信息保存成功";;// LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_RUCRUIT_TIPS_SUCCESS);
				
				titleStr = "发布成功";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_RUCRUIT_TITLE);
			}


			break;

		case 1:

			titleStr = "招募失败";

			str = "您的联盟已经达到人数上限，暂时无法加入新玩家！";

			break;

		case 2:

			titleStr = "招募失败";

			str = "公告字数超出限制，请重新编辑公告！";

			break;

		default:break;
		}

		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
