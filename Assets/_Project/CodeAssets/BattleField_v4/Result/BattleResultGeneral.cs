using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleResultGeneral : MonoBehaviour
{
	public BattleResultGeneralIcon iconTemple;


	public void refreshData(List<Enums.Currency> currencies, List<int> nums)
	{
		for(int i = 0; i < currencies.Count; i++)
		{
			Enums.Currency currency = currencies[i];

			int num = nums[i];

			GameObject iconObject = (GameObject)Instantiate(iconTemple.gameObject);

			iconObject.SetActive(true);

			iconObject.transform.parent = transform;

			iconObject.transform.localPosition = iconTemple.transform.localPosition + new Vector3(0, - 48 * i , 0);

			iconObject.transform.localEulerAngles = Vector3.zero;

			iconObject.transform.localScale = new Vector3(1, 1, 1);

			BattleResultGeneralIcon icon = iconObject.GetComponent<BattleResultGeneralIcon>();

			icon.refreshData(currency, num);
		}
	}

}
