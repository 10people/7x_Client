using UnityEngine;
using System.Collections;

public class DebugCameraAdjust : MonoBehaviour 
{
	public GameObject layerRoot;

	public string m_func_name;

	public UISlider sliderX;

	public UISlider sliderY;

	public UISlider sliderLength;

	public UISlider sliderOffset;
	
	public UILabel labelX;

	public UILabel labelY;

	public UILabel labelLength;

	public UILabel labelOffset;

	public float maxLength = 30;

	public float maxOffset = 30;

	public UISprite spriteShow;

	public UISprite spriteSave;


	[HideInInspector] public GameObject m_target;
	

	private static DebugCameraAdjust m_instance;

	private bool inShow;


	#region Mono

	void Awake()
	{
		m_instance = this;

		if( ConfigTool.GetBool( ConfigTool.CONST_SHOW_BATTLE_CAMERA_OPS) == false ){
			gameObject.SetActive(false);

			return;
		}

		gameObject.SetActive (true);

		inShow = false;

		spriteShow.transform.localEulerAngles += new Vector3 (0, 0, -spriteShow.transform.localEulerAngles.z + 180);

		layerRoot.SetActive (inShow);
	}

	public static DebugCameraAdjust Instance(){
		return m_instance;
	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion


	#region Interaction

	public void ValueChanged()
	{
		Vector4 t_param = new Vector4( GetX(), GetY(), GetLength(), GetOffset() );

		UpdateLabels();

		if (m_target == null) m_target = BattleControlor.Instance().getKing ().gameCamera.gameObject;

		m_target.SendMessage( m_func_name, t_param, SendMessageOptions.RequireReceiver );
	}

	#endregion


	#region Utilities

	private void UpdateLabels()
	{
		labelX.text = GetX ().ToString ("0.0");

		labelY.text = GetY ().ToString ("0.0");

		labelLength.text = GetLength ().ToString ("0");

		labelOffset.text = GetOffset ().ToString ("0");
	}

	private float GetX(){
		return sliderX.value * 180.0f;
	}

	private float GetY(){
		return sliderY.value * 360.0f;
	}

	private float GetLength(){
		return sliderLength.value * maxLength;
	}

	private float GetOffset()
	{
		return sliderOffset.value * maxOffset;
	}

	public void SetValue( float _x, float _y, float _length, float _offset )
	{
		SetX( _x );

		SetY( _y );

		SetLength ( _length );

		SetOffset ( _offset );

		UpdateLabels();
	}

	public void setInPut()
	{
		sliderX.value = float.Parse(labelX.text) / 180.0f;

		sliderY.value = float.Parse (labelY.text) / 360.0f;

		sliderLength.value = float.Parse (labelLength.text) / maxLength;

		sliderOffset.value = float.Parse (labelOffset.text) / maxOffset;
	}

	public void SetX( float _x )
	{
		_x = _x < 0 ? 0 : _x;

		_x = _x > 180 ? 180 : _x;

		sliderX.value = _x / 180.0f;
	}

	public void SetY( float _y )
	{
		_y = _y < 0 ? 0 : _y;
		
		_y = _y > 360 ? 360 : _y;

		sliderY.value = _y / 360.0f;
	}

	public void SetLength( float _length )
	{
		_length = _length < 0 ? 0 : _length;

		_length = _length > maxLength ? maxLength : _length;

		sliderLength.value = _length / maxLength;
	}

	public void SetOffset( float _offset )
	{
		_offset = _offset < 0 ? 0 : _offset;
		
		_offset = _offset > maxLength ? maxLength : _offset;
		
		sliderOffset.value = _offset / maxOffset;
	}

	public Vector4 get4Param()
	{
		return new Vector4 (GetX(), GetY(), GetLength(), GetOffset());
	}

	#endregion


	#region Layer

	public void ShowHide()
	{
		inShow = !inShow;

		layerRoot.SetActive (inShow);

		if(inShow == true)
		{
			spriteShow.transform.localEulerAngles += new Vector3 (0, 0, -spriteShow.transform.localEulerAngles.z);
		}
		else
		{
			spriteShow.transform.localEulerAngles += new Vector3 (0, 0, -spriteShow.transform.localEulerAngles.z + 180);
		}

		Camera.main.SendMessage( "setDebugCameraValue", SendMessageOptions.RequireReceiver );
	}

	public void saveCamaera()
	{
		Camera.main.SendMessage( "writeXml", SendMessageOptions.RequireReceiver );
	}

	public void saveCameraSucc()
	{
		spriteSave.spriteName = "battle_avatar_yellow";

		StartCoroutine (succAction());
	}

	private IEnumerator succAction()
	{
		yield return new WaitForSeconds(0.5f);

		spriteSave.spriteName = "battle_avatar_red";
	}

	#endregion
}
