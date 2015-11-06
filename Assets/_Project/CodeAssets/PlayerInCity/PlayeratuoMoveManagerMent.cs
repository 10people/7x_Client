using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayeratuoMoveManagerMent : MonoBehaviour
{
    public List<PlayerAutoNavManagerment> m_listAnim;
    public float m_WaitSeconds = 1.4f;
    public static bool  m_AnimOver = false;
    void Start()
    {


    }

    void Update()
    {
        if (m_AnimOver)
        {
            m_AnimOver = false;
            ShowAnim();
        }

    }

    void ShowAnim()
    {
     StartCoroutine(WaitForUpgaade());
    }

    IEnumerator WaitForUpgaade()
    {
        yield return new WaitForSeconds(m_WaitSeconds);
        for (int i = 0; i < m_listAnim.Count; i++)
        {
            m_listAnim[i].m_NavAnim.enabled = true;

        }

    }
}
