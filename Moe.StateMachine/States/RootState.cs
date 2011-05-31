using System;
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
		public override State ProcessEvent(State originalState, EventInstance eventToProcess)
		{
			Transition transition = transitions.MatchTransition(eventToProcess);
			if (transition != null)
				return TraverseDown(new TransitionEvent(this, transition, eventToProcess));

			return originalState;
		}

		/// <summary>
		/// Traversal that doesn't go to children of the root are for unknown states.
		/// </summary>
		/// <param name="transition"></param>
		/// <returns></returns>
		public override State TraverseUp(TransitionEvent transition)
		{
			// Traverse down to children?
			foreach (State substate in Substates)
			{
				if (substate.ContainsState(transition.TargetState))
				{
					return substate.TraverseDown(transition);
				}
			}

			throw new InvalidOperationException("Transition state [" + transition.TargetState.Id.ToString() + "] not found");
		}
	}
}
