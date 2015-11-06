using UnityEngine;
using System.Collections;

public class BaiZhanRules : MonoBehaviour {

	public UILabel rulesLabel;

	void Start ()
	{
		ShowRules ();
	}

	void ShowRules ()
	{
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_1);
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_2);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_3);
		string str3 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_4);
		string str4 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_5);
		string str5 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_6);
		string str6 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_7);
		string str7 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_8);
		string str8 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_9);

		rulesLabel.text = titleStr + "\n\n" + str1 + "\n\n" + str2 + "\n\n" + str3 + "\n\n" + str4 + "\n\n" + str5 + "\n\n" 
			+ str6 + "\n\n" + str7 + "\n\n" + str8;
	}

	public void BackBtn ()
	{
		BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
		Destroy (this.gameObject);
	}

	public void DestroyRoot ()
	{
		BaiZhanMainPage.baiZhanMianPage.DestroyBaiZhanRoot ();
	}
}
