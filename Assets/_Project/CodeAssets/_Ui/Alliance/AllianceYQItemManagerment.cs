using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AllianceYQItemManagerment : MonoBehaviour
{
    public List<EventIndexHandle> m_listEvent;
    public UILabel m_LabTime;
    public UILabel m_LabInfo;
    public delegate void OnClick_Operation(int index,int id);
    OnClick_Operation CallBackOperation;
    private int _SaveId = 0;
    void Start()
    {
        m_listEvent.ForEach(p => p.m_Handle += Touch);
    }

    void Touch(int index)
    {
        if (CallBackOperation != null)
        CallBackOperation(index, _SaveId);
    }
    public void ShowInfo(AllianceLayerManagerment.AllianceYaoQingInfo aii, OnClick_Operation cllback = null)
    {
        _SaveId = aii.id;
        CallBackOperation = cllback;
        m_LabTime.text = aii.time;
        m_LabInfo.text = MyColorData.getColorString(4, aii.name + "(lv." + aii.level.ToString() + ")") + MyColorData.getColorString(10, "邀请你加入");
    }
}
