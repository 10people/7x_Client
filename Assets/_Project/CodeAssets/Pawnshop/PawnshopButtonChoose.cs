using UnityEngine;
using System.Collections;

public class PawnshopButtonChoose : MonoBehaviour
{
	public GameObject spriteNormal;

	public GameObject spriteFocus;


	public void pawnshopFocus()
	{
		spriteNormal.SetActive(false);

		spriteFocus.SetActive(true);
	}

	public void pawnshopNormal()
	{
		spriteNormal.SetActive(true);
		
		spriteFocus.SetActive(false);
	}

}
