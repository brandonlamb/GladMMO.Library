﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GladMMO
{
	public interface IActionBarButtonStateChangedEventSubscribable
	{
		event EventHandler<ActionBarButtonStateChangedEventArgs> OnActionBarButtonStateChanged;
	}

	public sealed class ActionBarButtonStateChangedEventArgs : EventArgs
	{
		/// <summary>
		/// The action bar index.
		/// </summary>
		public ActionBarIndex Index { get; }

		public ActionBarIndexType ActionType { get; }

		public int ActionId { get; }

		public ActionBarButtonStateChangedEventArgs(ActionBarIndex index, ActionBarIndexType actionType, int actionId)
		{
			if (!Enum.IsDefined(typeof(ActionBarIndex), index)) throw new InvalidEnumArgumentException(nameof(index), (int) index, typeof(ActionBarIndex));
			if (actionId <= 0) throw new ArgumentOutOfRangeException(nameof(actionId));

			Index = index;
			ActionType = actionType;
			ActionId = actionId;
		}

		public ActionBarButtonStateChangedEventArgs(ActionBarIndex index)
		{
			if (!Enum.IsDefined(typeof(ActionBarIndex), index)) throw new InvalidEnumArgumentException(nameof(index), (int)index, typeof(ActionBarIndex));

			ActionType = ActionBarIndexType.Empty;
			Index = index;
		}
	}
}
