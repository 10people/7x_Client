using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class TenementPortal : MonoBehaviour
{
	void Start () 
    {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (!FunctionOpenTemp.GetWhetherContainID(8) && other.name.Equals("EffectPortal"))
        {
            if (MainCityUI.IsWindowsExist())
            {
                return;
            }
            ClientMain.m_UITextManager.createText(MyColorData.getColorString(1, FunctionOpenTemp.GetTemplateById(8).m_sNotOpenTips));
            return;
        }
        if (other.name.Equals("EffectPortal"))
        {
            if (MainCityUI.IsWindowsExist())
            {
                return;
            }
            PlayerModelController.m_playerModelController.m_isSendPos = false;
            MainCityUIRB.IsCanClickButtons = true;

            Vector3 v = new Vector3(8.34f, 9.36f, 21.0f);
            SpriteMove tempPositon = new SpriteMove();
                    tempPositon.posX = v.x;
                    tempPositon.posY = v.y;
                    tempPositon.posZ = v.z;
                    //tempPositon.uid = JunZhuData.Instance().m_junzhuInfo.id;

                    MemoryStream t_tream = new MemoryStream();
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    t_qx.Serialize(t_tream, tempPositon);

            

                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.Sprite_Move, ref t_protof, false);



            FunctionWindowsCreateManagerment.m_isJieBiao = true;
            GameObject obj = new GameObject();
            obj.name = "MainCityUIButton_" + 310;
            MainCityUI.m_MainCityUI.MYClick(obj);
        }
     
        ////Debug.Log("otherotherotherotherotherotherother :: " + other.name);
        //if (CityGlobalData.m_isAllianceTenentsScene && other.name.Equals("AllianceCityPortal"))
        //{
        //    CityGlobalData.m_isAllianceTenentsScene = false;

        //    CityGlobalData.m_isAllianceScene = true;
        //    //SceneManager.EnterAllianceCity();
        //}
        //else if (other.name.Equals("EffectBigHouse") || other.name.Equals("EffectPortal"))
        //{
        //    if (!PlayerModelController.m_isNavMesh)
        //    {
        //        TenementManagerment.Instance.NavgationToTenement(other.GetComponent<TenementEnterPortal>().m_indexNum - 1000);
        //    }
        //}
        //else if (other.name.Equals("TransferPortal"))
        //{
        //    CityGlobalData.m_isAllianceScene = false;
        //    CityGlobalData.m_iAllianceTenentsSceneNum = other.GetComponent<TenementEnterPortal>().m_indexNum;
        //    // Debug.Log(" CityGlobalData.m_iAllianceTenentsSceneNum CityGlobalData.m_iAllianceTenentsSceneNum CityGlobalData.m_iAllianceTenentsSceneNum ::" + CityGlobalData.m_iAllianceTenentsSceneNum);
        //    CityGlobalData.m_isAllianceTenentsScene = true;
        //    //  SceneManager.EnterAllianceCityTenentsCityOne();
        //}
        //else if (other.name.Equals("RangCollider"))
        //{
        //    other.transform.parent.GetComponent<TenementEnterPortal>().m_labName.gameObject.SetActive(true);
        //    //other.transform.parent.GetComponent<TenementEnterPortal>().m_ObjRang.SetActive(false);
        //}
    }

    //void OnTriggerExit(Collider other)
    //{
    //    //Debug.Log("otherotherotherotherotherotherother ::  " + other.name);
    //    if (other.name.Equals("TransferPortal"))
    //    {
    //        other.GetComponent<TenementEnterPortal>().m_ObjRang.SetActive(true);
    //    }
    //    else if (other.name.Equals("RangCollider"))
    //    {
    //        other.transform.parent.GetComponent<TenementEnterPortal>().m_labName.gameObject.SetActive(false);
    //    }
    //}
}
