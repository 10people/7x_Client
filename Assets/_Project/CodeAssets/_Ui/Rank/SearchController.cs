using UnityEngine;
using System.Collections;

namespace Rank
{
    public class SearchController : MonoBehaviour
    {
        public int m_RestrictNum;

        public UIButton m_OK_Button;
        public UIInput m_Input;
        public UILabel m_InputLabel;

        void Update()
        {
            //Check input is empty.
            m_OK_Button.isEnabled = !string.IsNullOrEmpty(m_Input.value);
        }

        public void OnUIInputSubmit()
        {
            //Check input num out of restrict.
            if (m_Input.value.Length > m_RestrictNum)
            {
                m_Input.value = m_Input.value.Substring(0, m_RestrictNum);
                m_InputLabel.text = m_Input.value;
            }
        }
    }
}
