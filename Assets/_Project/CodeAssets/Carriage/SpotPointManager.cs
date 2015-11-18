using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carriage
{
    public class SpotPointManager : MonoBehaviour
    {
        public RootManager m_RootManager;
        private GameObject m_spotPointParent;
        public List<List<GameObject>> m_SpotPointLineList = new List<List<GameObject>>();
        public List<List<Vector3>> m_SpotPointLineVecs = new List<List<Vector3>>();

        private void CreateSpotPoint()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SPOT_POINT), OnSpotPointLoadCallBack);
        }

        private void OnSpotPointLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            for (int i = 0; i < CartRouteTemplate.Templates.Count; i++)
            {
                List<GameObject> tempObjects = new List<GameObject>();
                List<Vector3> vectors = new List<Vector3>();

                var spotPointLine = CartRouteTemplate.Templates[i];
                for (int j = 0; j < spotPointLine.Position.Count; j++)
                {
                    var spotPoint = spotPointLine.Position[j];
                    var tempObject = Instantiate(loadedObject) as GameObject;
                    TransformHelper.ActiveWithStandardize(m_spotPointParent.transform, tempObject.transform);

                    tempObject.name = (i + 1) + "-" + (j + 1);
                    tempObject.transform.localPosition = new Vector3(spotPoint.x, m_RootManager.BasicYPosition + 0.5f, spotPoint.y);

                    tempObjects.Add(tempObject);
                    vectors.Add(tempObject.transform.localPosition);

                    //deactive spot point in the middle.
                    if (j != 0 && j != spotPointLine.Position.Count - 1)
                    {
                        tempObject.SetActive(false);
                    }

                }

                m_SpotPointLineList.Add(tempObjects);
                m_SpotPointLineVecs.Add(vectors);
            }

            Debug.Log("Spot point setted, begin to set carriages.");

            //m_RootManager.m_CarriageManager.LoadAndSetCarriages();
        }

        void Awake()
        {
            m_spotPointParent = gameObject;
            CreateSpotPoint();
        }

        void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            m_SpotPointLineList.ForEach(item => item.ForEach(item2 => Destroy(item2)));

            m_SpotPointLineList.Clear();
        }
    }
}
