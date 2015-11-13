using UnityEngine;
using System.Collections;

public class PlayerSelfNameManagerment : MonoBehaviour
{
    private static GameObject m_Parent;
    private static GameObject m_ParentName;
    private static GameObject m_ObjAutoNav;
	public GameObject m_ObjNameParent;

    void Awake()
    {
        m_ParentName = m_ObjNameParent;
    }
    void Start()
    {
        m_Parent = gameObject;
    }

    public static void AutoNav()
    {
        if (m_ObjAutoNav == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.AUTO_NAV),
                                   LoadSelfCallback);
        }
    }
    public static void ShowSelfeName(Object p_object)
    {
        PlayerNameManager.m_SelfName = m_ParentName;
        PlayerNameManager.CreateSelfeName(p_object);
    }
    private static void LoadSelfCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempPlayerName = (GameObject)Instantiate(p_object);
        m_ObjAutoNav = tempPlayerName;
        tempPlayerName.transform.parent = m_Parent.transform;
      
        tempPlayerName.transform.localPosition = new Vector3(0,-200,0);
        tempPlayerName.transform.localScale = Vector3.one;
    }

    public static void DestroyAutoNav()
    {
        Destroy(m_ObjAutoNav);
    }

}
