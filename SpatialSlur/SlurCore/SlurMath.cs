using static System.Math;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class SlurMath
    {
        /// <summary></summary>
        public const double ZeroTolerance = 1.0e-12;
        /// <summary></summary>
        public const double TwoPI = PI * 2.0;
        /// <summary></summary>
        public const double HalfPI = PI * 0.5;
        /// <summary></summary>
        public const double InvPI = 1.0 / PI;
        /// <summary></summary>
        public const double Sqrt2 = 1.41421356237309504880168872420969807;

        /// <summary></summary>
        public const float ZeroTolerancef = 1.0e-6f;
        /// <summary></summary>
        public const float PIf = (float)PI;
        /// <summary></summary>
        public const double TwoPIf = (float)TwoPI;
        /// <summary></summary>
        public const double HalfPIf = (float)HalfPI;
        /// <summary></summary>
        public const double InvPIf = (float)InvPI;
        /// <summary></summary>
        public const double Sqrt2f = 1.41421356237f;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool ApproxEquals(double x, double y, double tolerance = ZeroTolerance)
        {
            return Abs(x - y) < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool ApproxEquals(float x, float y, float tolerance = ZeroTolerancef)
        {
            return Abs(x - y) < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <returns></returns>
        public static double Lerp(double t0, double t1, double t)
        {
            return t0 + (t1 - t0) * t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <returns></returns>
        public static float Lerp(float t0, float t1, float t)
        {
            return t0 + (t1 - t0) * t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Normalize(double t, double t0, double t1)
        {
            return (t - t0) / (t1 - t0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Normalize(float t, float t0, float t1)
        {
            return (t - t0) / (t1 - t0);
        }


        /// <summary>
        /// Maps a number from one interval to another
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static double Remap(double t, double a0, double a1, double b0, double b1)
        {
            return Lerp(b0, b1, Normalize(t, a0, a1));
        }


        /// <summary>
        /// Maps a number from one interval to another
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static float Remap(float t, float a0, float a1, float b0, float b1)
        {
            return Lerp(b0, b1, Normalize(t, a0, a1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Saturate(double t)
        {
            return (t > 1.0) ? 1.0 : (t < 0.0) ? 0.0 : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Saturate(float t)
        {
            return (t > 1.0f) ? 1.0f : (t < 0.0f) ? 0.0f : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static double Clamp(double t, double range)
        {
            return (t < 0.0) ? 0.0 : (t > range) ? range : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float Clamp(float t, float range)
        {
            return (t < 0.0f) ? 0.0f : (t > range) ? range : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int Clamp(int t, int range)
        {
            return (t < 0) ? 0 : (t > range) ? range : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Clamp(double t, double min, double max)
        {
            return (t < min) ? min : (t > max) ? max : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Clamp(float t, float min, float max)
        {
            return (t < min) ? min : (t > max) ? max : t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>  
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Clamp(int t, int min, int max)
        {
            return (t < min) ? min : (t > max) ? max : t;
        }


        /// <summary>
        /// Fast containment check for array indices. Assumes range is positive.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool Contains(int index, int range)
        {
            return ((uint)index < (uint)range);
        }


        /// <summary>
        /// Exclusive containment check.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Contains(double t, double min, double max)
        {
            return t >= min && t < max;
        }


        /// <summary>
        /// Exclusive containment check.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Contains(float t, float min, float max)
        {
            return t >= min && t < max;
        }


        /// <summary>
        /// Exclusive containment check.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Contains(int t, int min, int max)
        {
            return t >= min && t < max;
        }


        /// <summary>
        /// Inclusive containment check.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool ContainsIncl(double t, double min, double max)
        {
            return t >= min && t <= max;
        }


        /// <summary>
        /// Inclusive containment check.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool ContainsIncl(float t, float min, float max)
        {
            return t >= min && t <= max;
        }


        /// <summary>
        /// Inclusive containment check.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool ContainsIncl(int t, int min, int max)
        {
            return t >= min && t <= max;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static double Nearest(double t, double t0, double t1)
        {
            return (Abs(t1 - t) < Abs(t0 - t)) ? t1 : t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static float Nearest(float t, float t0, float t1)
        {
            return (Abs(t1 - t) < Abs(t0 - t)) ? t1 : t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static int Nearest(int t, int t0, int t1)
        {
            return (Abs(t1 - t) < Abs(t0 - t)) ? t1 : t0;
        }


        /// <summary>
        /// Rounds x to the nearest power of y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double NearestPow(double x, double y)
        {
            return Pow(y, Round(Log(x, y)));
        }


        /// <summary>
        /// Returns fractional portion of t.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Fract(double value)
        {
            return value - Floor(value);
        }


        /// <summary>
        /// Returns fractional portion of t. 
        /// Also returns the whole portion in an out parameter.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static double Fract(double value, out int whole)
        {
            whole = (int)Floor(value);
            return value - whole;
        }
        

        /// <summary>
        /// Wraps t to the given interval.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static double Wrap(double t, double t0, double t1)
        {
            return Mod(t - t0, t1 - t0) + t0;
        }


        /// <summary>
        /// Wraps t to the given interval.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static int Wrap(int t, int t0, int t1)
        {
            return Mod(t - t0, t1 - t0) + t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Mod(double a, double n)
        {
            return a - Floor(a / n) * n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Mod(int a, int n)
        {
            a %= n;
            return (a * n < 0)? a + n : a;
        }


        /// <summary>
        /// Assumes that n is positive to save a few ticks.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Mod2(int a, int n)
        {
            a %= n;
            return (a < 0) ? a + n : a;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static double Step(double t, double edge)
        {
            return (t < edge) ? 0.0 : 1.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static float Step(float t, float edge)
        {
            return (t < edge) ? 0.0f : 1.0f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static double Ramp(double t, double t0, double t1)
        {
            return Saturate(Normalize(t, t0, t1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static float Ramp(float t, float t0, float t1)
        {
            return Saturate(Normalize(t, t0, t1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double SmoothStep(double t, double t0, double t1)
        {
            return HermiteC1(Saturate(Normalize(t, t0, t1)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SmoothStep(float t, float t0, float t1)
        {
            return HermiteC1(Saturate(Normalize(t, t0, t1)));
        }


        /// <summary>
        /// http://www.iquilezles.org/www/articles/functions/functions.htm
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double SmoothPulse(double t, double center, double width)
        {
            t = Abs(t - center);
            if (t > width) return 0.0;
            return HermiteC1(1.0 - t / width);
        }


        /// <summary>
        /// http://www.iquilezles.org/www/articles/functions/functions.htm
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SmoothPulse(float t, float center, float width)
        {
            t = Abs(t - center);
            if (t > width) return 0.0f;
            return HermiteC1(1.0f - t / width);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double SmootherStep(double t, double t0, double t1)
        {
            return HermiteC2(Saturate(Normalize(t, t0, t1)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SmootherStep(float t, float t0, float t1)
        {
            return HermiteC2(Saturate(Normalize(t, t0, t1)));
        }


        /// <summary>
        /// http://www.iquilezles.org/www/articles/functions/functions.htm
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double SmootherPulse(double t, double center, double width)
        {
            t = Abs(t - center);
            if (t > width) return 0.0;
            return HermiteC2(1.0 - t / width);
        }


        /// <summary>
        /// http://www.iquilezles.org/www/articles/functions/functions.htm
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SmootherPulse(float t, float center, float width)
        {
            t = Abs(t - center);
            if (t > width) return 0.0f;
            return HermiteC2(1.0f - t / width);
        }


        /// <summary>
        /// Assumes x is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double HermiteC1(double t)
        {
            return t * t * (3.0 - 2.0 * t);
        }


        /// <summary>
        /// Assumes x is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float HermiteC1(float t)
        {
            return t * t * (3.0f - 2.0f * t);
        }


        /// <summary>
        /// Assumes x is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double HermiteC2(double t)
        {
            return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
        }


        /// <summary>
        /// Assumes x is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float HermiteC2(float t)
        {
            return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
        }


        /// <summary>
        /// Assumes x is between 0 and 1 inclusive
        /// http://www.iquilezles.org/www/articles/functions/functions.htm
        /// </summary>
        /// <returns></returns>
        public static double PowCurve(double x, double a, double b)
        {
            double k = Pow(a + b, a + b) / (Pow(a, a) * Pow(b, b));
            return k * Pow(x, a) * Pow(1.0 - x, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Contour(double t, double range)
        {
            return Fract(t / range);
        }


        /// <summary>
        /// assumes t is between 0 and 1
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Contour(double t, int steps)
        {
            return Fract(t * steps);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Sec(double x)
        {
            return 1.0 / Cos(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Cosec(double x)
        {
            return 1.0 / Sin(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Cotan(double x)
        {
            return 1.0 / Tan(x);
        }


        /// <summary>
        /// Clamps the input to a valid range (-1 to 1).
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double AcosSafe(double x)
        {
            return Acos(Clamp(x, -1.0, 1.0));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Sigmoid(double t)
        {
            return 1.0 / (1.0 + Exp(t));
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double ToDegrees(double radians)
        {
            const double toDeg = 180.0 / PI;
            return radians * toDeg;
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ToRadians(double degrees)
        {
            const double toRad = PI / 180.0;
            return degrees * toRad;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static bool SolveQuadratic(double a, double b, double c, out double r0, out double r1)
        {
            // impl ref
            // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (5.6)

            if (a == 0.0 && b == 0.0)
            {
                r0 = r1 = 0.0;
                return true;
            }

            var disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
            {
                r0 = r1 = 0.0;
                return false;
            }

            var t = -0.5 * (b + Sign(b) * Sqrt(disc));
            r0 = t / a;
            r1 = c / t;
            return true;
        }
    }
}
