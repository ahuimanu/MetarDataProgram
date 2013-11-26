using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib
{
    public class WindData
    {

        //conversion taken from here: http://www.militaryfactory.com/conversioncalculators/speed_knots_to_miles_per_hour.asp
        public static int KnotsInMPH(int knots)
        {
            return (int)(Math.Round(knots * 1.15077945, 2));
        }

        private int direction;
        public int Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private int speed;
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        private int gust;
        public int Gust
        {
            get { return gust; }
            set { gust = value; }
        }

        private string cardinalDirection;
        public string CardinalDirection
        {
            get { return cardinalDirection; }
            set { cardinalDirection = value; }
        }

        /**
         * SET CARDINAL DIRECTION
         */
        public void SetCardinalDirection()
        {
            if (direction >= 315 || direction < 45)
            {
                CardinalDirection = "North";
            }
            else if (Direction >= 225 && Direction < 315)
            {
                CardinalDirection = "West";
            }
            else if (Direction >= 135 && Direction < 225)
            {
                CardinalDirection = "South";
            }
            else
            {
                CardinalDirection = "East";
            }
        }
    }
}
