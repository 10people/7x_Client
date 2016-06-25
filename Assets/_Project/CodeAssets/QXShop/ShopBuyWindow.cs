using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShopBuyWindow : MonoBehaviour
{

    public static ShopBuyWindow shopBuyWindow;

    private ShopGoodInfo goodInfo;

    private ShopData.ShopType shopType;

    public GameObject windowObj;

    public UISprite moneyIcon;
    public UILabel nameLabel;
    public UILabel desLabel;
    public UILabel numLabel;
    public UILabel needMoney;
    public UILabel buyTimeLabel;

    public GameObject SureBTNObject;

    private string desText;

    public List<EventHandler> buyHandlerList = new List<EventHandler>();

    private GameObject iconSamplePrefab;

    public ScaleEffectController sEffectController;

    void Awake()
    {
        shopBuyWindow = this;
    }

    void OnDestroy()
    {
        shopBuyWindow = null;
    }

    /// <summary>
    /// Gets the buy good info.
    /// </summary>
    /// <param name="tempInfo">Temp info.</param>
    public void SetShopBuyWindow(ShopGoodInfo tempInfo, ShopData.ShopType tempType)
    {
        sEffectController.OnOpenWindowClick();
        goodInfo = tempInfo;
        shopType = tempType;

        if (goodInfo.isAllianceCanBuy)
        {
            buyTimeLabel.text = (goodInfo.isRestrictBuy && goodInfo.countBuyTime > 0) ? ("(今日可购" + (tempInfo.countBuyTime <= 0 ? "[dc0600]" : "[00ff00]") + tempInfo.countBuyTime + "[-]次)") : "";
            moneyIcon.transform.localPosition = (goodInfo.isRestrictBuy && goodInfo.countBuyTime > 0) ? new Vector3(-90, 0, 0) : new Vector3(10, 0, 0);
        }
        else
        {
            LMDuiHuanTemplate template = LMDuiHuanTemplate.getLMDuiHuanTemplateByItemId(goodInfo.itemId);

            buyTimeLabel.text = MyColorData.getColorString(5, "联盟商铺升至" + template.needLv + "级可购买");
            moneyIcon.transform.localPosition = new Vector3(-90, 0, 0);
        }


        if (!goodInfo.isAllianceCanBuy || (tempInfo.isRestrictBuy && tempInfo.countBuyTime <= 0))
        {
            SureBTNObject.GetComponent<BoxCollider>().enabled = false;
            SureBTNObject.GetComponent<UISprite>().color = Color.grey;
        }
        else
        {
            SureBTNObject.GetComponent<BoxCollider>().enabled = true;
            SureBTNObject.GetComponent<UISprite>().color = Color.white;
        }

        moneyIcon = QXComData.MoneySprite(tempInfo.moneyType, moneyIcon, 0.66f);
        numLabel.text = MyColorData.getColorString(1, "购买" + tempInfo.itemNum + "件");
        needMoney.text = MyColorData.getColorString(1, tempInfo.needMoney.ToString());
        nameLabel.text = tempInfo.itemName;

        desText = DescIdTemplate.GetDescriptionById(goodInfo.itemId);
        desLabel.text = desText;

        if (iconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                                    LoadIconSamplePrefab);
        }
        else
        {
            InItIconSamplePrefab();
        }

        foreach (EventHandler handler in buyHandlerList)
        {
            handler.m_click_handler -= BuyHandlerClickBack;
            handler.m_click_handler += BuyHandlerClickBack;
        }

        //yindao 
        switch (shopType)
        {
            case ShopData.ShopType.WEIWANG:
                {
                    if (!ShopPage.shopPage.BuyFinished)
                    {
                        QXComData.YinDaoStateController(QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO, 100220, 4);
                    }
                    break;
                }
            case ShopData.ShopType.GONGXIAN:
                {
                    if (!ShopPage.shopPage.BuyFinished)
                    {
                        QXComData.YinDaoStateController(QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO, 400040, 2);
                    }
                    break;
                }
        }
    }

    void LoadIconSamplePrefab(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        iconSamplePrefab = (GameObject)Instantiate(p_object);

        iconSamplePrefab.SetActive(true);
        iconSamplePrefab.transform.parent = nameLabel.transform.parent;
        iconSamplePrefab.transform.localPosition = new Vector3(-155, 7, 0);

        InItIconSamplePrefab();
    }

    void InItIconSamplePrefab()
    {
        //0普通道具;3当铺材料;5秘宝碎片;6进阶材料;7基础宝石;8高级宝石;9强化材料
        IconSampleManager iconSample = iconSamplePrefab.GetComponent<IconSampleManager>();

        iconSample.SetIconByID(goodInfo.itemId, "", 3);
        iconSample.SetIconPopText(goodInfo.itemId, goodInfo.itemName, desText, 1);
        iconSamplePrefab.transform.localScale = Vector3.one * 0.9f;
    }

    void BuyHandlerClickBack(GameObject obj)
    {
        switch (obj.name)
        {
            case "ZheZhao":
                break;
            case "CancelBtn":
                break;
            case "SureBtn":

                //yindao 
                switch (shopType)
                {
                    case ShopData.ShopType.WEIWANG:
                        {
                            if (QXComData.CheckYinDaoOpenState(100220))
                            {
                                ShopPage.shopPage.BuyFinished = true;
                            }
                            break;
                        }
                    case ShopData.ShopType.GONGXIAN:
                        {
                            if (QXComData.CheckYinDaoOpenState(400040))
                            {
                                ShopPage.shopPage.BuyFinished = true;
                            }

                            break;
                        }
                }

                ShopData.Instance.ShopGoodsBuyReq(goodInfo);

                break;
        }
        gameObject.SetActive(false);

        //Close guide.
        UIYindao.m_UIYindao.CloseUI();
    }
}