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
			if (Entered != null)
				Entered(this, new StateTransitionEventArgs(transition));
		}

		protected virtual void Exit(TransitionEvent transition)
		{
			if (Exited != null)
				Exited(this, new StateTransitionEventArgs(transition));
		}

		public void AddTransition(Transition transition)
		{
			transitions.AddTransition(transition);
		}

		public override bool Equals(object obj)
		{
			if (obj is State)
				return ((State) obj).Id.Equals(this.Id);
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

		public virtual State ProcessEvent(State sourceState, EventInstance eventToProcess)
		{
			Transition transition = transitions.MatchTransition(eventToProcess);
			if (transition != null)
				return TraverseUp(new TransitionEvent(transition, eventToProcess));

			return parent.ProcessEvent(sourceState, eventToProcess);
		}

		public virtual State Accept(TransitionEvent transitionEvent)
		{
			// Have we arrived?
			if (transitionEvent.TargetState.Equals(this))
			{
				Enter(transitionEvent);
				return DispatchDefaults();
			}

			if (this.ContainsState(transitionEvent.TargetState))
				return TraverseDown(transitionEvent);

			return TraverseUp(transitionEvent);
		}

		protected virtual State TraverseDown(TransitionEvent transitionEvent)
		{
			// Traverse down to children?
			State substate = this.GetSubstatePath(transitionEvent.TargetState);
			if (substate != null)
			{
				Enter(transitionEvent);
				return substate.Accept(transitionEvent);
			}

			// Definitely, should never happen.
			throw new InvalidOperationException("Transition got lost in traversal");
		}

		protected virtual State TraverseUp(TransitionEvent transitionEvent)
		{
			Exit(transitionEvent);
			return parent.Accept(transitionEvent);
		}

		internal virtual State DispatchDefaults()
		{
			var eventToProcess = new SingleStateEventInstance(this, StateMachine.DefaultEntryEvent);
			Transition transition = transitions.MatchTransition(eventToProcess);
			if (transition != null)
			{
				TransitionEvent transitionEvent = new TransitionEvent(transition, eventToProcess);
				State substate = this.GetSubstatePath(transition.TargetState);
				if (substate != null)
					return substate.Accept(transitionEvent);

				throw new InvalidOperationException("Invalid default transition");
			}

			return this;
		}
	}
}