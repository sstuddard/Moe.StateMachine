using System;
using System.Collections.Generic;

namespace Jed.StateMachine
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
		internal IEnumerable<State> Substates { get { return substates.Values; } }
		internal State Parent { get { return parent; } }

		internal State AddChildState(object id)
		{
			substates[id] = new State(id, this);
			return substates[id];
		}

		protected virtual void Enter()
		{
			actions.PerformEnter();
		}

		protected virtual void Exit()
		{
			actions.PerformExit();
		}

		internal void AddTransition(Transition transition)
		{
			transitions.AddTransition(transition);
		}

		public void AddEnterAction(Action<object> action)
		{
			actions.AddEnter(action);
		}

		public void AddExitAction(Action<object> action)
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
				return Traverse(transition);

			return parent.ProcessEvent(sourceState, eventToProcess);
		}

		protected internal virtual State Traverse(Transition transition)
		{
			// Have we arrived?
			if (transition.TargetState.Equals(this))
			{
				Enter();
				return DispatchDefaults();
			}

			// Traverse down to children?
			foreach (State substate in substates.Values)
			{
				if (substate.ContainsState(transition.TargetState.Id))
				{
					Enter();
					return substate.Traverse(transition);
				}
			}

			Exit();
			return parent.Traverse(transition);
		}

		internal virtual State DispatchDefaults()
		{
			var defaultTransition = new SingleStateEventInstance(this, StateMachine.DefaultEntryEvent);
			if (transitions.MatchTransition(defaultTransition) != null)
				return ProcessEvent(this, defaultTransition);

			return this;
		}
	}
}