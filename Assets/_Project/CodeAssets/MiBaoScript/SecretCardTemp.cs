using UnityEngine;
using System.Collections;

public class SecretCardTemp : MonoBehaviour {


	public GameObject mPanel;
	public int[] dbId;//数据库中的唯一id
	public int[]  miBaoId;//配置文件mibao中id字段，品质可以根据它来查到
	public int[]  star;
	public int[]  level ;
	public int[]  suiPianNum;//拥有的碎片数量
	public int[]  needSuipianNum;//需要的碎片数量
	public int[]  gongJi;
	public int[]  fangYu;
	public int[]  shengMing;
	public int[]  wqSH;   //	武器伤害加深
	public int[]  wqJM ;  //	武器伤害减免
	public int[]  wqBJ;   //	武器暴击加深
	public int[]  wqRX;   //	武器暴击减免
	public int[]  jnSH ;  //	技能伤害加深
	public int[]  jnJM ;  //	技能伤害减免
	public int[]  jnBJ ;  //	技能暴击加深
	public int[]  jnRX ;  //	技能暴击减免


	void Start () {
	
	}
	

	void Update () {
	
	}

	public void hitSkill()
	{
		int posx = (int)(this.gameObject.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = -240;
	
		Vector3 pos = new Vector3 (posx,posy,0);
		GameObject theRoot = GameObject.Find ("SkillRoot");

		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			mSkillControl.Starposition = pos;
			mSkillControl.mID = 1;
			mSkillControl.Init();
		}
	}
	public void hitSecret1()
	{
		int posx = (int)(this.gameObject.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = 120;
		
		Vector3 pos = new Vector3 (posx,posy,0);
		GameObject theRoot = GameObject.Find ("SkillRoot");
		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			//mSkillControl.Starposition = pos;
			mSkillControl.mID = 1;
			mSkillControl.createMiBao(pos,1);
		}
	}
	public void hitSecret2()
	{
		int posx = (int)(this.gameObject.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = 0;
		
		Vector3 pos = new Vector3 (posx,posy,0);
		GameObject theRoot = GameObject.Find ("SkillRoot");
		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			//mSkillControl.Starposition = pos;
			mSkillControl.mID = 1;
			mSkillControl.createMiBao(pos,1);
		}
	}
	public void hitSecret3()
	{
		int posx = (int)(this.gameObject.transform.localPosition.x + mPanel.transform.localPosition.x);
		int posy = -120;
		
		Vector3 pos = new Vector3 (posx,posy,0);
		GameObject theRoot = GameObject.Find ("SkillRoot");
		
		if(theRoot)
		{
			SkillControl mSkillControl = theRoot.GetComponent<SkillControl>();
			//mSkillControl.Starposition = pos;
			mSkillControl.mID = 1;
			mSkillControl.createMiBao(pos,1);
		}
	}
}
