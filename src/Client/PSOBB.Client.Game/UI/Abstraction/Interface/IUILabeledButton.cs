﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladMMO;

namespace GladMMO
{
	/// <summary>
	/// UI combination of <see cref="IUIButton"/>
	/// and a label <see cref="IUIText"/>.
	/// </summary>
	public interface IUILabeledButton : IUIButton, IUIText
	{
		
	}
}
