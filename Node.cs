using System;
using System.Collections.Generic;

// Token: 0x02000134 RID: 308
public class Node<T>
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060007AA RID: 1962 RVA: 0x0002A6B7 File Offset: 0x000288B7
	// (set) Token: 0x060007AB RID: 1963 RVA: 0x0002A6BF File Offset: 0x000288BF
	public T Value { get; set; }

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x060007AC RID: 1964 RVA: 0x0002A6C8 File Offset: 0x000288C8
	// (set) Token: 0x060007AD RID: 1965 RVA: 0x0002A6D0 File Offset: 0x000288D0
	public Node<T> Parent { get; private set; }

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x060007AE RID: 1966 RVA: 0x0002A6D9 File Offset: 0x000288D9
	public List<Node<T>> Children { get; } = new List<Node<T>>();

	// Token: 0x060007AF RID: 1967 RVA: 0x0002A6E1 File Offset: 0x000288E1
	public Node(T value)
	{
		this.Value = value;
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002A6FC File Offset: 0x000288FC
	public Node<T> AddChild(T value)
	{
		Node<T> node = new Node<T>(value)
		{
			Parent = this
		};
		this.Children.Add(node);
		return node;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002A724 File Offset: 0x00028924
	public Node<T> AddChild(Node<T> child)
	{
		Node<T> parent = child.Parent;
		if (parent != null)
		{
			parent.RemoveChild(child);
		}
		this.Children.Add(child);
		child.Parent = this;
		return child;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002A74C File Offset: 0x0002894C
	public void RemoveChild(Node<T> child)
	{
		if (this.Children.Remove(child))
		{
			child.Parent = null;
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0002A763 File Offset: 0x00028963
	public IEnumerable<Node<T>> TraversePreOrder()
	{
		yield return this;
		foreach (Node<T> node in this.Children)
		{
			foreach (Node<T> node2 in node.TraversePreOrder())
			{
				yield return node2;
			}
			IEnumerator<Node<T>> enumerator2 = null;
		}
		List<Node<T>>.Enumerator enumerator = default(List<Node<T>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002A773 File Offset: 0x00028973
	public IEnumerable<Node<T>> TraverseBreadthFirst()
	{
		Queue<Node<T>> queue = new Queue<Node<T>>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{
			Node<T> current = queue.Dequeue();
			yield return current;
			foreach (Node<T> node in current.Children)
			{
				queue.Enqueue(node);
			}
			current = null;
		}
		yield break;
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0002A784 File Offset: 0x00028984
	public List<Node<T>> GetPath()
	{
		List<Node<T>> list = new List<Node<T>> { this };
		for (Node<T> node = this.Parent; node != null; node = node.Parent)
		{
			list.Insert(0, node);
		}
		return list;
	}
}
