using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DramaStoryControllor : MonoBehaviour
{
	[HideInInspector] public Dictionary<int, DramaStoryBoard> storyBoardList = new Dictionary<int, DramaStoryBoard>();


	private DramaStoryBoard curStroyBoard;
	
	private int index;

	private GameObject target;

	void OnDestroy(){
		storyBoardList.Clear();

		curStroyBoard = null;

		target = null;
	}

	public void init ()
	{
		//if (storyBoardList.Count != 0) return;

		//str = 0;

//		storyBoardList.Clear ();
		
		Component[] coms = GetComponentsInChildren (typeof(DramaStoryBoard));

		foreach(Component com in coms)
		{
			DramaStoryBoard dsb = (DramaStoryBoard)com;

//			if(storyBoardList.ContainsKey(dsb.storyBoardId))
//			{
//				Destroy(dsb.gameObject);
//
//				continue;
//			}

			dsb.init(gameObject);

//			storyBoardList.Add(dsb.storyBoardId, dsb);
		}

		foreach(DramaStoryBoard dsb in storyBoardList.Values)
		{
			dsb.gameObject.SetActive(false);
		}
	}

	public void recreateModel(int storyBoardId)
	{
		DramaStoryBoard _board = null;

		storyBoardList.TryGetValue (storyBoardId, out _board);

		if (_board != null) _board.createActors (true);
	}

	public void lightOn()
	{
		foreach(DramaStoryBoard dsb in storyBoardList.Values)
		{
			dsb.gameObject.SetActive(true);
		}
	}

	public void playNext(GameObject _target)
	{
		bool flag = playNext(_target, index);

		if(flag == true) index++;
	}

	public bool playNext(GameObject _target, int _index)
	{
		target = _target;

		DramaStoryBoard board = null;
		
		//storyBoardList.TryGetValue(_index, out board);

		int i = 0;

		foreach( DramaStoryBoard _board in storyBoardList.Values)
		{
			if(i == _index)
			{
				board = _board;

				break;
			}

			i++;
		}

		if (board == null) return false;

		//if(board.getCurActionDone() == true)
		{
			if(curStroyBoard != null) curStroyBoard.gameObject.SetActive(false);

			board.gameObject.SetActive (true);
			
			board.action ();

			curStroyBoard = board;

			return true;
		}
	}

	public void playAction(GameObject _target, int _index)
	{
		target = _target;

		DramaStoryBoard board = null;

		storyBoardList.TryGetValue(_index, out board);
		
		if (board == null)
		{
			Debug.LogError("CREATE DramaStoryBoard ERROR: " + _index + " is null");

			return;
		}

		//if(board.getCurActionDone() == true)
		{
			if(curStroyBoard != null) curStroyBoard.gameObject.SetActive(false);
			
			board.gameObject.SetActive (true);
			
			board.action ();
			
			curStroyBoard = board;
			
			index = _index + 1;
		}
	}

	public void storyBoardDone()
	{
		if (target != null) target.SendMessage ("dramaStoryDone");
	}

	public Dictionary<int, DramaStoryBoard> getStoryBoardList()
	{
		return storyBoardList;
	}

}
