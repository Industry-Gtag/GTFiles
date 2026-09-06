using System;
using System.Collections.Generic;

// Token: 0x0200012E RID: 302
public class GraphNode<T>
{
	// Token: 0x17000085 RID: 133
	// (get) Token: 0x0600076D RID: 1901 RVA: 0x00029C14 File Offset: 0x00027E14
	// (set) Token: 0x0600076E RID: 1902 RVA: 0x00029C1C File Offset: 0x00027E1C
	public T Value { get; set; }

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x0600076F RID: 1903 RVA: 0x00029C25 File Offset: 0x00027E25
	public List<GraphNode<T>> Parents { get; } = new List<GraphNode<T>>();

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000770 RID: 1904 RVA: 0x00029C2D File Offset: 0x00027E2D
	public List<GraphNode<T>> Children { get; } = new List<GraphNode<T>>();

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000771 RID: 1905 RVA: 0x00029C35 File Offset: 0x00027E35
	public int ChildCount
	{
		get
		{
			return this.Children.Count;
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00029C42 File Offset: 0x00027E42
	public GraphNode(T value)
	{
		this.Value = value;
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00029C67 File Offset: 0x00027E67
	public GraphNode(T value, GraphNode<T> parent)
	{
		this.Value = value;
		this.Parents.Add(parent);
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00029C98 File Offset: 0x00027E98
	public int GetSubtreeWidth(int depthLimit = 2147483647)
	{
		if (this.ChildCount == 0 || depthLimit == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			num += graphNode.GetSubtreeWidth(depthLimit - 1);
		}
		return num;
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00029D00 File Offset: 0x00027F00
	public GraphNode<T> AddChild(T value)
	{
		return this.AddChild(new GraphNode<T>(value));
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00029D0E File Offset: 0x00027F0E
	public GraphNode<T> AddChild(GraphNode<T> child)
	{
		if (child.Parents.Contains(this))
		{
			throw new InvalidOperationException("Cannot add child more than once");
		}
		this.Children.Add(child);
		child.Parents.Add(this);
		return child;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00029D42 File Offset: 0x00027F42
	public void RemoveChild(GraphNode<T> child)
	{
		if (this.Children.Remove(child))
		{
			child.Parents.Remove(this);
		}
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00029D5F File Offset: 0x00027F5F
	public IEnumerable<GraphNode<T>> TraversePreOrder()
	{
		yield return this;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			foreach (GraphNode<T> graphNode2 in graphNode.TraversePreOrder())
			{
				yield return graphNode2;
			}
			IEnumerator<GraphNode<T>> enumerator2 = null;
		}
		List<GraphNode<T>>.Enumerator enumerator = default(List<GraphNode<T>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00029D6F File Offset: 0x00027F6F
	public IEnumerable<GraphNode<T>> TraversePreOrderDistinct(HashSet<GraphNode<T>> visited = null)
	{
		if (visited == null)
		{
			visited = new HashSet<GraphNode<T>>();
		}
		if (!visited.Contains(this))
		{
			yield return this;
			visited.Add(this);
			foreach (GraphNode<T> graphNode in this.Children)
			{
				foreach (GraphNode<T> graphNode2 in graphNode.TraversePreOrderDistinct(visited))
				{
					yield return graphNode2;
				}
				IEnumerator<GraphNode<T>> enumerator2 = null;
			}
			List<GraphNode<T>>.Enumerator enumerator = default(List<GraphNode<T>>.Enumerator);
		}
		yield break;
		yield break;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x00029D86 File Offset: 0x00027F86
	public IEnumerable<GraphNode<T>> TraverseBreadthFirst()
	{
		Queue<GraphNode<T>> queue = new Queue<GraphNode<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			GraphNode<T> current = queue.Dequeue();
			yield return current;
			foreach (GraphNode<T> graphNode in current.Children)
			{
				queue.Enqueue(graphNode);
			}
			current = null;
		}
		yield break;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00029D96 File Offset: 0x00027F96
	public IEnumerable<GraphNode<T>> TraverseBreadthFirstDistinct()
	{
		Queue<GraphNode<T>> queue = new Queue<GraphNode<T>>();
		HashSet<GraphNode<T>> visited = new HashSet<GraphNode<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			GraphNode<T> current = queue.Dequeue();
			if (!visited.Contains(current))
			{
				visited.Add(current);
				yield return current;
				foreach (GraphNode<T> graphNode in current.Children)
				{
					queue.Enqueue(graphNode);
				}
				current = null;
			}
		}
		yield break;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00029DA8 File Offset: 0x00027FA8
	public int GetGraphDepth()
	{
		if (this.Children.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Children)
		{
			num = Math.Max(num, graphNode.GetGraphDepth());
		}
		return num + 1;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00029E18 File Offset: 0x00028018
	public int GetNodeDepth()
	{
		if (this.Parents.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GraphNode<T> graphNode in this.Parents)
		{
			num = Math.Max(num, graphNode.GetNodeDepth());
		}
		return num + 1;
	}
}
