using UnityEngine;
using System.Collections;

public class SelectNationBtn : MonoBehaviour {

	public UISprite tuiJian;

	public UISprite nationBg;

	public UILabel nationName;

	public GameObject selectBox;

	public NewSelectRole selectRole;

	private int nationId;

	private bool active = false;

	public void GetSelelctNationInfo (int tempNationId,int tuiJianId)
	{
		nationId = tempNationId;

		tuiJian.gameObject.SetActive (nationId == tuiJianId ? true : false);

		nationBg.spriteName = "nation_" + nationId.ToString ();

		nationName.text = NameIdTemplate.GetName_By_NameId (nationId);

		selectBox.SetActive (nationId == tuiJianId ? true : false);

		active = nationId == tuiJianId ? true : false;
	}

	public void OnClick ()
	{
		if (!active)
		{
			selectRole.SelectNation (nationId);
		}
	}

	public void ActiveSelectBox (bool isActive)
	{
		active = isActive;
		selectBox.SetActive (isActive);
	}
}
