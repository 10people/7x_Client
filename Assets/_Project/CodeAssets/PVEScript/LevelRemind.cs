using UnityEngine;
using System.Collections;

public class LevelRemind : MonoBehaviour {

	public UILabel JunZhuLv;
	public int Junzhulevel;
	public void init()
	{

		JunZhuLv.text = Junzhulevel.ToString();
	}
	public void destroyUI()
	{
		Destroy (this.gameObject);
	}
}
