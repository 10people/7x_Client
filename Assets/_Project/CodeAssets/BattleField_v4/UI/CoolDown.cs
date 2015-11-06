using UnityEngine;
using System.Collections;

public class CoolDown : MonoBehaviour
{
	public UISprite spriteCD;


	private float timeTotal;

	private float curTimeTotal;

	private float timeLst;

	private Vector3 tempPosition;

	private int id;

	private GameObject tempGc;


	public void init(int skillId)
	{
		id = skillId % 10;

		SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById (skillId);

		timeTotal = skillTemplate.timePeriod * 1f;
		
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan 
		   && CityGlobalData.m_tempSection == 1 
		   && CityGlobalData.m_tempLevel == 1)
		{
			if(skillId == 200011 || skillId == 200021)
			{
				timeTotal = 999;
			}
			else
			{
				timeTotal = id == 2 ? 16 : 10;
			}
		}

		spriteCD.gameObject.SetActive (false);

		tempPosition = transform.localPosition;
	}

	public void init(float _timeTotal)
	{
		timeTotal = _timeTotal;

		spriteCD.gameObject.SetActive (false);

		tempPosition = transform.localPosition;
	}

	public void refreshCDTime(int time = 0)
	{
		float ct = time == 0 ? timeTotal : time * 1f;

		if(ct < timeLst)
		{

		}
		else
		{
			curTimeTotal = ct;

			timeLst = curTimeTotal;
		}

		if(spriteCD.gameObject.activeSelf == false)
		{
			spriteCD.gameObject.SetActive (true);

			timeLst = curTimeTotal;
		}
	}

	public void CoolDownComplate()
	{
		updatePosition ();
		
		if(spriteCD.gameObject.activeSelf == false) return;	

		timeLst -= timeLst;
		
		if(timeLst <= 0)
		{
			spriteCD.gameObject.SetActive(false);

			if(tempGc == null)
			{
				tempGc = new GameObject();
				
				tempGc.transform.parent = transform;
				
				tempGc.transform.localPosition = Vector3.zero;
				
				tempGc.transform.localScale = new Vector3(1, 1, 1);
				
				UILabel label = tempGc.AddComponent<UILabel>();
				
				label.text = "";
			}
			
			UI3DEffectTool.Instance().ShowTopLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             tempGc, 
	             EffectIdTemplate.GetPathByeffectId(110000) );
		}
		else
		{
			spriteCD.fillAmount = timeLst / curTimeTotal;
		}
	}

	public void CoolDownUpdate ()
	{
		updatePosition ();

		if(spriteCD.gameObject.activeSelf == false) return;	

		if(timeTotal != 999) timeLst -= Time.deltaTime;

		if(timeLst <= 0)
		{
			spriteCD.gameObject.SetActive(false);

			if(tempGc == null)
			{
				tempGc = new GameObject();
				
				tempGc.transform.parent = transform;
				
				tempGc.transform.localPosition = Vector3.zero;
				
				tempGc.transform.localScale = new Vector3(1, 1, 1);
				
				UILabel label = tempGc.AddComponent<UILabel>();
				
				label.text = "";
			}
			
			UI3DEffectTool.Instance().ShowTopLayerEffect( 
	             UI3DEffectTool.UIType.FunctionUI_1, 
	             tempGc, 
	             EffectIdTemplate.GetPathByeffectId(110000) );
		}
		else
		{
			spriteCD.fillAmount = timeLst / curTimeTotal;
		}
	}

	private void updatePosition()
	{
		if(id == 1)
		{
			showOrHide(
				BattleUIControlor.Instance().btnDaoSkill_1.activeSelf
				|| BattleUIControlor.Instance().btnQiangSkill_1.activeSelf
				|| BattleUIControlor.Instance().btnGongSkill_1.activeSelf
				);
		}
		else
		{
			showOrHide(
				BattleUIControlor.Instance().btnDaoSkill_2.activeSelf
				|| BattleUIControlor.Instance().btnQiangSkill_2.activeSelf
				|| BattleUIControlor.Instance().btnGongSkill_2.activeSelf
				);
		}
	}

	private void showOrHide(bool show)
	{
		if (show == true) transform.localPosition = tempPosition;

		else transform.localPosition = new Vector3(100000, 0, 0);
	}

}
