using UnityEngine;
using System.Collections;

public class ButtonColorManagerment : MonoBehaviour 
{
    public GameObject m_BackObj;
    private GameObject m_ObjShake;
    public UILabel m_LabelTitle;

 
    private enum ShakeType
    {
        SHAKE_NONE,
        SHAKE_RUNNING,
        SHAKE_STOP,
    };
    private ShakeType _ShakeTypeInfo;
    private float _timeInterval = 0;
    private float _ValueTime = 2.0f;
    void Update()
    {
       
        if (_ShakeTypeInfo == ShakeType.SHAKE_RUNNING && _timeInterval < _ValueTime)
        {
           _timeInterval += Time.deltaTime;
        }

        if ( _timeInterval >= _ValueTime)
        {
            switch (_ShakeTypeInfo)
            {
                case ShakeType.SHAKE_RUNNING:
                    {
                        _timeInterval = 0;
                        iTween.ShakeRotation(m_ObjShake, new Vector3(0, 0, 20f), 0.8f);
                    }
                    break;
               case ShakeType.SHAKE_STOP:
                    {

                    }
                    break;
            }
        }
    }
 
    public void ButtonsControl(bool Enable ,int type = 0)
    {
        
        if (Enable)
        {
            if (m_LabelTitle != null)
            {
                m_LabelTitle.GetComponent<UILabelType>().setType(10);
            }

            m_BackObj.GetComponent<UISprite>().color = new Color(1.0f, 1.0f, 1.0f);
        }
        else 
        {
            if (m_LabelTitle != null)
            {
                m_LabelTitle.GetComponent<UILabelType>().setType(11);
            }

            m_BackObj.GetComponent<UISprite>().color = new Color(128 / 255.0f, 128 / 255.0f, 128 / 255.0f);
        }
        if (type == 0)
        {
            transform.GetComponent<Collider>().enabled = Enable;
        }
    }

   
    public void ShakeEffectShow(bool isTurn = false,GameObject obj = null)
    {
        if (isTurn)
        {
            if (obj)
            {
                m_ObjShake = obj;
            }
            else
            {
                m_ObjShake = m_BackObj;
            }
            _ShakeTypeInfo = ShakeType.SHAKE_RUNNING;
        }
        else
        {
            _ShakeTypeInfo = ShakeType.SHAKE_STOP;
        }
    }
 
}
