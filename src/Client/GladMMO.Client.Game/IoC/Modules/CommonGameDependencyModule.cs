﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Autofac;
using Common.Logging;
using FreecraftCore;
using Glader.Essentials;
using Refit;

namespace GladMMO.Client
{
	public sealed class CommonGameDependencyModule : NetworkServiceDiscoveryableAutofaceModule
	{
		/// <summary>
		/// The scene to load initializables for.
		/// </summary>
		private GameSceneType Scene { get; }

		private string ServiceDiscoveryUrl { get; }

		/// <summary>
		/// Default autofac ctor.
		/// </summary>
		public CommonGameDependencyModule()
		{
			//We shouldn't call this, I don't think.
		}

		//TODO: Shoudl we expose the ServiceDiscovery URL to the editor? Is there value in that?
		/// <inheritdoc />
		public CommonGameDependencyModule(GameSceneType scene, [NotNull] string serviceDiscoveryUrl = "http://192.168.0.3:5000")
		{
			if(!Enum.IsDefined(typeof(GameSceneType), scene)) throw new InvalidEnumArgumentException(nameof(scene), (int)scene, typeof(GameSceneType));

			Scene = scene;
			ServiceDiscoveryUrl = serviceDiscoveryUrl ?? throw new ArgumentNullException(nameof(serviceDiscoveryUrl));
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Set the sync context
			UnityAsyncHelper.InitializeSyncContext();

			builder.Register(context => LogLevel.All)
				.As<LogLevel>()
				.SingleInstance();

			builder.RegisterType<UnityLogger>()
				.As<ILog>()
				.SingleInstance();

			builder.RegisterType<LocalCharacterDataRepository>()
				.As<ICharacterDataRepository>()
				.SingleInstance();

			builder.RegisterType<AuthenticationTokenRepository>()
				.As<IAuthTokenRepository>()
				.As<IReadonlyAuthTokenRepository>()
				.SingleInstance();

			builder.RegisterType<GlobalEntityResourceLockingPolicy>()
				.AsSelf()
				.AsImplementedInterfaces()
				.SingleInstance();
			
			//Handlers aren't needed for all scenes, but for most.
			//TODO: We should expose SceneTypeCreatable or whatever on handlers
			builder.RegisterModule(new GameClientMessageHandlerAutofacModule(Scene));

			builder.RegisterModule(new EngineInterfaceRegisterationModule((int)Scene, GetType().Assembly));
			builder.RegisterModule(new UIDependencyRegisterationModule<UnityUIRegisterationKey>());

			//builder.RegisterModule<EntityMappableRegisterationModule<ObjectGuid>>();
			RegisterEntityContainers(builder);

			builder.Register<IServiceDiscoveryService>(context => RestService.For<IServiceDiscoveryService>(ServiceDiscoveryUrl))
				.As<IServiceDiscoveryService>()
				.SingleInstance();
		}

		private static void RegisterEntityContainers(ContainerBuilder builder)
		{
			//HelloKitty: We actually have to do this manually, and not use GladerEssentials because we use simplified interfaces.
			//The below is kinda a hack to register the non-generic types in the
			//removabale collection
			List<IEntityCollectionRemovable<ObjectGuid>> removableComponentsList = new List<IEntityCollectionRemovable<ObjectGuid>>(10);

			builder.RegisterGeneric(typeof(EntityGuidDictionary<>))
				.AsSelf()
				.As(typeof(IReadonlyEntityGuidMappable<>))
				.As(typeof(IEntityGuidMappable<>))
				.OnActivated(args =>
				{
					if(args.Instance is IEntityCollectionRemovable<ObjectGuid> removable)
						removableComponentsList.Add(removable);
				})
				.SingleInstance();

			//This will allow everyone to register the removable collection collection.
			builder.RegisterInstance(removableComponentsList)
				.As<IReadOnlyCollection<IEntityCollectionRemovable<ObjectGuid>>>()
				.AsSelf()
				.SingleInstance();
		}
	}
}