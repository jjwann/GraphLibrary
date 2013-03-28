using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Defines a graph implementation that is both publicly available and usable by algorithms
    /// </summary>
    /// <typeparam name="TVertexId">The type used for identifying vertices</typeparam>
    internal abstract class GraphImpl<TVertexId> : IGraphImplForAlgorithms<TVertexId>, IGraphImpl<TVertexId>
    {
        /// <summary>
        /// Maps vertex ids with the corresponding index in the actual graph
        /// </summary>
        protected Dictionary<TVertexId, int> indexLookup = new Dictionary<TVertexId, int>();

        public abstract int NumVertices { get; }

        public abstract IGraphImplForAlgorithms<TVertexId> MakeDeepCopy();
        public abstract int CreateAugmentedGraph(ICollection<KeyValuePair<int, int>> destinations, out IGraphImplForAlgorithms<TVertexId> newGraph);

        public abstract void ReweightGraph(IList<int> weights);
        public abstract ICollection<KeyValuePair<int, int>> GetEdgeInfoByIndex(int vertex);

        /// <summary>
        /// Adds a collection of vertices to the graph
        /// </summary>
        /// <param name="vertices">The collection</param>
        /// <returns>A list of key/value pairs; each pair relates a vertex id to the corresponding index in the actual graph</returns>
        protected abstract List<KeyValuePair<TVertexId, int>> AddVertexImpl(ICollection<TVertexId> vertices);

        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="vertex">The vertex</param>
        /// <returns>The index of the vertex in the actual graph</returns>
        protected abstract int AddVertexImpl(TVertexId vertex);

        /// <summary>
        /// Returns information of all the edges that the given vertex is connected to
        /// </summary>
        /// <param name="vertex">The index of the vertex in question</param>
        /// <returns>A list of vertices of which the vertex shares an edge, plus accompanying edge weight information</returns>
        protected abstract IList<KeyValuePair<TVertexId, int>> GetEdgeInfoImpl(int vertex);

        /// <summary>
        /// Creates a new edge between two vertices
        /// </summary>
        /// <param name="firstVertex">The index of the first vertex</param>
        /// <param name="secondVertex">The index of the second vertex</param>
        /// <param name="weight">The weight of the edge</param>
        /// <returns>true if the edge was created; false otherwise</returns>
        protected abstract bool CreateEdgeImpl(int firstVertex, int secondVertex, int weight);

        /// <summary>
        /// Gets the vertex id of the vertex represented by the given index in the actual graph
        /// </summary>
        /// <param name="index">The index in the graph</param>
        /// <param name="vertexId">The vertex ID</param>
        /// <returns>true if vertex ID is found; false otherwise</returns>
        protected abstract bool GetVertexId(int index, out TVertexId vertexId);

        /// <summary>
        /// Invoked the find the shortest path to each vertex from a given vertex
        /// </summary>
        /// <param name="grph">The graph on which to perform the algorithm</param>
        /// <param name="startIndex">The index of the vertex from which to determine the shortest paths</param>
        /// <param name="distances">A list of distances of the shortest paths to each vertex, organized by vertex</param>
        /// <returns>true if the graph has negative cost cycles; false otherwise</returns>
        protected delegate bool shortestPathDelegate(IGraphImplForAlgorithms<TVertexId> grph, int startIndex, out int?[] distances);
        protected shortestPathDelegate shortestPathFunc;

        protected GraphImpl()
        {
            shortestPathFunc = GraphShortestDistanceAlgorithms.Dijkstra;
        }

        public int AddVertex(ICollection<TVertexId> vertices)
        {
            int numCreated = 0;

            List<TVertexId> verticesToAdd = new List<TVertexId>(vertices.Count);
            foreach (TVertexId vertex in vertices)
            {
                if (!indexLookup.ContainsKey(vertex))
                {
                    verticesToAdd.Add(vertex);
                }
            }

            List<KeyValuePair<TVertexId, int>> addedVertexList = AddVertexImpl(verticesToAdd);
            if (addedVertexList != null)
            {
                foreach (KeyValuePair<TVertexId, int> vertex in addedVertexList)
                {
                    indexLookup.Add(vertex.Key, vertex.Value);
                    numCreated++;
                }
            }

            return numCreated;
        }

        public bool AddVertex(TVertexId vertex)
        {
            bool isSuccess = false;

            if (!indexLookup.ContainsKey(vertex))
            {
                int index = AddVertexImpl(vertex);
                if (index > -1)
                {
                    indexLookup.Add(vertex, index);
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public bool CreateEdge(TVertexId firstVertex, TVertexId secondVertex, int weight)
        {
            bool isSuccess = false;

            if (indexLookup.ContainsKey(firstVertex) && indexLookup.ContainsKey(secondVertex))
            {
                if (weight < 0)
                {
                    // Bellman-Ford should be used for graphs with negative edge weights
                    shortestPathFunc = GraphShortestDistanceAlgorithms.BellmanFord;
                }

                isSuccess = CreateEdgeImpl(indexLookup[firstVertex], indexLookup[secondVertex], weight);
            }

            return isSuccess;
        }

        public IList<KeyValuePair<TVertexId, int>> GetEdgeInfo(TVertexId vertex)
        {
            int index;
            return indexLookup.TryGetValue(vertex, out index) ? GetEdgeInfoImpl(index) : null;
        }

        public bool FindSingleSourceShortestPath(TVertexId startVertex, out SortedList<TVertexId, int?> distances, IComparer<TVertexId> comp = null)
        {
            if (!indexLookup.ContainsKey(startVertex))
            {
                throw new KeyNotFoundException("Vertex not found in this graph: " + startVertex.ToString());
            }

            int?[] distancesByIndex;
            bool hasCycles = shortestPathFunc(this, indexLookup[startVertex], out distancesByIndex);

            distances = new SortedList<TVertexId, int?>(distancesByIndex.Length, comp);
            TVertexId vertexId;

            for (int i = 0; i < distancesByIndex.Length; i++)
            {
                if (GetVertexId(i, out vertexId))
                {
                    distances.Add(vertexId, distancesByIndex[i]);
                }
            }

            return hasCycles;
        }

        public bool FindAllPairsShortestPath(out SortedList<TVertexId, SortedList<TVertexId, int?>> distanceMatrix, IComparer<TVertexId> comp = null)
        {
            distanceMatrix = null;

            int?[][] rawDistanceMatrix;
            bool hasCycles = GraphShortestDistanceAlgorithms.Johnson(this, out rawDistanceMatrix);

            if (!hasCycles)
            {
                distanceMatrix = new SortedList<TVertexId, SortedList<TVertexId, int?>>(rawDistanceMatrix.Length, comp);

                int?[] distanceList;
                TVertexId node, target;
                SortedList<TVertexId, int?> listToAdd;

                for (int i = 0; i < rawDistanceMatrix.Length; i++)
                {
                    distanceList = rawDistanceMatrix[i];

                    GetVertexId(i, out node);
                    listToAdd = new SortedList<TVertexId, int?>(distanceList.Length, comp);

                    for (int j = 0; j < distanceList.Length; j++)
                    {
                        GetVertexId(j, out target);
                        listToAdd.Add(target, distanceList[j]);
                    }

                    distanceMatrix.Add(node, listToAdd);
                }
            }

            return hasCycles;
        }
    }
}
