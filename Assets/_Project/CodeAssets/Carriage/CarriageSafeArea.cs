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
        public GameObject m_SafeAreaParent;

        void Start()
        {
            //Create safe area effect.
            YunBiaoSafeTemplate.Templates.ForEach(item => m_SafeAreaList.Add(item.m_SafeArea));
            m_SafeAreaList.ForEach(item => FxTool.PlayLocalFx(EffectTemplate.GetEffectPathByID(51020), m_SafeAreaParent, SetScale, new Vector3(item.AreaPos.x, -3.9f, item.AreaPos.y), Vector3.zero));
        }

        void SetScale(GameObject go)
        {
            go.transform.localScale = Vector3.one * m_SafeAreaList.First().AreaRadius;
        }

        void Awake()
        {
            YunBiaoSafeTemplate.LoadTemplates();
        }
    }
}
