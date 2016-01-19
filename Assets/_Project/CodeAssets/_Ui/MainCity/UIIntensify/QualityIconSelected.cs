using UnityEngine;
using System.Collections;

public class QualityIconSelected : MonoBehaviour
{

    public static string SelectQuality(int quality)
    {
        switch (quality)
        {

            case 1:

                return "pinzhi0";
            case 2:

                return "pinzhi1";

            case 3:

                return "pinzhi2";

            case 4:

                return "pinzhi3";

            case 5:

                return "pinzhi4";
            case 6:

                return "pinzhi5";

            case 7:

                return "pinzhi6";

            case 8:

                return "pinzhi7";

            case 9:

                return "pinzhi8";


            case 10:

                return "pinzhi9";

            case 11:

                return "pinzhi10";

            //case 12:

            //    return "pinzhi1";

            //case 13:

            //    return "Green1";

            //case 14:

            //    return "pinzhi2";

            //case 15:

            //    return "Blue1";

            //case 16:

            //    return "Blue2";

            //case 17:

            //    return "pinzhi3";
            //case 18:

            //    return "Purple1";

            //case 19:

            //    return "Purple2";

            //case 20:

            //    return "pinzhi4";

            //case 21:

            //    return "Orange1";

            default:
                return "";
        }
        /*
  	11：白色
	12：绿色
	13：绿色+1
	14：蓝色
	15：蓝色+1
	16：蓝色+2
	17：紫色
	18：紫色+1
	19：紫色+2
	20：橙色
	21：橙色+1
*/
    }
	public static int SelectQualityNum(int index)
	{
		if(index == 1)
		{
			return 0;
		}
		else if (index == 2)
		{
			return 1;
		}
		else if (index > 2 && index <= 5)
		{
			return 2;
		}
		else if (index > 5 && index <= 8)
		{
			return 3;
		}
		else  
		{
			return 4;
		}
	}
}
