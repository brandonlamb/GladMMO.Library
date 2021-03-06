﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Logging;
using Glader.Essentials;
using GladNet;
using Nito.AsyncEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GladMMO
{
	//This just selects the character as soon as one is clicked.
	[AdditionalRegisterationAs(typeof(IServerRequestedSceneChangeEventSubscribable))]
	[SceneTypeCreateGladMMO(GameSceneType.CharacterSelection)]
	public sealed class CharacterSelectionSelectCharacterImmediatelyOnButtonClickedEventListener : ButtonClickedEventListener<IEnterWorldButtonClickedEventSubscribable>, IServerRequestedSceneChangeEventSubscribable
	{
		private ILocalCharacterDataRepository CharacterData { get; }

		private ILog Logger { get; }

		/// <inheritdoc />
		public event EventHandler<ServerRequestedSceneChangeEventArgs> OnServerRequestedSceneChange;

		private NetworkEntityGuid SelectedCharacterGuid { get; set; }

		private ICharacterService CharacterServiceQueryable { get; }

		/// <inheritdoc />
		public CharacterSelectionSelectCharacterImmediatelyOnButtonClickedEventListener(
			IEnterWorldButtonClickedEventSubscribable enterWorldButtonClickableEventSubscribable,
			[NotNull] ICharacterSelectionButtonClickedEventSubscribable subscriptionService, 
			[NotNull] ILocalCharacterDataRepository characterData, 
			[NotNull] ILog logger,
			[NotNull] ICharacterService characterServiceQueryable)
			: base(enterWorldButtonClickableEventSubscribable)
		{
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterServiceQueryable = characterServiceQueryable ?? throw new ArgumentNullException(nameof(characterServiceQueryable));

			//Manually rig up.
			subscriptionService.OnCharacterButtonClicked += OnCharacterButtonClicked;
		}

		/// <inheritdoc />
		protected void OnCharacterButtonClicked(object source, CharacterButtonClickedEventArgs args)
		{
			SelectedCharacterGuid = args.CharacterGuid;
		}

		protected override void OnEventFired(object source, ButtonClickedEventArgs args)
		{
			args.Button.IsInteractable = false;

			UnityAsyncHelper.UnityMainThreadContext.PostAsync(async () =>
			{
				if(SelectedCharacterGuid == null)
				{
					Logger.Error($"Tried to enter the world without any selected character guid.");
					return;
				}

				//We do this before sending the player login BECAUSE of a race condition that can be caused
				//since I actually KNOW this event should disable networking. We should not handle messages in this scene after this point basically.
				//TODO: Don't hardcode this scene.
				OnServerRequestedSceneChange?.Invoke(this, new ServerRequestedSceneChangeEventArgs((PlayableGameScene) 2));

				CharacterSessionEnterResponse enterResponse = await CharacterServiceQueryable.TryEnterSession(SelectedCharacterGuid.EntityId);

				if (Logger.IsDebugEnabled)
					Logger.Debug($"Character Session Entry Response: {enterResponse.ResultCode}.");

				if (!enterResponse.isSuccessful)
					if (Logger.IsErrorEnabled)
						Logger.Error($"Failed to enter CharacterSession for Entity: {SelectedCharacterGuid} Reason: {enterResponse.ResultCode}");

				//TODO: handle character session failure
				CharacterData.UpdateCharacterId(SelectedCharacterGuid.EntityId);

				//TODO: Use the scene manager service.
				//TODO: Don't hardcode scene ids. Don't load scenes directly.
				SceneManager.LoadSceneAsync(GladMMOClientConstants.WORLD_DOWNLOAD_SCENE_NAME).allowSceneActivation = true;
			});
		}
	}
}
