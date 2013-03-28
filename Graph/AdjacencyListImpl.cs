using System.Collections.Generic;
using System.Linq;

namespace GraphLibrary
{
    /// <summary>
    /// Implementation of functionality for an adjacency list. 
    /// </summary>
    /// <typeparam name="TVertexId">The type used for identifying vertices</typeparam>
    /// <remarks>For now, the adjacency list only represents a directed graph</remarks>
    internal class AdjacencyListImpl<TVertexId> : GraphImpl<TVertexId>
    {
        /// <summary>
        /// The actual adjacency list
        /// </summary>
        private List<KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>> adjList;

        public override int NumVertices { get { return adjList.Count; } }

        internal AdjacencyListImpl()
            : base()
        {
            adjList = new List<KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>>();
        }

        internal AdjacencyListImpl(ICollection<TVertexId> vertices)
            : base()
        {
            adjList = new List<KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>>(vertices.Count);

            AddVertex(vertices);
        }

        internal AdjacencyListImpl(TVertexId vertex)
            : base()
        {
            adjList = new List<KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>>(1);

            AddVertexImpl(vertex);
        }

        public override IGraphImplForAlgorithms<TVertexId> MakeDeepCopy()
        {
            AdjacencyListImpl<TVertexId> newGraph = new AdjacencyListImpl<TVertexId>();
            newGraph.adjList = adjList.Select(x => new KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>(x.Key, new List<KeyValuePair<int, int>>(x.Value))).ToList();

            return newGraph;
        }

        public override int CreateAugmentedGraph(ICollection<KeyValuePair<int, int>> destinations, out IGraphImplForAlgorithms<TVertexId> newGraph)
        {
            newGraph = MakeDeepCopy();
            AdjacencyListImpl<TVertexId> augmentGraph = (AdjacencyListImpl<TVertexId>)newGraph;

            augmentGraph.adjList.Add(new KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>(default(TVertexId), new List<KeyValuePair<int, int>>()));
            int lastIndex = augmentGraph.NumVertices - 1;

            foreach (KeyValuePair<int, int> dest in destinations)
            {
                if (dest.Key < NumVertices)
                {
                    augmentGraph.InsertEdgeNode(augmentGraph.adjList[lastIndex].Value, dest.Key, dest.Value);
                }
            }

            return lastIndex;
        }

        public override void ReweightGraph(IList<int> weights)
        {
            if (weights.Count >= NumVertices)
            {
                List<KeyValuePair<int, int>> edgeList;

                for (int i = 0; i < NumVertices; i++)
                {
                    edgeList = adjList[i].Value;

                    for (int j = 0; j < edgeList.Count; j++)
                    {
                        edgeList[j] = new KeyValuePair<int, int>(edgeList[j].Key, (int)(edgeList[j].Value + weights[i] - weights[edgeList[j].Key]));
                    }
                }
            }
        }

        protected override List<KeyValuePair<TVertexId, int>> AddVertexImpl(ICollection<TVertexId> vertices)
        {
            List<KeyValuePair<TVertexId, int>> newVertices = new List<KeyValuePair<TVertexId, int>>();
            int index;

            foreach (TVertexId vertex in vertices)
            {
                index = AddVertexImpl(vertex);
                if (index > -1)
                {
                    newVertices.Add(new KeyValuePair<TVertexId, int>(vertex, index));
                }
            }

            return newVertices;
        }

        protected override int AddVertexImpl(TVertexId vertex)
        {
            int index = -1;

            if (vertex != null)
            {
                List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();

                adjList.Add(new KeyValuePair<TVertexId, List<KeyValuePair<int, int>>>(vertex, list));
                index = NumVertices - 1;
            }

            return index;
        }

        protected override IList<KeyValuePair<TVertexId, int>> GetEdgeInfoImpl(int vertex)
        {
            ICollection<KeyValuePair<int, int>> pairs = GetEdgeInfoByIndex(vertex);
            List<KeyValuePair<TVertexId, int>> vertexCostPair = new List<KeyValuePair<TVertexId, int>>();

            TVertexId vertexId;

            foreach (KeyValuePair<int, int> pair in pairs)
            {
                if (GetVertexId(pair.Key, out vertexId))
                {
                    vertexCostPair.Add(new KeyValuePair<TVertexId, int>(vertexId, pair.Value));
                }
            }

            return vertexCostPair;
        }

        public override ICollection<KeyValuePair<int, int>> GetEdgeInfoByIndex(int vertex)
        {
            return adjList[vertex].Value;
        }

        protected override bool CreateEdgeImpl(int firstVertex, int secondVertex, int weight)
        {
            bool isEdgeCreated = false;

            if (firstVertex < NumVertices && secondVertex < NumVertices)
            {
                isEdgeCreated = InsertEdgeNode(adjList[firstVertex].Value, secondVertex, weight);
            }

            return isEdgeCreated;
        }

        /// <summary>
        /// Inserts a vertex to a list of vertices representing the creation of an edge between the vertex and whichever vertex is associated with the list
        /// </summary>
        /// <param name="list">The list of vertices</param>
        /// <param name="vertex">The vertex to insert</param>
        /// <param name="weight">The weight of the newly created edge</param>
        /// <returns>true if the vertex has been added; false otherwise</returns>
        private bool InsertEdgeNode(List<KeyValuePair<int, int>> list, int vertex, int weight)
        {
            bool isNodeInserted = false;
            int index = list.Count - 1;

            while (index >= 0 && vertex < list[index].Key)
            {
                index--;
            }

            if (index < 0 || (index == list.Count - 1 && list[index].Key < vertex))
            {
                list.Add(new KeyValuePair<int, int>(vertex, weight));
                isNodeInserted = true;
            }
            else if (vertex != list[index].Key)
            {
                list.Insert(index, new KeyValuePair<int, int>(vertex, weight));
                isNodeInserted = true;
            }

            return isNodeInserted;
        }

        protected override bool GetVertexId(int index, out TVertexId vertexId)
        {
            bool foundId = false;
            vertexId = default(TVertexId);

            if (index < NumVertices)
            {
                vertexId = adjList[index].Key;
                foundId = true;
            }

            return foundId;
        }
    }
}
