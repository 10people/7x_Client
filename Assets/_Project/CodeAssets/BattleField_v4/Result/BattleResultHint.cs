using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class BattleResultHint : MonoBehaviour 
{
	private List<AwardItem> awardItems = new List<AwardItem> ();

	private BattleResultControllor controllor;


	public void refreshdata(List<AwardItem> _awardItems, BattleResultControllor _controllor)
	{
		gameObject.SetActive (true);

		awardItems = _awardItems;

		controllor = _controllor;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
	}

	private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject itemTemple = (GameObject)p_object;

		float left = (awardItems.Count - 1) * 120f / 2f;

		for(int i = 0; i < awardItems.Count; i++)
		{
			AwardItem awardItem = awardItems[i];

			GameObject gc = (GameObject)Instantiate (itemTemple);

			gc.SetActive(true);
			
			gc.transform.parent = transform;

			gc.transform.localPosition = new Vector3(i * 120 - left, -40, 0);
			
			gc.transform.eulerAngles = itemTemple.transform.eulerAngles;
			
			gc.transform.localScale = itemTemple.transform.localScale;
			
			IconSampleManager.IconType iconType = IconSampleManager.IconType.item;
			
			if (awardItem.awardItemType == 2)//ZhuangBei
			{
				iconType = IconSampleManager.IconType.equipment;
			}
			else if (awardItem.awardItemType == 4)//MiBao
			{
				iconType = IconSampleManager.IconType.OldMiBao;
			}
			else if (awardItem.awardItemType == 5)//MibaoSuiPian
			{
				iconType = IconSampleManager.IconType.OldMiBaoSuiPian;
			}
			
			IconSampleManager ism = gc.GetComponent<IconSampleManager>();
			
			ism.SetIconByID(iconType, awardItem.awardIconId, awardItem.awardNum + "", 500);
			
			GameObject gcTip = ism.SetIconPopText (awardItem.awardId);
		}
	}

	public void close()
	{
		transform.parent.gameObject.SetActive (false);

		controllor.closeHint ();
	}

}
