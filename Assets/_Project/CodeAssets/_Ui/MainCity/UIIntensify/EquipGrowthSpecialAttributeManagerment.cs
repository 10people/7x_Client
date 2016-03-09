using UnityEngine;
using System.Collections;

public class EquipGrowthSpecialAttributeManagerment : MonoBehaviour
{
    public UILabel m_labName;
    public UILabel m_labCount;
    public UILabel m_labMax;
    public GameObject m_Move;
    public GameObject m_Animation;
    private int _addCount = 0;
    void Start ()
    {
   
    }
    public void ShowInfo(EquipSuoData.WashInfo attribute, bool isadd, bool isnew)
    {
        m_labName.text = MyColorData.getColorString(ColorID(attribute._type), NameIdTemplate.GetName_By_NameId(attribute._nameid));
        m_labCount.text = MyColorData.getColorString(ColorID(attribute._type), attribute._count.ToString());
       // m_labMax.gameObject.SetActive(attribute._isMax);
        m_Animation.SetActive(attribute._isnew);
        if (!attribute._isMax && isadd && !isnew)
        {
            _addCount = attribute._add;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_MOVE_ITEM), ResourcesLoadAddCallBack);
        }

        if (attribute._isnew)
        {
            StartCoroutine(WatiFor(m_Animation));
        }
    }
    public void ShowEquipInfo(EquipSuoData.WashInfo attribute)
    {
        m_labName.text = MyColorData.getColorString(ColorID(attribute._type), NameIdTemplate.GetName_By_NameId(attribute._nameid));
        m_labCount.text = MyColorData.getColorString(ColorID(attribute._type), attribute._count.ToString());
    }
    IEnumerator WatiFor(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        m_Animation.SetActive(false);
    }
    private int ColorID(int type)
    {
        if (type == 0)
        {
            return 6;
        }
        else
        {
            return 7;
        }
    }
    public void ShowRangeInfo(EquipGrowthWashManagerment.RangeInfo rangeInfo)
    {
        m_labName.text = MyColorData.getColorString(2 , NameIdTemplate.GetName_By_NameId(rangeInfo._nameid));
        m_labCount.text = MyColorData.getColorString(5,rangeInfo._min.ToString()) + MyColorData.getColorString(2, "--") + MyColorData.getColorString(4, rangeInfo._max.ToString());
    }

    public void ShowAddInfo(EquipGrowthWashManagerment.AddInfo AddInfo)
    {
        m_labName.text = MyColorData.getColorString(2, NameIdTemplate.GetName_By_NameId(AddInfo._nameid));
        if (AddInfo._add >= 0)
        {
            m_labCount.text = MyColorData.getColorString(4,  AddInfo._add.ToString());
        }
        else
        {
            m_labCount.text = MyColorData.getColorString(5, AddInfo._add.ToString());
        }
    }

    public void ResourcesLoadAddCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject rewardShow = Instantiate(p_object) as GameObject;
        rewardShow.transform.parent = m_labCount.transform;
        rewardShow.transform.localScale = Vector3.one;
        rewardShow.transform.localPosition = Vector3.zero;
        if (_addCount < 0)
        {
            rewardShow.GetComponent<UILabel>().text = MyColorData.getColorString(5, _addCount.ToString());
        }
        else
        {
            rewardShow.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+" + _addCount.ToString());
        }
        rewardShow.AddComponent<TweenPosition>();
        rewardShow.AddComponent<TweenAlpha>();
        rewardShow.GetComponent<TweenPosition>().from = rewardShow.transform.localPosition;
        rewardShow.GetComponent<TweenPosition>().to = rewardShow.transform.localPosition + Vector3.up * 40;
        rewardShow.GetComponent<TweenPosition>().duration = 0.5f;
        rewardShow.GetComponent<TweenAlpha>().from = 1.0f;
        rewardShow.GetComponent<TweenAlpha>().to = 0;
        rewardShow.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiForee(rewardShow));
    }

    IEnumerator WatiForee(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
    }
//public void CreateClone(int content)
//{
//    GameObject clone = NGUITools.AddChild(m_labCount.gameObject, m_labCount);
//    clone.transform.localPosition = m_labCount.transform.localPosition;
//    clone.transform.localRotation = m_labCount.transform.localRotation;
//    clone.transform.localScale = m_labCount.transform.localScale;
//    if (clone.GetComponent<UILabel>() != null)
//    {
//        clone.GetComponent<UILabel>().text = "";
//    }
//    else
//    {
//        clone.AddComponent<UILabel>();
//    }

//    if (content < 0)
//    {
//        clone.GetComponent<UILabel>().text = MyColorData.getColorString(5, content.ToString());
//    }
//    else
//    {
//        clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+" + content.ToString());
//    }
//    clone.AddComponent<TweenPosition>();
//    clone.AddComponent<TweenAlpha>();
//    clone.GetComponent<TweenPosition>().from = m_labCount.transform.localPosition;
//    clone.GetComponent<TweenPosition>().to = m_labCount.transform.localPosition + Vector3.up * 40;
//    clone.GetComponent<TweenPosition>().duration = 0.5f;
//    clone.GetComponent<TweenAlpha>().from = 1.0f;
//    clone.GetComponent<TweenAlpha>().to = 0;
//    clone.GetComponent<TweenPosition>().duration = 0.8f;
//}      
}
