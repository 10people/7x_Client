using UnityEngine;
using System.Collections;

public class AllianceSelectIcon : MonoBehaviour 
{
    public UISprite m_SpriteIcon;
    public UISprite m_SpriteGou;
 
    public delegate void OnClick_Touch(GameObject obj ,int id);
    OnClick_Touch CallBackTouch;
    private int spriteIcon = 0;
	void Start () 
    {
	
	}
    void OnClick()
    {
       CallBackTouch(gameObject,spriteIcon);
    }
   

    public  void ShowIcon(int icon,OnClick_Touch callback)
    {
        spriteIcon = icon;
        m_SpriteIcon.spriteName = icon.ToString();
        CallBackTouch = callback;
    }
    
}
