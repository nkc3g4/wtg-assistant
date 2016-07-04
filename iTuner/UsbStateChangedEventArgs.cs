//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

namespace iTuner
{
	using System;


	/// <summary>
	/// Defines the signature of an event handler method for internally handling 
	/// USB device state changes.
	/// </summary>
	/// <param name="e">A description of the device state change.</param>

	public delegate void UsbStateChangedEventHandler (UsbStateChangedEventArgs e);

	
	/// <summary>
	/// Define the arguments passed internally from the DriverWindow to the KeyManager
	/// handlers.
	/// </summary>

	public class UsbStateChangedEventArgs : EventArgs
	{

		/// <summary>
		/// Initialize a new instance with the specified state and disk.
		/// </summary>
		/// <param name="state">The state change code.</param>
		/// <param name="disk">The USB disk description.</param>

		public UsbStateChangedEventArgs (UsbStateChange state, UsbDisk disk)
		{
			this.State = state;
			this.Disk = disk;
		}


		/// <summary>
		/// Gets the USB disk information.
		/// </summary>

		public UsbDisk Disk
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the state change code.
		/// </summary>

		public UsbStateChange State
		{
			get;
			private set;
		}
	}
}
