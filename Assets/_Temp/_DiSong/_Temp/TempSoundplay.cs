using UnityEngine;
using System.Collections;

public class TempSoundplay : MonoBehaviour 
{
	public AudioSource m_AudioSource;
	public Animator m_Animator;
	private int m_iCurIndex = 0;
	// Use this for initialization
	void Start () 
	{
		m_AudioSource = gameObject.AddComponent<AudioSource>();
		m_Animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 100, 50), "ʏ"))
		{
			m_iCurIndex --;
			if(m_iCurIndex < 0)
			{
				m_iCurIndex = 0;
			}
			m_Animator.SetInteger("Value", m_iCurIndex);
		}
		else if (GUI.Button(new Rect(230, 10, 100, 50), "Ђ"))
		{
			m_iCurIndex ++;
			m_Animator.SetInteger("Value", m_iCurIndex);
		}
	}  
	
	public void PlaySound(string path)
	{
//		int indexRan;

		string tempSId;
	
		if(path.IndexOf("|") != -1)
		{
			string[] tempPath = path.Split('|');
			tempSId = tempPath[Global.getRandom(tempPath.Length)];
		}
		else
		{
			tempSId = path;
		}
		AudioClip tempClip;
		try
		{
			int tempId = int.Parse(tempSId);

			ClientMain.m_sound_manager.getIdClip( tempId, ResourceLoadCallback );
		}
		catch
		{
//			tempClip = ClientMain.M_SOUNDMANAGER.getPathClip(tempSId);

			Debug.LogError( "Never Should Be Here." );

			return;
		}
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path, Object p_object ){
		ClientMain.m_sound_manager.getIdClipResLoad( ref p_www, p_path, p_object );

		m_AudioSource.clip = (AudioClip)p_object;
	
		m_AudioSource.Play();
	}

	public void ResetHitCount(){}
	public void attackDone(int actionId){}
	public void playAttackEffect(int attackId){}
	public void setWeaponTriggerTrue(int _aid){}
	public void setWeaponTriggerFalse(int hand){}
	public void createShadows(){}
	public void refreshShadows(int actionId){}
	public void attackDone(){}
	public void dieActionDone(){}
	public void attackedActionStart(){}
	public void attackedActionEnd(){}
	public void clearTrails(){}

}