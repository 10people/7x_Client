using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  旋风斩
*/
public class KingSkillXuanFengZhan : MonoBehaviour
{
	private KingControllor king;

	private int count;

	private int curCount;


	void Start()
	{
		king = gameObject.GetComponent<KingControllor> ();
		
		SkillTemplate skill = SkillTemplate.getSkillTemplateBySkillLevelIndex (CityGlobalData.skillLevelId.bahuanglieri, king);

		count = (int)skill.value2;

		curCount = 0;
	}

	void OnDestroy(){
		king = null;
	}

	public void resetXuanFengCount()
	{
		curCount = 0;

		ResetXuanFengCountCallback( king.mAnim );
	}

	public static void ResetXuanFengCountCallback( Animator p_animator ){
		p_animator.SetBool("XuanFengOver", false);
	}

	public void addXuanFengCount()
	{
		curCount++;

		if(curCount >= count)
		{
			AddXuanFengCountCallback( king.mAnim );
		}
	}

	public static void AddXuanFengCountCallback( Animator p_animator ){
		p_animator.SetBool("XuanFengOver", true);
	}

}
