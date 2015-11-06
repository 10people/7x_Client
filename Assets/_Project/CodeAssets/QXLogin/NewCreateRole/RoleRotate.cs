using UnityEngine;
using System.Collections;

public class RoleRotate : MonoBehaviour {

	public enum Direction
	{
		LEFT,
		RIGHT,
	}

	public GameObject rotateObj;

	private float speed = 10f;

	void Start ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.SHADOW_TEMPLE ),
		                        ShadowTempCallBack );
	}

	void ShadowTempCallBack ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject shadowTemp = Instantiate( p_object ) as GameObject;
		
		shadowTemp.SetActive (QualityTool.Instance.BattleField_ShowSimpleShadow());
		shadowTemp.transform.parent =this.transform.GetChild (0);
		
		shadowTemp.transform.localPosition = new Vector3 (0,0.03f,0);
		
		shadowTemp.transform.localScale = Vector3.one * 2;
		
		shadowTemp.transform.localRotation = Quaternion.Euler (90,0,0);
	}

	//拖动旋转
	public void DragRotate (Vector2 delta)
	{
		rotateObj.transform.localRotation = Quaternion.Euler(0f, -0.5f * delta.x, 0f) * rotateObj.transform.localRotation;
	}

	//点击旋转
	public void ClickRotate (Direction tempDirection)
	{
		float dir = 0;
		switch (tempDirection)
		{
		case Direction.LEFT:

			dir = 0.5f;

			break;
		case Direction.RIGHT:

			dir = -0.5f;

			break;
		default:
			break;
		}

		rotateObj.transform.localRotation = Quaternion.Euler(0f, dir *Vector2.one.x * speed, 0f) * rotateObj.transform.localRotation;
	}
}
