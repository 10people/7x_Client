using UnityEngine;
using System.Collections;

namespace Carriage
{
	public class EnterYunBiao : MonoBehaviour {

		void OnClick ()
		{
			BiaoJuData.Instance.OpenBiaoJu ();
		}
	}
}
