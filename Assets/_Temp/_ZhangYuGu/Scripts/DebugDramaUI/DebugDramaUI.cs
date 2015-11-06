using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugDramaUI : MonoBehaviour
{
	public DramaStoryControllor controllor;


	public void playNext()
	{
		controllor.init ();

		controllor.playNext (null);
	}

}
