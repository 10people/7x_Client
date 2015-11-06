using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

namespace Carriage
{
    public class CarriageManager : MonoBehaviour
    {
        public RootManager m_RootManager;
        public List<CarriageController> m_CarriageControllers = new List<CarriageController>();

        private GameObject m_carriageParent;

        /// <summary>
        /// initialize.
        /// </summary>
        public void InitAllCarriages()
        {
            //load and set carriages.
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.CARRIAGE), OnInitCarriageLoadCallBack);
        }

        public void RefreshACarriage(BiaoCheState l_biaoCheState)
        {
            List<CarriageController> controllers = m_CarriageControllers.Where(controller => controller.m_YabiaoJunZhuInfo.junZhuId == l_biaoCheState.junZhuId).ToList();
            if (controllers == null || controllers.Count == 0)
            {
                Debug.LogError("Fail to find carriage when refresh a carriage, kingId:" + l_biaoCheState.junZhuId);
                return;
            }

            foreach (CarriageController controller in controllers)
            {
                controller.m_YabiaoJunZhuInfo.state = l_biaoCheState.state;
                controller.m_YabiaoJunZhuInfo.hp = l_biaoCheState.hp;
                controller.m_YabiaoJunZhuInfo.worth = l_biaoCheState.worth;
                controller.m_YabiaoJunZhuInfo.usedTime = l_biaoCheState.usedTime;

                controller.SetStateMachine();
                controller.SetPanelContent();
            }
        }

        private YabiaoJunZhuInfo m_yabiaoJunZhuInfo;

        public void InitACarriage(YabiaoJunZhuInfo l_yabiaoJunZhuInfo)
        {
            List<CarriageController> controllers = m_CarriageControllers.Where(controller => controller.m_YabiaoJunZhuInfo.junZhuId == l_yabiaoJunZhuInfo.junZhuId).ToList();
            //List<CarriageController> controllers2 = m_CarriageControllers.Where(controller => controller.m_YabiaoJunZhuInfo.pathId == l_yabiaoJunZhuInfo.pathId).ToList();

            if (controllers != null && controllers.Count != 0)
            {
                Debug.LogError("cannot add duplicated carriage when add a carriage, kingId:" + l_yabiaoJunZhuInfo.junZhuId);
                return;
            }
            //if (controllers2 != null && controllers2.Count != 0)
            //{
            //    Debug.LogError("cannot add duplicated carriage when add a carriage, pathId:" + l_yabiaoJunZhuInfo.pathId);
            //    return;
            //}

            m_yabiaoJunZhuInfo = l_yabiaoJunZhuInfo;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.CARRIAGE), OnAddACarriageLoadCallBack);
        }

        private void OnInitCarriageLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            ////Clear before initialize.
            //foreach (var controller in m_CarriageControllers)
            //{
            //    controller.transform.parent = null;
            //    Destroy(controller.gameObject);
            //}
            //m_CarriageControllers.Clear();

            if (CarriageSceneManager.Instance.s_YabiaoJunZhuList == null ||
                CarriageSceneManager.Instance.s_YabiaoJunZhuList.yabiaoJunZhuList == null)
            {
                return;
            }

            //Instantiate and set.
            for (int i = 0; i < CarriageSceneManager.Instance.s_YabiaoJunZhuList.yabiaoJunZhuList.Count; i++)
            {
                YabiaoJunZhuInfo temp = CarriageSceneManager.Instance.s_YabiaoJunZhuList.yabiaoJunZhuList[i];

                ExecuteACarriage(temp, loadedObject);
            }
        }

        private void OnAddACarriageLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            ExecuteACarriage(m_yabiaoJunZhuInfo, loadedObject);
        }

        private void ExecuteACarriage(YabiaoJunZhuInfo l_yabiaoJunZhuInfo, Object loadedObject)
        {
            var tempObject = Instantiate(loadedObject) as GameObject;
            UtilityTool.ActiveWithStandardize(m_carriageParent.transform, tempObject.transform);
            tempObject.transform.localPosition = m_RootManager.m_SpotPointManager.m_SpotPointLineVecs[l_yabiaoJunZhuInfo.pathId - 1][0];

            CarriageController tempController = tempObject.GetComponent<CarriageController>() ??
                                                tempObject.AddComponent<CarriageController>();

            tempController.m_YabiaoJunZhuInfo = l_yabiaoJunZhuInfo;
            tempController.m_rootManager = m_RootManager;
            tempController.m_3DPositionList = m_RootManager.m_SpotPointManager.m_SpotPointLineVecs[l_yabiaoJunZhuInfo.pathId - 1];

            tempController.periodPrecent = tempController.m_YabiaoJunZhuInfo.usedTime / (float)tempController.m_YabiaoJunZhuInfo.totalTime;
            tempController.InitTransform();

            tempController.SetStateMachine();
            tempController.SetPanelContent();

            m_CarriageControllers.Add(tempController);
        }

        void Awake()
        {
            m_carriageParent = gameObject;
        }
    }

}
