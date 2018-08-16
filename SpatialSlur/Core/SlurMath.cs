
/*
 * Notes
 */

using System;
using static System.Math;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class SlurMath
    {
        /// <summary></summary>
        public const double ZeroToleranced = 1.0e-12;
        /// <summary></summary>
        public const double TwoPId = PI * 2.0;
        /// <summary></summary>
        public const double HalfPId = PI * 0.5;
        /// <summary></summary>
        public const double InvPId = 1.0 / PI;
        /// <summary></summary>
        public const double Sqrt2d = 1.41421356237309504880168872420969807;

        /// <summary></summary>
        public const float ZeroTolerancef = 1.0e-6f;
        /// <summary></summary>
        public const float PIf = (float)PI;
        /// <summary></summary>
        public const double TwoPIf = (float)TwoPId;
        /// <summary></summary>
        public const double HalfPIf = (float)HalfPId;
        /// <summary></summary>
        public const double InvPIf = (float)InvPId;
        /// <summary></summary>
        public const double Sqrt2f = 1.41421356237f;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool ApproxEquals(double x, double y, double epsilon = ZeroToleranced)
        {
            return Abs(x - y) < epsilon;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool ApproxEquals(float x, float y, float epsilon = ZeroTolerancef)
        {
            return Abs(x - y) < epsilon;
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
        /// Fast containment check for array indices. 
        /// Assumes range is positive.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool Contains(int i, int length)
        {
            return ((uint)i < (uint)length);
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
        public static double RoundToPow(double x, double y)
        {
            return Pow(y, Round(Log(x, y)));
        }


        /// <summary>
        /// Round x up to the nearest larger power of y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double CeilToPow(double x, double y)
        {
            return Pow(y, Ceiling(Log(x, y)));
        }


        /// <summary>
        /// Round x down to the nearest smaller power of y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double FloorToPow(double x, double y)
        {
            return Pow(y, Floor(Log(x, y)));
        }


        /// <summary>
        /// Assumes that divisor y is positive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int DivideCeil(int x, int y)
        {
            return (x + y - 1) / y;
        }


        /// <summary>
        /// Returns fractional portion of t.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Fract(double x)
        {
            return x - Floor(x);
        }


        /// <summary>
        /// Returns fractional portion of t. 
        /// Also returns the whole portion in an out parameter.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static double Fract(double x, out int whole)
        {
            var fx = Floor(x);
            whole = (int)fx;
            return x - fx;
        }
        

        /// <summary>
        /// Wraps t to the given interval.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static double Repeat(double t, double t0, double t1)
        {
            return Repeat(t - t0, t1 - t0) + t0;
        }


        /// <summary>
        /// Wraps t to the given interval.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static int Repeat(int t, int t0, int t1)
        {
            return Repeat(t - t0, t1 - t0) + t0;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double Repeat(double t, double length)
        {
            return t - Floor(t / length) * length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int Repeat(int t, int length)
        {
            t %= length;
            return (t * length < 0) ? t + length : t;
        }


        /// <summary>
        /// Assumes the given length is positive to save a few ticks.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int RepeatPositive(int t, int length)
        {
            t %= length;
            return (t < 0) ? t + length : t;
        }


        /// <summary>
        /// Bounces t back and forth within the given interval.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static double PingPong(double t, double t0, double t1)
        {
            return PingPong(t - t0, t1 - t0) + t0;
        }


        /// <summary>
        /// Bounces t back and forth within the given interval.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static int PingPong(int t, int t0, int t1)
        {
            return PingPong(t - t0, t1 - t0) + t0;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double PingPong(double t, double length)
        {
            t = Repeat(t, length + length);
            return length - Abs(t - length) * Sign(length);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int PingPong(int t, int length)
        {
            t = Repeat(t, length + length);
            return length - Abs(t - length) * Sign(length);
        }


        /// <summary>
        /// Assumes the given length is positive to save a few ticks.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int PingPongPositive(int t, int length)
        {
            t = RepeatPositive(t, length + length);
            return length - Abs(t - length);
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
        /// Assumes t is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double HermiteC1(double t)
        {
            return t * t * (3.0 - 2.0 * t);
        }


        /// <summary>
        /// Assumes t is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float HermiteC1(float t)
        {
            return t * t * (3.0f - 2.0f * t);
        }


        /// <summary>
        /// Assumes t is between 0 and 1 inclusive
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double HermiteC2(double t)
        {
            return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
        }


        /// <summary>
        /// Assumes t is between 0 and 1 inclusive
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
        /// <param name="length"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Contour(double t, double length)
        {
            return Fract(t / length);
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
        /// <param name="y"></param>
        /// <returns></returns>
        public static double AsinSafe(double y)
        {
            return Asin(Clamp(y, -1.0, 1.0));
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
        /// Solves for the roots of ax + b = 0.
        /// Returns the number of solutions found (0 or 1).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="root"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        private static int SolveLinear(double a, double b, out double root, double epsilon = ZeroToleranced)
        {
            if (Abs(a) < epsilon)
            {
                root = 0.0;
                return 0;
            }
            
            root = -b / a;
            return 1;
        }


        /// <summary>
        /// Solves for the roots of ax^2 + bx + c = 0.
        /// Returns the number of real solutions found (0, 1, or 2).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static int SolveQuadratic(double a, double b, double c, out double r0, out double r1, double epsilon = ZeroToleranced)
        {
            // impl ref
            // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (5.6)

            // check if actually linear
            if (Abs(a) < epsilon)
            {
                r1 = 0.0;
                return SolveLinear(b, c, out r0, epsilon);
            }

            var disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
            {
                r0 = r1 = 0.0;
                return 0;
            }

            var t = -0.5 * (b + Sign(b) * Sqrt(disc));
            r0 = t / a;
            r1 = c / t;
            return 2;
        }


        /// <summary>
        /// Solves for the roots of ax^3 + bx^2 + cx + d = 0.
        /// Returns the number of real solutions found (0, 1, 2, or 3).
        /// </summary>
        public static int SolveCubic(double a, double b, double c, double d, out double r0, out double r1, out double r2, double epsilon = ZeroToleranced)
        {
            // impl ref
            // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (5.6)

            // check if actually quadratic
            if(Abs(a) < epsilon)
            {
                r2 = 0.0;
                return SolveQuadratic(b, c, d, out r0, out r1, epsilon);
            }

            var inv = 1.0 / a;
            return SolveCubic(inv * b, inv * c, inv * d, out r0, out r1, out r2);
        }


        /// <summary>
        /// Solves for the roots of x^3 + ax^2 + bx + c = 0.
        /// Returns the number of real solutions found (0, 1, 2, or 3).
        /// </summary>
        public static int SolveCubic(double a, double b, double c, out double r0, out double r1, out double r2, double epsilon = ZeroToleranced)
        {
            // impl ref
            // https://www2.units.it/ipl/students_area/imm2/files/Numerical_Recipes.pdf (5.6)

            const double inv3 = 1.0 / 3.0;
            const double inv9 = 1.0 / 9.0;
            const double inv54 = 1.0 / 54.0;

            var aa = a * a;
            var q = (aa - 3.0 * b) * inv9;
            var r = (2.0 * aa * a - 9.0 * a * b + 27.0 * c) * inv54;

            var qqq = q * q * q;
            var rr = r * r;

            // if true, then 3 real roots exist
            if (rr < qqq)
            {
                var t = Acos(r / Sqrt(qqq));
                var k = -2.0 * Sqrt(q);
                var d = a * inv3;

                r0 = k * Cos(t * inv3) - d;
                r1 = k * Cos((t + TwoPId) * inv3) - d;
                r2 = k * Cos((t - TwoPId) * inv3) - d;
                return 3;
            }

            // only 1 real root exists
            var x = -Sign(r) * Pow(Abs(r) + Sqrt(rr - qqq), inv3);
            var y = Abs(x) < epsilon ? 0.0 : q / x;

            r0 = x + y - a * inv3;
            r1 = r2 = 0.0;
            return 1;
        }
    }
}
