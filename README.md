Whitted Raytracer for the course Advanced Graphics in 2019-2020 at Utrecht University

Created by:
Theo Harkes (5672120)

Required:
- .Net Framework 4.7.2 (Raytracer)
- .Net Core 2.1 (UnitTests)

Features:
- Raytracing architecture
- Camera: Position, Orientation, FOV, Aspect ratio
- Input handling: see controls
- Debug information
- Primitives: Planes, Spheres, Triangles
- Lights: Pointlights
- Materials: Diffuse, Specular, Dielectric (Snell & Fresnel), Glossy (Phong Shading)
- Acceleration Structure: SBVH, BVH (SAH, Binning, Split-Ordered-Traversal & BVH rendering)
- Multithreading: Threadpool with worker threads

Added For Final Assignment:
- SBVH (Split Bounding Volume Hierarchy)
- Unit tests for BVH/SBVH (node count)
- Randomized pixel sampling to increase FPS
- Save Camera Configuration on exit with ESC
- Raytracing architecture: I tried to setup my code from the rendered scene point of view. So theoretically this framework can easily duplicate the camera and support multiple gamewindows.
    This framework is by no means an efficient ray tracer, but I did try to make the code part of the features and therefore is hopefully easily digestible.

Controls:
- Arrow keys:   Rotate Camera
- W,A,S,D:      Move Camera
- Space:        Move Camera Up
- Shift:        Move Camera Down
- F1:           Toggle Debug Information
- F2:           Toggle BVHTraversal Color Scale
- Numpad '+':   Increase FOV
- Numpad '-':   Decrease FOV
- ESC           Exit the renderer and save the Camera Configuration

References:
- Course Slides (Graphics & Advanced Graphics)
- OpenTK: https://github.com/opentk/opentk
- Triangle Intersection: https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
- Sphere Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
- AABB Intersection: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
- Binning: http://www.sci.utah.edu/~wald/Publications/2007/ParallelBVHBuild/fastbuild.pdf
- Triangle Clipping: Efficient Triangle and Quadrilateral Clipping within Shaders - M. McGuire
- SBVH Idea: Spatial Splits in Bounding Volume Hierarchies - M. Stich, H. Friedrich, A. Dietrich
- SBVH Implementation: Parallel Spatial Splits in Bounding Volume Hierarchies - V. Fuetterling, C. Lojewski, F.-J Pfreundt and A. Ebert


Notes to Self

Monte Carlo integration required for:
- Area Lights (Random point on area of light)
- Indirect Illumination (Random bounce over hemisphere)
- Depth of Field (Introducing a lense somewhere)
- Anti-Aliasing (Random point on screenplane pixels)
- Motion Blur (Random point in time)

Monte Carlo integration preferred for:
- Multiple lightsources
- Dielectric refraction or reflection

Other interesting notes:
- Where there is Monte Carlo integration there can be Russian Roulette, Multiple Importance Sampling, Machine Learning
- Light is actually a spectrum