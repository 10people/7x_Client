using UnityEngine;
using System.Collections;
using System.Linq;

namespace Carriage
{
    public class CarriageValueCalctor
    {
        public static int GetRealValueOfCarriage(int p_value, int p_carriageLevel, int p_carriageBattleValue, int p_carriageQuality, bool isRevenge)
        {
            float realValue = CartTemplate.Templates.Where(item => item.Quality == p_carriageQuality).First().RobProfit * p_value;

            //Calc level
            if (p_carriageLevel - JunZhuData.Instance().m_junzhuInfo.level > 5)
            {
                p_carriageLevel = JunZhuData.Instance().m_junzhuInfo.level + 5;

                realValue = (float)p_value / (JunzhuShengjiTemplate.GetJunZhuShengJi(JunZhuData.Instance().m_junzhuInfo.level).xishu) * (JunzhuShengjiTemplate.GetJunZhuShengJi(p_carriageLevel).xishu);
            }

            //Calc battle value.
            float battleValueK = (float)p_carriageBattleValue / JunZhuData.Instance().m_junzhuInfo.zhanLi;
            battleValueK = (battleValueK > 1 ? 1 : battleValueK) * 100;
            realValue = realValue * RobCartXishuTemplate.GetXishuByScale((int)battleValueK);

            //Calc revenge.
            if (isRevenge)
            {
                realValue = realValue * (1 + float.Parse(YunBiaoTemplate.GetValueByKey(YunBiaoTemplate.enemyCartBonus)) / 100);
            }

            return (int)realValue;
        }
    }
}