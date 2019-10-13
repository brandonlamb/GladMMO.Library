﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Glader.Essentials;

namespace GladMMO
{
	[ServerSceneTypeCreate(ServerSceneType.Default)]
	public sealed class InitializeWorldActorInitializable : IGameInitializable
	{
		private IWorldActorRef WorldReference { get; }
		
		//hack to ensure all handlers are working.
		private IEnumerable<IEntityActorMessageHandler> Handlers { get; }

		public InitializeWorldActorInitializable([NotNull] IWorldActorRef worldReference, [NotNull] IEnumerable<IEntityActorMessageHandler> handlers)
		{
			WorldReference = worldReference ?? throw new ArgumentNullException(nameof(worldReference));
			Handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
		}

		public async Task OnGameInitialized()
		{
			//TODO: Eventually we should treat the world as a network object.
			WorldReference.Tell(new EntityActorStateInitializeMessage<DefaultEntityActorStateContainer>(new DefaultEntityActorStateContainer(new EntityFieldDataCollection(8), NetworkEntityGuid.Empty)));
		}
	}
}
