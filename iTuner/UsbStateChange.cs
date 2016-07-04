//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

namespace iTuner
{
	using System;


	/// <summary>
	/// Specifies the various state changes for USB disk devices.
	/// </summary>

	public enum UsbStateChange
	{

		/// <summary>
		/// A device has been added and is now available.
		/// </summary>

		Added,


		/// <summary>
		/// A device is about to be removed;
		/// allows consumers to intercept and deny the action.
		/// </summary>

		Removing,


		/// <summary>
		/// A device has been removed and is no longer available.
		/// </summary>

		Removed
	}
}