﻿using System;
using DevExpress.CodeRush.Engine;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Event arguments for document-related events.
	/// </summary>
	public class DocumentEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the document for which the event occurred.
		/// </summary>
		/// <value>
		/// A <see cref="DevExpress.CodeRush.Core.Document"/> for which the
		/// event is being raised.
		/// </value>
		public Document Document { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DX_FormatOnSave.DocumentEventArgs" /> class.
		/// </summary>
		/// <param name="doc">The document for which the event is occurring.</param>
		public DocumentEventArgs(Document doc)
		{
			this.Document = doc;
		}
	}
}
