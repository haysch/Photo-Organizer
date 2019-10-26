using System;

namespace PhotoOrganizer.Primitives
{
    /// <summary>Base class for specifying a fraction using rational numbers.</summary>
    public class Rational
    {
        /// <summary>Gets the Numerator value.</summary>
        public long Numerator { get; }
        /// <summary>Gets the Denominator value.</summary>
        public long Denominator { get; }

        /// <summary>Constructor using only numerator value. Denominator value is 1.</summary>
        /// <param name="num">A long.</param>
        public Rational(long num)
        {
            Numerator = num;
            Denominator = 1;
        }

        /// <summary>Constructor for specifying both numerator and denominator value.</summary>
        /// <param name="num">A long.</param>
        /// <param name="den">A long.</param>
        public Rational(long num, long den)
        {
            Numerator = num;
            Denominator = den;
        }

        /// <summary>Constructor for converting numerator and denominator value from uint to long.</summary>
        /// <param name="num">An unsigned int.</param>
        /// <param name="den">An unsigned int.</param>
        public Rational(uint num, uint den)
        {
            Numerator = Convert.ToInt64(num);
            Denominator = Convert.ToInt64(den);
        }

        /// <summary>Converts the Rational to a fraction string of form 15/25.</summary>
        /// <returns>String format of a fraction.</returns>
        public override string ToString()
        {
            if (this.Denominator == 1) return $"{Numerator}";
            return $"{Numerator}/{Denominator}";
        }

        /// <summary>Helper method for computing the greatest common divisor for simplifying a fraction.</summary>
        /// <returns>Long of the GCD.</returns>
        /// <param name="a">A long.</param>
        /// <param name="b">A long.</param>
        private long GCD(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        /// <summary>Simplify the fraction to shortest form.</summary>
        /// <returns>String on form e.g. 5/15 = 1/3 and 5/7 = 5/7.</returns>
        public string SimplifyFraction()
        {
            if (Numerator > Denominator)
                return $"{Numerator / Denominator}";

            long gcd = GCD(Numerator, Denominator);
            return $"{Numerator / gcd}/{Denominator / gcd}";
        }

        /// <summary>Returns the Rational on the F-Number form.</summary>
        /// <returns>String format of F-Number (e.g. f/1).</returns>
        public string ToFNumber()
        {
            double fnum = (double)this;
            if (fnum % 1 > 0)
                return $"f/{fnum:N1}";
            return $"f/{fnum}";
        }

        /// <summary>Explicit cast Rational to double.</summary>
        public double ToDouble() => Denominator == 0 ? 0.0 : (double)Numerator / Denominator;

        /// <summary>Implicit casts Rational to double.</summary>
        /// <param name="r">A Rational object.</param>
        public static implicit operator double(Rational r) => r.Denominator == 0 ? 0.0 : (double)r.Numerator / r.Denominator;
    }
}