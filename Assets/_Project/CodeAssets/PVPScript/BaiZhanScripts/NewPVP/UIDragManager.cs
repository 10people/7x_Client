using UnityEngine;
using System.Collections;

public class UIDragManager : MonoBehaviour {

	public Transform target;

	public Vector3 scale = Vector3.one;

	private Plane mPlane;
	private bool mPressed = false;
	private Vector3 mLastPos = Vector3.zero;

	void OnPress (bool pressed)
	{
		mPressed = pressed;

		if (pressed)
		{
			mLastPos = UICamera.lastHit.point;

			Transform trans = UICamera.currentCamera.transform;

			mPlane = new Plane (trans.rotation * Vector3.back,mLastPos);
		}
	}

	void OnDrag (Vector2 delta)
	{
		Ray ray = UICamera.currentCamera.ScreenPointToRay (UICamera.currentTouch.pos);
		float dist = 0f;

		if (mPlane.Raycast (ray,out dist))
		{
			Vector3 currentPos = ray.GetPoint (dist);
			Vector3 offset = currentPos - mLastPos;
			mLastPos = currentPos;

			if (offset.x != 0f || offset.y != 0f)
			{
				offset = target.InverseTransformDirection (offset);
				offset.Scale (scale);
				offset = target.TransformDirection (offset);
			}

			Vector3 pos = target.position;
			pos += offset;

			Camera curcam = UICamera.currentCamera;

			Bounds bs = NGUIMath.CalculateAbsoluteWidgetBounds (target);

			Vector3 _lb = new Vector3(target.position.x - bs.size.x / 2,target.position.y - bs.size.y / 2,0f);
			Vector3 lb = curcam.WorldToScreenPoint (_lb);

			Vector3 _rt = new Vector3(target.position.x + bs.size.x / 2,target.position.y + bs.size.y / 2,0f);
			Vector3 rt = curcam.WorldToScreenPoint (_rt);

			float width = rt.x - lb.x;
			float height = rt.y - lb.y;

			Vector3 ClampVector1 = new Vector3(width / 2,height / 2,0f);
			Vector3 ClampVector2 = new Vector3(Screen.width - width / 2,Screen.height - height / 2,0f);

			Vector3 scrPos = curcam.WorldToScreenPoint (pos);

			scrPos.x = Mathf.Clamp (scrPos.x,ClampVector1.x,ClampVector2.x);
			scrPos.y = Mathf.Clamp (scrPos.y,ClampVector1.y,ClampVector2.y);

			target.position = curcam.ScreenToWorldPoint (scrPos);
		}
	}
}
