using System;

namespace Clinician.Units
{
    public class BloodPressureUnitProvider : IFormatProvider
    {
        private String[] fmtStrings = {"mmHg", "Pa"};
        private Random rnd = new Random();

        public Object GetFormat(Type formatType)
        {
            return this;
        }

        public String Format
        {
            get { return this.fmtStrings[this.rnd.Next(0, this.fmtStrings.Length)]; }
        }
    }
}