using System;
using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<T>
{
    private List<(T item, int priority)> heap = new List<(T, int)>();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("PriorityQueue is empty.");
        T item = heap[0].item;
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);
        if (heap.Count > 0) HeapifyDown(0);
        return item;
    }

    public bool Contains(T item)
    {
        return heap.Any(x => EqualityComparer<T>.Default.Equals(x.item, item));
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (heap[parentIndex].priority <= heap[index].priority) break;
            (heap[parentIndex], heap[index]) = (heap[index], heap[parentIndex]);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        while (true)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallest = index;

            if (leftChild < heap.Count && heap[leftChild].priority < heap[smallest].priority)
                smallest = leftChild;

            if (rightChild < heap.Count && heap[rightChild].priority < heap[smallest].priority)
                smallest = rightChild;

            if (smallest == index) break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }
}
