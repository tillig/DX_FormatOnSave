using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.CodeRush.Core;
using DevExpress.DXCore.Interop;
using DevExpress.DXCore.Win32;
using Microsoft.VisualStudio.Shell.Interop;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Provides events from the running document table. These events do not have
	/// a first-class DXCore interface.
	/// </summary>
	public class RunningDocumentTableEventProvider : IVsRunningDocTableEvents3, IDisposable
	{
		/// <summary>
		/// Raised after an attribute of a document in changes.
		/// </summary>
		public event EventHandler<DocumentEventArgs> AttributeChanged;

		/// <summary>
		/// Raised after a document window is hidden.
		/// </summary>
		public event EventHandler<DocumentEventArgs> DocumentWindowHidden;

		/// <summary>
		/// Raised before displaying a document window.
		/// </summary>
		public event EventHandler<DocumentEventArgs> DocumentWindowShowing;

		/// <summary>
		/// Raised after application of the first read or edit lock to a document.
		/// </summary>
		public event EventHandler<DocumentEventArgs> FirstDocumentLockApplied;

		/// <summary>
		/// Raised before releasing the last read or edit lock on a document.
		/// </summary>
		public event EventHandler<DocumentEventArgs> LastDocumentUnlockReleasing;

		/// <summary>
		/// Raised after a document is saved.
		/// </summary>
		public event EventHandler<DocumentEventArgs> Saved;

		/// <summary>
		/// Raised before a document is saved.
		/// </summary>
		public event EventHandler<DocumentEventArgs> Saving;

		/// <summary>
		/// Indicates whether the object has already been disposed.
		/// </summary>
		private bool _disposed = false;

		/// <summary>
		/// Indicates whether the provider has been initialized.
		/// </summary>
		private bool _initialized = false;

		/// <summary>
		/// Connector to the Running Document Table.
		/// </summary>
		private IComConnectionPoint _rdtConnector = null;

		/// <summary>
		/// Lock object for synchronizing intialization.
		/// </summary>
		private object _syncroot = new object();

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="RunningDocumentTableEventProvider"/> is reclaimed by garbage collection.
		/// </summary>
		~RunningDocumentTableEventProvider()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing,
		/// or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources;
		/// <see langword="false" /> to release only unmanaged resources.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing)
			{
				if (this._rdtConnector != null)
				{
					this._rdtConnector.Unadvise();
					this._rdtConnector = null;
				}
				this._initialized = false;
			}
			this._disposed = true;
		}

		/// <summary>
		/// Gets the native document from the running doc table given a document cookie.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document to be located.</param>
		/// <returns>
		/// The object representing the document if found, or <see langword="null" /> if not.
		/// </returns>
		public static Document GetDocumentFromCookie(uint docCookie)
		{
			uint pgrfRDTFlags;
			uint pdwReadLocks;
			uint pdwEditLocks;
			string pbstrMkDocument; // This will be the full file path to the document.
			IVsHierarchy ppHier;
			uint pitemid;
			IntPtr ppunkDocData;
			int hresult = VisualStudioServices.VsRunningDocumentTable.GetDocumentInfo(
				docCookie,
				out pgrfRDTFlags,
				out pdwReadLocks,
				out pdwEditLocks,
				out pbstrMkDocument,
				out ppHier,
				out pitemid,
				out ppunkDocData);
			if (HResult.Failed(hresult))
			{
				return null;
			}
			var foundDoc = CodeRush.Documents.Get(pbstrMkDocument);
			// This is how you'd find the DTE document if you wanted it.
			//var foundDoc = VisualStudioServices.DTE.Documents.OfType<EnvDTE.Document>().Single(document => document.FullName.Equals(pbstrMkDocument));
			return foundDoc;
		}

		/// <summary>
		/// Initializes the event provider.
		/// </summary>
		public virtual void Initialize()
		{
			if (!this._initialized)
			{
				lock (this._syncroot)
				{
					if (this._initialized)
					{
						return;
					}
					this._rdtConnector = new RunningDocumentTableConnectionPoint();
					this._rdtConnector.Advise(this);
					this._initialized = true;
				}
			}
		}

		/// <summary>
		/// Called after an attribute of a document in the Running Document Table (RDT) changes.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have changed.</param>
		/// <param name="grfAttribs">
		/// Flags corresponding to the changed attributes. Values are taken from
		/// the <see cref="Microsoft.VisualStudio.Shell.Interop.__VSRDTATTRIB" />
		/// enumeration.
		/// </param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
		{
			return this.RaiseEvent(docCookie, this.AttributeChanged);
		}

		/// <summary>
		/// Called after a document attribute is changed. This is an advanced
		/// version of the <see cref="DX_FormatOnSave.RunningDocumentTableEventProvider.OnAfterAttributeChange"/>
		/// method.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have changed.</param>
		/// <param name="grfAttribs">
		/// Flags corresponding to the changed attributes. Values are taken from
		/// the <see cref="Microsoft.VisualStudio.Shell.Interop.__VSRDTATTRIB" />
		/// enumeration.
		/// </param>
		/// <param name="pHierOld">
		/// The <see cref="Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
		/// interface that previously owned the document.
		/// </param>
		/// <param name="itemidOld">
		/// The previous item identifier. This is a unique identifier or it can
		/// be one of the following values: <c>Microsoft.VisualStudio.VsConstants.VSITEMID_NIL</c>,
		/// <c>Microsoft.VisualStudio.VsConstants.VSITEMID_ROOT</c>, or
		/// <c>Microsoft.VisualStudio.VsConstants.VSITEMID_SELECTION</c>.
		/// </param>
		/// <param name="pszMkDocumentOld">Name of the old document.</param>
		/// <param name="pHierNew">
		/// The current <see cref="Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" />
		/// interface that owns the document.
		/// </param>
		/// <param name="itemidNew">
		/// The new item identifier. This is a unique identifier or it can
		/// be one of the following values: <c>Microsoft.VisualStudio.VsConstants.VSITEMID_NIL</c>,
		/// <c>Microsoft.VisualStudio.VsConstants.VSITEMID_ROOT</c>, or
		/// <c>Microsoft.VisualStudio.VsConstants.VSITEMID_SELECTION</c>.
		/// </param>
		/// <param name="pszMkDocumentNew">Name of the new document.</param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
		{
			return this.RaiseEvent(docCookie, this.AttributeChanged);
		}

		/// <summary>
		/// Called after a document window is hidden.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have been changed.</param>
		/// <param name="pFrame">
		/// The <see cref="Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame" />
		/// interface representing the document window's frame.
		/// </param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
		{
			return this.RaiseEvent(docCookie, this.DocumentWindowHidden);
		}

		/// <summary>
		/// Called after application of the first lock of the specified type to a document in the Running Document Table (RDT).
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have been changed.</param>
		/// <param name="dwRDTLockType">
		/// The document lock type. Values are taken from the
		/// <see cref="Microsoft.VisualStudio.Shell.Interop._VSRDTFLAGS" /> enumeration.
		/// </param>
		/// <param name="dwReadLocksRemaining">Specifies the number of remaining read locks.</param>
		/// <param name="dwEditLocksRemaining">Specifies the number of remaining edit locks.</param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
		{
			return this.RaiseEvent(docCookie, this.FirstDocumentLockApplied);
		}

		/// <summary>
		/// Called after a document in the Running Document Table (RDT) is saved.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have been changed.</param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnAfterSave(uint docCookie)
		{
			return this.RaiseEvent(docCookie, this.Saved);
		}

		/// <summary>
		/// Called before displaying a document window.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have been changed.</param>
		/// <param name="fFirstShow">Non-zero (TRUE) if the doc window is being displayed for the first time.</param>
		/// <param name="pFrame">
		/// The <see cref="Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame" />
		/// interface object representing the frame that contains the document's window.
		/// </param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
		{
			return this.RaiseEvent(docCookie, this.DocumentWindowShowing);
		}

		/// <summary>
		/// Called before releasing the last lock of the specified type on the specified document in the Running Document Table (RDT).
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document whose attributes have been changed.</param>
		/// <param name="dwRDTLockType">
		/// Type of lock being released. Values are taken from the
		/// <see cref="Microsoft.VisualStudio.Shell.Interop._VSRDTFLAGS" />
		/// enumeration.
		/// </param>
		/// <param name="dwReadLocksRemaining">Specifies the number of remaining read locks.</param>
		/// <param name="dwEditLocksRemaining">Specifies the number of remaining edit locks.</param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
		{
			return this.RaiseEvent(docCookie, this.LastDocumentUnlockReleasing);
		}

		/// <summary>
		/// Called before saving a document.
		/// </summary>
		/// <param name="docCookie">Abstract value representing the document about to be saved.</param>
		/// <returns>
		/// Always returns <see cref="DevExpress.DXCore.Win32.HResult.S_OK"/>
		/// </returns>
		public int OnBeforeSave(uint docCookie)
		{
			return this.RaiseEvent(docCookie, this.Saving);
		}

		/// <summary>
		/// Raises the specified event with the specified document and returns S_OK.
		/// </summary>
		/// <param name="docCookie">
		/// Abstract value representing the document to pass in the event arguments.
		/// </param>
		/// <param name="eventToRaise">
		/// The event to be raised.
		/// </param>
		/// <returns>
		/// Always returns S_OK.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "If an exception happens in an event handler we don't want to affect the event provider.")]
		private int RaiseEvent(uint docCookie, EventHandler<DocumentEventArgs> eventToRaise)
		{
			if (eventToRaise == null)
			{
				return HResult.S_OK;
			}
			var doc = GetDocumentFromCookie(docCookie);
			if (doc == null)
			{
				return HResult.S_OK;
			}
			try
			{
				var args = new DocumentEventArgs(doc);
				eventToRaise(this, args);
			}
			catch (Exception ex)
			{
				DevExpress.CodeRush.Diagnostics.Interop.Log.SendException("Error in raising event.", ex);
			}
			return HResult.S_OK;
		}
	}
}
