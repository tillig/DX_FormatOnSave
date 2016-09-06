using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.CodeRush.Platform.Diagnostics;
using DevExpress.CodeRush.VisualStudio;
using DevExpress.CodeRush.Win32;
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
		/// Initializes a new instance of the <see cref="RunningDocumentTableConnectionPoint"/> class.
		/// </summary>
		/// <param name="visualStudioServices">
		/// The Visual Studio services exposing the running document table.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="visualStudioServices" /> is <see langword="null" />.
		/// </exception>
		public RunningDocumentTableConnectionPoint(IVsServices visualStudioServices)
		{
			if (visualStudioServices == null)
			{
				throw new ArgumentNullException(nameof(visualStudioServices));
			}

			this.VisualStudioServices = visualStudioServices;
		}

		/// <summary>
		/// Gets an indicator of whether the connection point is attached to a running document table.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the connection point is attached to the
		/// running document table; <see langword="false" /> if not.
		/// </value>
		public bool Connected { get; private set; }

		/// <summary>
		/// Gets the Visual Studio services implementation.
		/// </summary>
		/// <value>
		/// The Visual Studio services exposing the running document table.
		/// </value>
		public IVsServices VisualStudioServices { get; private set; }

		/// <summary>
		/// Registers an object to handle running document table events.
		/// </summary>
		/// <param name="sink">The object that will handle the document table events.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="sink" /> is <see langword="null" />.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = nameof(IVsRunningDocTableEvents), Justification = "IVsRunningDocTableEvents is the name of the specific object type referenced in the error message.")]
		public void Advise(object sink)
		{
			if (sink == null)
			{
				throw new ArgumentNullException(nameof(sink));
			}

			var handler = sink as IVsRunningDocTableEvents;
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

					var hresult = this.VisualStudioServices.VsRunningDocumentTable.AdviseRunningDocTableEvents(handler, out this._cookie);
					if (HResult.Failed(hresult))
					{
						Log.SendErrorWithStackTrace("Failed to advise IVsRunningDocTableEvents sink (0x{0:x8}).", hresult);
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
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = nameof(IVsRunningDocTableEvents), Justification = "IVsRunningDocTableEvents is the name of the specific object type referenced in the error message.")]
		public void UnAdvise()
		{
			if (this.Connected)
			{
				lock (this._syncroot)
				{
					if (!this.Connected)
					{
						return;
					}

					var hresult = this.VisualStudioServices.VsRunningDocumentTable.UnadviseRunningDocTableEvents(this._cookie);
					if (HResult.Failed(hresult))
					{
						Log.SendErrorWithStackTrace("Failed to unadvise IVsRunningDocTableEvents sink (0x{0:x8}).", hresult);
						return;
					}

					this.Connected = false;
					this._cookie = 0;
				}
			}
		}

		/// <summary>
		/// Lock object for connect/disconnect synchronization.
		/// </summary>
		private object _syncroot = new object();
	}
}
