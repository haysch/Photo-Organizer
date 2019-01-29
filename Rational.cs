using System;

namespace PhotoOrganizer {
    /// <summary> TODO </summary>
    public  class Rational {
        long Numerator { get; }
        long Denominator { get; }

        public Rational(long num) {
            Numerator = num;
        }

        public Rational (long num, long den) {
            Numerator = num;
            Denominator = den;
        }

        public override string ToString() {
            if (this.Denominator == 1) return $"{this.Numerator}";
            return $"{Numerator}/{Denominator}";
        }

        public string ToFNumber() {
            return $"f/{(Numerator / Denominator)}";
        }

        /// <summary> Implicit casts Rational to double. </summary>
        public static implicit operator double(Rational r) => r.Numerator == 0 ? 0.0 : (double)r.Numerator / r.Denominator;
    }
}