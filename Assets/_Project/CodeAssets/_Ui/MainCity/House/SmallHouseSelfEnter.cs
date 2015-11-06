using UnityEngine;
using System.Collections;

public class SmallHouseSelfEnter : MonoBehaviour
{
    public SmallHouseSelf m_SmallHouseSelf;

    public UILabel BigTitle;
    public UILabel SmallTitle;
    public UILabel DescLabel;
    public UIEventListener EnterBtnListener;
    public UIEventListener OperBtnListener;
    public UIEventListener BgListener;

    public UIEventListener BookListener;

    public GameObject OldBookRedAlert;
    public GameObject OperateRedAlert;

    public void SetEnterInfo()
    {
        //[FIX]disable enter
        //EnterBtnListener.gameObject.SetActive(!m_SmallHouseSelf.IsBigHouseExist);
        //OperBtnListener.gameObject.SetActive(m_SmallHouseSelf.IsBigHouseExist);        
        EnterBtnListener.gameObject.SetActive(false);
        OperBtnListener.gameObject.SetActive(true);

        BigTitle.text = m_SmallHouseSelf.m_HouseSimpleInfo.jzName;
        SmallTitle.text = "<" + FangWuInfoTemplate.GetNameById(m_SmallHouseSelf.m_HouseSimpleInfo.locationId + 100) +
                          ">-" + HouseBasic.GetStateStr(m_SmallHouseSelf.m_HouseSimpleInfo.state, true);
        DescLabel.text = string.Format(FangWuInfoTemplate.GetDescriptionById(m_SmallHouseSelf.m_HouseSimpleInfo.locationId + 100), m_SmallHouseSelf.m_HouseSimpleInfo.firstOwner, m_SmallHouseSelf.m_HouseSimpleInfo.firstHoldTime);

        OldBookRedAlert.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(500050));
        OperateRedAlert.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(500030)||PushAndNotificationHelper.IsShowRedSpotNotification(500040));
    }

    private void OnEnterClick(GameObject go)
    {
        m_SmallHouseSelf.OnEnterClick();
    }

    private void OnOperClick(GameObject go)
    {
        gameObject.SetActive(false);
        m_SmallHouseSelf.OnOperationClick();
    }

    private void OnBGClick(GameObject go)
    {
        Destroy(m_SmallHouseSelf.gameObject);
    }

    private void OnBookClick(GameObject go)
    {
        gameObject.SetActive(false);
        m_SmallHouseSelf.OnBookClick();
    }

    void OnEnable()
    {
        EnterBtnListener.onClick = OnEnterClick;
        OperBtnListener.onClick = OnOperClick;
        BgListener.onClick = OnBGClick;
        BookListener.onClick = OnBookClick;
    }

    void OnDisable()
    {
        EnterBtnListener.onClick = null;
        OperBtnListener.onClick = null;
        BgListener.onClick = null;
        BookListener.onClick = null;
    }
}
