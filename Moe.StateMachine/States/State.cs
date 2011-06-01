using System;
using System.Collections.Generic;
using Moe.StateMachine.Events;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.States
{
	public class State
	{
		public event EventHandler<StateTransitionEventArgs> Entered;
		public event EventHandler<StateTransitionEventArgs> Exited;
		public event EventHandler<StateEventArgs> Traversing;

		private State parent;
		private object id;
		private List<State> substates;
		protected TransitionDirector transitions;

		public State(object id, State parent)
		{
			this.id = id;
			this.parent = parent;
			this.substates = new List<State>();
			this.transitions = new TransitionDirector();

			Entered += delegate { };
			Exited += delegate { };
			Traversing += delegate { };
		}

		public object Id { get { return id; } }
		public IEnumerable<State> Substates { get { return new List<State>(substates); } }
		public State Parent { get { return parent; } }

		public void AddChildState(State substate)
		{
			substates.Add(substate);
		}

		protected virtual void Enter(TransitionEvent transition)
		{
			Entered(this, new StateTransitionEventArgs(transition));
		}

		protected virtual void Exit(TransitionEvent transition)
		{
			Exited(this, new StateTransitionEventArgs(transition));
		}

		public void AddTransition(Transition transition)
		{
			transitions.AddTransition(transition);
		}

		public override bool Equals(object obj)
		{
			if (obj is State)
				return ((State) obj).Id.Equals(Id);
			return false;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		public override string ToString()
		{
			return "[State: " + id.ToString() + "]";
		}

		public virtual State ProcessEvent(State originalState, EventInstance eventToProcess)
		{
			Transition transition = transitions.MatchTransition(eventToProcess);
			if (transition != null)
			{
				var transitionEvent = new TransitionEvent(originalState, transition, eventToProcess);

				// If originating from substate
				if (!this.Equals(originalState))
					return originalState.TraverseUp(transitionEvent);
				else
				{
					Exit(transitionEvent);
					return parent.TraverseUp(transitionEvent);
				}
			}

			// Not handled, pass it up the chain
			return parent.ProcessEvent(originalState, eventToProcess);
		}

		public virtual State TraverseDown(TransitionEvent transitionEvent)
		{
			Traversing(this, new StateEventArgs(this));
			Enter(transitionEvent);

			if (!this.Equals(transitionEvent.TargetState))
				return this.GetSubstatePath(transitionEvent.TargetState).TraverseDown(transitionEvent);

			return DispatchDefaults();
		}

		public virtual State TraverseUp(TransitionEvent transitionEvent)
		{
			Traversing(this, new StateEventArgs(this));
			if (this.Equals(transitionEvent.TargetState))
				return DispatchDefaults();

			if (this.ContainsState(transitionEvent.TargetState))
				return this.GetSubstatePath(transitionEvent.TargetState).TraverseDown(transitionEvent);

			Exit(transitionEvent);
			return parent.TraverseUp(transitionEvent);
		}

		protected virtual State DispatchDefaults()
		{
			var eventToProcess = new SingleStateEventInstance(this, StateMachine.DefaultEntryEvent);
			Transition transition = transitions.MatchTransition(eventToProcess);
			if (transition != null)
			{
				TransitionEvent transitionEvent = new TransitionEvent(this, transition, eventToProcess);
				State substate = this.GetSubstatePath(transition.TargetState);
				if (substate != null)
					return substate.TraverseDown(transitionEvent);

				throw new InvalidOperationException("Invalid default transition");
			}

			return this;
		}
	}
}