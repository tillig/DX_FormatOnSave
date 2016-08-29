using System;
using DevExpress.CodeRush.TextEditor;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Event arguments for document-related events.
	/// </summary>
	public class DocumentEventArgs : EventArgs
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="DX_FormatOnSave.DocumentEventArgs" /> class.
		/// </summary>
		/// <param name="doc">The document for which the event is occurring.</param>
		public DocumentEventArgs(ITextDocument doc)
		{
			this.Document = doc;
		}

		/// <summary>
		/// Gets the document for which the event occurred.
		/// </summary>
		/// <value>
		/// A <see cref="ITextDocument"/> for which the
		/// event is being raised.
		/// </value>
		public ITextDocument Document { get; private set; }
	}
}
