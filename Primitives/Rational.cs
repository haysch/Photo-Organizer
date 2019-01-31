using System;

namespace PhotoOrganizer.Primitives {
    /// <summary> TODO </summary>
    public  class Rational {
        public long Numerator { get; }
        public long Denominator { get; }

        public Rational(long num) {
            Numerator = num;
            Denominator = 1;
        }

        public Rational(long num, long den) {
            Numerator = num;
            Denominator = den;
        }

        public Rational(uint num, uint den) {
            Numerator = Convert.ToInt64(num);
            Denominator = Convert.ToInt64(den);
        }

        /// <summary> Returns the Rational as a fraction of form 15/25 </summary>
        public override string ToString() {
            if (this.Denominator == 1) return $"{Numerator}";
            return $"{Numerator}/{Denominator}";
        }

        /// <summary> Returns the Rational on the f-number form. </summary>
        public string ToFNumber() {
            if ((Numerator / Denominator) % 1 == 0)
                return $"f/{(Numerator / Denominator)}";
            return $"f/{(Numerator / Denominator):N1}";
        }

        /// <summary> Implicit casts Rational to double. </summary>
        public static implicit operator double(Rational r) => r.Numerator == 0 ? 0.0 : (double)r.Numerator / r.Denominator;
    }
}