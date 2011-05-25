using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Extensions
{
	/// <summary>
	/// Wrapper with a string/string dictionary context attached to it for convenience.
	/// </summary>
	/// <typeparam name="T">Type for event</typeparam>
	public class EventWithLookup<T>
	{
		private readonly T eventId;
		private readonly Dictionary<string, string> context;

		public EventWithLookup(T eventId)
		{
			this.eventId = eventId;
			this.context = new Dictionary<string, string>();
		}

		public EventWithLookup(T eventId, Dictionary<string, string> lookup)
			: this(eventId)
		{
			this.context = new Dictionary<string, string>(lookup);
		}

		public T EventId { get { return eventId; } }

		public string this[string idx]
		{
			get { return context[idx]; }
			set { context[idx] = value; }
		}

		public override bool Equals(object obj)
		{
			return eventId.Equals(obj);
		}

		public override int GetHashCode()
		{
			return eventId.GetHashCode();
		}

		public override string ToString()
		{
			return eventId.ToString();
		}
	}

}
