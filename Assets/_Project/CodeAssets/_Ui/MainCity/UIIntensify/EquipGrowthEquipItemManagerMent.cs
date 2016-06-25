using UnityEngine;
using System.Collections;

public class EquipGrowthEquipItemManagerMent : MonoBehaviour
{
    public UILabel m_Level;
    public GameObject m_Suo;
    public UISprite m_SpritePinZhi;
    public UISprite m_SpriteIcon;
    public GameObject m_ObjTanHao;
    public EventIndexHandle m_Event;
    public GameObject m_ObjEffect;
    private int _addCount = 0;
    private int _FontSize = 0;
    public void MoveLabel(int _add,int size = 0)
    {
        _FontSize = size;
        _addCount = _add;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_MOVE_ITEM), ResourcesLoadAddCallBack);
    }
    public void ResourcesLoadAddCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject rewardShow = Instantiate(p_object) as GameObject;
        rewardShow.transform.parent = m_SpriteIcon.transform;
        rewardShow.transform.localScale = Vector3.one;
        rewardShow.transform.localPosition = Vector3.zero;

        if (_FontSize > 0)
        {
            rewardShow.GetComponent<UILabel>().fontSize = _FontSize;
        }
        
        {
            rewardShow.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+" + _addCount.ToString());
        }
        rewardShow.AddComponent<TweenPosition>();
        rewardShow.AddComponent<TweenAlpha>();
        rewardShow.GetComponent<TweenPosition>().from = rewardShow.transform.localPosition;
        rewardShow.GetComponent<TweenPosition>().to = rewardShow.transform.localPosition + Vector3.up * 40;
        rewardShow.GetComponent<TweenPosition>().duration = 0.1f;
        rewardShow.GetComponent<TweenAlpha>().from = 1.0f;
        rewardShow.GetComponent<TweenAlpha>().to = 0;
        rewardShow.GetComponent<TweenPosition>().duration = 0.15f;
        StartCoroutine(WatiForee(rewardShow));
    }

    IEnumerator WatiForee(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
        m_ObjEffect.SetActive(false);
        EquipGrowthEquipInfoManagerment.m_isEffect = true;
    }
}
