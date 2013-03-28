using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Defines functionality for a graph implementation that is accessible by implementations of graph algorithms
    /// </summary>
    /// <typeparam name="TVertexId">The type used for identifying vertices</typeparam>
    internal interface IGraphImplForAlgorithms<TVertexId>
    {
        /// <summary>
        /// The number of vertices in the graph
        /// </summary>
        int NumVertices { get; }

        /// <summary>
        /// Creates a deep copy of the graph
        /// </summary>
        /// <returns>The copy</returns>
        IGraphImplForAlgorithms<TVertexId> MakeDeepCopy();

        /// <summary>
        /// Creates a deep copy of the graph and adds a vertex to the graph and connects vertex to specified vertices with edges
        /// </summary>
        /// <param name="destinations">A collection of vertices to connect to, along with accompanying edge weights</param>
        /// <param name="newGraph">The new graph with the extra vertex and edges</param>
        /// <returns>The index of the new vertex</returns>
        /// <remarks>This method is meant to be used for Johnson's all pairs shortest path algorithm</remarks>
        int CreateAugmentedGraph(ICollection<KeyValuePair<int, int>> destinations, out IGraphImplForAlgorithms<TVertexId> newGraph);

        /// <summary>
        /// Reweight the graph according to Johnson's shortest path algorithm
        /// </summary>
        /// <param name="weights">The weights corresponding to vertex</param>
        void ReweightGraph(IList<int> weights);

        /// <summary>
        /// Returns information of all the edges that the given vertex is connected to
        /// </summary>
        /// <param name="vertex">The index of the vertex in question</param>
        /// <returns>A list of indices for each vertex of which the given vertex shares an edge, plus accompanying edge weight information</returns>
        ICollection<KeyValuePair<int, int>> GetEdgeInfoByIndex(int vertex);
    }
}
