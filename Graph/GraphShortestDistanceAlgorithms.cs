using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphLibrary
{
    /// <summary>
    /// Contains methods that implement some of the better known single source and all pairs shortest path algorithms
    /// </summary>
    internal static class GraphShortestDistanceAlgorithms
    {
        /// <summary>
        /// Implementation of Johnson's all pairs shortest path algorithm
        /// </summary>
        /// <typeparam name="TVertexId">Data type for key used to identify a vertex in the graph</typeparam>
        /// <param name="grph">The graph to run the algorithm on</param>
        /// <param name="distanceMatrix">
        ///     Each vertex is mapped to an array. This parameter contains a list with entries that are indexed in accordance with how the vertices are indexed in the array.
        ///     Each entry contains an array of distances to vertices. These values are likewise indexed. A null value means that the relevant vertex is unreachable.
        ///     This parameter is null if a negative cost cycle is detected.
        /// </param>
        /// <returns>true if a negative cost cycle is detected; false otherwise</returns>
        internal static bool Johnson<TVertexId>(IGraphImplForAlgorithms<TVertexId> grph, out int?[][] distanceMatrix)
        {
            distanceMatrix = null;

            // Augment the graph with an extra vertex with a directed edge to every other node.
            KeyValuePair<int, int>[] dest = new KeyValuePair<int, int>[grph.NumVertices];

            for (int i = 0; i < grph.NumVertices; i++)
            {
                dest[i] = new KeyValuePair<int, int>(i, 0);
            }

            IGraphImplForAlgorithms<TVertexId> enhancedGraph;
            int newIndex = grph.CreateAugmentedGraph(dest, out enhancedGraph);

            // Run Bellman-Ford on this graph with the extra vertex as the start vertex.
            int?[] weights;

            bool hasCycles = BellmanFord<TVertexId>(enhancedGraph, newIndex, out weights);
            if (!hasCycles)
            {
                // No negative cost cycles were detected. Reweigh the original graph according to the results of Bellman-Ford to ensure correctness of Dijkstra.
                IGraphImplForAlgorithms<TVertexId> newGraph = grph.MakeDeepCopy();
                newGraph.ReweightGraph(weights.Select(x => x.HasValue ? x.Value : 0).ToArray());

                int?[][] distanceMatrixCopy = new int?[newGraph.NumVertices][];

                // Run Dijkstra for every vertex as the start vertex. Reweigh again to get the correct minimal possible distances.
                // Each thread has its own heap, so care must be taken regarding memory requirements
                Parallel.For(0, newGraph.NumVertices, i =>
                {
                    int?[] distances;

                    Dijkstra<TVertexId>(newGraph, i, out distances);

                    for (int j = 0; j < distances.Length; j++)
                    {
                        distances[j] = distances[j] + weights[j] - weights[i];
                    }

                    distanceMatrixCopy[i] = distances;
                });

                distanceMatrix = distanceMatrixCopy;
            }

            return hasCycles;
        }

        /// <summary>
        /// Implementation of the Bellman-Ford single source shortest path algorithm
        /// </summary>
        /// <typeparam name="TVertexId">Data type for key used to identify a vertex in the graph</typeparam>
        /// <param name="grph">The graph to run the algorithm on</param>
        /// <param name="startIndex">Each vertex is mapped to an array. This is the index of the vertex that is to be the start vertex.</param>
        /// <param name="distances">
        ///     List of distances to vertices which are indexed in accordance with how the vertices are indexed in the aforementioned array. A null distance means that the relevant vertex is unreachable.
        ///     This is null if a negative cost cycle is detected.
        /// </param>
        /// <returns>true if a negative cost cycle is detected; false otherwise</returns>
        internal static bool BellmanFord<TVertexId>(IGraphImplForAlgorithms<TVertexId> grph, int startIndex, out int?[] distances)
        {
            bool hasCycles = false;

            int?[] prevDistances = new int?[grph.NumVertices];
            List<int> visitedList = new List<int>(grph.NumVertices);

            prevDistances[startIndex] = 0;
            visitedList.Add(startIndex);
            int?[] curDistances = prevDistances;

            int counter = 0;
            int index, numVisited;
            bool hasDifferentData;

            ICollection<KeyValuePair<int, int>> edgeReference;
            int calculatedDistance, baseDistance;

            // The set of visited vertices begins with the start vertex. At each iteration, for each visited vertex, calculate distance to neighboring vertices based on known shortest path from the start vertex to the visited vertex.
            // If calculated distance is less than known shortest distance to neighboring vertex, or vertex hasn't been visited yet, then the distance is the known shortest distance.
            // If neighboring vertex hasn't been visited yet, include it among the visited vertices.
            do
            {
                hasDifferentData = false;
                prevDistances = curDistances;

                numVisited = visitedList.Count;

                for (int i = 0; i < numVisited; i++)
                {
                    index = visitedList[i];
                    baseDistance = prevDistances[index].Value;

                    edgeReference = grph.GetEdgeInfoByIndex(index);
                    foreach (KeyValuePair<int, int> vertexData in edgeReference)
                    {
                        calculatedDistance = vertexData.Value + baseDistance;
                        if (!curDistances[vertexData.Key].HasValue)
                        {
                            visitedList.Add(vertexData.Key);
                            curDistances[vertexData.Key] = calculatedDistance;
                            hasDifferentData = true;
                        }
                        else if (curDistances[vertexData.Key].Value > calculatedDistance)
                        {
                            curDistances[vertexData.Key] = calculatedDistance;
                            hasDifferentData = true;
                        }
                    }
                }

                counter++;
            } while (hasDifferentData && counter < grph.NumVertices);

            // A loop is detected if, an extra iteration is taken after all the vertices have been visited and new shortest distances have been calculated.
            if (counter == grph.NumVertices && hasDifferentData)
            {
                hasCycles = true;
                distances = prevDistances;
            }
            else
            {
                distances = curDistances;
            }

            return hasCycles;
        }

        /// <summary>
        /// Implementation of Dijkstra's single source shortest path algorithm
        /// </summary>
        /// <typeparam name="TVertexId">Data type for key used to identify a vertex in the graph</typeparam>
        /// <param name="grph">The graph to run the algorithm on</param>
        /// <param name="startIndex">Each vertex is mapped to an array. This is the index of the vertex that is to be the start vertex.</param>
        /// <param name="distances">List of distances to vertices which are indexed in accordance with how the vertices are indexed in the aforementioned array. A null distance means that the relevant vertex is unreachable.</param>
        /// <returns>false, since Dijkstra's algorithm cannot detect negative cost cycles</returns>
        /// <remarks>This implementation of Dijkstra uses a binary heap</remarks>
        internal static bool Dijkstra<TVertexId>(IGraphImplForAlgorithms<TVertexId> grph, int startIndex, out int?[] distances)
        {
            byte[] placedInHeap = new byte[(grph.NumVertices >> 3) + 1];

            distances = new int?[grph.NumVertices];
            distances[startIndex] = 0;

            IPriorityQueue<int, int> pQueue = new MinBinaryHeap<int, int>();

            // Place all edges adjoining the start vertex in the heap
            ICollection<KeyValuePair<int, int>> edgeInfo = grph.GetEdgeInfoByIndex(startIndex);
            foreach (KeyValuePair<int, int> edge in edgeInfo)
            {
                pQueue.Insert(edge.Value, edge.Key);
                placedInHeap[edge.Key >> 3] |= (byte)(0x1 << (edge.Key & 0x7));
            }

            KeyValuePair<int, int> nextNode;
            int calculatedDistance;

            int index;
            byte bitToSet;

            // At each iteration, take the edge corresponding to the shortest path represented on the heap. The non-visited vertex at one end of the edge now counts among the visited vertices.
            // Take all the edges adjoining the vertex and add their weights to the distance of the aforementioned shortest path. 
            // The resulting distances are keys for inserting new edges to the heap or recalculating the keys of edges already in the heap.
            while (pQueue.Extract(out nextNode))
            {
                distances[nextNode.Value] = nextNode.Key;
                edgeInfo = grph.GetEdgeInfoByIndex(nextNode.Value);
                foreach (KeyValuePair<int, int> edge in edgeInfo)
                {
                    if (distances[edge.Key] == null)
                    {
                        calculatedDistance = nextNode.Key + edge.Value;

                        index = edge.Key >> 3;
                        bitToSet = (byte)(0x1 << (edge.Key & 0x7));

                        if ((placedInHeap[index] & bitToSet) != 0)
                        {
                            pQueue.ChangeKeyAndMoveUp(calculatedDistance, edge.Key);
                        }
                        else
                        {
                            pQueue.Insert(calculatedDistance, edge.Key);
                            placedInHeap[index] |= bitToSet;
                        }
                    }
                }
            }

            // Dijkstra's algorithm is unsuitable for detecting negative cost cycles.  Return false.
            return false;
        }
    }
}
