using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//[RequireComponent(typeof(NpcStateManager))]
public class NpcObjectItem : MonoBehaviour {

	[HideInInspector]
	public NpcCityTemplate m_template;

	public UILabel m_nameLabel;

	public UISprite m_iconSprite;

    public GameObject m_Shadow;

	Transform m_transform;
	

	void Awake()
	{
		m_transform = this.transform;
	}
    //void Update()
    //{
    //    Debug.Log(" transform.position transform.position transform.position" + transform.position);
    //}

	public void InitWithNpc(NpcCityTemplate template) //初始化npc
	{
		m_template = template;

		m_transform.localEulerAngles = new Vector3(0,m_template.m_Angles,0);
        m_transform.gameObject.AddComponent<SoundPlayEff>();
		m_transform.localPosition = m_template.m_position;
		//m_nameLabel.text =MyColorData.getColorString(4,"[b]" + NameIdTemplate.GetName_By_NameId(template.m_npcName) + "[/b]");

        m_nameLabel.text = MyColorData.getColorString(4, "[b]" + NameIdTemplate.GetName_By_NameId(template.m_npcName) + "[/b]");

//		m_nameLabel.text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(template.m_npcName));
//		m_nameLabel.effectStyle = UILabel.Effect.Shadow;
//		m_nameLabel.effectColor = Color.black;

		m_iconSprite.spriteName = template.m_npcIcon.ToString();
        m_Shadow.SetActive( Quality_Shadow.InCity_ShowSimpleShadow());
        m_nameLabel.transform.localEulerAngles = new Vector3(m_template.NameDirectX, m_template.NameDirectY, 0);
        m_iconSprite.transform.localEulerAngles = new Vector3(m_template.NameDirectX, m_template.NameDirectY, 0);
       // m_nameLabel.transform.localEulerAngles = new Vector3(0, -m_template.m_Angles, 0);

        if (CityGlobalData.m_isAllianceTenentsScene)
        {
            m_iconSprite.transform.localEulerAngles = new Vector3(0, m_template.NameDirectY, 0);
        }

		if(SceneManager.IsInBattleFieldScene())
		{
			m_iconSprite.gameObject.SetActive(false);
			m_nameLabel.gameObject.SetActive(false);
		}
	}

    public void InitWithTenementNpc(NpcCityTemplate template) //初始化房屋npc
    {
        m_template = template;

        if (gameObject.name.Equals("TransferPortal") || gameObject.name.Equals("AllianceCityPortal") || gameObject.name.Equals("EffectBigHouse") || gameObject.name.Equals("EffectPortal"))
        {
            m_transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else
        {

            m_transform.localEulerAngles = new Vector3(270, 0, 0);
        }

        m_transform.localPosition = m_template.m_position;
     
        //		m_nameLabel.text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(template.m_npcName));
        //		m_nameLabel.effectStyle = UILabel.Effect.Shadow;
        //		m_nameLabel.effectColor = Color.black;

   

         //m_nameLabel.transform.localEulerAngles = new Vector3(270, 0, 0);

         //m_iconSprite.transform.localEulerAngles = new Vector3(0, -m_template.m_Angles, 0);
    }
}
