using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
	public class QuadTree<T>
	{
		public struct Point
		{
			public readonly Vector2 Position;
			public readonly T Value;

			public Point(Vector2 position, T value)
			{
				Position = position;
				Value = value;
			}

			public static implicit operator Vector2(Point point) => point.Position;
		}
	
		private const int CAPACITY = 4; // Maximum points in a node before subdividing
		private Rect bounds;           // The area this QuadTree node covers
		private List<Point> points;  // The points contained in this node
		private QuadTree<T>[] children;   // Subdivisions (child nodes)

		public QuadTree(Rect bounds)
		{
			this.bounds = bounds;
			points = new List<Point>();
			children = null;
		}

		public bool Insert(Vector2 position, T value) => Insert(new Point(position, value));
		private bool Insert(Point point)
		{
			// Ignore if the point is outside the bounds
			if (!bounds.Contains(point))
				return false;

			// If there’s space and no children, add the point here
			if (points.Count < CAPACITY && children == null)
			{
				points.Add(point);
				return true;
			}

			// Otherwise, subdivide and delegate to children
			if (children == null)
				Subdivide();

			foreach (var child in children)
			{
				if (child.Insert(point))
					return true;
			}

			return false; // Shouldn't reach here
		}

		public List<Point> QueryRect(Rect range, List<Point> found = null)
		{
			found ??= new List<Point>();

			// If the range does not intersect this node, return
			if (!bounds.Overlaps(range))
				return found;

			// Check points at this level
			foreach (var point in points)
			{
				if (range.Contains(point))
				{
					found.Add(point);
				}
			}

			// Check points in children
			if (children != null)
			{
				foreach (var child in children)
				{
					child.QueryRect(range, found);
				}
			}

			return found;
		}
		
		public List<Point> QueryCircle(Vector2 center, float radius, List<Point> found = null)
		{
			if (found == null)
			{
				found = new List<Point>();
			}

			// Skip this node if the bounds do not overlap the circle's bounding box
			Rect circleBounds = new Rect(center.x - radius, center.y - radius, radius * 2, radius * 2);
			if (!bounds.Overlaps(circleBounds))
				return found;

			// Check points at this level
			float radiusSquared = radius * radius; // Use squared radius to avoid unnecessary sqrt calculations
			foreach (var point in points)
			{
				if ((point.Position - center).sqrMagnitude <= radiusSquared)
				{
					found.Add(point);
				}
			}

			// Check points in children
			if (children != null)
			{
				foreach (var child in children)
				{
					child.QueryCircle(center, radius, found);
				}
			}

			return found;
		}

		private void Subdivide()
		{
			float x = bounds.xMin;
			float y = bounds.yMin;
			float halfWidth = bounds.width   / 2f;
			float halfHeight = bounds.height / 2f;

			children = new QuadTree<T>[4];
			children[0] = new QuadTree<T>(new Rect(x, y, halfWidth, halfHeight));       // Top-left
			children[1] = new QuadTree<T>(new Rect(x    + halfWidth, y, halfWidth, halfHeight)); // Top-right
			children[2] = new QuadTree<T>(new Rect(x, y + halfHeight, halfWidth, halfHeight)); // Bottom-left
			children[3] = new QuadTree<T>(new Rect(x    + halfWidth, y + halfHeight, halfWidth, halfHeight)); // Bottom-right
		}


		public bool TryFindClosest(Vector2 position, out Point result)
		{
			result = default;
			Point? closestPoint = null;
			float closestDistance = float.MaxValue;

			// Helper function for recursive search
			void Search(QuadTree<T> node)
			{
				// Skip if the node's bounds are too far from the target point
				if (!node.bounds.Overlaps(new Rect(position.x - closestDistance, position.y - closestDistance, closestDistance * 2, closestDistance * 2)))
					return;

				// Check points in this node
				foreach (var point in node.points)
				{
					float distance = Vector2.Distance(position, (Vector2)point);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closestPoint = point;
					}
				}

				// Check children recursively
				if (node.children != null)
				{
					foreach (var child in node.children)
					{
						Search(child);
					}
				}
			}

			// Start the recursive search
			Search(this);

			// Return the closest point found or false if no points exist
			if (closestPoint.HasValue)
			{
				result = closestPoint.Value;
				return true;
			}

			return false;
		}

		public bool Remove(Vector2 position, T value) => Remove(new Point(position, value));

		private bool Remove(Point point)
		{
			// If the point is not within bounds, it cannot be removed
			if (!bounds.Contains(point))
				return false;

			// Check if the point exists in this node
			if (points.Remove(point))
				return true;

			// If there are child nodes, attempt to remove the point from them
			if (children != null)
			{
				foreach (var child in children)
				{
					if (child.Remove(point))
					{
						// Check if we can consolidate after removal
						Consolidate();
						return true;
					}
				}
			}

			return false;
		}

		private void Consolidate()
		{
			// If there are no children, no need to consolidate
			if (children == null)
				return;

			// Count total points across children
			var totalPoints = new List<Point>();
			foreach (var child in children)
			{
				totalPoints.AddRange(child.points);
			}

			// If the total points are fewer than capacity, merge children into this node
			if (totalPoints.Count <= CAPACITY)
			{
				points.AddRange(totalPoints);
				children = null; // Remove subdivisions
			}
		}

		public void DrawGizmos()
		{
			Gizmos.DrawWireCube(bounds.center,bounds.size);
			if(children != null)
			{
				foreach (QuadTree<T> quadTree in children)
				{
					quadTree.DrawGizmos();
				}
			}
		}
	}
	
	
}