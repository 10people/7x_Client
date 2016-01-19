using UnityEngine;
using System.Collections;

public class ReplayUIControllor : MonoBehaviour
{
	public ReplayTroopAvatar troopAvatarTemple;

	public ReplayCameraFocus cameraFocus;


	private static ReplayUIControllor _instance;


	void Awake() { _instance = this; }
	
	public static ReplayUIControllor Instance() { return _instance; }

	void OnDestroy(){
		_instance = null;
	}

	public void addAvatar()
	{
		troopAvatarTemple.gameObject.SetActive(false);
		
		for(int i = 0; i < BattleReplayControlor.Instance().enemyTroops.Count; i++)
		{
			TroopReplay troop = BattleReplayControlor.Instance().enemyTroops[i];
			
			GameObject avatarObject = (GameObject)Instantiate(troopAvatarTemple.gameObject);
			
			avatarObject.SetActive(true);
			
			avatarObject.transform.parent = gameObject.transform;
			
			avatarObject.transform.localScale = new Vector3(1, 1, 1);
			
			ReplayTroopAvatar avatar = (ReplayTroopAvatar)avatarObject.GetComponent("ReplayTroopAvatar");
			
			avatar.init(troop);
			
			Component[] coms = avatarObject.GetComponentsInChildren(typeof(UIWidget));
			
			foreach(Component com in coms)
			{
				UIWidget widget = (UIWidget)com;
				
				widget.depth += i * 10;
			}
		}
	}

	public void moveCamera(Vector3 offset)
	{
		cameraFocus.move(-offset);
	}

	public void changeSpeed()
	{

	}

}
