using System;
using System.Collections.Generic;

namespace Jed.StateMachine
{
	public class State
	{
		private State parent;
		private object id;
		private Dictionary<object, State> substates;
		private TransitionDirector transitions;
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

		internal void Enter()
		{
			actions.PerformEnter();
		}

		internal void Exit()
		{
			actions.PerformExit();
		}

		internal TransitionInstance EvaluateEvent(object eventToReceive)
		{
			return transitions.AcceptTransition(eventToReceive);
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
	}
}