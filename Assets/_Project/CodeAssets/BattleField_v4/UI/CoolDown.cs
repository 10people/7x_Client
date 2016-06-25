using UnityEngine;
using System.Collections;

public class CoolDown : MonoBehaviour
{
	public UISprite spriteCD;

	public UILabel labelCDTime;

	public BattleWeaponSkillDolt spriteWeaponDolt;

	public UISprite spriteIcon;


	private float timeTotal;

	private float curTimeTotal;

	private float timeLst;

	private Vector3 tempPosition;

	private int id;

	private GameObject tempGc;

	private Color iconColor = new Color(1f, 1f, 1f, 1f);

	private Color iconGray = new Color(0f, 0f, 0f, 1f);


	void Awake(){

	}

	public void init(int skillId)
	{
		id = skillId % 10;

		SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById (skillId);

		timeTotal = skillTemplate.timePeriod * 1f;
		
		spriteCD.gameObject.SetActive (false);

		setIconColor(iconColor);

		tempPosition = transform.localPosition;
	}

	public void init(float _timeTotal)
	{
		timeTotal = _timeTotal;

		spriteCD.gameObject.SetActive (false);

		setIconColor(iconColor);

		tempPosition = transform.localPosition;
	}

	public void setIconColor(Color color)
	{
		if (spriteIcon == null) return;

		SparkleEffectItem.CloseSparkle (spriteIcon.gameObject);

		spriteIcon.color = color;
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

		setSpriteDoltOn(false);

		if(spriteCD.gameObject.activeSelf == false)
		{
			spriteCD.gameObject.SetActive (true);

			setIconColor(iconGray);

			timeLst = curTimeTotal;

			if(labelCDTime != null) labelCDTime.text = "" + (int)(timeLst + 1);
		}
	}

	public void CoolDownComplate()
	{
		updatePosition ();
		
		if(spriteCD.gameObject.activeSelf == false) return;	

		timeLst -= timeLst;

		if(timeLst <= 0)
		{
			setSpriteDoltOn(true);

			spriteCD.gameObject.SetActive(false);

			setIconColor(iconColor);

			if(tempGc == null)
			{
				tempGc = new GameObject();
				
				tempGc.transform.parent = transform;
				
				tempGc.transform.localPosition = Vector3.zero;
				
				tempGc.transform.localScale = new Vector3(1, 1, 1);
				
				UILabel label = tempGc.AddComponent<UILabel>();
				
				label.text = "";
			}

			if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING)
			{
				UI3DEffectTool.ShowTopLayerEffect( 
		             UI3DEffectTool.UIType.FunctionUI_1, 
		             tempGc, 
		             EffectIdTemplate.GetPathByeffectId(110000) );
			}
		}
		else
		{
			spriteCD.fillAmount = timeLst / curTimeTotal;

			if(labelCDTime != null) labelCDTime.text = "" + (int)(timeLst + 1);
		}
	}

	public void CoolDownUpdate ()
	{
		updatePosition ();

		if(spriteCD.gameObject.activeSelf == false) return;	

		if(timeTotal != 999) timeLst -= Time.deltaTime;

		if(timeLst <= 0)
		{
			setSpriteDoltOn(true);

			spriteCD.gameObject.SetActive(false);

			setIconColor(iconColor);

			if(tempGc == null)
			{
				tempGc = new GameObject();
				
				tempGc.transform.parent = transform;
				
				tempGc.transform.localPosition = Vector3.zero;
				
				tempGc.transform.localScale = new Vector3(1, 1, 1);
				
				UILabel label = tempGc.AddComponent<UILabel>();
				
				label.text = "";
			}

			if(BattleControlor.Instance().result == BattleControlor.BattleResult.RESULT_BATTLING)
			{
				UI3DEffectTool.ShowTopLayerEffect( 
		             UI3DEffectTool.UIType.FunctionUI_1, 
		             tempGc, 
		             EffectIdTemplate.GetPathByeffectId(110000) );
			}
		}
		else
		{
			spriteCD.fillAmount = timeLst / curTimeTotal;

			if(labelCDTime != null) labelCDTime.text = "" + (int)(timeLst + 1);
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

	private void setSpriteDoltOn(bool turnOn)
	{
		if (spriteWeaponDolt != null) spriteWeaponDolt.setCooldown (turnOn);
	}

}
