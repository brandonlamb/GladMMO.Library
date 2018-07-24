﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Guardians
{
	public sealed class PlayerEntityEntryManager : IGameTickable
	{
		private IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext>> PlayerEntitySessionDequeable { get; }

		private IFactoryCreatable<GameObject, PlayerEntityCreationContext> PlayerFactory { get; }

		private INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> SpawnPayloadSender { get; }

		/// <inheritdoc />
		public PlayerEntityEntryManager(
			[NotNull] IDequeable<KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext>> playerEntitySessionDequeable, 
			[NotNull] IFactoryCreatable<GameObject, PlayerEntityCreationContext> playerFactory,
			[NotNull] INetworkMessageSender<GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>> spawnPayloadSender)
		{
			PlayerEntitySessionDequeable = playerEntitySessionDequeable ?? throw new ArgumentNullException(nameof(playerEntitySessionDequeable));
			PlayerFactory = playerFactory ?? throw new ArgumentNullException(nameof(playerFactory));
			SpawnPayloadSender = spawnPayloadSender ?? throw new ArgumentNullException(nameof(spawnPayloadSender));
		}

		/// <inheritdoc />
		public void Tick()
		{
			//If we have no new players we can just 
			if(PlayerEntitySessionDequeable.isEmpty)
				return;

			//TODO: Should we limit this? We might want to stagger this or else under extreme conditions we could lag the main thread.
			while(!PlayerEntitySessionDequeable.isEmpty)
			{
				KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext> dequeuedPlayerSession = PlayerEntitySessionDequeable.Dequeue();

				//We don't need to do anything with the returned object.
				GameObject playerGameObject = PlayerFactory.Create(new PlayerEntityCreationContext(dequeuedPlayerSession.Key, dequeuedPlayerSession.Value));

				//Once added we then need to send to the client a packet indicating its creation
				SpawnPayloadSender.Send(BuildSpawnEventPayload(dequeuedPlayerSession));

				//TODO: If we want to do anything post-creation with the provide gameobject we could. But we really don't want to at the moment.
			}
		}

		private static GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload> BuildSpawnEventPayload(KeyValuePair<NetworkEntityGuid, PlayerEntitySessionContext> dequeuedPlayerSession)
		{
			//TODO: get real movement info
			EntityCreationData data = new EntityCreationData(dequeuedPlayerSession.Key, new MovementInformation(Vector3.zero, 0.0f));

			return new GenericSingleTargetMessageContext<PlayerSelfSpawnEventPayload>(dequeuedPlayerSession.Key, new PlayerSelfSpawnEventPayload(data));
		}
	}
}
