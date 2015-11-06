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
    public void ShowMaterialInfo(int id, string icon, string count, bool isMaxExp, int pinzhi,Over_JunZhu_Level callback)
    {
        Over_Step = callback;
        pinzhiSave = pinzhi;
        MaterialUsingCount = 0;
        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
        m_Itemid = id;
        MaterialAllCount = count;
        TouchController = isMaxExp;

        bool isNoMaterial = (string.IsNullOrEmpty(icon) || string.IsNullOrEmpty(count));

        //Set iconSample.
        IconSampleManager.SetIconType(IconSampleManager.IconType.equipment);
        IconSampleManager.SetIconBasic(0,
            icon, !string.IsNullOrEmpty(icon) ? count : "",
            pinzhi == 200 ? "" : IconSampleManager.QualityPrefix + QualityIconSelected.SelectQualityNum(pinzhi));

        if (isNoMaterial)
        {
            IconSampleManager.SetIconBasicDelegate(false, true, null);
            IconSampleManager.SetIconButtonDelegate(null, null, null);
        }
        else
        {

            IconSampleManager.SetIconBasicDelegate(true, true, OnMaterialClick,OnMaterialClickPress, OnMaterialClickFinish);
            IconSampleManager.SetIconButtonDelegate(null, null, OnSubButtonClick);

          // IconSampleManager.SetIconBasicDelegate(false, true, OnMaterialClick);
         //   IconSampleManager.SetIconButtonDelegate(null, null, OnSubButtonClick);
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
            
           // if (UIYindao.m_UIYindao.m_isOpenYindao)
            {
                //    CityGlobalData.m_isRightGuide = false;
                if (FreshGuide.Instance().IsActive(100160) && TaskData.Instance.m_TaskInfoDic[100160].progress >= 0)
                {
                 
                    TaskData.Instance.m_iCurMissionIndex = 100160;

                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                    tempTaskData.m_iCurIndex = 3;
                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                }
                else if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
                {
                    TaskData.Instance.m_iCurMissionIndex = 100080;

                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                    tempTaskData.m_iCurIndex = 3;
                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                }
                else
                {
                    UIYindao.m_UIYindao.CloseUI();
                }
            }


            if (TouchController && pinzhiSave != 200)
            {
                EquipGrowthMaterialUseManagerment.m_MaterialId = m_Itemid;

                if (EquipGrowthMaterialUseManagerment.touchIsEnable)
                {

                    if (MaterialUsingCount < int.Parse(MaterialAllCount))
                    {
                        EquipGrowthMaterialUseManagerment.materialItemTouched = true;
                        MaterialUsingCount++;
                        IconSampleManager.RightButtomCornorLabel.text = MaterialUsingCount.ToString() + "/" + MaterialAllCount;
                        EquipGrowthMaterialUseManagerment.AddUseMaterials(m_Itemid);
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
           // if (UIYindao.m_UIYindao.m_isOpenYindao)
            //{
            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //}
            if (TouchController && pinzhiSave != 200)
            {
                EquipGrowthMaterialUseManagerment.m_MaterialId = m_Itemid;

                if (MaterialUsingCount > 0)
                {
                    EquipGrowthMaterialUseManagerment.materialItemTouched = true;
                    EquipGrowthMaterialUseManagerment.materialItemReduce = true;
                    MaterialUsingCount--;
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



}
