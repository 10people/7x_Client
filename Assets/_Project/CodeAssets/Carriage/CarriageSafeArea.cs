using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Carriage
{
    public class CarriageSafeArea : MonoBehaviour
    {
        public RootManager m_RootManager;

        public List<YunBiaoSafeTemplate.SafeArea> m_SafeAreaList = new List<YunBiaoSafeTemplate.SafeArea>();
        public List<GameObject> m_CarriageNPCList = new List<GameObject>();
        public GameObject m_SafeAreaParent;

        public GameObject m_CarriageNPCPrefab;

        private const int NumPerCircle = 12;

        void Start()
        {
            m_CarriageNPCPrefab = Resources.Load<GameObject>("_3D/Models/Carriage/CarriageNPC");

            YunBiaoSafeTemplate.Templates.ForEach(item => m_SafeAreaList.Add(item.m_SafeArea));
            m_SafeAreaList.ForEach(item =>
            {
                //Create safe area effect.
                for (int i = 0; i < NumPerCircle; i++)
                {
                    var pos = MathHelper.GetPointAtCircle(new Vector2(item.AreaPos.x, item.AreaPos.y), item.AreaRadius, NumPerCircle, i);

                    FxHelper.PlayLocalFx(EffectTemplate.GetEffectPathByID(600169), m_SafeAreaParent, SetScale, new Vector3(pos.x, RootManager.BasicYPosition, pos.y), Vector3.zero);
                }

                var temp = Instantiate(m_CarriageNPCPrefab);
                TransformHelper.ActiveWithStandardize(m_RootManager.PlayerParentObject.transform, temp.transform);

                temp.transform.position = new Vector3(m_SafeAreaList[m_CarriageNPCList.Count].AreaPos.x, RootManager.BasicYPosition, m_SafeAreaList[m_CarriageNPCList.Count].AreaPos.y);

                m_CarriageNPCList.Add(temp);
                //Create npc in safe area.
                //Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(NpcCityTemplate.GetNpcItemById(301).m_npcShowId),
                //                        LoadModelCallback);
            });
        }

        private void LoadModelCallback(ref WWW p_www, string p_path, Object p_object)
        {
            var temp = Instantiate(p_object) as GameObject;
            temp.transform.position = new Vector3(m_SafeAreaList[m_CarriageNPCList.Count].AreaPos.x, RootManager.BasicYPosition, m_SafeAreaList[m_CarriageNPCList.Count].AreaPos.y);

            m_CarriageNPCList.Add(temp);
        }

        void SetScale(GameObject go)
        {
            //go.transform.localScale = Vector3.one * m_SafeAreaList.First().AreaRadius;
        }
    }
}
