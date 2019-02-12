﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;

namespace PSOBB
{
	//PlayerModelChangeRequestPayload
	public sealed class PlayerModelChangeRequestHandler : ControlledEntityRequestHandler<PlayerModelChangeRequestPayload>
	{
		private IReadonlyEntityGuidMappable<IEntityDataFieldContainer> EntityFieldMap { get; }

		/// <inheritdoc />
		public PlayerModelChangeRequestHandler(
			ILog logger, 
			IReadonlyConnectionEntityCollection connectionIdToEntityMap, 
			IContextualResourceLockingPolicy<NetworkEntityGuid> lockingPolicy,
			[NotNull] IReadonlyEntityGuidMappable<IEntityDataFieldContainer> entityFieldMap) 
			: base(logger, connectionIdToEntityMap, lockingPolicy)
		{
			EntityFieldMap = entityFieldMap ?? throw new ArgumentNullException(nameof(entityFieldMap));
		}

		/// <inheritdoc />
		protected override Task HandleMessage(IPeerSessionMessageContext<GameServerPacketPayload> context, PlayerModelChangeRequestPayload payload, NetworkEntityGuid guid)
		{
			//At this point, the player wants to change his model.
			//However we really can't be sure it's a valid model
			//so we validate it before setting the model id.
			//TODO: Validate the model id.
			ProjectVersionStage.AssertAlpha();
			IEntityDataFieldContainer entityDataFieldContainer = GetEntityMappedObject(EntityFieldMap, context);

			//This change will be broadcast to anyone interested.
			entityDataFieldContainer.SetFieldValue((int)EntityDataFieldType.ModelId, payload.ModelId);
			
			return Task.CompletedTask;
		}
	}
}
