﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Helpers;

namespace MeshKernelNETCore.Native
{
    // never hit by code coverage because tests use remoting and this only contains dll imports
    [ExcludeFromCodeCoverage]
    internal static class MeshKernelDll
    {
        private const string MeshKernelDllName = "MeshKernelApi.dll";

        static MeshKernelDll()
        {
            var dir = Path.GetDirectoryName(typeof(MeshKernelDll).Assembly.Location);
            NativeLibrary.LoadNativeDll(MeshKernelDllName, Path.Combine(dir, "Lib"));
        }

        #region State management

        /// <summary>
        /// Create a new mesh state and return the generated <param name="meshKernelId"/>
        /// </summary>
        /// <param name="meshKernelId">Identifier for the created grid state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_allocate_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CreateGridState([In, Out] ref int meshKernelId);

        /// <summary>
        /// Deallocate mesh state (collections of mesh arrays with auxiliary variables)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_deallocate_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RemoveGridState([In] int meshKernelId);

        /// <summary>
        /// Gets the mesh state as a <see cref="Mesh2DGeometry"/> structure
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="meshGeometryDimensions">Mesh dimensions</param>
        /// <param name="meshGeometry">Grid data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetMeshState([In] int meshKernelId, [In, Out] ref Mesh2DGeometryDimensions meshGeometryDimensions, [In, Out] ref Mesh2DGeometry meshGeometry);

        /// <summary>
        /// Gets the mesh state as a <see cref="Mesh2DGeometry"/> structure including faces information
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="meshGeometryDimensions">Grid dimensions</param>
        /// <param name="meshGeometry">Grid data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_faces_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetGridStateWithCells([In] int meshKernelId, [In, Out] ref Mesh2DGeometryDimensions meshGeometryDimensions, [In, Out] ref Mesh2DGeometry meshGeometry);

        /// <summary>
        /// Synchronize provided mesh (<param name="meshGeometryDimensions"/> and <param name="meshGeometry"/>) with the mesh state with <param name="meshKernelId"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="meshGeometryDimensions">Mesh dimensions</param>
        /// <param name="meshGeometry">Mesh data</param>
        /// <param name="IsGeographic">Cartesian or spherical mesh</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_set_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetState([In] int meshKernelId, [In] ref Mesh2DGeometryDimensions meshGeometryDimensions, [In] ref Mesh2DGeometry meshGeometry, [In] bool IsGeographic);

        #endregion

        #region Node operations

        /// <summary>
        /// Deletes a node with specified <param name="nodeIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="nodeIndex">The nodeIndex to delete</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_delete_node_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DeleteNode([In] int meshKernelId, [In] int nodeIndex);

        #endregion

        #region Links operations  

        /// <summary>
        /// Flip the edges
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="isTriangulationRequired">The option to triangulate also non triangular cells (if activated squares becomes triangles) </param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_flip_edges_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FlipEdges([In] int meshKernelId, [In] int isTriangulationRequired, [In] int projectToLandBoundaryOption);

        /// <summary>
        /// Insert a new edge connecting <param name="startVertexIndex"/> and <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="startVertexIndex">The index of the first vertex to connect</param>
        /// <param name="endVertexIndex">The index of the second vertex to connect</param>
        /// <param name="newEdgeIndex">The index of the new edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_insert_edge_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int InsertEdge([In] int meshKernelId, [In] int startVertexIndex, [In] int endVertexIndex, [In, Out] ref int edgeIndex);
        #endregion

        #region Vertices operations  
        /// <summary>
        /// Merges vertex <param name="startVertexIndex"/> to <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="startVertexIndex">The index of the first vertex to merge</param>
        /// <param name="endVertexIndex">The index of the second vertex to merge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_merge_two_nodes_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MergeTwoVertices([In] int meshKernelId, [In] int startVertexIndex, [In] int endVertexIndex);

        /// <summary>
        /// Merges vertices within a distance of 0.001 m, effectively removing small edges 
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_merge_nodes_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MergeVertices([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Inserts a new mesh node
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <param name="vertexIndex">The index of the new mesh node</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_insert_node_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InsertVertex([In] int meshKernelId, [In] double xCoordinate, [In] double yCoordinate, [In, Out] ref int vertexIndex);


        /// <summary>
        /// Deletes a mesh in a polygon using several options
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <param name="deletionOption">The deletion option (to be detailed)</param>
        /// <param name="invertDeletion">Inverts the deletion of selected features</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_delete_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DeleteMeshWithOptions([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int deletionOption, [In] bool invertDeletion);

        /// <summary>
        /// Get the index of the closest vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input point coordinates</param>
        /// <param name="searchRadius">the radius where to search for the vertex</param>
        /// <param name="vertexIndex">the index of the closest vertex</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_node_index_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVertexIndex([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] double searchRadius, [In, Out] ref int vertexIndex);

        /// <summary>
        /// Get the coordinate of the closest vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input point coordinates</param>
        /// <param name="searchRadius">the radius where to search for the vertex</param>
        /// <param name="geometryListOut">the coordinate of the closest vertex</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_closest_node_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVertexCoordinate([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] double searchRadius, [In, Out] ref GeometryListNative geometryListOut);

        #endregion

        #region Orthogonalization
        /// <summary>
        /// Orthogonalization
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParametersNative">The structure containing the orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_compute_orthogonalization_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Orthogonalization([In] int meshKernelId, [In] int projectToLandBoundaryOption, [In] ref OrthogonalizationParametersNative orthogonalizationParametersNative, [In] ref GeometryListNative geometryListNativePolygon, [In] ref GeometryListNative geometryListNativeLandBoundaries);

        /// <summary>
        /// Orthogonalization initialization (first function to use in interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParameters">The structure containing the user defined orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_initialize_orthogonalization_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int OrthogonalizationInitialize([In] int meshKernelId, [In] int projectToLandBoundaryOption, [In] ref OrthogonalizationParametersNative orthogonalizationParametersNative, [In] ref GeometryListNative geometryListNativePolygon, [In] ref GeometryListNative geometryListNativeLandBoundaries);

        /// <summary>
        /// Prepare outer orthogonalization iteration (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_prepare_outer_iteration_orthogonalization_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int OrthogonalizationPrepareOuterIteration([In] int meshKernelId);

        /// <summary>
        /// Perform inner orthogonalization iteration (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_compute_inner_ortogonalization_iteration_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int OrthogonalizationInnerIteration([In] int meshKernelId);

        /// <summary>
        /// Perform outer orthogonalization iteration  (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_finalize_inner_ortogonalization_iteration_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int OrthogonalizationFinalizeOuterIteration([In] int meshKernelId);

        /// <summary>
        /// Clean up back-end orthogonalization algorithm  (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_delete_orthogonalization_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int OrthogonalizationDelete([In] int meshKernelId);

        #endregion

        #region Make grid
        /// <summary>
        /// Make a new mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters </param>
        /// <param name="geometryListNative">The polygon to account for</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_make_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeGrid([In] int meshKernelId, [In] ref MakeGridParametersNative makeGridParameters, [In] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Make a triangular grid in a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The polygon where to triangulate</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_make_mesh_from_polygon_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeTriangularGridFromPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);


        /// <summary>
        /// Make a triangular mesh from samples
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The samples where to triangulate</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_make_mesh_from_samples_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeTriangularGridFromSamples([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);

        #endregion

        #region Curvilinear grids

        /// <summary>
        /// Get spline intermediate points 
        /// </summary>
        /// <param name="disposableGeometryListIn">The input corner vertices of the splines</param>
        /// <param name="disposableGeometryListOut">The output spline </param>
        /// <param name="numberOfPointsBetweenVertices">The number of spline vertices between the corners points</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSplines([In] ref GeometryListNative geometryListNativeIn, [In, Out] ref GeometryListNative geometryListNativeOut, [In] int numberOfPointsBetweenVertices);

        /// <summary>
        /// Generates curvilinear grid from splines with transfinite interpolation
        /// </summary>
        /// <param name="meshKernelId"></param>
        /// <param name="geometryListNativeIn"></param>
        /// <param name="curvilinearParametersNative"></param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_compute_transfinite_curvilinear_from_splines_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeGridFromSplines([In] int meshKernelId, [In] ref GeometryListNative geometryListNativeIn, [In] ref CurvilinearParametersNative curvilinearParametersNative);

        /// <summary>
        /// Make curvilinear grid from splines with an advancing front.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input splines corners</param>
        /// <param name="curvilinearParametersNative">The input parameters to generate the curvilinear grid</param> 
        /// <param name="splinesToCurvilinearParametersNative">The parameters of the advancing front algorithm</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_compute_orthogonal_curvilinear_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeOrthogonalGridFromSplines([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] ref CurvilinearParametersNative curvilinearParametersNative, [In] ref SplinesToCurvilinearParametersNative splinesToCurvilinearParametersNative);

        /// <summary>
        /// Computes a curvilinear mesh in a polygon. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">>Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">The third node<</param>
        /// <param name="useFourthSide">Use (true/false) the fourth polygon side to compute the curvilinear grid</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_compute_transfinite_curvilinear_from_polygon_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeCurvilinearGridFromPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int firstNode, [In] int secondNode, [In] int thirdNode, [In] bool useFourthSide);

        /// <summary>
        /// Computes a curvilinear mesh in a triangle. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">>Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">The third node<</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_compute_transfinite_curvilinear_from_triangle_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MakeCurvilinearGridFromTriangle([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int firstNode, [In] int secondNode, [In] int thirdNode);

        #endregion

        #region Mesh operations
        /// <summary>
        /// Counts the number of polygon vertices contained in the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="numberOfPolygonVertices">The number of polygon points</param>
        /// <returns>Error code</returns>          
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_count_mesh_boundaries_to_polygon_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CountMeshBoundaryPolygonVertices([In] int meshKernelId, [In, Out] ref int numberOfPolygonVertices);


        /// <summary>
        /// Retrives the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryList">The output network boundary polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_mesh_boundaries_to_polygon_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetMeshBoundaryPolygon([In] int meshKernelId, [In, Out] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Refine a grid based on the samples contained in the geometry list
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The sample set</param>
        /// <param name="interpolationParametersNative">The interpolation parameters</param>
        /// <param name="sampleRefineParametersNative">The interpolation settings related to the samples</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_refine_based_on_samples_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RefineGridBasedOnSamples([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] ref InterpolationParametersNative interpolationParametersNative, [In] ref SampleRefineParametersNative sampleRefineParametersNative);

        /// <summary>
        /// Refine a grid based on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The closed polygon where to perform the refinement</param>
        /// <param name="interpolationParametersNative">The interpolation parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_refine_based_on_polygon_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RefineGridBasedOnPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] ref InterpolationParametersNative interpolationParametersNative);

        #endregion

        #region Polygon operations

        /// <summary>
        /// Get the number of vertices of the offsetted polygon 
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The coordinate of the offset point</param>
        /// <param name="innerPolygon">Compute inner/outer polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices of the generated polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_count_offsetted_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CountVerticesOffsettedPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] bool innerPolygon, [In] double distance, [In, Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Get the offsetted polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The coordinate of the offset point</param>
        /// <param name="innerPolygon">Compute inner/outer polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="geometryListOut">The offsetted polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_offsetted_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetOffsettedPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] bool innerPolygon, [In] double distance, [In, Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Count the number of vertices after polygon refinment
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices after refinement </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_count_refine_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CountVerticesRefinededPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In] double distance, [In, Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Gets the refined polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="geometryListOut"></param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_refine_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetRefinededPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In] double distance, [In, Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Get the double value used in the back-end library as separator and missing value
        /// </summary>
        /// <returns>The double missing value</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_separator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetSeparator();

        /// <summary>
        /// Get the double value used in the back-end library to separate the inner part of a polygon from its outer part
        /// </summary>
        /// <returns>The double missing value</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_inner_outer_separator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetInnerOuterSeparator();

        #endregion

        /// <summary>
        /// Counts the number of selected mesh node indexes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="numberOfMeshVertices">The number of selected nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_count_nodes_in_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CountVerticesInPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int inside, [In, Out] ref int numberOfMeshVertices);

        /// <summary>
        /// Gets the selected mesh node indexes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="selectedVerticesPtr">The selected vertices nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_nodes_in_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSelectedVerticesInPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int inside, [In, Out] ref IntPtr selectedVerticesPtr);

        /// <summary>
        /// Gets the orthogonality
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The orthogonality values of each edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_orthogonality_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetOrthogonality([In] int meshKernelId, [In, Out] ref GeometryListNative geometryListIn);

        /// <summary>
        /// Gets the smoothness 
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The smoothness values of each edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_smoothness_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSmoothness([In] int meshKernelId, [In, Out] ref GeometryListNative geometryListIn);

        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input point coordinates</param>
        /// <param name="searchRadius">The search radius</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_delete_edge_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DeleteEdge([In] int meshKernelId, [In] ref GeometryListNative geometryListIn);


        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input point coordinates</param>
        /// <param name="searchRadius">The search radius</param>
        /// <param name="edgeIndex">The edge index</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_find_edge_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FindEdge([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In, Out] ref int edgeIndex);

        /// <summary>
        /// Function to move a selected node to a new position
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The new coordinate</param>
        /// <param name="vertexIndex">The node index (to be detailed)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_move_node_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int MoveVertex([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int vertexIndex);


        /// <summary>
        /// Selects points in polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="inputPolygon">The polygon(s) used for selection</param>
        /// <param name="inputPoints">The points to select</param>
        /// <param name="selectedPoints">The selected points in the zCoordinates field (0.0 not selected, 1.0 selected)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_points_in_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PointsInPolygon([In] int meshKernelId, [In] ref GeometryListNative inputPolygon, [In] ref GeometryListNative inputPoints, [In, Out] ref GeometryListNative selectedPoints);

    }
}