using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMibaoShowControllor : MonoBehaviour
{
	public GameObject spriteIcon;

	public GameObject spritePowerup;

	public GameObject layerMibaoNum;

	public GameObject layerPowerup;

	public UISprite spriteNumTen; 

	public UISprite spriteNumOne;

	public UILabel labelPowerup;



	private int mibaoNum = 21;

	private int powerUp = 12345;

	private bool inShowNum;


	void Start ()
	{
		mibaoNum = CityGlobalData.t_resp.selfTroop.nodes [0].mibaoCount;

		powerUp = CityGlobalData.t_resp.selfTroop.nodes [0].mibaoPower;

		spriteNumTen.spriteName = "battle_" + (mibaoNum / 10);

		spriteNumOne.spriteName = "battle_" + (mibaoNum % 10);

		inShowNum = true;
	}
	
	void Update () 
	{
		if (inShowNum) StartCoroutine(hideMiBaoNum());
	}

	IEnumerator hideMiBaoNum()
	{
		inShowNum = false;

		UI3DEffectTool.ShowBottomLayerEffect ( UI3DEffectTool.UIType.MainUI_0, spriteIcon, EffectIdTemplate.getEffectTemplateByEffectId(620219).path);

		yield return new WaitForSeconds(2.75f);

		layerMibaoNum.SetActive (false);

		layerPowerup.SetActive (true);

		labelPowerup.text = LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SHOW_MIBAO) + powerUp;

		UI3DEffectTool.ShowBottomLayerEffect ( UI3DEffectTool.UIType.MainUI_0, spritePowerup, EffectIdTemplate.getEffectTemplateByEffectId(100191).path);

		BattleEffectControllor.Instance ().PlayEffect (600241, BattleControlor.Instance().getKing().gameObject);

		yield return new WaitForSeconds(1f);

		layerPowerup.SetActive (false);

		yield return new WaitForSeconds(1f);

		gameObject.SetActive (false);
	}

}
