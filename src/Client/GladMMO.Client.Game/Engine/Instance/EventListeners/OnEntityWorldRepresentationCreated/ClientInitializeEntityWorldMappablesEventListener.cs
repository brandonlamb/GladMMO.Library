﻿using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using UnityEngine;

namespace GladMMO
{
	/// <summary>
	/// Event subscriber that links the <see cref="GameObject"/> and <see cref="NetworkEntityGuid"/> of an Entity
	/// together in a two-way relationship.
	/// </summary>
	[SceneTypeCreateGladMMO(GameSceneType.InstanceServerScene)]
	public sealed class ClientInitializeEntityWorldMappablesEventListener : SharedInitializeEntityWorldMappablesEventListener
	{
		public ClientInitializeEntityWorldMappablesEventListener(IEntityWorldRepresentationCreatedEventSubscribable subscriptionService, IEntityGuidMappable<GameObject> guidToGameObjectMappable, IGameObjectToEntityMappable gameObjectToEntityMap) 
			: base(subscriptionService, guidToGameObjectMappable, gameObjectToEntityMap)
		{

		}
	}
}