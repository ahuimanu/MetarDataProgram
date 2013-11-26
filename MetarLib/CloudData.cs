using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib
{
    public class CloudData : IComparable<CloudData>
    {
        public static Dictionary<CloudCoverageCode, string> CloudCoverageAbbreviations;

        /*
         * STATIC CONSTRUCTOR - inits all statics at loadup
         */
        static CloudData()
        {
            CloudCoverageAbbreviations = new Dictionary<CloudCoverageCode, string>();
            CloudCoverageAbbreviations.Add(CloudCoverageCode.SKC, "Sky clear");
            CloudCoverageAbbreviations.Add(CloudCoverageCode.CLR, "Clear - No clouds below 12,000 feet");
            //Few 1-2 Oktas
            CloudCoverageAbbreviations.Add(CloudCoverageCode.FEW, "Few clouds at ");
            //Scattered 3-4 Oktas
            CloudCoverageAbbreviations.Add(CloudCoverageCode.SCT, "Scattered clouds at ");
            //Broken 5-7 Oktas
            CloudCoverageAbbreviations.Add(CloudCoverageCode.BKN, "Broken clouds at ");
            //Overcast 8 Oktas
            CloudCoverageAbbreviations.Add(CloudCoverageCode.OVC, "Overcast clouds at ");
            CloudCoverageAbbreviations.Add(CloudCoverageCode.VV, "Sky Obscured by fog or heavy precipitation, vertical visibility at ");
        }

        private CloudCoverageCode code;
        public CloudCoverageCode Code
        {
            get { return code; }
            set { code = value; }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        /*determines which Cloud Data object is "greater" than the other*/
        public int CompareTo(CloudData otherData)
        {
            if (this.Height > otherData.Height)
            {
                return 1;
            }
            else if (this.Height < otherData.Height)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public enum CloudCoverageCode
    {
        SKC,
        CLR,
        FEW,
        SCT,
        BKN,
        OVC,
        VV
    }
}
