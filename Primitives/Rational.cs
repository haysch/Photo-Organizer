using System;

namespace PhotoOrganizer.Primitives {
    /// <summary> TODO </summary>
    public  class Rational {
        long Numerator { get; set; }
        long Denominator { get; set; }

        public Rational(long num) {
            Numerator = num;
        }

        public Rational(long num, long den) {
            Numerator = num;
            Denominator = den;
        }

        /// <summary> Returns the Rational as a fraction of form 15/25 </summary>
        public override string ToString() {
            if (this.Denominator == 1) return $"{Numerator}";
            return $"{Numerator}/{Denominator}";
        }

        /// <summary> Returns the Rational on the f-number form. </summary>
        public string ToFNumber() => $"f/{(Numerator / Denominator)}";

        /// <summary> Implicit casts Rational to double. </summary>
        public static implicit operator double(Rational r) => r.Numerator == 0 ? 0.0 : (double)r.Numerator / r.Denominator;
    }
}