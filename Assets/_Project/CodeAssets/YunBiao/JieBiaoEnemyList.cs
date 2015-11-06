using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JieBiaoEnemyList : MonoBehaviour,SocketProcessor {

	public static JieBiaoEnemyList jbEnemy;
	/// <summary>
	/// The enemy grid.
	/// </summary>
	public GameObject enemyGrid;
	/// <summary>
	/// 仇人item
	/// </summary>
	public GameObject enemyItemObj;
	/// <summary>
	/// 仇家个数
	/// </summary>
	public UILabel numLabel;
	/// <summary>
	/// 没有仇家
	/// </summary>
	public UILabel desLabel;
	/// <summary>
	/// 规则说明
	/// </summary>
	public UILabel rulesLabel;
	/// <summary>
	/// The enemies list.
	/// </summary>
	private List<GameObject> enemiesList = new List<GameObject>();
	/// <summary>
	/// 要打劫的君主信息
	/// </summary>
	private EnemiesInfo selectEInfo;
	/// <summary>
	/// 可否打劫
	/// </summary>
	private bool canJieBiao;
	/// <summary>
	/// 选择的君主id
	/// </summary>
	private long enemyId;

	public ScaleEffectController sEffectControl;

	private string titleStr;
	private string str;

	void Awake ()
	{
		jbEnemy = this;

		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		canJieBiao = false;

		sEffectControl.OnOpenWindowClick ();

		JbEnemiesListReq ();
		StartCoroutine (InfoReq (10));
	}

	IEnumerator InfoReq (int time)
	{
		while (time > 0) 
		{
			time --;

			if (time == 0) 
			{
				JbEnemiesListReq ();

				time = 10;
			}

			yield return new WaitForSeconds (1);
		}
	}


	/// <summary>
	/// 仇人列表请求
	/// </summary>
	void JbEnemiesListReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_ENEMY_RSQ,"3424");
		Debug.Log ("仇人列表请求：" + ProtoIndexes.C_YABIAO_ENEMY_RSQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YABIAO_ENEMY_RESP://仇人信息返回
			{
				Debug.Log ("仇人列表返回：" + ProtoIndexes.S_YABIAO_ENEMY_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				EnemiesResp jbEnemiesRes = new EnemiesResp();
				
				t_qx.Deserialize(t_stream, jbEnemiesRes, jbEnemiesRes.GetType());

				if (jbEnemiesRes != null)
				{
					if (jbEnemiesRes.enemyList == null)
					{
						jbEnemiesRes.enemyList = new List<EnemiesInfo>();
					}

					InItEnemyPage (jbEnemiesRes);
				}

				return true;
			}
			}
		}

		return false;
	}

	/// <summary>
	/// 初始化仇家页面
	/// </summary>
	void InItEnemyPage (EnemiesResp tempResp)
	{
		numLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_71) + tempResp.enemyList.Count + "/50";//仇家

		YunBiaoMainPage.yunBiaoMainData.MoveBackToTop (enemyGrid,tempResp.enemyList.Count,3);

		if (tempResp.enemyList.Count > 0)
		{
			desLabel.text = "";

			ClearItems ();

			for (int i = 0;i < tempResp.enemyList.Count;i ++)
			{
				GameObject enemyItem = (GameObject)Instantiate (enemyItemObj);

				enemyItem.SetActive (true);
				enemyItem.transform.parent = enemyGrid.transform;

				enemyItem.transform.localPosition = Vector3.zero;
				enemyItem.transform.localScale = Vector3.one;

				EnemyItemInfo enemyInfo = enemyItem.GetComponent<EnemyItemInfo> ();
				enemyInfo.GetEnemyInfo (tempResp.enemyList[i],enemyId);

				enemiesList.Add (enemyItem);
			}

			enemyGrid.GetComponent<UIGrid> ().repositionNow = true;
		}
		else
		{
			ClearItems ();

			desLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_77);//还无人劫镖
		}

		string ruleStr = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_72);//规则说明
		string[] ruleStrLength = ruleStr.Split ('：');

		rulesLabel.text = ruleStrLength[0] + "：\n       " + ruleStrLength[1];
	}

	/// <summary>
	/// 获得要打劫的君主信息
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void GetSelectEnemyInfo (EnemiesInfo tempInfo)
	{
		selectEInfo = tempInfo;

		enemyId = tempInfo.junZhuId;

		canJieBiao = true;

		foreach (GameObject obj in enemiesList)
		{
			EnemyItemInfo enemy = obj.GetComponent<EnemyItemInfo> ();

			if (enemy.enemyInfo.junZhuId == tempInfo.junZhuId)
			{
				enemy.selectBoxObj.SetActive (true);
			}
			else
			{
				enemy.selectBoxObj.SetActive (false);
			}
		}
	}
		
	/// <summary>
	/// 劫镖按钮
	/// </summary>
	public void JieBiaoBtn ()
	{
		if (YunBiaoData.Instance.yunBiaoRes.isOpen)
		{
			if (canJieBiao)
			{
				if (selectEInfo.eRoomId == -1)
				{
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        FailJieBiaoLoadBack );
				}
				else
				{
					//进入劫镖场景
					CarriageSceneManager.Instance.EnterCarriage (selectEInfo.eRoomId);
				}
			}
			else
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        FailJieBiaoLoadBack );
			}
		}
		else
		{
			YunBiaoMainPage.yunBiaoMainData.YunBiaoNotOpen ();
		}
	}

	void FailJieBiaoLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		titleStr = "提示";

		str = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_76);//请选择一个正在运镖的仇家

		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, YunBiaoMainPage.yunBiaoMainData.confirmStr, null,
		             null);
	}

	/// <summary>
	/// Clears the items.
	/// </summary>
	void ClearItems ()
	{
		foreach (GameObject obj in enemiesList)
		{
			Destroy (obj);
		}
		enemiesList.Clear ();
	}

	public void CloseBtn ()
	{
		sEffectControl.CloseCompleteDelegate = CloseBtnCallBack;
		sEffectControl.OnCloseWindowClick ();
	}

	void CloseBtnCallBack ()
	{
		Destroy (this.gameObject);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
