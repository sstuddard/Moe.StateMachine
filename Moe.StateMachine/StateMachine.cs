using System;
using System.Collections.Generic;
using Moe.StateMachine.Events;
using Moe.StateMachine.Extensions;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public interface IStateMachineInitializer
	{
		State Initialize(StateMachine sm);
	}

	public enum StateMachineState
	{
		Building,
		Ready,
		Started,
		Stopped
	}

    public class StateMachine : IDisposable
    {
		public event EventHandler<EventPostedArgs> EventPosted;
		public event EventHandler<StateEventPostedArgs> EventProcessed;
		public event EventHandler<EventArgs> Starting;
    	public event EventHandler<EventArgs> Stopping;

		public static readonly object DefaultEntryEvent = "DefaultEntry";

    	protected readonly List<IPlugIn> plugins;
		protected readonly State root;
		protected State current;
		protected EventProcessor eventProcessor;
    	protected StateMachineState stateMachineState;

		public StateMachine(string fsmName, IStateMachineInitializer initializer)
		{
			Name = fsmName;
			stateMachineState = StateMachineState.Building;

			plugins = new List<IPlugIn>();

			// Wire null event handlers for simplicity
			EventPosted += delegate { };
			EventProcessed += delegate { };
			Starting += delegate { };
			Stopping += delegate { };

			AddPlugIn(new SynchronousEventProcessor());

			root = initializer.Initialize(this);
			InitializeStates();

			stateMachineState = StateMachineState.Ready;
		}

		public string Name { get; private set; }
		public State RootNode { get { return root; } }
		public State CurrentState { get { return current; } }
		public bool IsRunning { get { return stateMachineState == StateMachineState.Started; } }

    	public State this[object stateId]
    	{
    		get { return RootNode.GetState(stateId); }
    	}

		/// <summary>
		/// Returns bool indicating if the machine is in the given state (at any level)
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public virtual bool InState(object state)
		{
			bool result = false;
			if (current != null)
				current.VisitParentChain(s => result |= s.Id.Equals(state));
			return result;
		}

		/// <summary>
		/// Starts the finite state machine
		/// </summary>
    	public virtual void Start()
		{
			stateMachineState = StateMachineState.Started;
			Starting(this, new EventArgs());
    		current = root.ProcessEvent(root, new SingleStateEventInstance(root, DefaultEntryEvent));
		}

		/// <summary>
		/// Stops the finite state machine
		/// </summary>
		public virtual void Stop()
		{
			Stopping(this, new EventArgs());
			stateMachineState = StateMachineState.Stopped;
		}

		/// <summary>
		/// Allow for custom event posters (such as the async state machine)
		/// </summary>
		/// <param name="processor"></param>
		private void RegisterEventProcessor(EventProcessor processor)
		{
			if (eventProcessor != null)
				eventProcessor.EventProcessed -= OnStateChanged;

			eventProcessor = processor;
			eventProcessor.EventProcessed += OnStateChanged;
		}

		/// <summary>
		/// Post an event to the state machine
		/// </summary>
		/// <param name="eventToPost"></param>
		public virtual void PostEvent(object eventToPost)
		{
			EventInstance newEvent = eventToPost as EventInstance;
			if (newEvent == null)
				newEvent = new EventInstance(eventToPost);
			EventPosted(this, new EventPostedArgs(newEvent));
			eventProcessor.AddEvent(newEvent);
		}

		public void AddPlugIn(IPlugIn plugin)
		{
			plugins.Add(plugin);

			// If it is an event processor, register it
			if (plugin is EventProcessor)
				RegisterEventProcessor(plugin as EventProcessor);

			plugin.Initialize(this);
		}

		private void OnStateChanged(object sender, StateEventPostedArgs args)
		{
			current = args.State;
			EventProcessed(this, args);
		}

		private void InitializeStates()
		{
			root.VisitChildren(s =>
			                    {
			                        s.Traversing += (o, args) => current = args.State;
			                    });
		}

		public virtual void Dispose()
		{
		}

		public T GetPlugIn<T>()
		{
			return (T) plugins.Find(pi => pi is T);
		}
	}
}
