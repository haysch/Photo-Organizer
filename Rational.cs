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
            if (this.Denominator == 1) return String.Format("{0}", this.Numerator);
            return String.Format("{0}/{1}", Numerator, Denominator);
        }

        /// <summary> Implicit casts Rational to double. </summary>
        public static implicit operator double(Rational f) {
            return (double) f.Numerator / f.Denominator;
        }

        // public double ToDouble() => Numerator == 0 ? 0.0 : Numerator / (double) Denominator;
    }
}