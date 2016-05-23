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
        public GameObject m_OccupyPointGizmosPrefab;
        public GameObject m_HoldPointPrefab;
        public GameObject m_ShieldPrefab;

        public int CurrentShowingID = -1;

        public class OccupyPoint
        {
            public GameObject OccupyObject;
            public UISprite m_OccupyGizmosSprite;
            public Vector3 Position;
            public bool IsMovingLeft;
            public float OccupyValue;
            public float MaxValue;
            public float Range;

            public bool IsProtecter;
            public int Type;
            public int ID;

            public bool IsCanAlert = true;
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

        public void UpdateHoldPoints(List<CampInfo> p_campInfos)
        {
            p_campInfos.ForEach(item =>
            {
                if (HoldPointDic.ContainsKey(item.id))
                {
                    if (item.cursorPos > HoldPointDic[item.id].OccupyValue)
                    {
                        TryPlayAlert(item.id, true);
                    }
                    else if (item.cursorPos < HoldPointDic[item.id].OccupyValue)
                    {
                        TryPlayAlert(item.id, false);
                    }

                    HoldPointDic[item.id].IsMovingLeft = item.cursorDir == 1;
                    HoldPointDic[item.id].OccupyValue = item.cursorPos;
                }
            });

            TryUpdateHoldPointUI();
        }

        public void TryPlayAlert(int id, bool isValueBigger)
        {
            if ((m_RootManager.MyPart == 1 && !isValueBigger && !HoldPointDic[id].IsProtecter) || (m_RootManager.MyPart == 2 && isValueBigger && HoldPointDic[id].IsProtecter))
            {
                if (HoldPointDic[id].IsCanAlert)
                {
                    if (HoldPointDic[id].Type == 4)
                    {
                        ClientMain.m_UITextManager.createText("我方基地受到攻击，速度回防!");
                    }
                    else
                    {
                        ClientMain.m_UITextManager.createText("我方" + id + "营地受到攻击，速去支援!");
                    }
                    m_RootManager.m_AllianceBattleMain.m_MapController.m_MapBeenAttackEffectController.BlinkBeenAttackEffect(-1*id,HoldPointDic[id].Position);

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

        public void TryUpdateHoldPointUI()
        {
            if (CurrentShowingID > 0 && HoldPointDic.ContainsKey(CurrentShowingID))
            {
                m_RootManager.m_AllianceBattleMain.UpdateOccupyBar(HoldPointDic[CurrentShowingID].OccupyValue / HoldPointDic[CurrentShowingID].MaxValue, HoldPointDic[CurrentShowingID].IsMovingLeft);
            }
        }

        public void TryAddShield()
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

        public void TryRemoveShield()
        {
            if (ShieldObjectList.Count > 0)
            {
                ShieldObjectList.ForEach(item => Destroy(item));
                ShieldObjectList.Clear();
            }
        }

        void Update()
        {
            if (m_RootManager.m_SelfPlayerController == null || m_RootManager.m_SelfPlayerCultureController == null) return;

            foreach (var item in HoldPointDic)
            {
                if (Vector3.Distance(m_RootManager.m_SelfPlayerController.transform.position, item.Value.OccupyObject.transform.position) < HoldPointRange)
                {
                    CurrentShowingID = item.Key;
                    m_RootManager.m_AllianceBattleMain.ShowOccupyBar(true, HoldPointDic[CurrentShowingID].MaxValue);
                    return;
                }
            }

            CurrentShowingID = -1;
            m_RootManager.m_AllianceBattleMain.ShowOccupyBar(false, 0);
        }

        void Start()
        {
            HoldPointDic.Values.ToList().ForEach(item =>
            {
                var temp = Instantiate(m_HoldPointPrefab);
                TransformHelper.ActiveWithStandardize(transform, temp.transform);
                temp.transform.position = item.Position;
                temp.transform.eulerAngles = new Vector3(0, 180, 0);
                temp.transform.localScale = Vector3.one * 0.2f;
                item.OccupyObject = temp;

                var temp2 = Instantiate(m_OccupyPointGizmosPrefab);
                TransformHelper.ActiveWithStandardize(m_RootManager.m_AllianceBattleMain.m_MapController.transform, temp2.transform);
                temp2.transform.localPosition = m_RootManager.m_AllianceBattleMain.m_MapController.GizmosPositionTransfer(item.Position);
                temp2.SetActive(true);
                item.m_OccupyGizmosSprite = temp2.GetComponent<UISprite>();

                //Add camp to map gizmos.
                m_RootManager.m_AllianceBattleMain.m_MapController.m_ItemGizmosDic.Add(-1 * item.ID, temp2.transform);
            });

            PrepareForAllianceBattle.UpdateLoadProgress(PrepareForAllianceBattle.LoadModule.INIT, "AB_HoldPoint");
        }

        void Awake()
        {
            HoldPointDic.Clear();

            //Read data from xmls.
            var temp = LMZBuildingTemplate.GetTemplates(2, 1).Concat(LMZBuildingTemplate.GetTemplates(3, 1)).Concat(LMZBuildingTemplate.GetTemplates(4, 1)).ToList();
            temp.ForEach(item => HoldPointDic.Add(item.Id, new OccupyPoint()
            {
                Position = new Vector3(item.Position.x, RootManager.BasicYPosition, item.Position.y),
                MaxValue = item.ZhanlingzhiMax,
                Range = item.Radius,
                IsProtecter = true,
                Type = item.Type,
                ID = item.Id
            }));

            var temp2 = LMZBuildingTemplate.GetTemplates(4, 2);
            temp2.ForEach(item => HoldPointDic.Add(item.Id, new OccupyPoint()
            {
                Position = new Vector3(item.Position.x, RootManager.BasicYPosition, item.Position.y),
                MaxValue = item.ZhanlingzhiMax,
                Range = item.Radius,
                IsProtecter = false,
                Type = item.Type,
                ID = item.Id
            }));

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
