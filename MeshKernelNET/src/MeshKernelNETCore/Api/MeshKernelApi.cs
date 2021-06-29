﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Helpers;
using MeshKernelNETCore.Native;

namespace MeshKernelNETCore.Api
{
    public sealed class MeshKernelApi : IMeshKernelApi
    {
        /// <inheritdoc />
        public int CreateGridState()
        {
            var meshKernelId = 0;
            MeshKernelDll.CreateGridState(ref meshKernelId);
            return meshKernelId;
        }

        /// <inheritdoc />
        public bool RemoveGridState(int meshKernelId)
        {
            return MeshKernelDll.RemoveGridState(meshKernelId) == 0;
        }

        /// <inheritdoc />
        public bool SetGridState(int meshKernelId, Disposable2DMeshGeometry disposableMeshGeometry, bool isGeographic)
        {
            var meshGeometryDimensions = disposableMeshGeometry.CreateMeshDimensions();
            var meshGeometry = disposableMeshGeometry.CreateMeshGeometry();

            return MeshKernelDll.SetState(meshKernelId, ref meshGeometryDimensions, ref meshGeometry, isGeographic) == 0;
        }

        /// <inheritdoc />
        public Disposable2DMeshGeometry GetGridState(int meshKernelId)
        {
            var newMeshGeometry = new Mesh2DGeometry();
            var newMeshDimensions = new Mesh2DGeometryDimensions();

            MeshKernelDll.GetMeshState(meshKernelId, ref newMeshDimensions, ref newMeshGeometry);

            return CreateDisposableMeshGeometry(newMeshDimensions, newMeshGeometry);
        }

        public Disposable2DMeshGeometry GetGridStateWithCells(int meshKernelId)
        {
            var newMeshGeometry = new Mesh2DGeometry();
            var newMeshDimensions = new Mesh2DGeometryDimensions();

            MeshKernelDll.GetGridStateWithCells(meshKernelId, ref newMeshDimensions, ref newMeshGeometry);

            return CreateDisposableMeshGeometry(newMeshDimensions, newMeshGeometry, true);
        }

        /// <inheritdoc />
        public bool DeleteVertex(int meshKernelId, int vertexIndex)
        {
            return MeshKernelDll.DeleteNode(meshKernelId, vertexIndex) == 0;
        }

        public bool FlipEdges(int meshKernelId, bool isTriangulationRequired, ProjectToLandBoundaryOptions projectToLandBoundaryOption)
        {
            int isTriangulationRequiredInt = isTriangulationRequired ? 1 : 0;
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            return MeshKernelDll.FlipEdges(meshKernelId, isTriangulationRequiredInt, projectToLandBoundaryOptionInt) == 0;
        }

        /// <inheritdoc />
        public bool InsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex)
        {
            return MeshKernelDll.InsertEdge(meshKernelId, startVertexIndex, endVertexIndex, ref edgeIndex) == 0;
        }

        /// <inheritdoc />
        public bool MergeTwoVertices(int meshKernelId, int startVertexIndex, int endVertexIndex)
        {
            return MeshKernelDll.MergeTwoVertices(meshKernelId, startVertexIndex, endVertexIndex) == 0;
        }

        /// <inheritdoc />
        public bool MergeVertices(int meshKernelId, DisposableGeometryList disposableGeometryList)
        {
            var geometryList = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.MergeVertices(meshKernelId, ref geometryList) == 0;
        }

        public bool OrthogonalizationInitialize(int meshKernelId,
            ProjectToLandBoundaryOptions projectToLandBoundaryOption,
            OrthogonalizationParameters orthogonalizationParameters, DisposableGeometryList geometryListNativePolygon,
            DisposableGeometryList geometryListNativeLandBoundaries)
        {
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;

            var nativeOrthogonalizationParameters = orthogonalizationParameters.ToOrthogonalizationParametersNative();

            var geometryListPolygon = geometryListNativePolygon?.CreateGeometryListNative() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = geometryListNativeLandBoundaries?.CreateGeometryListNative() ??
                                             new GeometryListNative { numberOfCoordinates = 0 };

            return MeshKernelDll.OrthogonalizationInitialize(meshKernelId, projectToLandBoundaryOptionInt,
                       ref nativeOrthogonalizationParameters, ref geometryListPolygon, ref geometryListLandBoundaries) == 0;
        }

        public bool OrthogonalizationPrepareOuterIteration(int meshKernelId)
        {
            return MeshKernelDll.OrthogonalizationPrepareOuterIteration(meshKernelId) == 0;
        }

        public bool OrthogonalizationInnerIteration(int meshKernelId)
        {
            return MeshKernelDll.OrthogonalizationInnerIteration(meshKernelId) == 0;
        }

        public bool OrthogonalizationFinalizeOuterIteration(int meshKernelId)
        {
            return MeshKernelDll.OrthogonalizationFinalizeOuterIteration(meshKernelId) == 0;
        }

        public bool OrthogonalizationDelete(int meshKernelId)
        {
            return MeshKernelDll.OrthogonalizationDelete(meshKernelId) == 0;
        }

        public bool MakeGrid(int meshKernelId, MakeGridParameters makeGridParameters, DisposableGeometryList disposableGeometryListIn)
        {
            var makeGridParametersNative =
                makeGridParameters.ToMakeGridParametersNative();
            var geometryListNative = disposableGeometryListIn.CreateGeometryListNative();

            return MeshKernelDll.MakeGrid(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool GetSplines(DisposableGeometryList disposableGeometryListIn, ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices)
        {
            var geometryListIn = disposableGeometryListIn.CreateGeometryListNative();
            var geometryListOut = disposableGeometryListOut.CreateGeometryListNative();
            if (MeshKernelDll.GetSplines(ref geometryListIn, ref geometryListOut,
                     numberOfPointsBetweenVertices) != 0)
            {
                return false;
            }
            disposableGeometryListOut.NumberOfCoordinates = geometryListOut.numberOfCoordinates;
            return true;
        }

        public bool MakeGridFromSplines(int meshKernelId, DisposableGeometryList disposableGeometryListIn,
             CurvilinearParameters curvilinearParameters)
        {
            var geometryListIn = disposableGeometryListIn.CreateGeometryListNative();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.MakeGridFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative) == 0;
        }

        public bool MakeOrthogonalGridFromSplines(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
             ref CurvilinearParameters curvilinearParameters, ref SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNative = disposableGeometryListIn.CreateGeometryListNative();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.MakeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                        ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative) == 0;
        }

        public bool MakeTriangularGridInPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.MakeTriangularGridFromPolygon(meshKernelId, ref geometryListNative) == 0;
        }

        public bool MakeTriangularGridFromSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.MakeTriangularGridFromSamples(meshKernelId, ref geometryListNative) == 0;
        }

        public bool CountMeshBoundaryPolygonVertices(int meshKernelId, ref int numberOfPolygonVertices)
        {
            return MeshKernelDll.CountMeshBoundaryPolygonVertices(meshKernelId, ref numberOfPolygonVertices) == 0;
        }

        public bool GetMeshBoundaryPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.GetMeshBoundaryPolygon(meshKernelId, ref geometryListNative) == 0;
        }

        public bool CountVerticesOffsettedPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            return MeshKernelDll.CountVerticesOffsettedPolygon(meshKernelId, ref geometryListNativeIn, innerPolygon, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool GetOffsettedPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            var geometryListNativeOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.GetOffsettedPolygon(meshKernelId, ref geometryListNativeIn, innerPolygon, distance, ref geometryListNativeOut) == 0;
        }

        public bool CountVerticesRefinededPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListInNative = disposableGeometryListIn.CreateGeometryListNative();
            return MeshKernelDll.CountVerticesRefinededPolygon(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool GetRefinededPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex,
             int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            var geometryListNativeOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.GetRefinededPolygon(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut) == 0;
        }

        public bool RefineGridBasedOnSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, InterpolationParameters interpolationParameters, SamplesRefineParameters samplesRefineParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateGeometryListNative();
            var interpolationParametersNative = interpolationParameters.ToInterpolationParametersNative();
            var samplesRefineParametersNative = samplesRefineParameters.ToSampleRefineParametersNative();
            return MeshKernelDll.RefineGridBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, ref interpolationParametersNative, ref samplesRefineParametersNative) == 0;
        }

        public bool RefineGridBasedOnPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, InterpolationParameters interpolationParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateGeometryListNative();
            var interpolationParametersNative = interpolationParameters.ToInterpolationParametersNative();
            return MeshKernelDll.RefineGridBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref interpolationParametersNative) == 0;
        }

        public int[] GetSelectedVertices(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int inside)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            int numberOfMeshVertices = -1;
            MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices);
            IntPtr selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
            MeshKernelDll.GetSelectedVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref selectedVerticesPtr);
            int[] selectedVertices = new int[numberOfMeshVertices];
            Marshal.Copy(selectedVerticesPtr, selectedVertices, 0, numberOfMeshVertices);
            return selectedVertices;
        }

        public bool GetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.GetOrthogonality(meshKernelId, ref geometryListNativeInOut) == 0;
        }

        public bool GetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.GetSmoothness(meshKernelId, ref geometryListNativeInOut) == 0;
        }

        public bool InsertVertex(int meshKernelId, double xCoordinate, double yCoordinate, ref int vertexIndex)
        {
            return MeshKernelDll.InsertVertex(meshKernelId, xCoordinate, yCoordinate, ref vertexIndex) == 0;
        }

        public bool GetVertexIndex(int meshKernelId, DisposableGeometryList geometryListIn,
            double searchRadius, ref int vertexIndex)
        {
            var geometryListNativeIn = geometryListIn.CreateGeometryListNative();
            return MeshKernelDll.GetVertexIndex(meshKernelId, ref geometryListNativeIn, searchRadius, ref vertexIndex) == 0;
        }

        public bool GetVertexCoordinates(int meshKernelId, DisposableGeometryList geometryListIn, double searchRadius,
             ref DisposableGeometryList geometryListOut)
        {
            var geometryListNativeIn = geometryListIn.CreateGeometryListNative();
            var geometryListNativeOut = geometryListOut.CreateGeometryListNative();
            return MeshKernelDll.GetVertexCoordinate(meshKernelId, ref geometryListNativeIn, searchRadius, ref geometryListNativeOut) == 0;
        }

        public bool DeleteEdge(int meshKernelId, ref DisposableGeometryList geometryListIn)
        {
            var geometryListNativeIn = geometryListIn.CreateGeometryListNative();
            return MeshKernelDll.DeleteEdge(meshKernelId, ref geometryListNativeIn) == 0;
        }

        public bool FindEdge(int meshKernelId, ref DisposableGeometryList geometryListIn, ref int edgeIndex)
        {
            var geometryListNativeIn = geometryListIn.CreateGeometryListNative();
            return MeshKernelDll.FindEdge(meshKernelId, ref geometryListNativeIn, ref edgeIndex) == 0;
        }

        public bool DeleteMeshWithOptions(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut, int deletionOption, bool invertDeletion)
        {
            var geometryListNativeIn = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.DeleteMeshWithOptions(meshKernelId, ref geometryListNativeIn, deletionOption, invertDeletion) == 0;
        }

        public bool MoveVertex(int meshKernelId, ref DisposableGeometryList disposableGeometryLisIn, int vertexIndex)
        {
            var geometryListNativeIn = disposableGeometryLisIn.CreateGeometryListNative();
            return MeshKernelDll.MoveVertex(meshKernelId, ref geometryListNativeIn, vertexIndex) == 0;
        }

        public bool PointsInPolygon(int meshKernelId, ref DisposableGeometryList inputPolygon, ref DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            var geometryListNativeInputPolygon = inputPolygon.CreateGeometryListNative();
            var geometryListNativeinputPoints = inputPoints.CreateGeometryListNative();
            var geometryListNativeSelectedPoints = selectedPoints.CreateGeometryListNative();
            return MeshKernelDll.PointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints) == 0;
        }

        public bool MakeCurvilinearGridFromPolygon(int meshKernelId, DisposableGeometryList geometryList, int firstNode,
            int secondNode, int thirdNode, bool useFourthSide)
        {

            var geometryListNative = geometryList.CreateGeometryListNative();
            return MeshKernelDll.MakeCurvilinearGridFromPolygon(meshKernelId,
                                                                 ref geometryListNative,
                                                                 firstNode,
                                                                 secondNode,
                                                                 thirdNode,
                                                                 useFourthSide) == 0;
        }

        public bool MakeCurvilinearGridFromTriangle(int meshKernelId, DisposableGeometryList geometryList, int firstNode,
            int secondNode, int thirdNode)
        {
            var geometryListNative = geometryList.CreateGeometryListNative();
            return MeshKernelDll.MakeCurvilinearGridFromTriangle(meshKernelId,
                                                                  ref geometryListNative,
                                                                  firstNode,
                                                                  secondNode,
                                                                  thirdNode) == 0;
        }

        public double GetSeparator()
        {
            // To activate after update of the MeshKernel nuget
            //return MeshKernelDll.GetSeparator();
            return -999.0;
        }

        public double GetInnerOuterSeparator()
        {
            // To activate after update of the MeshKernel nuget
            //return MeshKernelDll.GetInnerOuterSeparator();
            return -998.0;
        }

        public void Dispose()
        {
            // Do nothing because no remoting is used
        }

        private Disposable2DMeshGeometry CreateDisposableMeshGeometry(Mesh2DGeometryDimensions newMeshDimensions, Mesh2DGeometry newMeshGeometry, bool addCellInformation = false)
        {
            var disposableMeshGeometry = new Disposable2DMeshGeometry
            {
                Name = new string(newMeshDimensions.name),
                NodesX = newMeshGeometry.nodex.CreateValueArray<double>(newMeshDimensions.numnode),
                NodesY = newMeshGeometry.nodey.CreateValueArray<double>(newMeshDimensions.numnode),
                NodesZ = newMeshGeometry.nodez.CreateValueArray<double>(newMeshDimensions.numnode),
                EdgeNodes = newMeshGeometry.edge_nodes.CreateValueArray<int>(newMeshDimensions.numedge * 2).ToArray()
            };

            if (addCellInformation && newMeshDimensions.numface > 0)
            {
                disposableMeshGeometry.MaxNumberOfFaceNodes = newMeshDimensions.maxnumfacenodes;
                disposableMeshGeometry.FaceNodes = newMeshGeometry.face_nodes.CreateValueArray<int>(newMeshDimensions.numface * newMeshDimensions.maxnumfacenodes);
                disposableMeshGeometry.FaceX = newMeshGeometry.facex.CreateValueArray<double>(newMeshDimensions.numface);
                disposableMeshGeometry.FaceY = newMeshGeometry.facey.CreateValueArray<double>(newMeshDimensions.numface);
            }

            return disposableMeshGeometry;
        }
    }
}