﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GladMMO
{
	[JsonObject]
	public sealed class PlayerSpawnPointInstanceModel
	{
		[JsonProperty]
		[JsonRequired]
		public int SpawnPointId { get; private set; }

		/// <summary>
		/// The initial spawn position for the entry.
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		[JsonConverter(typeof(Vector3Converter))] //TODO: Make custom attribute
		public Vector3 InitialPosition { get; private set; }

		/// <summary>
		/// Creature's initial rotation around the Y-Axis.
		/// </summary>
		[JsonRequired]
		[JsonProperty]
		public float YAxisRotation { get; private set; }

		[JsonProperty]
		[JsonRequired]
		public bool isReserved { get; private set; }

		/// <inheritdoc />
		public PlayerSpawnPointInstanceModel(int spawnPointId, Vector3 initialPosition, float yRotation, bool isReserved)
		{
			if (spawnPointId <= 0) throw new ArgumentOutOfRangeException(nameof(spawnPointId));

			SpawnPointId = spawnPointId;
			InitialPosition = initialPosition;
			YAxisRotation = yRotation;
			this.isReserved = isReserved;
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		protected PlayerSpawnPointInstanceModel()
		{
			
		}
	}
}
