using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipGrowthMaterialItem : MonoBehaviour
{
    [HideInInspector]
    public IconSampleManager IconSampleManager;

    private int MaterialUsingCount = 0;
    private string MaterialAllCount;
    private bool TouchController = false;

    public delegate void Over_JunZhu_Level(int index);
    Over_JunZhu_Level Over_Step;
    [HideInInspector]
    public int m_Itemid = 0;
    public long m_ItemDB = 0;
    private int pinzhiSave = 0;
    [HideInInspector]
    public bool m_IntenseShow = false;


    private float _timeInterval = 0.0f;
    private bool _isPressDownAdd = false;
    private bool _isPressDownSub = false;

    void Update()
    {
        if (_isPressDownAdd)
        {
            _timeInterval += Time.deltaTime;
            if (_timeInterval >= 0.2f)
            {
                _timeInterval = 0.0f;
                OnMaterialClick(gameObject);
            }
        }


        if (_isPressDownSub)
        {
            _timeInterval += Time.deltaTime;
            if (_timeInterval >= 0.2f)
            {
                _timeInterval = 0.0f;
                OnSubButtonClick(gameObject);
            }
        }
    }

    public struct MaterialNeed
    {
        public int _itemid;
        public long _dbid;
        public string _icon;
        public string _count;
        public int _pinzhi;
    };
  //  public void ShowMaterialInfo(int id, string icon, string count, bool isMaxExp, Over_JunZhu_Level callback)
    public void ShowMaterialInfo(MaterialNeed material, bool isMaxExp ,Over_JunZhu_Level callback)
    {
        IconSampleManager.transform.localScale = Vector3.one * 0.5f;
        Over_Step = callback;
        pinzhiSave = material._pinzhi;
        MaterialUsingCount = 0;
        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
        m_Itemid = material._itemid;
        m_ItemDB = material._dbid;
        MaterialAllCount = material._count;
        TouchController = isMaxExp;
        bool isNoMaterial = (string.IsNullOrEmpty(material._icon) || string.IsNullOrEmpty(material._count));

        //Set iconSample.
        if (!string.IsNullOrEmpty(material._count))
        {
            IconSampleManager.SetIconByID(material._itemid, material._count);
        }
        else 
        {
            IconSampleManager.SetIconByID(-1);
        }
        IconSampleManager.transform.localScale = Vector3.one * 0.85f;
        if (isNoMaterial)
        {
            IconSampleManager.SetIconBasicDelegate(false, true, null);
            IconSampleManager.SetIconButtonDelegate(null, null, null);
        }
        else
        {
            IconSampleManager.SetIconBasicDelegate(false, true, OnMaterialClick,null, null);
            IconSampleManager.SetIconButtonDelegate(null, null, OnSubButtonClick);
        }

        IconSampleManager.RightButtomCornorLabel.gameObject.SetActive(true);
    }

    private void OnMaterialClickPress(GameObject go)
    {
        _isPressDownAdd = true;
    }
    private void OnMaterialClickFinish(GameObject go)
    {
        _isPressDownAdd = false;
    }

    private void OnMaterialClick(GameObject go)
    {
        if (m_IntenseShow)
        {

            if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100040;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 4;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
            }
            // if (UIYindao.m_UIYindao.m_isOpenYindao)
            {
                //    CityGlobalData.m_isRightGuide = false;
                //if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress >= 0)
                //{
                 
                //    //TaskData.Instance.m_iCurMissionIndex = 100040;

                //    //ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                //    //tempTaskData.m_iCurIndex = 4;
                //    //UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                //}
                //else if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
                //{
                //    TaskData.Instance.m_iCurMissionIndex = 100080;

                //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                //    tempTaskData.m_iCurIndex = 3;
                //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                //}
                //else
                //{
                //    UIYindao.m_UIYindao.CloseUI();
                //}
            }

            //Debug.Log("TouchControllerTouchController :L::" + TouchController);
            if (TouchController && pinzhiSave != 200)
            {
                EquipGrowthMaterialUseManagerment.TopuchInfo tinfo = new EquipGrowthMaterialUseManagerment.TopuchInfo();
                tinfo._itemid = m_Itemid;
                tinfo._dbid = m_ItemDB;
                EquipGrowthMaterialUseManagerment.m_MaterialId = tinfo;
                //Debug.Log("touchIsEnabletouchIsEnabletouchIsEnable :L::" + EquipGrowthMaterialUseManagerment.touchIsEnable);
                if (EquipGrowthMaterialUseManagerment.touchIsEnable)
                {

                    if (MaterialUsingCount < int.Parse(MaterialAllCount))
                    {
                        EquipGrowthMaterialUseManagerment.materialItemTouched = true;
                        MaterialUsingCount++;
                        IconSampleManager.RightButtomCornorLabel.text = MaterialUsingCount.ToString() + "/" + MaterialAllCount;
                        EquipGrowthMaterialUseManagerment.AddUseMaterials(EquipGrowthMaterialUseManagerment.m_MaterialId);
                    }
                    IconSampleManager.SubButton.SetActive(true);
                }
                else
                {
                    if (MaterialUsingCount < int.Parse(MaterialAllCount))
                    {
                        if (Over_Step != null)
                        {
                            Over_Step(1);
                        }
                    }
                }
            }
            else if (!TouchController && pinzhiSave != 200)
            {
                Over_Step(-1);
            }
        }
        else
        {
            if (Over_Step != null)
            {
                Over_Step(1);
            }
        }
    }

    private void OnSubButtonClickPress(GameObject go)
    {
        _isPressDownSub = true;
    }

    private void OnSubButtonClickFinish(GameObject go)
    {
        _isPressDownSub = false;
    }

    private void OnSubButtonClick(GameObject go)
    {
        if (m_IntenseShow)
        {
            if (TouchController && pinzhiSave != 200)
            {
                EquipGrowthMaterialUseManagerment.m_MaterialId._itemid = m_Itemid;

                if (MaterialUsingCount > 0)
                {
                    EquipGrowthMaterialUseManagerment.materialItemTouched = true;
                    EquipGrowthMaterialUseManagerment.materialItemReduce = true;
                    MaterialUsingCount--;
                    EquipGrowthMaterialUseManagerment.m_MaterialId._itemid = m_Itemid;
                    EquipGrowthMaterialUseManagerment.m_MaterialId._dbid = m_ItemDB;
                    //EquipGrowthMaterialUseManagerment.ReduceUseMaterials(EquipGrowthMaterialUseManagerment.m_MaterialId);
                    if (MaterialUsingCount == 0)
                    {
                        IconSampleManager.SubButton.SetActive(false);
                        IconSampleManager.RightButtomCornorLabel.text = MaterialAllCount;
                    }
                    else
                    {
                        IconSampleManager.RightButtomCornorLabel.text = MaterialUsingCount + "/" + MaterialAllCount;
                    }
                }
            }
        }
    }
    public void showLabInfo(int count_use)
    {
        MaterialUsingCount = count_use;
        IconSampleManager.SubButton.SetActive(true);
        IconSampleManager.RightButtomCornorLabel.text = MaterialUsingCount + "/" + MaterialAllCount;
    }

}
