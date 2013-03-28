using System.Collections.Generic;

namespace GraphLibrary
{
    /// <summary>
    /// Provides access to an underlying adjacency list
    /// </summary>
    /// <typeparam name="TVertexId">The type used for identifying vertices</typeparam>
    public class AdjacencyList<TVertexId> : Graph<TVertexId>
    {
        protected override IGraphImpl<TVertexId> CreateInnerGraph()
        {
            return new AdjacencyListImpl<TVertexId>();
        }

        public AdjacencyList()
            : base()
        {
        }

        public AdjacencyList(ICollection<TVertexId> vertices)
        {
            innerGraph = new AdjacencyListImpl<TVertexId>(vertices);
        }

        public AdjacencyList(TVertexId vertex)
        {
            innerGraph = new AdjacencyListImpl<TVertexId>(vertex);
        }
    }
}
