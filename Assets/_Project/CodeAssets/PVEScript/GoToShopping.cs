using UnityEngine;
using System.Collections;

public class GoToShopping : MonoBehaviour {

	public GameObject  Supperg;
	void Start () {
	
	}
	
	void OnClick()
	{

		GoShopping ();
		Destroy (Supperg);
	}
	void GoShopping()
	{
        //Debug.Log ("前往商店购买体力");//待处理
        EquipSuoData.TopUpLayerTip();
    }
}
