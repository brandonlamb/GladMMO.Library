﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GladMMO
{
	//TODO: Refactor with server-side input generator
	public class CharacterControllerInputMovementGenerator : BaseMovementGenerator<PositionChangeMovementData>
	{
		private Vector3 CachedMovementDirection;

		//TODO: We shouldn't do this here
		private float DefaultPlayerSpeed = 3.0f;

		private long LastMovementUpdateTime { get; set; }

		protected Lazy<CharacterController> Controller { get; }

		public CharacterControllerInputMovementGenerator(PositionChangeMovementData movementData, [NotNull] Lazy<CharacterController> controller) 
			: base(movementData)
		{
			Controller = controller ?? throw new ArgumentNullException(nameof(controller));
		}

		protected override Vector3 Start([NotNull] GameObject entity, long currentTime)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));
			if (Controller == null) throw new ArgumentNullException(nameof(Controller));

			//Now, we should also create the movement direction
			CachedMovementDirection = new Vector3(MovementData.Direction.x, 0.0f, MovementData.Direction.y).normalized;
			LastMovementUpdateTime = MovementData.TimeStamp;

			return entity.transform.position;
		}

		/// <inheritdoc />
		protected override Vector3 InternalUpdate(GameObject entity, long currentTime)
		{
			//TODO: We should have real handling at some point.
			float diff = DiffFromStartTime(currentTime);

			//gravity
			//Don't need to subtract the cached direction Y because it should be 0, or treated as 0.
			CachedMovementDirection.y = (-9.8f * diff);
			Controller.Value.Move(entity.transform.worldToLocalMatrix.inverse * CachedMovementDirection * diff);

			//Our new last movement time is now the current time.
			LastMovementUpdateTime = currentTime;

			return entity.transform.position;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float DiffFromStartTime(long currentTime)
		{
			return (float)(currentTime - LastMovementUpdateTime) / TimeSpan.TicksPerSecond;
		}
	}
}