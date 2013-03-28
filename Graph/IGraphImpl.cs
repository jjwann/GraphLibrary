using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Defines functionality for a graph implementation that is publically available
    /// </summary>
    /// <typeparam name="TVertexId">The type used for identifying vertices</typeparam>
    public interface IGraphImpl<TVertexId>
    {
        /// <summary>
        /// The number of vertices in the graph
        /// </summary>
        int NumVertices { get; }

        /// <summary>
        /// Adds a collection of vertices to the graph
        /// </summary>
        /// <param name="vertices">The collection</param>
        /// <returns>The number of vertices that were actually added</returns>
        int AddVertex(ICollection<TVertexId> vertices);

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="vertex">The vertex</param>
        /// <returns>true if the vertex was successfully added; false otherwise</returns>
        bool AddVertex(TVertexId vertex);

        /// <summary>
        /// Creates a new edge between two vertices
        /// </summary>
        /// <param name="firstVertex">The first vertex</param>
        /// <param name="secondVertex">The second vertex</param>
        /// <param name="weight">The weight of the edge</param>
        /// <returns>true if the edge was created; false otherwise</returns>
        bool CreateEdge(TVertexId firstVertex, TVertexId secondVertex, int weight);

        /// <summary>
        /// Returns information of all the edges that the given vertex is connected to
        /// </summary>
        /// <param name="vertex">The vertex in question</param>
        /// <returns>A list of vertices of which the vertex shares an edge, plus accompanying edge weight information</returns>
        IList<KeyValuePair<TVertexId, int>> GetEdgeInfo(TVertexId vertex);

        /// <summary>
        /// Determines the shortest path from a given vertex to each of the other vertices
        /// </summary>
        /// <param name="startVertex">The vertex in question</param>
        /// <param name="distances">A list of distances of the shortest paths to each vertex, organized by vertex</param>
        /// <param name="comp">Used to compare vertices for sorting</param>
        /// <returns>true if the graph has negative cost cycles; false otherwise</returns>
        bool FindSingleSourceShortestPath(TVertexId startVertex, out SortedList<TVertexId, int?> distances, IComparer<TVertexId> comp = null);

        /// <summary>
        /// Returns the shortest path from each vertex to each vertex
        /// </summary>
        /// <param name="distanceMatrix">For each vertex, this lists the distances of the shortest path to each vertex, organized by vertex</param>
        /// <param name="comp">Used to compare vertices for sorting</param>
        /// <returns>true if the graph has negative cost cycles; false otherwise</returns>
        bool FindAllPairsShortestPath(out SortedList<TVertexId, SortedList<TVertexId, int?>> distanceMatrix, IComparer<TVertexId> comp = null);
    }
}
