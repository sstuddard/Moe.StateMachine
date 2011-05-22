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
		private List<Action<object>> enterActions;

		internal State(object id, State parent)
		{
			this.enterActions = new List<Action<object>>();
			this.substates = new Dictionary<object, State>();
			this.id = id;
			this.transitions = new TransitionDirector();
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
			foreach (Action<object> enterAction in enterActions)
				enterAction(Id);
		}

		internal void Exit()
		{
		}

		internal TransitionInstance EvaluateEvent(object eventToReceive)
		{
			TransitionInstance transition = null;
			Transition matching = transitions.FindTransition(eventToReceive);
			if (matching != null)
				transition = new TransitionInstance(this, matching);

			return transition;
		}

		internal void AddTransition(object eventTarget, StateLocator targetState)
		{
			transitions.AddTransition(eventTarget, targetState);
		}

		public void AddEnterAction(Action<object> action)
		{
			enterActions.Add(action);
		}
	}
}