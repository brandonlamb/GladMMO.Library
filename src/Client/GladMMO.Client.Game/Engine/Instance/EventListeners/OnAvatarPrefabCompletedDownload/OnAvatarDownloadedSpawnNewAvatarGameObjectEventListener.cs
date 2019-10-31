﻿using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using GladMMO.FinalIK;
using GladMMO.SDK;
using UnityEngine;

namespace GladMMO
{
	[SceneTypeCreateGladMMO(GameSceneType.InstanceServerScene)]
	public sealed class OnAvatarDownloadedSpawnNewAvatarGameObjectEventListener : BaseSingleEventListenerInitializable<IContentPrefabCompletedDownloadEventSubscribable, ContentPrefabCompletedDownloadEventArgs>
	{
		private IReadonlyEntityGuidMappable<EntityGameObjectDirectory> GameObjectDirectoryMappable { get; }

		private ILog Logger { get; }

		public OnAvatarDownloadedSpawnNewAvatarGameObjectEventListener(IContentPrefabCompletedDownloadEventSubscribable subscriptionService,
			[NotNull] ILog logger,
			[NotNull] IReadonlyEntityGuidMappable<EntityGameObjectDirectory> gameObjectDirectoryMappable) 
			: base(subscriptionService)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			GameObjectDirectoryMappable = gameObjectDirectoryMappable ?? throw new ArgumentNullException(nameof(gameObjectDirectoryMappable));
		}

		protected override void OnEventFired(object source, ContentPrefabCompletedDownloadEventArgs args)
		{
			//Only interested in players.
			if (args.EntityGuid.EntityType != EntityType.Player)
				return;

			if(Logger.IsInfoEnabled)
				Logger.Info($"About to create new Avatar for Entity: {args.EntityGuid}");

			try
			{
				//Now we've assigned the handle, we need to actually handle the spawning/loading of the avatar.
				GameObject ikRootGameObject = GameObjectDirectoryMappable.RetrieveEntity(args.EntityGuid).GetGameObject(EntityGameObjectDirectory.Type.IKRoot);
				GameObject currentAvatarRootGameObject = ikRootGameObject.transform.GetChild(0).gameObject;
				GameObject newlySpawnedAvatar = InstantiateNewFromPrefab(args.DownloadedPrefabObject, currentAvatarRootGameObject);

				//Try to get AvatarBoneSDKData from root spawned model
				AvatarBoneSDKData boneSdkData = newlySpawnedAvatar.GetComponent<AvatarBoneSDKData>();

				//TODO: Head height.
				//We can set relative camera height for VR users or first person users.
				//Don't do it for desktop.
				if (boneSdkData != null)
				{
					GameObject nameRoot = GameObjectDirectoryMappable.RetrieveEntity(args.EntityGuid).GetGameObject(EntityGameObjectDirectory.Type.NameRoot);
					if (nameRoot != null)
					{
						nameRoot.transform.localPosition = new Vector3(nameRoot.transform.localPosition.x, boneSdkData.FloatingNameHeight, nameRoot.transform.localPosition.z);
					}
				}

				GameObject.DestroyImmediate(currentAvatarRootGameObject, false);

				//This will actually re-initialize the IK for the new avatar, since the old one is now gone.
				ikRootGameObject.GetComponent<IIKReinitializable>().ReInitialize();
			}
			catch (Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to create Avatar for Entity: {args.EntityGuid}. Error: {e.Message}\n\nStack: {e.StackTrace}");

				throw;
			}
		}

		private GameObject InstantiateNewFromPrefab([NotNull] GameObject prefab, [NotNull] GameObject oldAvatar)
		{
			if (prefab == null) throw new ArgumentNullException(nameof(prefab));
			if (oldAvatar == null) throw new ArgumentNullException(nameof(oldAvatar));

			//This should be the avatar prefab
			//and we should be on the main thread here so we can now do the spawning and such
			//Replace the current avatar root gameobject with the new one.
			GameObject newAvatarRoot = GameObject.Instantiate(prefab, oldAvatar.transform.parent);
			newAvatarRoot.transform.localScale = oldAvatar.transform.localScale;
			newAvatarRoot.transform.localPosition = Vector3.zero;
			newAvatarRoot.transform.localRotation = Quaternion.identity;
			return newAvatarRoot;
		}
	}
}
