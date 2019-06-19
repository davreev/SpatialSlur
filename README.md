# Spatial Slur
Spatial Slur is an open source library of geometric data structures and algorithms intended to facilitate the development of novel computational design methods. It is written in C# and compiled against .NET 4.6.

<p align="center">
  <img src="https://github.com/daveReeves/SpatialSlur/blob/master/Examples/Gallery/170720_Relief_02.gif" alt="Header">
</p>

## Overview
Below is a brief outline of the library by namespace. For further detail, take a look at the [latest reference documentation](http://www.spatialslur.com/documentation/0_3_1/index.html) or just dive into the source files and poke around. If you have any questions, comments, or suggestions, feel free to [get in touch](mailto:darthur.reeves@gmail.com).

- **SpatialSlur**
  - Primitive data types and algorithms for geometry processing in 2 and 3 dimensions
- **SpatialSlur.Collections** 
  - Generic data structures for spatial queries and related algorithms
  - Extension methods for .NET collection types
- **SpatialSlur.Dynamics**
  - Position-based constraint solver and various constraint types for shape exploration and optimization
- **SpatialSlur.Fields**
  - Generic data structures for discrete and continuous field representations (scalar, vector etc.)
  - Algorithms for image/signal processing and visualization
- **SpatialSlur.Meshes** 
  - Generic half-edge data structures for discrete representations of networks, surfaces, and (eventually) volumes
  - Algorithms for mesh/graph processing and visualization
- **SpatialSlur.Tools** 
  - Tools and applications that utilize functionality offered by the rest of library

The following additional namespaces contain classes and convenience methods for interfacing with external APIs.

- **SpatialSlur.Rhino**
- **SpatialSlur.Unity**

## Setup
To get started, download the precompiled binaries from the [latest release](https://github.com/daveReeves/SpatialSlur/releases). If on Windows, the downloaded files may be blocked by default so follow [these instructions](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/) to make sure they're unblocked before referencing them within your own project(s).

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
See below for links to reference documentation by version number.

* [0.2.4](https://spatialslur.gitlab.io/docs/0_2_4)
* [0.2.3](https://spatialslur.gitlab.io/docs/0_2_3)
