using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Actions;
using Moe.StateMachine.Events;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.States
{
	public class RootState : State
	{
		public const string RootStateId = "Root";

		public RootState() 
			: base(RootStateId, null)
		{
		}

		/// <summary>
		/// By returning the current state of the fsm, we nullify this event because nobody responded.
		/// </summary>
		/// <param name="eventToProcess"></param>
		/// <returns></returns>
		public override State ProcessEvent(State sourceState, EventInstance eventToProcess)
		{
			Transition transition = transitions.MatchTransition(eventToProcess);
			if (transition != null)
				return TraverseDown(new TransitionEvent(transition, eventToProcess));

			return sourceState;
		}

		/// <summary>
		/// Traversal that doesn't go to children of the root are for unknown states.
		/// </summary>
		/// <param name="transition"></param>
		/// <returns></returns>
		protected override State TraverseUp(TransitionEvent transition)
		{
			// Traverse down to children?
			foreach (State substate in Substates)
			{
				if (substate.ContainsState(transition.TargetState.Id))
				{
					return substate.Accept(transition);
				}
			}

			throw new InvalidOperationException("Transition state [" + transition.TargetState.Id.ToString() + "] not found");
		}
	}
}
