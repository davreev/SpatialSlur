# Spatial Slur
Spatial Slur is an open source library of geometric data structures and algorithms intended to facilitate the development of novel computational design methods. It is written in C# and compiled against .NET 4.6.

<p align="center">
  <img src="https://github.com/daveReeves/SpatialSlur/blob/master/Examples/Gallery/170720_Relief_02.gif" alt="Header">
</p>

## Overview
Below is a brief outline of the library by namespace. For further detail, take a look at the [latest reference documentation](http://www.spatialslur.com/documentation/0_3_1/index.html) or just dive into the source files and poke around. If you have any questions, comments, or suggestions, feel free to [get in touch](http://spatialslur.com/contact/).

+ __SpatialSlur__ contains primitive data types and algorithms for geometry processing in 2 and 3 dimensions.

+ __SpatialSlur.Collections__ contains generic data structures for spatial queries and related algorithms. Also contains various general-purpose extension methods for .NET collection types.

+ __SpatialSlur.Dynamics__ contains a projection based constraint solver and various constraint types for geometry optimization and shape exploration. This is based on methods described in [(1)](http://lgg.epfl.ch/publications/2012/shapeup/paper.pdf) and follows many of the implementation details given in [(2)](http://lgg.epfl.ch/publications/2015/ShapeOp/ShapeOp_DMSC15.pdf).

+ __SpatialSlur.Fields__ contains generic data structures for discrete and continuous field representations (scalar, vector, and tensor) along with various algorithms for processing, compositing, and visualization.

+ __SpatialSlur.Meshes__ contains generic half-edge data structures for discrete representations of networks, surfaces, and volumes (coming soon) along with various algorithms for geometry processing, graph processing, segmentation, and subdivision.

+ __SpatialSlur.Tools__ contains more specific tools and applications that utilize functionality offered by the rest of library such as dynamic remeshing and Steiner tree relaxation.

Additionally, the following namespaces contain classes and convenience methods for interfacing with .NET APIs of their respective applications.

+ __SpatialSlur.Rhino__

+ __SpaitalSlur.Unity__

## Setup
To get started, download the precompiled binaries from the [latest release](https://github.com/daveReeves/SpatialSlur/releases). Depending on your operating system, the downloaded files may be blocked by default so follow [these instructions](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/) to make sure they're unblocked before referencing them within your own project(s).

### Grasshopper
To use within Grasshopper, start by moving the contents of the **Binaries > Rhino** folder to your Grasshopper components folder (**File > Special Folders > Components Folder**). 

Once done, restart Rhino, launch Grasshopper, and drop a C#/VB Script component on the canvas. Right click on the component icon/name, and go to **Manage Assemblies**. Add **SpatialSlur.dll** from your Grasshopper components folder to the list of referenced assemblies and click **OK** to confirm. SpatialSlur types will now be accessible from code written within this component.

### Unity
Before using within Unity, the editor must to be configured to target .NET 4.6 (available in Unity 2017.1 or later). To do so, go to **Edit > Project Settings > Player** and expand the **Other Settings** dropdown. Under **Configruation**, set the **Scripting Runtime Version** to **Experimental (.Net 4.6 Equivalent)**.

Once this has been set, import **SpatialSlur.unitypackage** into your current project (**Assets > Import Package > Custom Package...**). SpatialSlur types will now be accessible from any C# script in this project.

## Building From Source
If you're interested in keeping up with the latest developments between releases, you'll need to make a [clone](https://help.github.com/articles/cloning-a-repository/) and compile the binaries on your own machine. The following goes over a few important build-related details to be aware of while doing so.

### IDE
While all projects in the solution currently target .NET 4.6, they do make use of some backwards compatible C# 7 features so you'll likely run into problems with older IDEs. For Windows users, [Visual Studio 2017](https://www.visualstudio.com/downloads/) or later is recommended.

### Configurations
The solution file contains different build configurations for different use cases - each of which has a unique set of dependencies as detailed below.

__Default__ has no dependencies beyond the .NET Framework

__Rhino__ has the following additional dependencies:
* RhinoCommon.dll
* Grasshopper.dll
* GH_IO.dll

__Unity__ has the following additional dependencies:
* UnityEngine.dll
* UnityEditor.dll

## Reference Documentation
See below for links to reference documentation by version number. For earlier versions please refer to the corresponding [release](https://github.com/daveReeves/SpatialSlur/releases).

* [0.3.1](http://spatialslur.com/documentation/0_3_1/index.html)
* [0.2.4](http://spatialslur.com/documentation/0_2_4/index.html)
* [0.2.3](http://spatialslur.com/documentation/0_2_3/index.html)
* [0.2.2](http://spatialslur.com/documentation/0_2_2/index.html)