using UnityEngine;
using System.Collections;

public class EquipScrollViewManager : MonoBehaviour {

    public GameObject m_equipsScrollView;

    public GameObject m_caiLiaoScrollView;

    public void ShowEquipScrollView()
    {
        m_equipsScrollView.SetActive(true);

        m_caiLiaoScrollView.SetActive(false);
    }

    public void ShowCaiLiaoScrollView()
    {
        m_equipsScrollView.SetActive(false);

        m_caiLiaoScrollView.SetActive(true);
    }

}
