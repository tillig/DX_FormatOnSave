using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.DXCore.Interop;
using DevExpress.DXCore.Win32;
using Microsoft.VisualStudio.Shell.Interop;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Manages connections to the running document table for use in handling
	/// events that aren't exposed first-class via DXCore.
	/// </summary>
	public class RunningDocumentTableConnectionPoint : IComConnectionPoint
	{
		/// <summary>
		/// The event registration cookie.
		/// </summary>
		private uint _cookie = 0;

		/// <summary>
		/// Lock object for connect/disconnect synchronization.
		/// </summary>
		private object _syncroot = new object();

		/// <summary>
		/// Gets an indicator of whether the connection point is attached to a running document table.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the connection point is attached to the
		/// running document table; <see langword="false" /> if not.
		/// </value>
		public bool Connected { get; private set; }

		/// <summary>
		/// Registers an object to handle running document table events.
		/// </summary>
		/// <param name="sink">The object that will handle the document table events.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="sink" /> is <see langword="null" />.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IVsRunningDocTableEvents", Justification = "IVsRunningDocTableEvents is the name of the specific object type referenced in the error message.")]
		public void Advise(object sink)
		{
			if (sink == null)
			{
				throw new ArgumentNullException("sink");
			}
			IVsRunningDocTableEvents handler = sink as IVsRunningDocTableEvents;
			if (handler == null)
			{
				throw new ArgumentException("The event sink must implement IVsRunningDocTableEvents.");
			}
			if (!this.Connected)
			{
				lock (this._syncroot)
				{
					if (this.Connected)
					{
						return;
					}

					int hresult = VisualStudioServices.VsRunningDocumentTable.AdviseRunningDocTableEvents(handler, out this._cookie);
					if (HResult.Failed(hresult))
					{
						DevExpress.CodeRush.Diagnostics.Interop.Log.SendErrorWithStackTrace("Failed to advise IVsRunningDocTableEvents sink (0x{0:x8}).", hresult);
						return;
					}

					this.Connected = true;
				}
			}
		}

		/// <summary>
		/// Unregisters the previously attached handler.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "unadvise", Justification = "Unadvise is the name of the operation per the interface implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IVsRunningDocTableEvents", Justification = "IVsRunningDocTableEvents is the name of the specific object type referenced in the error message.")]
		public void Unadvise()
		{
			if (this.Connected)
			{
				lock (this._syncroot)
				{
					if (!this.Connected)
					{
						return;
					}

					int hresult = VisualStudioServices.VsRunningDocumentTable.UnadviseRunningDocTableEvents(this._cookie);
					if (HResult.Failed(hresult))
					{
						DevExpress.CodeRush.Diagnostics.Interop.Log.SendErrorWithStackTrace("Failed to unadvise IVsRunningDocTableEvents sink (0x{0:x8}).", hresult);
						return;
					}

					this.Connected = false;
					this._cookie = 0;
				}
			}
		}
	}
}
