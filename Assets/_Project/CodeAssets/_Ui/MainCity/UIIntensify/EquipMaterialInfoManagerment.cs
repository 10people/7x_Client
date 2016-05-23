using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipMaterialInfoManagerment : MonoBehaviour {
  
    private int MaterialUsingCount = 0;
    private string MaterialAllCount;
    private bool TouchController = false;
 

    public List<ButtonOnPressManagerment> m_listPress;
    public UILabel m_LabCount;
    public UISprite m_SpriteIcon;
    public UISprite m_SpriteQuality;
    public GameObject SubButton;

    public delegate void Over_JunZhu_Level(int index);
    Over_JunZhu_Level Over_Step;
    [HideInInspector]
    public int m_Itemid = 0;
    private int pinzhiSave = 0;
    [HideInInspector]
    public bool m_IntenseShow = false;

    private float _timeInterval = 0.0f;
    private bool _isPressDown = false;
    private int _IndexTouch = 0;

    void Start()
    {
        m_listPress.ForEach(p => p.m_PressHandle += ButtonPress);
     
    }

    void Update()
    {
        if (_isPressDown)
        {
            _timeInterval += Time.deltaTime;
            if (_timeInterval >= 0.2f)
            {
                _timeInterval = 0.0f;
                if (_IndexTouch == 0)
                {
                    OnMaterialClick();
                }
                else
                {
                    OnSubButtonClick();
                }
            }
        }
    }
    void ButtonPress(int index,bool isdown)
    {
       _IndexTouch = index;
       _isPressDown = isdown;
    }
    

    public void ShowMaterialInfo(int id, string icon, string count, bool isMaxExp, int pinzhi, Over_JunZhu_Level callback)
    {
        Over_Step = callback;
        pinzhiSave = pinzhi;
        MaterialUsingCount = 0;
        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
        m_Itemid = id;
        MaterialAllCount = count;
        TouchController = isMaxExp;

        bool isNoMaterial = (!string.IsNullOrEmpty(icon) || !string.IsNullOrEmpty(count));

        //Set iconSample.


        if (isNoMaterial)
        {
            m_SpriteIcon.spriteName = icon;
            m_LabCount.text = count;
 
            m_SpriteQuality.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(icon)).color);
            m_SpriteIcon.gameObject.SetActive(true);
            m_SpriteQuality.gameObject.SetActive(true);
            SubButton.transform.GetComponent<Collider>().enabled = true;
            m_SpriteIcon.transform.GetComponent<Collider>().enabled = true;
     
        }
        else
        {
            m_SpriteIcon.gameObject.SetActive(false);
            m_SpriteQuality.gameObject.SetActive(false);
            SubButton.SetActive(false);
            SubButton.transform.GetComponent<Collider>().enabled = false;
            m_SpriteIcon.transform.GetComponent<Collider>().enabled = false;
            m_LabCount.text = count;
     
    
        }

        //  IconSampleManager.RightButtomCornorLabel.gameObject.SetActive(true);
    }
    private bool _isTouched = false;
    private void OnMaterialClick()
    {
        if (MaterialUsingCount > 0)
        {
            SubButton.transform.GetComponent<Collider>().enabled = true;
        }
        else
        {
            SubButton.transform.GetComponent<Collider>().enabled = false;
        }
        if (m_IntenseShow)
        {
            if (UIYindao.m_UIYindao.m_isOpenYindao && !_isTouched)
            {
                _isTouched = true;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }


            if (TouchController && pinzhiSave != 200)
            {
                EquipGrowthMaterialUseManagerment.m_MaterialId._itemid = m_Itemid;

                if (EquipGrowthMaterialUseManagerment.touchIsEnable)
                {

                    if (MaterialUsingCount < int.Parse(MaterialAllCount))
                    {
                        EquipGrowthMaterialUseManagerment.materialItemTouched = true;
                        MaterialUsingCount++;
                        m_LabCount.text = MaterialUsingCount.ToString() + "/" + MaterialAllCount;
                      //  EquipGrowthMaterialUseManagerment.AddUseMaterials(m_Itemid);
                    }
                    SubButton.SetActive(true);
                    SubButton.transform.GetComponent<Collider>().enabled = true;
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

    private void OnSubButtonClick( )
    {
        if (m_IntenseShow)
        {
   
            if (TouchController && pinzhiSave != 200)
            {
                //EquipGrowthMaterialUseManagerment.m_MaterialId = m_Itemid;

                if (MaterialUsingCount > 0)
                {
                    EquipGrowthMaterialUseManagerment.materialItemTouched = true;
                    EquipGrowthMaterialUseManagerment.materialItemReduce = true;
                    MaterialUsingCount--;
                    if (MaterialUsingCount == 0)
                    {
                        SubButton.SetActive(false);
                        SubButton.transform.GetComponent<Collider>().enabled = false;
                        m_LabCount.text = MaterialAllCount;
                    }
                    else
                    {
                        m_LabCount.text = MaterialUsingCount + "/" + MaterialAllCount;
                    }
                }
            }
        }
    }
 
}
