using System;
using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Provides access to an underlying graph implementation
    /// </summary>
    /// <typeparam name="TVertexId">The type used for identifying vertices</typeparam>
    public abstract class Graph<TVertexId>
    {
        /// <summary>
        /// The underlying graph implementation
        /// </summary>
        protected IGraphImpl<TVertexId> innerGraph;

        /// <summary>
        /// Invoked to return a new instance of a graph
        /// </summary>
        /// <returns>The new instance</returns>
        protected abstract IGraphImpl<TVertexId> CreateInnerGraph();

        protected Graph()
        {
            innerGraph = CreateInnerGraph();
        }

        /// <summary>
        /// The number of vertices in the graph
        /// </summary>
        public int NumVertices { get { return innerGraph == null ? 0 : innerGraph.NumVertices; } }

        /// <summary>
        /// Adds a collection of vertices to the graph
        /// </summary>
        /// <param name="vertices">The collection</param>
        /// <returns>The number of vertices that were actually added</returns>
        public int AddVertex(ICollection<TVertexId> vertices)
        {
            int numCreated = 0;

            if (innerGraph != null)
            {
                numCreated = innerGraph.AddVertex(vertices);
            }

            return numCreated;
        }

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="vertex">The vertex</param>
        /// <returns>true if the vertex was successfully added; false otherwise</returns>
        public bool AddVertex(TVertexId vertex)
        {
            bool isSuccess = false;

            if (innerGraph != null)
            {
                isSuccess = innerGraph.AddVertex(vertex);
            }

            return isSuccess;
        }

        /// <summary>
        /// Creates a new edge between two vertices
        /// </summary>
        /// <param name="firstVertex">The first vertex</param>
        /// <param name="secondVertex">The second vertex</param>
        /// <param name="weight">The weight of the edge</param>
        /// <returns>true if the edge was created; false otherwise</returns>
        public bool CreateEdge(TVertexId firstVertex, TVertexId secondVertex, int weight)
        {
            return innerGraph != null ? innerGraph.CreateEdge(firstVertex, secondVertex, weight) : false;
        }

        /// <summary>
        /// Returns information of all the edges that the given vertex is connected to
        /// </summary>
        /// <param name="vertex">The vertex in question</param>
        /// <returns>A list of vertices of which the vertex shares an edge, plus accompanying edge weight information</returns>
        public IList<KeyValuePair<TVertexId, int>> GetEdgeInfo(TVertexId vertex)
        {
            return innerGraph != null ? innerGraph.GetEdgeInfo(vertex) : null;
        }

        /// <summary>
        /// Determines the shortest path from a given vertex to each of the other vertices
        /// </summary>
        /// <param name="startVertex">The vertex in question</param>
        /// <param name="distances">A list of distances of the shortest paths to each vertex, organized by vertex</param>
        /// <param name="comp">Used to compare vertices for sorting</param>
        /// <returns>true if the graph has negative cost cycles; false otherwise</returns>
        public bool FindSingleSourceShortestPath(TVertexId startVertex, out SortedList<TVertexId, int?> distances, IComparer<TVertexId> comp = null)
        {
            if (innerGraph == null)
            {
                throw new NullReferenceException("There is no internal implementation of a graph.");
            }

            return innerGraph.FindSingleSourceShortestPath(startVertex, out distances, comp);
        }

        /// <summary>
        /// Returns the shortest path from each vertex to each vertex
        /// </summary>
        /// <param name="distanceMatrix">For each vertex, this lists the distances of the shortest path to each vertex, organized by vertex</param>
        /// <param name="comp">Used to compare vertices for sorting</param>
        /// <returns>true if the graph has negative cost cycles; false otherwise</returns>
        public bool FindAllPairsShortestPath(out SortedList<TVertexId, SortedList<TVertexId, int?>> distanceMatrix, IComparer<TVertexId> comp = null)
        {
            if (innerGraph == null)
            {
                throw new NullReferenceException("There is no internal implementation of a graph.");
            }

            return innerGraph.FindAllPairsShortestPath(out distanceMatrix, comp);
        }
    }
}
