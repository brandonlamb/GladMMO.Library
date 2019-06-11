﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Glader.Essentials;
using UnityEngine;

namespace GladMMO
{
	[AdditionalRegisterationAs(typeof(IMovementInputChangedEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.InstanceServerScene)]
	public sealed class MovementInputBroadcastingTickable : OnLocalPlayerSpawnedEventListener, IGameTickable, IMovementInputChangedEventSubscribable
	{
		/// <inheritdoc />
		public event EventHandler<MovementInputChangedEventArgs> OnMovementInputDataChanged;

		private float LastHoritzontalInput { get; set; }

		private float LastVerticalInput { get; set; }

		private bool isLocalPlayerSpawned { get; set; } = false;

		private ILog Logger { get; }

		/// <inheritdoc />
		public MovementInputBroadcastingTickable(ILocalPlayerSpawnedEventSubscribable subscriptionService,
			[NotNull] ILog logger)
			: base(subscriptionService)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public void Tick()
		{
			if(!isLocalPlayerSpawned)
				return;

			bool changed = false;

			float horizontal = Input.GetAxisRaw("Horizontal");

			if(Math.Abs(LastHoritzontalInput - horizontal) > 0.005f)
			{
				changed = true;
				LastHoritzontalInput = horizontal;
			}

			float vertical = Input.GetAxisRaw("Vertical");

			if(Math.Abs(LastVerticalInput - vertical) > 0.005f)
			{
				changed = true;
				LastVerticalInput = vertical;
			}

			//If the input has changed we should dispatch to anyone interested.
			if(changed)
				OnMovementInputDataChanged?.Invoke(this, new MovementInputChangedEventArgs(vertical, horizontal));
		}

		/// <inheritdoc />
		protected override void OnLocalPlayerSpawned(LocalPlayerSpawnedEventArgs args)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"Movement input enabled.");

			//Local player is spawned, we should actually handle input now.
			isLocalPlayerSpawned = true;
		}
	}
}