using UnityEngine;
using System.Collections.Generic;

//An implementation of A* Pathfinding Algorithm
public class NavGrid : MonoBehaviour
{
	public Vector2 gridBounds;
	public Vector2Int gridDimensions;
	public int extension;

	public Obsticle[] obsticles;
	
	private static PathNode[,] s_grid;
	private static float s_nodeSize;
	private static Vector2 s_worldBounds;
	private static List<Obsticle> s_obsticles;

	private void Awake()
	{
		InitializeGrid(gridDimensions, gridBounds, extension);
		foreach (Obsticle obsticle in obsticles)
			AddObsticle(obsticle);
	}

    public static PathNode[,] GetGrid() => s_grid;
	public static int GetGridLengthX() => s_grid.GetLength(0);
	public static int GetGridLengthY() => s_grid.GetLength(1);

	public static void InitializeGrid(Vector2Int dimensions, Vector2 bounds, int extension)
	{
		s_obsticles = new List<Obsticle>();

		float ratio = bounds.x / bounds.y;
		float height = bounds.y;
		Vector2 offset = new Vector2(height * ratio / 2.0f, height / 2.0f);

		s_grid = new PathNode[dimensions.x + extension * 2 + 1, dimensions.y + extension * 2 + 1];
		s_nodeSize = height / dimensions.y;
		s_worldBounds = new Vector2(GetGridLengthX() * s_nodeSize, GetGridLengthY() * s_nodeSize);

		for (int y = 0; y < GetGridLengthY(); y++) 
		{
			for (int x = 0; x < GetGridLengthX(); x++)
			{
				Vector3 worldPosition = new Vector3(x * s_nodeSize - offset.x - extension * s_nodeSize, 0, y * s_nodeSize - offset.y - extension * s_nodeSize);
				Vector2Int gridPosition = new Vector2Int(x, y);
				s_grid[x, y] = new PathNode(worldPosition, gridPosition);
			}
		}
	}

	public static List<PathNode> GetNeighbours(PathNode node)
	{
		List<PathNode> neighbours = new List<PathNode>();
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;
				
				Vector2Int checkPos = node.GetGridPosition() + new Vector2Int(x, y);
				
				if (checkPos.x >= 0 && checkPos.x < GetGridLengthX() &&
					checkPos.y >= 0 && checkPos.y < GetGridLengthY())
				{
					neighbours.Add(s_grid[checkPos.x, checkPos.y]);
				}
			}
		}

		return neighbours;
	}

	public static PathNode GetNodeFromWorldPosition(Vector3 worldPosition)
	{
		float percentX = Mathf.Clamp01((worldPosition.x + s_worldBounds.x / 2.0f) / s_worldBounds.x);
		float percentY = Mathf.Clamp01((worldPosition.z + s_worldBounds.y / 2.0f) / s_worldBounds.y);

		int x = Mathf.RoundToInt((GetGridLengthX() - 1) * percentX);
		int y = Mathf.RoundToInt((GetGridLengthY() - 1) * percentY);

		return s_grid[x, y];
	}

	public static void AddObsticle(Obsticle obsticle)
	{
		if (s_obsticles.Contains(obsticle))
			return;

		s_obsticles.Add(obsticle);
		Vector2Int nodeGridPos = GetNodeFromWorldPosition(obsticle.transform.position).GetGridPosition();
		int effectRadius = Mathf.RoundToInt(obsticle.radius / s_nodeSize);

		for (int y = -effectRadius; y <= effectRadius; y++)
		{
			for (int x = -effectRadius; x <= effectRadius; x++)
			{
				if (x == 0 && y == 0)
                {
					s_grid[nodeGridPos.x, nodeGridPos.y].movementPenalty = 255;
					continue;
                }
				
				Vector2Int gridCheckPos = new Vector2Int(nodeGridPos.x + x, nodeGridPos.y + y);
				if (gridCheckPos.x >= 0 && gridCheckPos.x < GetGridLengthX() &&
					gridCheckPos.y >= 0 && gridCheckPos.y < GetGridLengthY())
                {
					PathNode node = s_grid[gridCheckPos.x, gridCheckPos.y];
					float distance = Vector3.Distance(node.GetWorldPosition(), obsticle.transform.position);
					//node.movementPenalty = (distance <= obsticle.radius) ? Mathf.Clamp(node.movementPenalty + Mathf.RoundToInt((1 - distance / obsticle.radius) * 255 * obsticle.dangerLevel), 0, 255) : node.movementPenalty + 0;
                }
			}
		}
	}

	public static void RemoveObsticle(Obsticle remove)
    {
		s_obsticles.Remove(remove);

		for (int y = 0; y < GetGridLengthY(); y++)
		{
			for (int x = 0; x < GetGridLengthX(); x++)
			{
				s_grid[x, y].movementPenalty = 0;
			}
		}

		foreach (Obsticle obsticle in s_obsticles)
		{
			Vector2Int nodeGridPos = GetNodeFromWorldPosition(obsticle.transform.position).GetGridPosition();
			int effectRadius = Mathf.RoundToInt(obsticle.radius / s_nodeSize);

			for (int y = -effectRadius; y <= effectRadius; y++)
			{
				for (int x = -effectRadius; x <= effectRadius; x++)
				{
					if (x == 0 && y == 0)
					{
						s_grid[nodeGridPos.x, nodeGridPos.y].movementPenalty = 255;
						continue;
					}

					Vector2Int gridCheckPos = new Vector2Int(nodeGridPos.x + x, nodeGridPos.y + y);
					if (gridCheckPos.x >= 0 && gridCheckPos.x < GetGridLengthX() &&
						gridCheckPos.y >= 0 && gridCheckPos.y < GetGridLengthY())
					{
						PathNode node = s_grid[gridCheckPos.x, gridCheckPos.y];
						float distance = Vector3.Distance(node.GetWorldPosition(), obsticle.transform.position);
						//node.movementPenalty = (distance <= obsticle.radius) ? Mathf.Clamp(node.movementPenalty + Mathf.RoundToInt((1 - distance / obsticle.radius) * 255 * obsticle.dangerLevel), 0, 255) : node.movementPenalty + 0;
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	[Space]
	[Header("Editor Assistance")]
	[SerializeField]
	private bool displayCameraFrustom = true;
	[SerializeField]
	private bool displayGrid = true;

	private void OnDrawGizmos()
	{
		if (displayCameraFrustom)
		{
			Gizmos.color = Color.white;

			Camera cam = Camera.main;

			Gizmos.matrix = Matrix4x4.TRS(cam.transform.position, cam.transform.rotation, Vector3.one);
			if (cam.orthographic)
				Gizmos.DrawWireCube(Vector3.zero, new Vector3(cam.orthographicSize * cam.aspect * 2, cam.orthographicSize * 2));
			else
            	Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
		}

		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, new Vector3(gridBounds.x, 0, gridBounds.y));

		if (!Application.isPlaying)
			return;

		if (displayGrid)
		{
			foreach (PathNode node in s_grid)
			{
				float g = 1 - node.movementPenalty / 255.0f;
				Gizmos.color = new Color(g, g, g, 1.0f);
				Gizmos.DrawCube(node.GetWorldPosition(), Vector3.one * s_nodeSize * 0.9f);
			}
		}
	
		/*
		Gizmos.color = Color.cyan;
		for (int i = 0; i < path.Length - 1; i++)
        {
			Gizmos.DrawLine(path[i], path[i + 1]);
        }
		*/
	}
#endif
}

public class Pathfinding
{
	public static Vector3[] FindPath(Vector3 startPosition, Vector3 targetPosition)
	{
		PathNode start = NavGrid.GetNodeFromWorldPosition(startPosition);
		PathNode target = NavGrid.GetNodeFromWorldPosition(targetPosition);

		List<PathNode> nodesOpen = new List<PathNode>();
		HashSet<PathNode> nodesClosed = new HashSet<PathNode>();

		nodesOpen.Add(start);
		while (nodesOpen.Count > 0)
		{
			PathNode currentNode = nodesOpen[0];
			for (int i = 0; i < nodesOpen.Count; i++)
			{
				if (nodesOpen[i].fCost < currentNode.fCost || 
					nodesOpen[i].fCost == currentNode.fCost && 
					nodesOpen[i].hCost < currentNode.hCost)
				{
					currentNode = nodesOpen[i];
				}
			}

			nodesOpen.Remove(currentNode);
			nodesClosed.Add(currentNode);

			if (currentNode == target)
				return RetracePath(start, target);
			
			foreach (PathNode neighbour in NavGrid.GetNeighbours(currentNode))
			{
				if (!neighbour.IsAccessible() || nodesClosed.Contains(neighbour))
					continue;

				int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour) + Mathf.RoundToInt(neighbour.movementPenalty);
				if (newMoveCost < neighbour.gCost || !nodesOpen.Contains(neighbour))
				{

					neighbour.gCost = newMoveCost;
					neighbour.hCost = GetDistance(neighbour, target);
					neighbour.parent = currentNode;

					if (!nodesOpen.Contains(neighbour))
						nodesOpen.Add(neighbour);
				}
			}
		}

		return null;
	}

	static Vector3[] RetracePath(PathNode start, PathNode target)
	{
		List<Vector3> path = new List<Vector3>();
		PathNode currentNode = target;

		while(currentNode != start)
		{
			path.Add(currentNode.GetWorldPosition());
			currentNode = currentNode.parent;
		}

		path.Reverse();
		return path.ToArray();
	}

	static int GetDistance(PathNode nodeA, PathNode nodeB)
	{
		int dstX = Mathf.Abs(nodeA.GetGridPosition().x - nodeB.GetGridPosition().x);
		int dstY = Mathf.Abs(nodeA.GetGridPosition().y - nodeB.GetGridPosition().y);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);

		return 14 * dstX + 10 * (dstY - dstX);
	}
}

public class PathNode
{
	private Vector2Int m_gridPosition;
	private Vector3 m_worldPosition;
	private bool m_accessible = true;

	public Obsticle obsticle = null;
	public PathNode parent = null;
	public int movementPenalty = 0;

	public int gCost = 1;
	public int hCost = 1;
	public int fCost { get { return gCost + hCost; } }

	public PathNode(Vector3 position, Vector2Int grid)
	{
		m_worldPosition = position;
		m_gridPosition = grid;
	}

	public Vector3 GetWorldPosition() => m_worldPosition;
	public Vector2Int GetGridPosition() => m_gridPosition;
	public bool IsAccessible() => m_accessible;
}
