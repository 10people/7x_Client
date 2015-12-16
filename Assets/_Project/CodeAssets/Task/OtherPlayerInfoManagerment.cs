using UnityEngine;
using System.Collections;

public class OtherPlayerInfoManagerment : MonoBehaviour
{
    public static OtherPlayerInfoManagerment m_OtherInfo;
    public string m_OtherPlayerId;
    public EventIndexHandle m_ChaKan;
    public ScaleEffectController m_SEC;
    public GameObject m_ObjChaKan;

    void Awake()
    {
        m_OtherInfo = this;
    }

	void Start ()
    {
        m_ChaKan.m_Handle += ChaKan;
        m_SEC.OpenCompleteDelegate += ShowInfo;
    }

    void ShowInfo()
    {
        m_ObjChaKan.SetActive(true);
    }

    void ChaKan (int index)
    {
	
	}
    void OnDisable()
    {
        m_OtherInfo = null;
    }
}
