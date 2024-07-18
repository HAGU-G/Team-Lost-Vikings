using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<T> heap = new List<T>();
    private Comparison<T> comparison;

    public bool IsEmpty
    {
        get
        {
            return heap.Count == 0;
        }
    }

    public int Count
    {
        get { return heap.Count; }
    }


    public PriorityQueue(Comparison<T> comparison)
    {
        this.comparison = comparison;
    }

    public void Enqueue(T item)
    {
        heap.Add(item);
        int index = heap.Count - 1;
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparison(heap[index], heap[parentIndex]) >= 0)
                break;
            T temp = heap[index];
            heap[index] = heap[parentIndex];
            heap[parentIndex] = temp;
            index = parentIndex;
        }
    }

    public T Dequeue()
    {
        T ret = heap[0];
        heap[0] = heap[heap.Count - 1];
        int newCount = heap.Count - 1;

        int index = 0;
        while (true)
        {
            int lChildIndex = index * 2 + 1;
            int rChildIndex = index * 2 + 2;
            if (lChildIndex >= newCount)
            {
                break;
            }
            int swapIndex = lChildIndex;
            if (rChildIndex < newCount && comparison(heap[rChildIndex], heap[lChildIndex]) < 0)
            {
                swapIndex = rChildIndex;
            }
            if (comparison(heap[index], heap[swapIndex]) <= 0)
            {
                break;
            }

            T temp = heap[swapIndex];
            heap[swapIndex] = heap[index];
            heap[index] = temp;

            index = swapIndex;
        }

        heap.RemoveAt(heap.Count - 1);
        return ret;
    }

    public T Peek()
    {
        return heap[0];
    }
}
