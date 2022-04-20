
using System;


namespace GPS_NotePad.Helpers
{
    public struct Vec2
    {

        private readonly double _x;
        private readonly double _y;

        public Vec2(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public static Vec2 operator +(Vec2 v1, Vec2 v)
        {
            return new Vec2(v1._x + v._x, v1._y + v._y);
        }

        public static Vec2 operator -(Vec2 v1, Vec2 v)
        {
            return new Vec2(v1._x - v._x, v1._y - v._y);
        }

        public static Vec2 operator *(Vec2 v, double f)
        {
            return new Vec2(v._x * f, v._y * f);
        }

        public double DistanceToSquared(Vec2 p)//2 -> this
        {
            double dX = p._x - this._x;
            double dY = p._y - this._y;

            return dX * dX + dY * dY;
        }

        public double DistanceTo(Vec2 p)
        {
            return Math.Sqrt(DistanceToSquared(p));
        }

        public double DotProduct(Vec2 p)
        {
            return this._x * p._x + this._y * p._y;
        }

    }


    public class DistanceToPoint
    {
  
        public double DistanceFromLineSegmentToPoint(Vec2 v, Vec2 w, Vec2 p) // (startLine, endLine, point, null)
        {

            double distSq = v.DistanceToSquared(w); // i.e. |w-v|^2 ... avoid a sqrt

            if (distSq == 0.0)
            {
                // v == w case
                return v.DistanceTo(p);
            }

            // consider the line extending the segment, parameterized as v + t (w - v)
            // we find projection of point p onto the line
            // it falls where t = [(p-v) . (w-v)] / |w-v|^2

            double t = (p - v).DotProduct(w - v) / distSq;

            if (t < 0.0)
            {            
                return v.DistanceTo(p);
            }
            else if (t > 1.0)
            {
                return w.DistanceTo(p);
            }

            // projection falls on the segment
           Vec2 projection = v + ((w - v) * t);

            return p.DistanceTo(projection);

        }

    }
}
