# SpatialSlur
SpatialSlur is an open source library of geometric data structures and algorithms intended to facilitate the development of novel computational design methods. It is written in C# and compiled against .NET 4.6.

<p align="center">
  <img src="https://github.com/daveReeves/SpatialSlur/blob/master/Examples/Gallery/170720_Relief_02.gif" alt="Banner">
</p>

## Overview
This repository consists of a standalone core library along with a number of extension libraries for interfacing with .NET APIs from other applications such as Rhino and Unity.

Below is a brief outline of the core library by namespace. For further detail, take a look at the [reference documentation](http://www.spatialslur.com/documentation/0_2_3/index.html) or dive into the source files and poke around. If you have specific questions, comments, or suggestions, feel free to [get in touch](http://spatialslur.com/contact/).

+ __SlurCore__ contains primitive data types for geometry processing, various utility methods, and extension methods for .NET Framework types.

+ __SlurData__ contains generic data structures for efficient spatial queries and related algorithms.

+ __SlurDynamics__ contains a projection based constraint solver and various constraint types for geometry optimization and form-finding. This is an implementation of principles described in (1) and follows many of the implementation details given in (2).

+ __SlurField__ contains generic data structures for discrete and continuous field representations along with various algorithms for processing and visualization.

+ __SlurMesh__ contains half-edge data structures for discrete representations of networks and surfaces along with various algorithms for geometry processing, topological traversal, segmentation, and subdivision.

## Dependencies
While the core library has no dependencies beyond the .NET framework, extension libraries typically reference additional assemblies from their respective applications. If these dependencies can't be resolved for a particular extension library, simply remove its project folder from the solution before compiling.

## Setup
To get started, either download precompiled binaries from the [latest release](https://github.com/daveReeves/SpatialSlur/releases) or clone/download the repo and compile locally. Note that .dll files might be blocked by default when downloaded from the web so follow [these instructions](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/) to unblock. If compiling locally, you'll need [Visual Studio 2017](https://www.visualstudio.com/vs/whatsnew/) or later as the library makes use of some C# 7 features.

### Grasshopper
To use within Grasshopper, drop a C#/VB scripting component on the canvas, right click on the component icon/name, and go to **Manage Assemblies**. Add SpatialSlur.dll and SlurRhino.dll to the list of referenced assemblies and click **OK** to confirm. SpatialSlur types will now be accessible within this scripting component.

### Unity
Before using in Unity, the editor must to be configured to target .NET 4.6 (available in Unity 2017.1 or later). To do so, go to **Edit > Project Settings > Player** and expand the **Other Settings** dropdown. Under **Configruation**, set the **Scripting Runtime Version** to **Experimental (.Net 4.6 Equivalent)**.

Once this has been set, create a **Libraries** folder within the **Assets** folder of the current project and copy in SpatialSlur.dll along with System.ValueTuple.dll. SpatialSlur types will now be accessible from any C# script in this project.

___
#### References
1. _<http://lgg.epfl.ch/publications/2012/shapeup/paper.pdf>_
2. _<http://lgg.epfl.ch/publications/2015/ShapeOp/ShapeOp_DMSC15.pdf>_
