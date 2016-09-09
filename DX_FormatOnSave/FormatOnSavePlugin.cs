using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using DevExpress.CodeAnalysis.Workspaces;
using DevExpress.CodeRush.Engine;
using DevExpress.CodeRush.Platform.Diagnostics;
using DevExpress.CodeRush.Platform.Options;
using DevExpress.CodeRush.TextEditor;
using Microsoft.CodeAnalysis.Text;

namespace DX_FormatOnSave
{
	/// <summary>
	/// DXCore plugin to format documents when they are saved.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The general algorithm is:
	/// </para>
	/// <list type="bullet">
	/// <item>
	/// <term>User clicks the save button.</term>
	/// </item>
	/// <item>
	/// <term>One or more document saved events get raised.</term>
	/// </item>
	/// <item>
	/// <term>After the VS undo manager has done all its work to save the document, we start the format process:</term>
	/// </item>
	/// <item>
	/// <term>If the document is already in the process of getting formatted, exit or we get in an endless format/save/format/save loop.</term>
	/// </item>
	/// <item>
	/// <term>The document gets added to a list of documents being formatted.</term>
	/// </item>
	/// <item>
	/// <term>The document gets formatted and saved (which will raise the event again, hence the need for tracking).</term>
	/// </item>
	/// <item>
	/// <term>The document gets removed from the list of documents being formatted.</term>
	/// </item>
	/// </list>
	/// </remarks>
	public class FormatOnSavePlugin : StandardPlugIn
	{
		/// <summary>
		/// Raises events for documents being saved.
		/// </summary>
		private RunningDocumentTableEventProvider _docEvents = null;

		[Import]
		public IFormattingServices Formatting { get; set; }

		/// <summary>
		/// Gets or sets the options for the plugin.
		/// </summary>
		/// <value>
		/// The <see cref="DX_FormatOnSave.OptionSet"/> that the current plugin
		/// is using.
		/// </value>
		public OptionSet Options { get; set; }

		[Import]
		public IOptionsStorageService OptionsStorage { get; set; }

		/// <summary>
		/// Gets the full name/path for a document even if it's closed.
		/// </summary>
		/// <param name="doc">
		/// The document for which the name should be retrieved.
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> with the document full name/path.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="doc" /> is <see langword="null" />.
		/// </exception>
		public static string GetDocFullName(ITextDocument doc)
		{
			if (doc == null)
			{
				throw new ArgumentNullException(nameof(doc));
			}

			return null;
			//var docFullName = doc.FullName;
			//if (docFullName == null && _fullNameFieldInfo != null)
			//{
			//	// The document name will be null if the file is closed, but the
			//	// private instance property will still have the filename we need
			//	// to re-format.
			//	docFullName = _fullNameFieldInfo.GetValue(doc) as string;
			//}
			//return docFullName;
		}
		/// <summary>
		/// Finalizes the plug in.
		/// </summary>
		public override void FinalizePlugIn()
		{
			this._docEvents.Dispose();
			this._docEvents = null;
			if (this.OptionsStorage != null)
			{
				this.OptionsStorage.OptionsChanged -= this.FormatOnSavePlugin_OptionsChanged;
			}

			base.FinalizePlugIn();
		}

		/// <summary>
		/// Formats and re-saves a document.
		/// </summary>
		/// <param name="doc">The document to format.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="doc" /> is <see langword="null" />.
		/// </exception>
		public void FormatDocument(ITextDocument doc)
		{
			if (doc == null)
			{
				throw new ArgumentNullException(nameof(doc));
			}

			//// Issue #147: You have to handle documents that were save-on-close
			//// differently than documents that are currently open.
			//// Closed documents return null for the full name because they've
			//// been disposed and there's no backing VS DocumentObject.
			//var isClosed = doc.FullName == null;
			//var docFullName = GetDocFullName(doc);

			//// The TextBuffers collection will have the document if it's open.
			//var textBuffer = doc.TextBuffer;
			//if (textBuffer == null && docFullName != null)
			//{
			//	// If the document has already closed, we can re-open it in the
			//	// background to format it.
			//	textBuffer = CodeRush.TextBuffers.Open(docFullName);
			//}
			//if (textBuffer == null)
			//{
			//	Log.SendError("Unable to load text buffer for formatting document '{0}'", docFullName);
			//	return;
			//}

			//// Format the document and if it's successful write the changes back.
			//this.Formatting.Format(doc, new TextSpan(0, textBuffer.Length));
			//if (isClosed)
			//{
			//	UpdateClosedDocument(docFullName, textBuffer.Text);
			//}
			//else
			//{
			//	doc.Save(docFullName);
			//}
		}

		/// <summary>
		/// Initializes the plug in.
		/// </summary>
		public override void InitializePlugIn()
		{
			base.InitializePlugIn();
			if (this.OptionsStorage != null)
			{
				this.OptionsStorage.OptionsChanged += this.FormatOnSavePlugin_OptionsChanged;
			}

			this._docEvents = new RunningDocumentTableEventProvider();
			this._docEvents.Initialize();
			this._docEvents.Saving += this.DocumentSaving;
			this.RefreshOptions();
		}

		/// <summary>
		/// Determines whether a document should be formatted based on the provided
		/// language ID and the user's selected options.
		/// </summary>
		/// <param name="language">The language ID for the document in question.</param>
		/// <returns>
		/// <see langword="true" /> if the user elected to format documents of the
		/// given language; <see langword="false" /> if not.
		/// </returns>
		public bool LanguageSelectedForFormatting(string language)
		{
			if (string.IsNullOrEmpty(language))
			{
				return false;
			}

			try
			{
				var enumValue = (DocumentLanguages)Enum.Parse(typeof(DocumentLanguages), language, true);
				return this.Options.LanguagesToFormat.Contains(enumValue);
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Refreshes the set of options being used by this plugin.
		/// </summary>
		public void RefreshOptions()
		{
			this.Options = this.OptionsStorage.GetOptions<OptionSet>();
		}

		/// <summary>
		/// Saves/overwrites the contents of a closed document.
		/// </summary>
		/// <param name="path">
		/// The path to the document to overwrite.
		/// </param>
		/// <param name="content">
		/// The new content that should be in the file.
		/// </param>
		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "The file stream gets disposed through the using and inside the writer, but it's OK to double-dispose a stream.")]
		private static void UpdateClosedDocument(string path, string content)
		{
			// If the document is closed, we have to manually save
			// it using an exclusive locking filestream. Without the
			// locking, VS may not have fully written the doc yet and we
			// end up in a race condition where the file contents
			// get all mangled.
			using (var stream = File.Open(path, FileMode.Truncate, FileAccess.Write, FileShare.None))
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(content);
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching any problems that go on during post-save formatting.")]
		private void DocumentSaving(object sender, DocumentEventArgs e)
		{
			var doc = e.Document;
			if (!this.DocumentShouldBeFormatted(doc))
			{
				return;
			}

			lock (this._listSync)
			{
				// If we're already in the process of formatting the doc, bail.
				if (this._docsBeingFormatted.Contains(doc))
				{
					return;
				}

				// We're not already formatting the doc, so add it to the list.
				this._docsBeingFormatted.Add(doc);
			}

			// Execute document formatting after the VsLinkedUndoTransactionManager.CloseLinkedUndo
			// method has finished so we don't mess up IntelliSense or Undo.
			// http://www.devexpress.com/Support/Center/Question/Details/B223163
			SynchronizationContext.Current.Post(state =>
				{
					try
					{
						this.FormatDocument(e.Document);
					}
					catch (Exception ex)
					{
						// Issue #147: Unhandled exception while attempting to format the document.
						// This happens if the user closes the document and has
						// unsaved changes - they elect to save on close and this
						// will run AFTER the doc is already closed so we can't
						// cause the document to focus and can't format.
						Log.SendException("Error formatting document on save.", ex);
					}
					finally
					{
						lock (this._listSync)
						{
							// Formatting is done; remove the marker.
							this._docsBeingFormatted.Remove(doc);
						}
					}
				}, null);


		}

		/// <summary>
		/// Determines whether a given document should have formatting executed on it.
		/// </summary>
		/// <param name="doc">The document to check.</param>
		/// <returns>
		/// <see langword="true" /> if formatting is enabled and the document
		/// is one of the selected languages to format; <see langword="false" />
		/// otherwise.
		/// </returns>
		private bool DocumentShouldBeFormatted(ITextDocument doc)
		{
			// If the user disabled the plugin, bail.
			if (!this.Options.Enabled)
			{
				return false;
			}

			// If the document isn't text or an enabled language, bail.
			return this.LanguageSelectedForFormatting(doc.GetLanguage());
		}

		private void FormatOnSavePlugin_OptionsChanged(object sender, OptionsChangedEventArgs ea)
		{
			this.RefreshOptions();
		}

		/// <summary>
		/// Reflection lookup for a document full name. Used if the document has
		/// already been closed so we can find the original path.
		/// </summary>
		//private static readonly FieldInfo _fullNameFieldInfo = typeof(Document).GetField("_FullName", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);

		/// <summary>
		/// Keeps track of which documents are currently being formatted so
		/// we don't end up in an endless loop of format/save.
		/// </summary>
		private List<ITextDocument> _docsBeingFormatted = new List<ITextDocument>();

		/// <summary>
		/// Synchronizes access to the list of docs being formatted.
		/// </summary>
		private object _listSync = new object();
	}
}