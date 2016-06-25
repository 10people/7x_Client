using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using qxmobile.protobuf;

namespace AllianceBattle
{
    /// <summary>
    /// 1 for red side, 2 for blue side.
    /// </summary>
    public class ABHoldPointManager : MonoBehaviour
    {
        public RootManager m_RootManager;
        public GameObject m_RedNorGizmosPrefab;
        public GameObject m_RedAdGizmosPrefab;
        public GameObject m_RedBaseGizmosPrefab;
        public GameObject m_BlueNorGizmosPrefab;
        public GameObject m_BlueAdGizmosPrefab;
        public GameObject m_BlueBaseGizmosPrefab;
        public GameObject m_RedNorHoldPrefab;
        public GameObject m_RedAdHoldPrefab;
        public GameObject m_BlueNorHoldPrefab;
        public GameObject m_BlueAdHoldPrefab;
        public GameObject m_ShieldPrefab;

        public int CurrentShowingID = -1;

        public class OccupyPoint
        {
            public GameObject OccupyObject;
            public ABHoldCultureController CultureController;
            public UISprite m_OccupyGizmosSprite;
            public Vector3 Position;
            public bool IsMovingLeft;
            public float OccupyValue;
            public float MaxValue;
            public float Range;
            public string Name;

            public bool IsProtecter;
            /// <summary>
            /// 2 for normal holds, 3 for advanced holds, 4 for base.
            /// </summary>
            public int Type;
            /// <summary>
            /// 1 for protecter, 2 for attacker.
            /// </summary>
            public int Side;
            public int ID;
            public int UID;

            public bool IsCanAlert = true;
            public bool IsDestroyed = true;
        }
        public Dictionary<int, OccupyPoint> HoldPointDic = new Dictionary<int, OccupyPoint>();

        public class ShieldPoint
        {
            public GameObject ShieldObject;
            public Vector3 Position;
            public Vector3 Rotation;
            public float Length;
        }
        public List<ShieldPoint> ShieldList = new List<ShieldPoint>();

        public List<GameObject> ShieldObjectList = new List<GameObject>();

        [HideInInspector]
        public float HoldPointRange = 5;

        public void TryPlayAlert(int id)
        {
            if ((m_RootManager.MyPart == 1 && !HoldPointDic[id].IsProtecter) || (m_RootManager.MyPart == 2 && HoldPointDic[id].IsProtecter))
            {
                if (HoldPointDic[id].IsCanAlert)
                {
                    if (HoldPointDic[id].Type == 4)
                    {
                        m_RootManager.m_AllianceBattleMain.ShowWowAlert("我方基地受到攻击，速度回防!");
                    }
                    else
                    {
                        m_RootManager.m_AllianceBattleMain.ShowWowAlert("我方" + HoldPointDic[id].Name + "营地受到攻击，速去支援!");
                    }
                    m_RootManager.m_AllianceBattleMain.m_MapController.m_MapEffectController.PlayUIEffect(m_RootManager.m_AllianceBattleMain.m_MapController.m_ItemGizmosDic[HoldPointDic[id].UID].gameObject, 620241);

                    HoldPointDic[id].IsCanAlert = false;
                    TimeHelper.Instance.AddOneDelegateToTimeCalc("ABHoldPointAlert" + id, 5f, UpdateAlertState);
                }
            }
        }

        private void UpdateAlertState(string key)
        {
            TimeHelper.Instance.RemoveFromTimeCalc(key);

            if (!key.Contains("ABHoldPointAlert"))
            {
                Debug.LogError("ABHoldPointAlert got error when refresh alert state");
                return;
            }

            HoldPointDic[int.Parse(key.Replace("ABHoldPointAlert", ""))].IsCanAlert = true;
        }

        private bool isShieldAdded;

        public void TryAddShield()
        {
            if (!isShieldAdded)
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE))
                {

                }
                else
                {
                    if (ShieldObjectList.Count == 0)
                    {
                        ShieldList.ForEach(item =>
                        {
                            var temp = Instantiate(m_ShieldPrefab);
                            TransformHelper.ActiveWithStandardize(transform, temp.transform);
                            temp.transform.position = item.Position;
                            temp.transform.eulerAngles = item.Rotation;
                            temp.transform.localScale = Vector3.one * item.Length;

                            ShieldObjectList.Add(temp);
                        });
                    }
                }
                isShieldAdded = true;
            }
        }

        private bool isShieldRemoved;

        public void TryRemoveShield()
        {
            if (!isShieldRemoved)
            {
                if (ShieldObjectList.Count > 0)
                {
                    ShieldObjectList.ForEach(item => Destroy(item));
                    ShieldObjectList.Clear();
                }

                isShieldRemoved = true;
            }
        }

        public void AddMapGizmos(OccupyPoint occupyPoint, int uID)
        {
            GameObject gizmosPrefab;
            if (occupyPoint.Side == 1)
            {
                if (occupyPoint.Type == 2)
                {
                    gizmosPrefab = m_BlueNorGizmosPrefab;
                }
                else if (occupyPoint.Type == 3)
                {
                    gizmosPrefab = m_BlueAdGizmosPrefab;
                }
                else
                {
                    gizmosPrefab = m_BlueBaseGizmosPrefab;
                }
            }
            else
            {
                gizmosPrefab = m_RedBaseGizmosPrefab;
            }

            var holdPointGizmos = Instantiate(gizmosPrefab);
            TransformHelper.ActiveWithStandardize(m_RootManager.m_AllianceBattleMain.m_MapController.transform, holdPointGizmos.transform);
            holdPointGizmos.transform.localPosition = m_RootManager.m_AllianceBattleMain.m_MapController.GizmosPositionTransfer(occupyPoint.Position);
            holdPointGizmos.SetActive(true);
            occupyPoint.m_OccupyGizmosSprite = holdPointGizmos.GetComponent<UISprite>();

            //Add camp to map gizmos.
            m_RootManager.m_AllianceBattleMain.m_MapController.m_ItemGizmosDic.Add(uID, holdPointGizmos.transform);
        }

        void Update()
        {
            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null) return;

            foreach (var item in HoldPointDic)
            {
                if (!item.Value.IsDestroyed && Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.OccupyObject.transform.position) < HoldPointRange)
                {
                    CurrentShowingID = item.Key;
                    m_RootManager.m_AllianceBattleMain.ShowOccupyBar(true, CurrentShowingID);
                    return;
                }
            }

            CurrentShowingID = -1;
            m_RootManager.m_AllianceBattleMain.ShowOccupyBar(false, CurrentShowingID);
        }

        void Start()
        {
            HoldPointDic.Values.ToList().ForEach(item =>
            {
                GameObject m_HoldPointPrefab = null;
                if (item.Side == 1)
                {
                    if (item.Type == 2 || item.Type == 3)
                    {
                        m_HoldPointPrefab = m_BlueNorHoldPrefab;
                    }
                    else if (item.Type == 4)
                    {
                        m_HoldPointPrefab = m_BlueAdHoldPrefab;
                    }
                }
                else if (item.Side == 2)
                {
                    if (item.Type == 2 || item.Type == 3)
                    {
                        m_HoldPointPrefab = m_RedNorHoldPrefab;
                    }
                    else if (item.Type == 4)
                    {
                        m_HoldPointPrefab = m_RedAdHoldPrefab;
                    }
                }

                if (m_HoldPointPrefab == null)
                {
                    Debug.LogError("Cannot init ab hold point cause error in find specific hold perfab.");
                    return;
                }

                var holdPoint = Instantiate(m_HoldPointPrefab);
                TransformHelper.ActiveWithStandardize(transform, holdPoint.transform);
                holdPoint.transform.position = item.Position;
                holdPoint.transform.eulerAngles = new Vector3(0, 180, 0);
                item.OccupyObject = holdPoint;

                var holdPointCultureController = holdPoint.GetComponent<ABHoldCultureController>() ?? holdPoint.AddComponent<ABHoldCultureController>();
                holdPointCultureController.m_OccupyPoint = item;
                holdPointCultureController.TrackCamera = m_RootManager.TrackCamera;
                item.CultureController = holdPointCultureController;
                item.IsDestroyed = true;
                holdPointCultureController.SetHoldPointState(false);

                var otherPlayerController = holdPoint.GetComponent<OtherPlayerController>() ?? holdPoint.AddComponent<OtherPlayerController>();
                otherPlayerController.m_CharacterLerpDuration = 1.0f;
                otherPlayerController.TrackCamera = m_RootManager.TrackCamera;

                otherPlayerController.DeactiveMove();
            });

            m_RootManager.m_AllianceBattleMain.RefreshAttackerGainedBuff();

            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.INIT, "AB_HoldPoint");
        }

        void Awake()
        {
            //Load prefabs.
            m_RedNorHoldPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/RedNorHold");
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            m_RedAdHoldPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/RedAdHold");
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            m_BlueNorHoldPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/BlueNorHold");
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            m_BlueAdHoldPrefab = Resources.Load<GameObject>("_3D/Models/AllianceBattle/BlueAdHold");
            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.MODEL, "AB_MODEL");

            HoldPointDic.Clear();

            //Read data from xmls.
            //Protecter.
            var temp = LMZBuildingTemplate.GetTemplates(2, 1).Concat(LMZBuildingTemplate.GetTemplates(3, 1)).Concat(LMZBuildingTemplate.GetTemplates(4, 1)).ToList();
            temp.ForEach(item => HoldPointDic.Add(item.Id, new OccupyPoint()
            {
                Position = new Vector3(item.Position.x, RootManager.BasicYPosition, item.Position.y),
                MaxValue = item.ZhanlingzhiMax,
                Range = item.Radius,
                IsProtecter = true,
                Type = item.Type,
                Side = item.Side,
                ID = item.Id,
                Name = item.Name
            }));

            //Attacker.
            var temp2 = LMZBuildingTemplate.GetTemplates(4, 2);
            temp2.ForEach(item => HoldPointDic.Add(item.Id, new OccupyPoint()
            {
                Position = new Vector3(item.Position.x, RootManager.BasicYPosition, item.Position.y),
                MaxValue = item.ZhanlingzhiMax,
                Range = item.Radius,
                IsProtecter = false,
                Type = item.Type,
                Side = item.Side,
                ID = item.Id,
                Name = item.Name
            }));

            //Shield wall.
            var temp3 = LMZBuildingTemplate.GetTemplatesByType(5);
            temp3.ForEach(item => ShieldList.Add(new ShieldPoint()
            {
                Position = new Vector3(item.Position.x, RootManager.BasicYPosition, item.Position.y),
                Rotation = new Vector3(0, item.Rotation, 0),
                Length = item.Radius
            }));

            HoldPointRange = 5;
        }

        void OnDestroy()
        {
            HoldPointDic.Clear();
        }
    }
}
