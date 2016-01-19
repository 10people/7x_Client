using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AllianceBattle
{
    /// <summary>
    /// 1 for red side, 2 for blue side.
    /// </summary>
    public class AllianceBattleHoldPointManager : MonoBehaviour
    {
        public RootManager m_RootManager;

        public GameObject m_OccupyPointGizmosPrefab;
        public const string BlueSpriteName = "bulebg";
        public const string RedSpriteName = "redbg";
        public const string WhiteSpriteName = "backGround_Common_big";

        public GameObject m_TempHoldPointMesh;

        public class OccupyPoint
        {
            public static int CurrentShowingID = -1;
            public static RootManager RootManager;

            public GameObject OccupyObject;
            public UISprite m_OccupyGizmosSprite;
            public Vector3 OccupyPosition;
            public float OccupyValue;
            public float MovingSpeed;
            public int id;

            public void RefreshValue(int elapseTime)
            {
				if ( Application.loadedLevelName != SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_BATTLE ) ){
                    if ( TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleOccupyBar" + id ) ){
                        TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleOccupyBar" + id );
                    }

                    return;
                }

                float nowOccupyBarValue = OccupyValue + MovingSpeed * elapseTime;

                if (CurrentShowingID == id)
                {
                    //Set occupy bar.
                    RootManager.m_AllianceBattleUi.ShowOccupyBar(true);
                    RootManager.m_AllianceBattleUi.SetOccupyBar(nowOccupyBarValue);
                }

                //Set small map gizmos.
                if (nowOccupyBarValue < RootManager.m_AllianceBattleUi.CriticalValueInProgressBar1)
                {
                    HoldPointList.Where(item => item.id == id).First().m_OccupyGizmosSprite.spriteName = BlueSpriteName;
                }
                else if (nowOccupyBarValue > RootManager.m_AllianceBattleUi.CriticalValueInProgressBar2)
                {
                    HoldPointList.Where(item => item.id == id).First().m_OccupyGizmosSprite.spriteName = RedSpriteName;
                }
                else
                {
                    HoldPointList.Where(item => item.id == id).First().m_OccupyGizmosSprite.spriteName = WhiteSpriteName;
                }
            }
        }
        public static List<OccupyPoint> HoldPointList = new List<OccupyPoint>();

        [HideInInspector]
        public float HoldPointRange = 5;

        public void SetConfig()
        {
            HoldPointList.ForEach(item =>
            {
                if (TimeHelper.Instance.IsTimeCalcKeyExist("AllianceBattleOccupyBar" + item.id))
                {
                    TimeHelper.Instance.RemoveFromTimeCalc("AllianceBattleOccupyBar" + item.id);
                }

                TimeHelper.Instance.AddEveryDelegateToTimeCalc("AllianceBattleOccupyBar" + item.id, float.MaxValue, item.RefreshValue);
            });
        }

        void Update()
        {
            if (m_RootManager.m_AbPlayerController == null || m_RootManager.m_AbPlayerCultureController == null) return;

            for (int i = 0; i < HoldPointList.Count; i++)
            {
                if (Vector3.Distance(m_RootManager.m_AbPlayerController.transform.position, HoldPointList[i].OccupyObject.transform.position) < HoldPointRange)
                {
                    OccupyPoint.CurrentShowingID = HoldPointList[i].id;
                    return;
                }
            }

            OccupyPoint.CurrentShowingID = -1;
            m_RootManager.m_AllianceBattleUi.ShowOccupyBar(false);
        }

        void Start()
        {
            HoldPointList.ForEach(item =>
            {
                var temp = new GameObject("HoldPoint");
                TransformHelper.ActiveWithStandardize(transform, temp.transform);
                temp.transform.position = item.OccupyPosition;
                item.OccupyObject = temp;

                //Add temp hold point mesh.
                var temp3 = Instantiate(m_TempHoldPointMesh);
                TransformHelper.ActiveWithStandardize(transform, temp3.transform);
                temp3.transform.position = item.OccupyPosition;

                var temp2 = Instantiate(m_OccupyPointGizmosPrefab);
                TransformHelper.ActiveWithStandardize(m_OccupyPointGizmosPrefab.transform.parent, temp2.transform);
                temp2.transform.localPosition = m_RootManager.m_AllianceBattleUi.SmallMapPositionTransfer(item.OccupyPosition);
                temp2.SetActive(true);
                item.m_OccupyGizmosSprite = temp2.GetComponent<UISprite>();
            });
        }

        void Awake()
        {
            OccupyPoint.RootManager = m_RootManager;
            HoldPointList.Clear();

            //Read data from xmls.
            var temp = LMZBuildingTemplate.GetTemplatesBySide(0);

            temp.ForEach(item => HoldPointList.Add(new OccupyPoint()
            {
                OccupyPosition = new Vector3(item.Position.x, RootManager.BasicYPosition, item.Position.y),
                id = item.Id
            }));
        }

		void OnDestroy(){
			HoldPointList.Clear();
		}
    }
}
