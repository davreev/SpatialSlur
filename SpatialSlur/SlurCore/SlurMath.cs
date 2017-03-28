using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class SlurMath
    {
        public const double TwoPI = Math.PI * 2.0;
        public const double HalfPI = Math.PI * 0.5;
        public const double InvPI = 1.0 / Math.PI;

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool ApproxEquals(double x, double y, double epsilon)
        {
            return Math.Abs(x - y) < epsilon;
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
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Normalize(double t, double t0, double t1)
        {
            return (t - t0) / (t1 - t0);
        }


        /// <summary>
        /// remaps a number from one  domain to another
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
        public static int Clamp(int t, int min, int max)
        {
            return (t < min) ? min : (t > max) ? max : t;
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
            return (Math.Abs(t1 - t) < Math.Abs(t0 - t)) ? t1 : t0;
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
            return (Math.Abs(t1 - t) < Math.Abs(t0 - t)) ? t1 : t0;
        }


        /// <summary>
        /// Rounds x to the nearest power of y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double NearestPow(double x, double y)
        {
            return Math.Pow(y, Math.Round(Math.Log(x, y)));
        }


        /// <summary>
        /// Returns fractional portion of t.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Fract(double t)
        {
            return t - Math.Floor(t);
        }


        /// <summary>
        /// Returns fractional portion of t. 
        /// Also returns the whole portion in an out parameter.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static double Fract(double t, out int whole)
        {
            whole = (int)Math.Floor(t);
            return t - whole;
        }
        

        /// <summary>
        /// Wraps t to the domain.
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
        /// Wraps t to the domain defined by t0 and t1.
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
            return a - Math.Floor(a / n) * n;
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
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double SmoothPulse(double t, double center, double width)
        {
            t = Math.Abs(t - center);
            if (t > width) return 0.0;
            return HermiteC1(1.0 - t / width);
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
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double SmootherPulse(double t, double center, double width)
        {
            t = Math.Abs(t - center);
            if (t > width) return 0.0;
            return HermiteC2(1.0 - t / width);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double HermiteC1(double t)
        {
            return t * t * (3.0 - 2.0 * t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double HermiteC2(double t)
        {
            return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
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
            return 1.0 / Math.Cos(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Cosec(double x)
        {
            return 1.0 / Math.Sin(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Cotan(double x)
        {
            return 1.0 / Math.Tan(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double Sigmoid(double t)
        {
            return 1.0 / (1.0 + Math.Exp(t));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double ToDegrees(double radians)
        {
            const double toDeg = 180.0 / Math.PI;
            return radians * toDeg;
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ToRadians(double degrees)
        {
            const double toRad = Math.PI / 180.0;
            return degrees * toRad;
        }
    }
}
