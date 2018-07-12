﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guardians
{
	/// <summary>
	/// Generic dictionary with <see cref="NetworkEntityGuid"/> key types.
	/// </summary>
	/// <typeparam name="TValue">Value type.</typeparam>
	public class EntityGuidDictionary<TValue> : Dictionary<NetworkEntityGuid, TValue>, IReadonlyEntityGuidMappable<TValue>, IEntityGuidMappable<TValue>
	{
		public EntityGuidDictionary()
			: base(NetworkGuidEqualityComparer<NetworkEntityGuid>.Instance)
		{

		}

		public EntityGuidDictionary(int capacity)
			: base(capacity, NetworkGuidEqualityComparer<NetworkEntityGuid>.Instance)
		{

		}

		public EntityGuidDictionary(IDictionary<NetworkEntityGuid, TValue> dictionary)
			: base(dictionary, NetworkGuidEqualityComparer<NetworkEntityGuid>.Instance)
		{

		}
	}
}
