using UnityEngine;
using System.Collections;

public class BigHouseSelfEnter : MonoBehaviour
{
    public BigHouseSelf m_BigHouseSelf;

    public UILabel BigTitle;
    public UILabel SmallTitle;
    public UILabel DescLabel;
    //[FIX]use enter as oper btn
    public UIEventListener EnterBtnListener;
    public UIEventListener BgListener;

    public UIEventListener BookListener;

    public void SetEnterInfo()
    {
        BigTitle.text = m_BigHouseSelf.m_HouseSimpleInfo.jzName;
        SmallTitle.text = "<" + FangWuInfoTemplate.GetNameById(m_BigHouseSelf.m_HouseSimpleInfo.locationId - 100) +
                          ">-" + HouseBasic.GetStateStr(m_BigHouseSelf.m_HouseSimpleInfo.state, false);

        if (m_BigHouseSelf.m_HouseSimpleInfo.jzId == -1)
        {
            EnterBtnListener.gameObject.SetActive(false);
            DescLabel.text = string.Format(FangWuInfoTemplate.GetNoOwnerDescriptionById(m_BigHouseSelf.m_HouseSimpleInfo.locationId - 100), m_BigHouseSelf.m_HouseSimpleInfo.firstOwner, m_BigHouseSelf.m_HouseSimpleInfo.firstHoldTime);
        }
        else
        {
            DescLabel.text = string.Format(FangWuInfoTemplate.GetDescriptionById(m_BigHouseSelf.m_HouseSimpleInfo.locationId - 100), m_BigHouseSelf.m_HouseSimpleInfo.firstOwner, m_BigHouseSelf.m_HouseSimpleInfo.firstHoldTime);
        }
    }

    private void OnEnterClick(GameObject go)
    {
        //[FIX]disable enter
        //m_BigHouseSelf.OnEnterClick();
        m_BigHouseSelf.OnOperationClick();
    }

    private void OnBGClick(GameObject go)
    {
        Destroy(m_BigHouseSelf.gameObject);
    }

    private void OnBookClick(GameObject go)
    {
        gameObject.SetActive(false);
        m_BigHouseSelf.OnBookClick();
    }

    void OnEnable()
    {
        EnterBtnListener.onClick = OnEnterClick;
        BgListener.onClick = OnBGClick;
        BookListener.onClick = OnBookClick;
    }

    void OnDisable()
    {
        EnterBtnListener.onClick = null;
        BgListener.onClick = null;
        BookListener.onClick = null;
    }
}
