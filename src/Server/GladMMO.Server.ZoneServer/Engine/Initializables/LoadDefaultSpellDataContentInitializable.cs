﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glader.Essentials;

namespace GladMMO
{
	//TODO: Consolidate with clientside loader.
	[ServerSceneTypeCreate(ServerSceneType.Default)]
	public sealed class LoadDefaultSpellDataContentInitializable : IGameInitializable
	{
		private ISpellEntryDataServiceClient SpellDataService { get; }

		private ISpellDataCollection SpellDataCollection { get; }

		public LoadDefaultSpellDataContentInitializable([NotNull] ISpellEntryDataServiceClient spellDataService, [NotNull] ISpellDataCollection spellDataCollection)
		{
			SpellDataService = spellDataService ?? throw new ArgumentNullException(nameof(spellDataService));
			SpellDataCollection = spellDataCollection ?? throw new ArgumentNullException(nameof(spellDataCollection));
		}

		public async Task OnGameInitialized()
		{
			SpellDefinitionCollectionResponseModel model = await SpellDataService.GetDefaultSpellDataAsync();

			foreach (var spell in model.SpellEntries)
				SpellDataCollection.AddSpellDefinition(spell);

			foreach (var spellEffect in model.SpellEffects)
				SpellDataCollection.AddSpellEffectDefinition(spellEffect);
		}
	}
}
