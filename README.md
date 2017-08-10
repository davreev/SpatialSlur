# SpatialSlur
SpatialSlur is an open source library of geometric data structures and algorithms intended to facilitate the development of novel computational design methods. It is written in C# and compiled against .NET 4.5.

<p align="center">
  <img src="https://github.com/daveReeves/SpatialSlur/blob/master/Examples/Gallery/170720_Relief_02.gif" alt="Banner">
</p>

## Overview
This repository consists of a standalone core library along with a number of extension libraries for interfacing with .NET APIs from 3d modeling applications such as Rhino and Unity.

Below is a brief outline of the core library by namespace. For further detail, take a look at the [reference documentation](http://www.spatialslur.com/documentation/0_2_2/index.html) or dive into the source files and poke around. If you have specific questions, comments, or suggestions, feel free to [get in touch](http://spatialslur.com/contact/).

+ __SlurCore__ contains base geometric data types and utility methods as well as extension methods for .NET Framework types.

+ __SlurData__ contains generic data structures and algorithms for statistical analysis and efficient spatial queries.

+ __SlurDynamics__ contains a projection based constraint solver and various constraint types for geometry optimization and form-finding. This is an implementation of principles described in (1) and follows many of the implementation details given in (2).

+ __SlurField__ contains generic data structures for discrete and continuous tensor field representations along with various algorithms for processing, sampling, and visualization.

+ __SlurMesh__ contains half-edge data structures for discrete representations of networks and surfaces along with various algorithms for geometry processing, topological traversal, segmentation, and subdivision.

## Dependencies
While the core library has no dependencies outside of the .NET framework, each extension library references assemblies from its target application. If these dependencies can't be resolved for a particular extension library, simply remove its project folder from the solution before compiling.

## Setup
To get started, either download precompiled binaries from the [latest release](https://github.com/daveReeves/SpatialSlur/releases) or clone/download the repo and compile locally. Note that .dll files may be blocked by default when downloaded from the web so follow [these instructions](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/) to unblock.

### Grasshopper
To use within Grasshopper, drop a C#/VB scripting component on the canvas, right click on the component icon/name, and go to “Manage Assemblies”. Add SpatialSlur.dll and SlurRhino.dll to the list of referenced assemblies and click “OK” to confirm. You should now be able to access all library functionality within this component.

### Unity
Coming soon.

___
#### References
1. _<http://lgg.epfl.ch/publications/2012/shapeup/paper.pdf>_
2. _<http://lgg.epfl.ch/publications/2015/ShapeOp/ShapeOp_DMSC15.pdf>_
