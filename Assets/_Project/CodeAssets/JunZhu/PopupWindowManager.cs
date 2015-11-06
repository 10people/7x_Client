using UnityEngine;
using System.Collections;

public class PopupWindowManager : MonoBehaviour { //引用所有弹窗，方便调用显示

    public static PopupWindowManager m_manager;

    public GameObject m_upgradeTechnology;

    public GameObject m_warning;

	public GameObject m_noMoney;

    public GameObject m_equipError;

    void Awake()
    {
        m_manager = this;
    }
}
