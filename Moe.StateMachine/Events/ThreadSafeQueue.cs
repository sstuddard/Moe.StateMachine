using System.Collections.Generic;

namespace Moe.StateMachine.Events
{
	/// <summary>
	/// A relatively naive thread safe queue that will fit the need.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ThreadSafeQueue<T> where T : class
	{
		private Queue<T> queue;

		public ThreadSafeQueue()
		{
			queue = new Queue<T>();
		}

		public bool HasItems
		{
			get
			{
				lock (queue)
					return queue.Count > 0;
			}
		}

		public T Dequeue()
		{
			lock (queue)
			{
				if (HasItems)
					return queue.Dequeue();
				
				return null;
			}
		}

		public void Enqueue(T item)
		{
			lock (queue)
				queue.Enqueue(item);
		}
	}
}
