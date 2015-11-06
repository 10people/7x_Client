using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
    public float DestroyTime;
    private float StartCalcTime = -1;

    public delegate void VoidDelegate();

    public event VoidDelegate m_DestroyCallBack;

    public void StartDestroyCalc()
    {
        StartCalcTime = Time.realtimeSinceStartup;

        Destroy(gameObject);
    }

    void Update()
    {
        if (StartCalcTime < 0) return;

        if (Time.realtimeSinceStartup - StartCalcTime > DestroyTime)
        {
            if (m_DestroyCallBack != null)
            {
                StartCalcTime = -1;
                m_DestroyCallBack();
            }
        }
    }
}
