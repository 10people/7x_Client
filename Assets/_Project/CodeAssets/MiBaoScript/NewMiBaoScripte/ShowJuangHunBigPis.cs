using UnityEngine;
using System.Collections;

public class ShowJuangHunBigPis : MonoBehaviour {

	private float moveTime = 0.7f;

	private Vector3 targetPos;

	public UITexture JiangHunIcon;

	public UILabel mLabel;

	private string desStr = "点击任意位置退出查看";
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Init(int mibaoid)
	{
		MiBaoXmlTemp mmibaoxml = MiBaoXmlTemp.getMiBaoXmlTempById (mibaoid);
		JiangHunIcon.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON) + mmibaoxml.icon.ToString ());

		mLabel.text = "";

		CardMove ();
	}
	void CardMove ()
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("scale",Vector3.one);
		scale.Add ("time",moveTime);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		iTween.ScaleTo (gameObject,scale);
		
		Hashtable move = new Hashtable ();
		move.Add ("position",Vector3.zero);
		move.Add ("time",moveTime);
		move.Add ("islocal",true);
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("oncomplete","CardMoveEnd");
		move.Add ("oncompletetarget",gameObject);
		iTween.MoveTo (gameObject,move);
		StartCoroutine ("ShowLabel");
	}
	IEnumerator ShowLabel()
	{
		yield return new WaitForSeconds (1f);
		mLabel.text = desStr;
	}
	public void Close()
	{
		Destroy (this.gameObject);
	}
}
