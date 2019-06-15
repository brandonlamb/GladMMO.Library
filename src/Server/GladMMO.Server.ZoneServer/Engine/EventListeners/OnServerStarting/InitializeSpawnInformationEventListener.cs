﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glader.Essentials;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GladMMO
{
	//TODO: Eventually we need to do this after we download the server map data.
	[ServerSceneTypeCreate(ServerSceneType.Default)]
	public sealed class InitializeSpawnInformationEventListener : BaseSingleEventListenerInitializable<IServerStartingEventSubscribable>
	{
		private PlayerSpawnStrategyQueue SpawnStrategyQueue { get; }

		public InitializeSpawnInformationEventListener(IServerStartingEventSubscribable subscriptionService,
			[NotNull] PlayerSpawnStrategyQueue spawnStrategyQueue) 
			: base(subscriptionService)
		{
			SpawnStrategyQueue = spawnStrategyQueue ?? throw new ArgumentNullException(nameof(spawnStrategyQueue));
		}

		protected override void OnEventFired(object source, EventArgs args)
		{
			//This locates all spawnpoint strats in the scene
			foreach (var spawn in Resources.FindObjectsOfTypeAll<MonoBehaviour>()
				.Where(b => b?.gameObject?.scene != null && !String.IsNullOrEmpty(b?.gameObject?.scene.name))
				.Select(b => b as ISpawnPointStrategy)
				.Where(b => b != null))
			{
				SpawnStrategyQueue.Enqueue(spawn);
			}

			//It's possible the creator didn't specify a spawnpoint, so we just use a default
			if (SpawnStrategyQueue.Count == 0)
				SpawnStrategyQueue.Enqueue(new DefaultSpawnPointStrategy());
		}
	}
}