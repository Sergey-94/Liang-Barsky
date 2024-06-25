using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR1_AKG
{
    public class PointDoub
    {
        public float xEnd;
        public float yEnd;
        public float xStart;
        public float yStart;

        public PointDoub()
        {

        }

        public PointDoub(float xE, float yE, float xS, float yS)
        {
            xEnd = xE;
            yEnd = yE;
            xStart = xS;
            yStart = yS;
        }
    }
}
