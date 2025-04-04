using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Experimental.AI;

namespace ProjectDawn.Navigation
{
    /// <summary>
    /// Represents a boundary on the navigation mesh.
    /// </summary>
    public struct NavMeshBoundary : IComponentData
    {
#pragma warning disable CS0618
        /// <summary>
        /// The location of the boundary.
        /// </summary>
        public NavMeshLocation Location;
#pragma warning restore CS0618
        /// <summary>
        /// The radius of the boundary.
        /// </summary>
        public float Radius;
    }

    /// <summary>
    /// Represents a wall on the navigation mesh.
    /// </summary>
    public struct NavMeshWall : IBufferElementData
    {
        /// <summary>
        /// The start point of the wall.
        /// </summary>
        public float3 Start;

        /// <summary>
        /// The end point of the wall.
        /// </summary>
        public float3 End;

        public float3 Direction => math.normalize(End - Start);

        /// <summary>
        /// Constructs a new instance of the NavMeshWall struct.
        /// </summary>
        /// <param name="from">The start point of the wall.</param>
        /// <param name="to">The end point of the wall.</param>
        public NavMeshWall(float3 from, float3 to)
        {
            Start = from;
            End = to;
        }
    }
}
