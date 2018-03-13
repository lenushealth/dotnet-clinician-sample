using System;

namespace Clinician.Units
{
    public struct BloodPressureUnit : IFormattable
    {
        private readonly decimal valueInPascalUnits;

        private static BloodPressureUnitCategory CategoriseSystolicValue(BloodPressureUnit unit)
        {
            var systolic = unit.MmHg;
            if (systolic < 90)
            {
                return BloodPressureUnitCategory.Low;
            }

            if (systolic > 140)
            {
                return BloodPressureUnitCategory.High;
            }

            if (systolic > 120)
            {
                return BloodPressureUnitCategory.PreHigh;
            }

            return BloodPressureUnitCategory.Normal;
        }

        private static BloodPressureUnitCategory CategoriseDiastolicValue(BloodPressureUnit unit)
        {
            var diastolic = unit.MmHg;
            if (diastolic < 60)
            {
                return BloodPressureUnitCategory.Low;
            }

            if (diastolic > 90)
            {
                return BloodPressureUnitCategory.High;
            }

            if (diastolic > 80)
            {
                return BloodPressureUnitCategory.PreHigh;
            }

            return BloodPressureUnitCategory.Normal;
        }

        public static BloodPressureUnitCategory Categorise(BloodPressureUnit unit, BloodPressureUnits asUnit)
        {
            switch (asUnit)
            {
                case BloodPressureUnits.Diastolic:
                    return CategoriseDiastolicValue(unit);
                case BloodPressureUnits.Systolic:
                    return CategoriseSystolicValue(unit);
            }
            throw new NotSupportedException("Unsupported BloodPressureUnits specified");
        }

        public static (BloodPressureUnitCategory SystolicCategory, BloodPressureUnitCategory DiastolicCategory) CategoriseBloodPressureReading(BloodPressureUnit systolic, BloodPressureUnit diastolic)
        {
            return (CategoriseSystolicValue(systolic), CategoriseDiastolicValue(diastolic));
        }

        private static int ConvertToMmHg(decimal value)
        {
            return (int)Math.Round((value * 0.00750061683m), 0);
        }

        private static decimal ConvertToPascals(decimal value)
        {
            return value / 0.00750061683m;
        }

        public static implicit operator decimal(BloodPressureUnit value)
        {
            return value.valueInPascalUnits;
        }

        public static implicit operator BloodPressureUnit(decimal value)
        {
            return new BloodPressureUnit(value);
        }

        public static implicit operator BloodPressureUnit(int value)
        {
            return new BloodPressureUnit(value);
        }

        private BloodPressureUnit(decimal pascalUnits)
        {
            this.valueInPascalUnits = pascalUnits;
        }

        private BloodPressureUnit(int mmHgUnits)
        {
            this.valueInPascalUnits = ConvertToPascals(mmHgUnits);
        }

        public override string ToString()
        {
            return $"{this.valueInPascalUnits} Pa";
        }

        public string ToString(string format) => this.ToString(format, new BloodPressureUnitProvider());

        public int MmHg => ConvertToMmHg(this.valueInPascalUnits);
        public decimal Pascals => this.valueInPascalUnits;
        public BloodPressureUnitCategory Category(BloodPressureUnits asUnit) => Categorise(this, asUnit);

        public String ToString(string format, IFormatProvider provider) 
        {
            BloodPressureUnitProvider formatter = null;
            if (provider != null) 
                formatter = provider.GetFormat(typeof(BloodPressureUnitProvider)) 
                    as BloodPressureUnitProvider;

            if (String.IsNullOrWhiteSpace(format))
            {
                if (formatter != null)
                {
                    format = formatter.Format;
                }
                else
                {
                    format = "Pa";
                }
            }

            switch (format.ToUpper()) {
                case "PA":
                    return $"{this.valueInPascalUnits}";
                case "MMHG":
                    return $"{ConvertToMmHg(this.valueInPascalUnits)}";
                default:
                    throw new FormatException(String.Format("'{0}' is not a valid format specifier for blood pressure.", format));
            }
        }                             
    }
}