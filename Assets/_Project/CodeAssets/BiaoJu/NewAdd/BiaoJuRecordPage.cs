using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

namespace Carriage
{
	public class BiaoJuRecordPage : MonoBehaviour {

		public static BiaoJuRecordPage bjRecordPage;

		private BiaoJuRecordData.RecordType recordType;

		public GameObject historyObj;
		public GameObject enemyPageObj;

		public GameObject historyBtnObj;
		public GameObject enemyBtnObj;

		public UILabel noRecordLabel;
		public UILabel recordNumLabel;
		public UILabel recordDesLabel;

		public List<EventHandler> recordHandlerList = new List<EventHandler> ();

		private float itemDis = 87;

		#region BiaoJuHistory
		private YBHistoryResp historyResp;
		
		public UIScrollView historySc;
		public UIScrollBar historySb;

		public GameObject historyItemObj;
		private List<GameObject> historyItemList = new List<GameObject> ();
		#endregion

		#region BiaoJuEnemy
		private EnemiesResp enemyResp;
		
		public UIScrollView enemySc;
		public UIScrollBar enemySb;
		
		public GameObject enemyItemObj;
		private List<GameObject> enemyItemList = new List<GameObject> ();
	
		public UILabel enemyRules;
		private float cdTime = 10;
		#endregion

		void Awake ()
		{
			bjRecordPage = this;
		}

		void Start ()
		{
			foreach (EventHandler handler in recordHandlerList)
			{
				handler.m_click_handler -= RecordHandlerClickBack;
				handler.m_click_handler += RecordHandlerClickBack;
			}
		}

		void OnDestroy ()
		{
			bjRecordPage = null;
		}

		/// <summary>
		/// Sets the record page.
		/// </summary>
		/// <param name="tempType">Temp type.</param>
		public void SetRecordPage (BiaoJuRecordData.RecordType tempType)
		{
			recordType = tempType;

			SetBtnState (historyBtnObj,recordType == BiaoJuRecordData.RecordType.HISTORY ? true : false);
			SetBtnState (enemyBtnObj,recordType == BiaoJuRecordData.RecordType.ENEMY ? true : false);
			
			historyObj.SetActive (tempType == BiaoJuRecordData.RecordType.HISTORY ? true : false);
			enemyPageObj.SetActive (tempType == BiaoJuRecordData.RecordType.ENEMY ? true : false);
		}

		/// <summary>
		/// Ins it history page.
		/// </summary>
		/// <param name="tempType">Temp type.</param>
		/// <param name="tempResp">Temp resp.</param>
		public void InItHistoryPage (YBHistoryResp tempResp)
		{
			StopCoroutine ("EnemyPageRefresh");

			PushAndNotificationHelper.SetRedSpotNotification (313,false);

			historyResp = tempResp;

			historyItemList = QXComData.CreateGameObjectList (historyItemObj,tempResp.historyList.Count,historyItemList);

			for (int i = 0;i < tempResp.historyList.Count - 1;i ++)
			{
				for (int j = 0;j < tempResp.historyList.Count - i - 1;j ++)
				{
					if (tempResp.historyList[j].time < tempResp.historyList[j + 1].time)
					{
						YBHistory tempHis = tempResp.historyList[j];
						tempResp.historyList[j] = tempResp.historyList[j + 1];
						tempResp.historyList[j + 1] = tempHis;
					}
				}
			}
			
			for (int i = 0;i < historyItemList.Count;i ++)
			{
				historyItemList[i].transform.localPosition = new Vector3(0,-i * itemDis,0);
				historySc.UpdateScrollbars (true);
				BiaoJuRecordItem record = historyItemList[i].GetComponent<BiaoJuRecordItem> ();
				record.InItRecordItem (tempResp.historyList[i]);
			}

			historySc.enabled = tempResp.historyList.Count > 4 ? true : false;
			historySb.gameObject.SetActive (tempResp.historyList.Count > 4 ? true : false);

			noRecordLabel.text = historyItemList.Count > 0 ? "" : "劫镖记录为空";
			recordDesLabel.text = "超出条目部分会自动移除";
			recordNumLabel.text = "条数" + tempResp.historyList.Count + "/50";
		}

		/// <summary>
		/// Ins it enemy page.
		/// </summary>
		/// <param name="tempType">Temp type.</param>
		/// <param name="tempResp">Temp resp.</param>
		public void InItEnemyPage (EnemiesResp tempResp)
		{
			enemyResp = tempResp;
		
			PushAndNotificationHelper.SetRedSpotNotification (315,false);

			enemyItemList = QXComData.CreateGameObjectList (enemyItemObj,tempResp.enemyList.Count,enemyItemList);

			for (int i = 0;i < enemyItemList.Count;i ++)
			{
				enemyItemList[i].transform.localPosition = new Vector3(0,-i * itemDis,0);
				enemySc.UpdateScrollbars (true);
				BiaoJuEnemyItem enemy = enemyItemList[i].GetComponent<BiaoJuEnemyItem> ();
				enemy.InItEnemyItem (tempResp.enemyList[i]);
			}

			enemySc.enabled = tempResp.enemyList.Count > 3 ? true : false;
			enemySb.gameObject.SetActive (tempResp.enemyList.Count > 3 ? true : false);

			string enemyRule = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_72);//规则说明
			string[] enemyRuleLength = enemyRule.Split ('：');
			enemyRules.text = MyColorData.getColorString (41,enemyRuleLength[0] + "：\n       " + enemyRuleLength[1]);

			noRecordLabel.text = enemyItemList.Count > 0 ? "" : LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_77);//还无人劫镖
			recordDesLabel.text = "一周未运镖的仇家将自动于记录内移除";
			recordNumLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_71) + tempResp.enemyList.Count + "/50";//仇家

			cdTime = 10;
			StartCoroutine ("EnemyPageRefresh");
		}

		IEnumerator EnemyPageRefresh ()
		{
			while (cdTime > 0)
			{
				cdTime --;

				yield return new WaitForSeconds (1);

				if (cdTime == 0)
				{
					BiaoJuRecordData.Instance.BiaoJuRecordReq (BiaoJuRecordData.RecordType.ENEMY);
				}
			}
		}

		void RecordHandlerClickBack (GameObject obj)
		{
			switch (obj.name)
			{
			case "CloseBtn":

				CloseRecord ();

				break;
			case "ZheZhao":

				CloseRecord ();

				break;
			case "HistoryBtn":

				StopCoroutine ("EnemyPageRefresh");
				if (recordType != BiaoJuRecordData.RecordType.HISTORY)
				{
					BiaoJuRecordData.Instance.BiaoJuRecordReq (BiaoJuRecordData.RecordType.HISTORY);
				}

				break;
			case "EnemyBtn":

				if (recordType != BiaoJuRecordData.RecordType.ENEMY)
				{
					BiaoJuRecordData.Instance.BiaoJuRecordReq (BiaoJuRecordData.RecordType.ENEMY);
				}

				break;
			default:
				break;
			}
		}

		void SetBtnState (GameObject targetObj,bool isCur)
		{
			UIWidget[] widgets = targetObj.GetComponentsInChildren <UIWidget> ();
			foreach (UIWidget widget in widgets)
			{
				widget.color = isCur ? Color.white : Color.gray;
			}
		}

		/// <summary>
		/// Closes the record.
		/// </summary>
		public void CloseRecord ()
		{
			gameObject.SetActive (false);
		}
	}
}
