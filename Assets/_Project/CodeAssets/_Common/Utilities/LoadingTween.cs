using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadingTween : MonoBehaviour
{
    public struct LoadingTweenConfig
    {
        public List<GameObject> m_TargetGameObjectList;
        public float m_StartValue;
        public float m_EndValue;
        public float m_GapDuration;
        public float m_WaitDuration;
        public DelegateUtil.FloatDelegate m_FloatDelegate;
    }

    private LoadingTweenConfig m_loadingTweenConfig;

    public static void StartLoadingTween(GameObject parentGameObject, List<GameObject> targetGameObjectList, float startValue, float endValue, float gapDuration, float waitDuration, DelegateUtil.FloatDelegate floatDelegate)
    {
        var loadingTween = parentGameObject.GetComponent<LoadingTween>() ?? parentGameObject.AddComponent<LoadingTween>();

        loadingTween.m_loadingTweenConfig = new LoadingTweenConfig()
        {
            m_TargetGameObjectList = targetGameObjectList,
            m_StartValue = startValue,
            m_EndValue = endValue,
            m_GapDuration = gapDuration,
            m_WaitDuration = waitDuration,
            m_FloatDelegate = floatDelegate
        };

        loadingTween.StopAllCoroutines();
        loadingTween.StartCoroutine("DoLoadingTween");
    }

    public static void StopLoadingTween(GameObject parentGameObject)
    {
        var loadingTween = parentGameObject.GetComponent<LoadingTween>();

        if (loadingTween != null)
        {
            loadingTween.Stop();
        }
    }

    private IEnumerator DoLoadingTween()
    {
        m_loadingTweenConfig.m_TargetGameObjectList.ForEach(item => item.SetActive(false));

        for (int i = 0; i < m_loadingTweenConfig.m_TargetGameObjectList.Count; i++)
        {
            m_loadingTweenConfig.m_TargetGameObjectList[i].SetActive(true);

            if (i >= (m_loadingTweenConfig.m_TargetGameObjectList.Count - 1))
            {
                yield return new WaitForSeconds(m_loadingTweenConfig.m_WaitDuration);

                break;
            }
            else
            {
                yield return new WaitForSeconds(m_loadingTweenConfig.m_GapDuration);
            }
        }

        ReStart();
    }

    private void ReStart()
    {
        Stop();
        StartCoroutine("DoLoadingTween");
    }

    public void Stop()
    {
        StopAllCoroutines();
    }
}
