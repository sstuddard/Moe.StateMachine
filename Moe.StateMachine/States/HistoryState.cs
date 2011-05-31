using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Events;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.States
{
	/// <summary>
	/// Shallow history representation
	/// </summary>
	public class HistoryState : State
	{
		public const string HistoryTransition = "HistoryTransition";

		private State lastState;

		public HistoryState(State parent)
			: base(new HistoryStateId(parent.Id), parent)
		{
			Parent.Exited += OnParentExit;
		}

		public override State TraverseDown(TransitionEvent transitionEvent)
		{
			Enter(transitionEvent);

			// If we have history, transition there
			if (lastState != null)
			{
				Transition transition = new Transition(this, HistoryTransition, lastState);
				TransitionEvent transitionFromHistory = new TransitionEvent(this, transition, new EventInstance(HistoryTransition));

				return TraverseUp(transitionFromHistory);
			}
			// If we have no history, transition to parent
			else
			{
				Transition transition = new Transition(this, StateMachine.DefaultEntryEvent, Parent);
				TransitionEvent transitionToDefault = 
					new TransitionEvent(this, transition, new EventInstance(StateMachine.DefaultEntryEvent));

				return TraverseUp(transitionToDefault);
			}
		}

		private void OnParentExit(object sender, StateTransitionEventArgs args)
		{
			State sourceState = args.TransitionEvent.SourceState;
			lastState = Parent.GetSubstatePath(sourceState);
		}
	}

	public class HistoryStateId
	{
		public HistoryStateId(object id)
		{
			Id = String.Format("{0}:History", id);
		}

		public object Id { get; private set; }

		public override bool Equals(object obj)
		{
			if (obj is HistoryStateId)
				return Id.Equals(((HistoryStateId) obj).Id);
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return Id.ToString();
		}
	}
}
