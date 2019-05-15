﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreecraftCore;
using Glader.Essentials;

namespace GladMMO
{
	/// <summary>
	/// Generic dictionary with <see cref="ObjectGuid"/> key types.
	/// </summary>
	/// <typeparam name="TValue">Value type.</typeparam>
	public class EntityGuidDictionary<TValue> : Glader.Essentials.EntityGuidDictionary<ObjectGuid, TValue>, IReadonlyEntityGuidMappable<TValue>, IEntityGuidMappable<TValue>
	{
		public EntityGuidDictionary()
			: base(ObjectGuidEqualityComparer<ObjectGuid>.Instance)
		{

		}
	}
}
