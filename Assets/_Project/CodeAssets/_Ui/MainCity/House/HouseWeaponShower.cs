using UnityEngine;
using System.Collections;
using System.Linq;
using qxmobile.protobuf;

public class HouseWeaponShower : MonoBehaviour
{
    [HideInInspector]
    public HouseBasic m_House;

    public UIEventListener OKListener;
    public UILabel WeaponName;
    public UILabel WeaponDesc;

    public Transform m_IconSampleParent;

    private IconSampleManager m_IconSampleManager;
    private GameObject IconSamplePrefab;

    private BagItem m_bagItem;

    public void SetWindow(BagItem tempItem)
    {
        m_bagItem = tempItem;

        WeaponName.text = m_bagItem.name;
        WeaponDesc.text = m_bagItem.desc;

        if (IconSamplePrefab != null)
        {
            WWW temp = null;
            OnIconSampleLoadCallBack(ref temp, "", IconSamplePrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
    }

    private void OnIconSampleLoadCallBack(ref WWW www, string temp, Object loadedObject)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = loadedObject as GameObject;
        }

        var iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
        UtilityTool.ActiveWithStandardize(m_IconSampleParent, iconSampleObject.transform);
        m_IconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

        m_IconSampleManager.SetIconByID(IconSampleManager.IconType.equipment, m_bagItem.itemId);
    }

    private void OnOKClick(GameObject go)
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        OKListener.onClick = OnOKClick;

        HouseModelController.TryAddToHouseDimmer(gameObject);
    }

    void OnDisable()
    {
        OKListener.onClick = null;

        HouseModelController.TryRemoveFromHouseDimmer(gameObject);
    }
}
