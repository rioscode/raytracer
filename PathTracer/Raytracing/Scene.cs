﻿using OpenTK.Mathematics;
using PathTracer.Raytracing.AccelerationStructures;
using PathTracer.Raytracing.SceneObjects;
using PathTracer.Raytracing.SceneObjects.CameraParts;
using PathTracer.Raytracing.SceneObjects.Primitives;
using PathTracer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathTracer.Raytracing {
    /// <summary> The 3d scene in which the ray tracing takes place </summary>
    public class Scene {
        /// <summary> The camera in the scene </summary>
        public Camera Camera { get; }
        /// <summary> The acceleration structure used to find intersections </summary>
        public IAccelerationStructure AccelerationStructure { get; }
        /// <summary> The primitives in the scene </summary>
        public ICollection<Primitive> Primitives { get; }
        /// <summary> The lightsources in the scene </summary>
        public ICollection<Primitive> Lights { get; }

        /// <summary> Create a new scene with some default objects </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="primitives">The primitives in the scene</param>
        public Scene(IScreen screen, List<Primitive> primitives) { 
            Camera = new Camera(screen);
            Primitives = primitives;
            Lights = primitives.FindAll(p => p.Material.Emitting);
            Stopwatch timer = Stopwatch.StartNew();
            AccelerationStructure = new SBVHTree(primitives);
            Console.WriteLine(timer.ElapsedMilliseconds + "\t| (S)BVH Building ms");
        }

        /// <summary> Create an empty scene </summary>
        /// <param name="screen">The screen to draw the raytracing</param>
        /// <returns>An empty scene</returns>
        public static Scene Empty(IScreen screen) {
            return new Scene(screen, new List<Primitive>());
        }
        
        /// <summary> Create the default scene </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <returns>A default scene</returns>
        public static Scene Default(IScreen screen) {
            return new Scene(screen, DefaultPrimitives);
        }

        /// <summary> Create the default scene with random spheres </summary>
        /// <param name="screen">Te screen to draw the raytracing to</param>
        /// <param name="randomSpheres">The amount of random spheres in the scene</param>
        /// <returns>A default scene with random spheres</returns>
        public static Scene DefaultWithRandomSpheres(IScreen screen, int randomSpheres) {
            List<Primitive> defaultPrimitives = DefaultPrimitives;
            List<Primitive> primitives = new List<Primitive>(defaultPrimitives);
            for (int i = 0; i < randomSpheres; i++) {
                Vector3 spheresCenter = new Vector3(0f, -30f, 0f);
                Vector3 spheresBox = new Vector3(60f, 30f, 60f);
                Vector3 pos = Utils.DetRandom.Vector() * spheresBox - 0.5f * spheresBox + spheresCenter;
                float radius = (float)Utils.DetRandom.NextDouble();
                primitives.Add(new Sphere(pos, radius));
            }
            return new Scene(screen, primitives);
        }

        /// <summary> Create the default scene with random spheres </summary>
        /// <param name="screen">Te screen to draw the raytracing to</param>
        /// <param name="randomTriangles">The amount of random triangles in the scene</param>
        /// <returns>A default scene with random spheres</returns>
        public static Scene DefaultWithRandomTriangles(IScreen screen, int randomTriangles) {
            List<Primitive> defaultPrimitives = DefaultPrimitives;
            List<Primitive> primitives = new List<Primitive>(defaultPrimitives);
            for (int i = 0; i < randomTriangles; i++) {
                Vector3 trianglesCenter = new Vector3(0f, -30f, 0f);
                Vector3 trianglesBox = new Vector3(60f, 30f, 60f);
                Vector3 p1 = Utils.DetRandom.Vector() * trianglesBox - 0.5f * trianglesBox + trianglesCenter;
                Vector3 p2 = p1 + Utils.DetRandom.Vector(4f);
                Vector3 p3 = p1 - Utils.DetRandom.Vector(4f);
                primitives.Add(new Triangle(p1, p2, p3));
            }
            return new Scene(screen, primitives);
        }

        /// <summary> The primitives in the default scene </summary>
        public static List<Primitive> DefaultPrimitives => new List<Primitive>() {
            new Sphere(new Vector3(-3, -1, 5), 1, Material.DiffuseGreen),
            new Sphere(new Vector3(3, -1, 5), 1, Material.GlossyRed),
            new Sphere(new Vector3(0, -1, 5), 1, Material.Mirror),
            new Sphere(new Vector3(-1, -1, 2), 0.5f, Material.Glass),
            new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 0), new Vector3(5, 0, 10), null, Material.GlossyPurpleMirror),
            new Triangle(new Vector3(-5, 0, 0), new Vector3(5, 0, 10), new Vector3(-5, 0, 10), null, Material.GlossyGreen),
            new PointLight(new Vector3(0, -8, 3), new Vector3(1, 1, 0.75f), 200),
            //new Sphere(new Vector3(0, -8, 3), 0.5f, new Material(200, new Vector3(1, 1, 0.75f)))
        };

        /// <summary> Intersect the scene with a ray and calculate the color found by the ray </summary>
        /// <param name="ray">The ray to intersect the scene with</param>
        /// <param name="recursionDepth">The recursion depth if this is a secondary ray</param>
        /// <returns>The color at the origin of the ray</returns>
        public Vector3 CastRay(Ray ray, int recursionDepth = 0) {
            // Intersect with Scene
            Intersection? intersection = AccelerationStructure.Intersect(ray);
            if (intersection == null) return Vector3.Zero;
            Vector3 directIllumination = intersection.Primitive.Material.Specularity < 1 ? CastShadowRays(intersection) : Vector3.Zero;
            Vector3 radianceOut;
            
            if (intersection.Primitive.Material.Specularity > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Specular
                Vector3 reflectedIn = CastRay(intersection.GetReflectedRay(), recursionDepth + 1);
                Vector3 reflectedOut = reflectedIn * intersection.Primitive.Material.Color;
                radianceOut = directIllumination * (1 - intersection.Primitive.Material.Specularity) + reflectedOut * intersection.Primitive.Material.Specularity;
            } else if (intersection.Primitive.Material.Dielectric > 0 && recursionDepth < Ray.MaxRecursionDepth) {
                // Dielectric
                float reflected = intersection.GetReflectivity();
                float refracted = 1 - reflected;
                Ray? refractedRay = intersection.GetRefractedRay();
                Vector3 incRefractedLight = refractedRay != null ? CastRay(refractedRay, recursionDepth + 1) : Vector3.Zero;
                Vector3 incReflectedLight = CastRay(intersection.GetReflectedRay(), recursionDepth + 1);
                radianceOut = directIllumination * (1f - intersection.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * intersection.Primitive.Material.Dielectric * intersection.Primitive.Material.Color;
            } else {
                // Diffuse
                radianceOut = directIllumination;
            }

            if (intersection.Primitive.Material.Emitting) {
                radianceOut += intersection.Primitive.Material.EmittingLight / ray.DistanceAttenuation;
            }
            return radianceOut;
        }

        /// <summary> Cast shadow rays from an intersection to every light and calculate the color </summary>
        /// <param name="intersection">The intersection to cast the shadow rays from</param>
        /// <returns>The color at the intersection</returns>
        public Vector3 CastShadowRays(Intersection intersection) {
            Vector3 radianceOut = Vector3.Zero;
            foreach (Primitive light in Lights) {
                Ray shadowRay = intersection.GetShadowRay(light);
                if (Vector3.Dot(intersection.Normal, shadowRay.Direction) < 0) continue;
                if (AccelerationStructure.IntersectBool(shadowRay)) continue;
                Vector3 radianceIn = light.Material.EmittingLight * shadowRay.DistanceAttenuation;
                Vector3 irradiance;
                // N dot L
                float NdotL = Vector3.Dot(intersection.Normal, shadowRay.Direction);
                if (intersection.Primitive.Material.Glossyness > 0) {
                    // Glossy Object: Phong-Shading
                    Vector3 glossyDirection = -shadowRay.Direction - 2 * Vector3.Dot(-shadowRay.Direction, intersection.Normal) * intersection.Normal;
                    float dot = Vector3.Dot(glossyDirection, -intersection.Ray.Direction);
                    if (dot > 0) {
                        float glossyness = (float)Math.Pow(dot, intersection.Primitive.Material.GlossSpecularity);
                        irradiance = radianceIn * ((1 - intersection.Primitive.Material.Glossyness) * NdotL + intersection.Primitive.Material.Glossyness * glossyness);
                    } else {
                        irradiance = radianceIn * (1 - intersection.Primitive.Material.Glossyness) * NdotL;
                    }
                } else {
                    // Diffuse
                    irradiance = radianceIn * NdotL;
                }
                // Absorption
                radianceOut += irradiance * intersection.Primitive.Material.Color;
            }
            return radianceOut;
        }
    }
}