﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using SceneJect.Common;
using UnityEngine;

namespace Guardians.Unity
{
	/// <summary>
	/// Default game framework manager that calls init
	/// and ticks tickables.
	/// </summary>
	[Injectee]
	public sealed class DefaultGameFrameworkManager : MonoBehaviour
	{
		/// <summary>
		/// Collection of all game initializables.
		/// </summary>
		[Inject]
		private IEnumerable<IGameInitializable> Initializables { get; }

		[Inject]
		private IEnumerable<IGameTickable> Tickables { get; }

		[Inject]
		private ILog Logger { get; }

		private bool isInitializationFinished = false;

		private async Task Start()
		{
			//The default way to handle this is to just await all initializables.
			//Preferably you'd want this to always run on the main thread, or continue to the main thread
			//but called code could avoid caputring the sync context, so it's out of our control
			foreach(var init in Initializables)
				try
				{
					await init.OnGameInitialized()
						.ConfigureAwait(true);
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Encountered Exception in {nameof(IGameInitializable.OnGameInitialized)} for Type: {init.GetType().Name}. Reason: {e.Message}\n\nStack: {e.StackTrace}");
					throw;
				}

			isInitializationFinished = true;
		}

		private void Update()
		{
			//The reason we don't update until initialization is finished
			//is because we CAN'T let potential tickables that may depend on
			//initializablizes having run, so to avoid this issue we don't run them until they are init
			if(!isInitializationFinished)
				return;

			foreach(IGameTickable tickable in Tickables)
				tickable.Tick();
		}
	}
}
