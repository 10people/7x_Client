using System;
using UnityEngine;
using System.Collections;

public class MapBeenAttackController : MonoBehaviour
{
    public int m_BeenAttackUid = -1;

    public GameObject BeenAttackEffect1;
    public GameObject BeenAttackEffect2;

    public void ShowEffect()
    {
        BeenAttackEffect2.SetActive(true);

        if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageBeenAttackEffect" + m_BeenAttackUid))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("CarriageBeenAttackEffect" + m_BeenAttackUid);
        }
        TimeHelper.Instance.AddFrameDelegateToTimeCalc("CarriageBeenAttackEffect" + m_BeenAttackUid, MapEffectController.EffectDuration, SetEffectState);
    }

    private void SetEffectState(float elapseTime)
    {
        if (MapEffectController.EffectDuration - elapseTime <= 0)
        {
            if (MapEffectController.BeenAttackEffectUidList.Contains(m_BeenAttackUid))
            {
                MapEffectController.BeenAttackEffectUidList.Remove(m_BeenAttackUid);
            }

            TimeHelper.Instance.RemoveFromTimeCalc("CarriageBeenAttackEffect" + m_BeenAttackUid);
            BeenAttackEffect2.SetActive(false);

            Destroy(gameObject);
        }
        else
        {
            var flag = elapseTime % MapEffectController.BeenAttackPeriodDuration / MapEffectController.BeenAttackPeriodDuration;
            BeenAttackEffect2.transform.localScale = Vector3.one * (1 - flag);
        }
    }
}
