using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIBox : MonoBehaviour
{
    public ScaleEffectController m_ScaleEffectController;

    public UILabel m_labelTile;
    public UILabel m_labelDis1;
    public UILabel m_labelDis2;
    public UIButton m_button1;
    public UIButton m_button2;
    public UILabel m_labelButton1;
    public UILabel m_labelButton2;
    public UIPanel m_Panel;
    public int m_ButtonX = 0;
	public Camera m_Camera;
    private int m_ButtonYDown = -192;
    private int m_ButtonYUp = -12;
    private int m_LabelY = -30;
    private float m_fScale = 0.1f;

	public UIFont UI_TitleFont;

	public UIFont UI_btnFont;

    private static bool isShowNumBelow = false;

    public delegate void onclick(int i );

    public delegate void OnBoxCreated(GameObject p_game_object);

    public onclick m_onclick;

    private class SetBoxCallback
    {
        public UIBox m_ui_box;

        public BagItem m_bag_item;

        public int m_pos_x;

        public int m_pos_y;

        public void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            IconSampleManager tempManager = tempObject.GetComponent<IconSampleManager>();
            tempManager.transform.parent = m_ui_box.m_labelTile.transform.parent;
            tempManager.transform.localScale = Vector3.one;
            tempManager.transform.localPosition = new Vector3(m_pos_x, m_pos_y, 0);

            try
            {
                tempManager.SetIconByID(m_bag_item.itemType == 20000 ? IconSampleManager.IconType.equipment : IconSampleManager.IconType.item, m_bag_item.itemId, UIBox.isShowNumBelow ? "" : ("x" + m_bag_item.cnt), 5);
                tempManager.SetIconPopText(m_bag_item.itemId);
            }
            catch
            {
                tempManager.SetIconType(m_bag_item.itemType == 20000 ? IconSampleManager.IconType.equipment : IconSampleManager.IconType.item);
                tempManager.SetIconBasic(5, "20501", UIBox.isShowNumBelow ? "" : ("x" + m_bag_item.cnt));
				tempManager.SetIconPopText(m_bag_item.itemId);
            }

            if (UIBox.isShowNumBelow)
            {
                ShowBagItemNums(tempObject, m_bag_item);
            }
        }

        private void ShowBagItemNums(GameObject iconObject, BagItem bagItem)
        {
            var child = Instantiate(m_ui_box.m_labelDis1.gameObject) as GameObject;
            child.transform.parent = iconObject.transform;

            //Add child label below icon.
            child.transform.localPosition = new Vector3(0, -55, 0);
            child.transform.rotation = new Quaternion(0, 0, 0, 0);
            child.transform.localScale = Vector3.one;

            //Set label.
            var label = child.GetComponent<UILabel>();
            label.overflowMethod = UILabel.Overflow.ResizeFreely;
            label.applyGradient = false;
            label.fontSize = 30;
            label.color = new Color(1, 1, 1, 1);
            label.depth = 2;

            label.text = "×" + bagItem.cnt;
        }
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (m_fScale < 1)
            {
                if (1 - m_fScale < 0.1f)
                {
                    m_button1.GetComponent<EventHandler>().m_handler += OnClick;
                    m_button2.GetComponent<EventHandler>().m_handler += OnClick;
                    m_fScale = 1;
                }
                else
                {
                    m_fScale += ((1 - m_fScale) / 2);
                }
                m_Panel.transform.localScale = new Vector3(m_fScale, m_fScale, m_fScale);
            }
        }
    }

    //	public void setBox(string tile, string dis1, string dis2, List<BagItem> bagItem, string buttonname1, string buttonname2, UIBox.onclick onClcik)
    //	{
    //		setBox(tile, dis1, dis2, bagItem, buttonname1, buttonname2, onClcik);
    //	}

    public void setBox(
        string tile, string dis1, string dis2,
        List<BagItem> bagItem,
        string buttonname1, string buttonname2,
        UIBox.onclick onClick,
        UIFont uifontTile = null, UIFont uifontButton1 = null, UIFont uifontButton2 = null,
        bool isShowBagItemNumBelow = false, bool isSetDepth = true, bool isBagItemTop = true)
    {
        m_onclick = onClick;
        m_ButtonX = 0;

        if (uifontTile != null)
        {
            //			m_labelTile.font = uifontTile;
            m_labelTile.bitmapFont = uifontTile;
        }
		else
		{
			m_labelTile.bitmapFont = UI_TitleFont;
		}

        m_labelTile.text = tile;//设置tile文字

        //设置上 介绍文字
        if (string.IsNullOrEmpty(dis1))
        {
            m_labelDis1.text = "";
            m_LabelY = 55;
        }
        else
        {
            m_labelDis1.text = dis1;
        }

        //设置物品
        if (bagItem != null)
        {
            UIBox.isShowNumBelow = isShowBagItemNumBelow;
            m_ButtonX = (bagItem.Count - 1) * -100;
            for (int i = 0; i < bagItem.Count; i++)
            {
                createIcon(bagItem[i], m_ButtonX + (i * 200), isBagItemTop ? m_ButtonYUp : m_ButtonYDown);
            }

            m_LabelY = string.IsNullOrEmpty(dis1) ? 105 : 70;
        }
        else
        {
            UIBox.isShowNumBelow = false;
        }

        //设置下介绍
        if (string.IsNullOrEmpty(dis2))
        {
            m_labelDis2.text = "";
        }
        else
        {
            m_labelDis2.text = dis2;
        }

        m_labelDis2.transform.localPosition = new Vector3(0, m_LabelY, 0);
        if (uifontButton1 != null)
        {
            //			m_labelButton1.font = uifontButton1;
			m_labelButton1.bitmapFont  = UI_btnFont;
        }
		else
		{
			m_labelButton1.bitmapFont  = UI_btnFont;
		}
        if (uifontButton2 != null)
        {
           
			m_labelButton2.bitmapFont  = UI_btnFont;
        }
		else
		{
			m_labelButton2.bitmapFont  = UI_btnFont;
		}
        if (string.IsNullOrEmpty(buttonname2))
        {
            m_labelButton1.text = buttonname1;

            m_button1.transform.localPosition = new Vector3(0, m_button1.transform.localPosition.y, m_button1.transform.localPosition.z);

            m_button2.gameObject.SetActive(false);
        }
        else
        {
            m_labelButton1.text = buttonname1;

            m_labelButton2.text = buttonname2;
        }

//		Debug.Log(isSetDepth);

		if(isSetDepth)
		{
			m_Camera.depth = 45;
			m_Panel.depth = 1000;
		}
		else
		{
			m_Camera.depth = 100;
			m_Panel.depth = 1005;
		}
    }

    public void createIcon(BagItem bagItem, int p_pos_x, int p_pos_y)
    {
        SetBoxCallback t_set_box = new SetBoxCallback();

        {
            t_set_box.m_ui_box = this;

            t_set_box.m_bag_item = bagItem;

            t_set_box.m_pos_x = p_pos_x;

            t_set_box.m_pos_y = p_pos_y;
        }

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                                t_set_box.ResourceLoadCallback);
    }

    public void OnClick(GameObject obj)
    {
//		Debug.Log( "OnClick()" );

		m_button1.GetComponent<BoxCollider> ().enabled = false;
		m_button2.GetComponent<BoxCollider> ().enabled = false;

        if (m_onclick != null)
        {
            m_onclick( int.Parse(obj.name.Substring(6, 1)) );
        }

        DoCloseWindow();
    }

    void DoCloseWindow()
    {
		if( gameObject.activeSelf ){
			gameObject.SetActive( false );

			Destroy(gameObject);
		}
    }

    void OnEnable()
    {
        MainCityUI.TryAddToObjectList(gameObject);
        HouseModelController.TryAddToHouseDimmer(gameObject);
    }

    void OnDisable()
    {
        MainCityUI.TryRemoveFromObjectList(gameObject);
        HouseModelController.TryRemoveFromHouseDimmer(gameObject);
    }
}