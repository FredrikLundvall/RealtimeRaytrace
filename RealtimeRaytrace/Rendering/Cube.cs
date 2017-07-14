using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RealtimeRaytrace
{
    public class Cube : Entity
    {
        Color _color = Color.White;
        float _radius = 1;

        public Cube(int index, bool isIndexedByPosition, Vector3 position, Color color)
            : base(index, isIndexedByPosition, position)
        {
            _color = color;
        }

        public Intersection Intersect(Ray ray)
        {
            Vector3 el = GetPosition() - ray.GetStart();
            float d = Vector3.Dot(el, ray.GetVector());
            float els = Vector3.Dot(el, el);
            float rs = _radius * _radius;

            if (d < 0 && els > rs)
            {
                return new Intersection(true);
            }
            float ms = els - d * d;
            if (ms > rs)
            {
                return new Intersection(true);
            }
            float q = (float)Math.Sqrt(rs - ms);
            float t;
            if (els > rs)
            {
                t = d - q;
            }
            else
            {
                t = d + q;
            }
            Vector3 p = ray.GetStart() + Vector3.Multiply(ray.GetVector(), t);
            return new Intersection(p, Vector3.Normalize(Vector3.Divide(p - GetPosition(), _radius)), t, this);
        }

        public Color GetColor()
        {
            return _color;
        }

        public override string ToString()
        {
            return string.Format("{0}, color: {1}, size: {2}", this._color.ToString(), _color.ToString(), "");
        }
    }
}




//bool TestRayOBBIntersection(
//    glm::vec3 ray_origin,        // Ray origin, in world space
//    glm::vec3 ray_direction,     // Ray direction (NOT target position!), in world space. Must be normalize()'d.
//    glm::vec3 aabb_min,          // Minimum X,Y,Z coords of the mesh when not transformed at all.
//    glm::vec3 aabb_max,          // Maximum X,Y,Z coords. Often aabb_min*-1 if your mesh is centered, but it's not always the case.
//    glm::mat4 ModelMatrix,       // Transformation applied to the mesh (which will thus be also applied to its bounding box)
//    float& intersection_distance // Output : distance between ray_origin and the intersection with the OBB
//)
//{

//    // Intersection method from Real-Time Rendering and Essential Mathematics for Games

//    float tMin = 0.0f;
//    float tMax = 100000.0f;

//    glm::vec3 OBBposition_worldspace(ModelMatrix[3].x, ModelMatrix[3].y, ModelMatrix[3].z);

//    glm::vec3 delta = OBBposition_worldspace - ray_origin;

//    // Test intersection with the 2 planes perpendicular to the OBB's X axis
//    {
//        glm::vec3 xaxis(ModelMatrix[0].x, ModelMatrix[0].y, ModelMatrix[0].z);
//        float e = glm::dot(xaxis, delta);
//        float f = glm::dot(ray_direction, xaxis);

//        if (fabs(f) > 0.001f)
//        { // Standard case

//            float t1 = (e + aabb_min.x) / f; // Intersection with the "left" plane
//            float t2 = (e + aabb_max.x) / f; // Intersection with the "right" plane
//                                             // t1 and t2 now contain distances betwen ray origin and ray-plane intersections

//            // We want t1 to represent the nearest intersection, 
//            // so if it's not the case, invert t1 and t2
//            if (t1 > t2)
//            {
//                float w = t1; t1 = t2; t2 = w; // swap t1 and t2
//            }

//            // tMax is the nearest "far" intersection (amongst the X,Y and Z planes pairs)
//            if (t2 < tMax)
//                tMax = t2;
//            // tMin is the farthest "near" intersection (amongst the X,Y and Z planes pairs)
//            if (t1 > tMin)
//                tMin = t1;

//            // And here's the trick :
//            // If "far" is closer than "near", then there is NO intersection.
//            // See the images in the tutorials for the visual explanation.
//            if (tMax < tMin)
//                return false;

//        }
//        else
//        { // Rare case : the ray is almost parallel to the planes, so they don't have any "intersection"
//            if (-e + aabb_min.x > 0.0f || -e + aabb_max.x < 0.0f)
//                return false;
//        }
//    }


//    // Test intersection with the 2 planes perpendicular to the OBB's Y axis
//    // Exactly the same thing than above.
//    {
//        glm::vec3 yaxis(ModelMatrix[1].x, ModelMatrix[1].y, ModelMatrix[1].z);
//        float e = glm::dot(yaxis, delta);
//        float f = glm::dot(ray_direction, yaxis);

//        if (fabs(f) > 0.001f)
//        {

//            float t1 = (e + aabb_min.y) / f;
//            float t2 = (e + aabb_max.y) / f;

//            if (t1 > t2) { float w = t1; t1 = t2; t2 = w; }

//            if (t2 < tMax)
//                tMax = t2;
//            if (t1 > tMin)
//                tMin = t1;
//            if (tMin > tMax)
//                return false;

//        }
//        else
//        {
//            if (-e + aabb_min.y > 0.0f || -e + aabb_max.y < 0.0f)
//                return false;
//        }
//    }


//    // Test intersection with the 2 planes perpendicular to the OBB's Z axis
//    // Exactly the same thing than above.
//    {
//        glm::vec3 zaxis(ModelMatrix[2].x, ModelMatrix[2].y, ModelMatrix[2].z);
//        float e = glm::dot(zaxis, delta);
//        float f = glm::dot(ray_direction, zaxis);

//        if (fabs(f) > 0.001f)
//        {

//            float t1 = (e + aabb_min.z) / f;
//            float t2 = (e + aabb_max.z) / f;

//            if (t1 > t2) { float w = t1; t1 = t2; t2 = w; }

//            if (t2 < tMax)
//                tMax = t2;
//            if (t1 > tMin)
//                tMin = t1;
//            if (tMin > tMax)
//                return false;

//        }
//        else
//        {
//            if (-e + aabb_min.z > 0.0f || -e + aabb_max.z < 0.0f)
//                return false;
//        }
//    }

//    intersection_distance = tMin;
//    return true;

//}







///// Ray-AABB intersection test, by the slab method.  Highly optimized.
//static inline bool
//dmnsn_ray_box_intersection(dmnsn_optimized_ray optray, dmnsn_aabb box, double t)
//{
//    // This is actually correct, even though it appears not to handle edge cases
//    // (ray.n.{x,y,z} == 0).  It works because the infinities that result from
//    // dividing by zero will still behave correctly in the comparisons.  Rays
//    // which are parallel to an axis and outside the box will have tmin == inf
//    // or tmax == -inf, while rays inside the box will have tmin and tmax
//    // unchanged.

//    double tx1 = (box.min.X - optray.x0.X) * optray.n_inv.X;
//    double tx2 = (box.max.X - optray.x0.X) * optray.n_inv.X;

//    double tmin = dmnsn_min(tx1, tx2);
//    double tmax = dmnsn_max(tx1, tx2);

//    double ty1 = (box.min.Y - optray.x0.Y) * optray.n_inv.Y;
//    double ty2 = (box.max.Y - optray.x0.Y) * optray.n_inv.Y;

//    tmin = dmnsn_max(tmin, dmnsn_min(ty1, ty2));
//    tmax = dmnsn_min(tmax, dmnsn_max(ty1, ty2));

//    double tz1 = (box.min.Z - optray.x0.Z) * optray.n_inv.Z;
//    double tz2 = (box.max.Z - optray.x0.Z) * optray.n_inv.Z;

//    tmin = dmnsn_max(tmin, dmnsn_min(tz1, tz2));
//    tmax = dmnsn_min(tmax, dmnsn_max(tz1, tz2));

//    return tmax >= dmnsn_max(0.0, tmin) && tmin < t;
//}