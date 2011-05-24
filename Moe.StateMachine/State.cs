using System;
using System.Collections.Generic;

namespace Moe.StateMachine
{
	public class State
	{
		private State parent;
		private object id;
		private Dictionary<object, State> substates;
		protected TransitionDirector transitions;
		private StateActions actions;

		internal State(object id, State parent)
		{
			this.actions = new StateActions(this);
			this.substates = new Dictionary<object, State>();
			this.id = id;
			this.transitions = new TransitionDirector(this);
			this.parent = parent;
		}

		public object Id { get { return id; } }
		public IEnumerable<State> Substates { get { return substates.Values; } }
		public State Parent { get { return parent; } }

		public State AddChildState(object id)
		{
			substates[id] = new State(id, this);
			return substates[id];
		}

		protected virtual void Enter(TransitionEvent transition)
		{
			actions.PerformEnter(transition.EventInstance);
		}

		protected virtual void Exit(TransitionEvent transition)
		{
			actions.PerformExit(transition.EventInstance);
		}

		public void AddTransition(Transition transition)
		{
			transitions.AddTransition(transition);
		}

		public void AddEnterAction(Action<object> action)
		{
			actions.AddEnter(action);
		}

		public void AddEnterAction<T>(Action<object,T> action)
		{
			actions.AddEnter(action);
		}

		public void AddExitAction(Action<object> action)
		{
			actions.AddExit(action);
		}

		public void AddExitAction<T>(Action<object,T> action)
		{
			actions.AddExit(action);
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
				return Traverse(new TransitionEvent(transition, eventToProcess));

			return parent.ProcessEvent(sourceState, eventToProcess);
		}

		public virtual State Accept(TransitionEvent transitionEvent)
		{
			return Traverse(transitionEvent);
		}

		protected virtual State Traverse(TransitionEvent transition)
		{
			// Have we arrived?
			if (transition.TargetState.Equals(this))
			{
				Enter(transition);
				return DispatchDefaults();
			}

			// Traverse down to children?
			foreach (State substate in substates.Values)
			{
				if (substate.ContainsState(transition.TargetState.Id))
				{
					Enter(transition);
					return substate.Accept(transition);
				}
			}

			Exit(transition);
			return parent.Accept(transition);
		}

		internal virtual State DispatchDefaults()
		{
			return ProcessEvent(this, new SingleStateEventInstance(this, StateMachine.DefaultEntryEvent));
		}
	}
}