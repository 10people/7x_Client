using UnityEngine;
using System.Collections;

public class PlayerWeaponManagerment : MonoBehaviour
{

    public GameObject m_HeavyParent;
    public GameObject m_LightParent;
    public GameObject m_BowParent;
    void Start ()
    {
	
	}
    int index_Type = 0;
	void Update () {
	
	}

    public void ShowWeapon(int index)
    {
        if (index_Type != index)
        {
            index_Type = index;
            if (m_HeavyParent.transform.childCount > 0)
            {
                int size = m_HeavyParent.transform.childCount;
                for (int i = 0; i < size; i++)
                {
                    Destroy(m_HeavyParent.transform.GetChild(i).gameObject);
                }
            }

            if (m_LightParent.transform.childCount > 0)
            {
                int size = m_LightParent.transform.childCount;
                for (int i = 0; i < size; i++)
                {
                    Destroy(m_LightParent.transform.GetChild(i).gameObject);
                }
            }

            if (m_BowParent.transform.childCount > 0)
            {
                int size = m_BowParent.transform.childCount;
                for (int i = 0; i < size; i++)
                {
                    Destroy(m_BowParent.transform.GetChild(i).gameObject);
                }
            }


            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index))
            {
                Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).modelId),
                                                 TaskLoadCallback);
            }
            else
            {
                EquipSuoData.ShowSignal("提示", "没有穿戴该装备!", "");
            }
        }
    }
    public void TaskLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        if (index_Type == 3)
        {
            tempObject.transform.parent = m_HeavyParent.transform;
        }
        else if (index_Type == 4)
        {
            tempObject.transform.parent = m_LightParent.transform;
        }
        else
        {
            tempObject.transform.parent = m_BowParent.transform;
        }

        tempObject.transform.localPosition = Vector3.zero;
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.localRotation = Quaternion.identity ;
    }
}
