using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using DevExpress.CodeRush.Engine;
using DevExpress.CodeRush.Platform.Diagnostics;
using DevExpress.CodeRush.PlugInCore;

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
	public partial class FormatOnSavePlugin : StandardPlugIn
	{
		/// <summary>
		/// Reflection lookup for a document full name. Used if the document has
		/// already been closed so we can find the original path.
		/// </summary>
		private static readonly FieldInfo _fullNameFieldInfo = typeof(Document).GetField("_FullName", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);

		/// <summary>
		/// Keeps track of which documents are currently being formatted so
		/// we don't end up in an endless loop of format/save.
		/// </summary>
		private List<Document> _docsBeingFormatted = new List<Document>();

		/// <summary>
		/// Synchronizes access to the list of docs being formatted.
		/// </summary>
		private object _listSync = new object();

		/// <summary>
		/// Raises events for documents being saved.
		/// </summary>
		private RunningDocumentTableEventProvider _docEvents = null;

		/// <summary>
		/// Gets or sets the options for the plugin.
		/// </summary>
		/// <value>
		/// The <see cref="DX_FormatOnSave.OptionSet"/> that the current plugin
		/// is using.
		/// </value>
		public OptionSet Options { get; set; }

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
						FormatDocument(e.Document);
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
		private bool DocumentShouldBeFormatted(Document doc)
		{
			// If the user disabled the plugin, bail.
			if (!this.Options.Enabled)
			{
				return false;
			}

			// If the document isn't text or an enabled language, bail.
			TextDocument textDoc = doc as TextDocument;
			if (textDoc == null || !this.LanguageSelectedForFormatting(textDoc.Language))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Finalizes the plug in.
		/// </summary>
		public override void FinalizePlugIn()
		{
			this._docEvents.Dispose();
			this._docEvents = null;
			base.FinalizePlugIn();
		}

		/// <summary>
		/// Formats and re-saves a document.
		/// </summary>
		/// <param name="doc">The document to format.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="doc" /> is <see langword="null" />.
		/// </exception>
		public static void FormatDocument(Document doc)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}

			// Issue #147: You have to handle documents that were save-on-close
			// differently than documents that are currently open.
			// Closed documents return null for the full name because they've
			// been disposed and there's no backing VS DocumentObject.
			var isClosed = doc.FullName == null;
			var docFullName = GetDocFullName(doc);

			// The TextBuffers collection will have the document if it's open.
			var textBuffer = CodeRush.TextBuffers[docFullName];
			if (textBuffer == null && docFullName != null)
			{
				// If the document has already closed, we can re-open it in the
				// background to format it.
				textBuffer = CodeRush.TextBuffers.Open(docFullName);
			}
			if (textBuffer == null)
			{
				Log.SendError("Unable to load text buffer for formatting document '{0}'", docFullName);
				return;
			}

			// Format the document and if it's successful write the changes back.
			var result = textBuffer.Format(textBuffer.Range);
			if (result != FormatResult.Success)
			{
				Log.SendError("Unable to format document '{0}'.", docFullName);
			}
			if (isClosed)
			{
				UpdateClosedDocument(docFullName, textBuffer.Text);
			}
			else
			{
				doc.Save(docFullName);
			}
		}

		private void FormatOnSavePlugin_OptionsChanged(OptionsChangedEventArgs ea)
		{
			this.RefreshOptions();
		}

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
		public static string GetDocFullName(Document doc)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}
			var docFullName = doc.FullName;
			if (docFullName == null && _fullNameFieldInfo != null)
			{
				// The document name will be null if the file is closed, but the
				// private instance property will still have the filename we need
				// to re-format.
				docFullName = _fullNameFieldInfo.GetValue(doc) as String;
			}
			return docFullName;
		}

		/// <summary>
		/// Initializes the plug in.
		/// </summary>
		public override void InitializePlugIn()
		{
			base.InitializePlugIn();
			this.OptionsChanged += FormatOnSavePlugin_OptionsChanged;
			this._docEvents = new RunningDocumentTableEventProvider();
			this._docEvents.Initialize();
			this._docEvents.Saving += DocumentSaving;
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
			if (String.IsNullOrEmpty(language))
			{
				return false;
			}
			switch (language)
			{
				case DevExpress.DXCore.Constants.Str.Language.CPlusPlus:
					return (this.Options.LanguagesToFormat & DocumentLanguages.CPlusPlus) == DocumentLanguages.CPlusPlus;
				case DevExpress.DXCore.Constants.Str.Language.CSharp:
					return (this.Options.LanguagesToFormat & DocumentLanguages.CSharp) == DocumentLanguages.CSharp;
				case DevExpress.DXCore.Constants.Str.Language.CSS:
					return (this.Options.LanguagesToFormat & DocumentLanguages.Css) == DocumentLanguages.Css;
				case DevExpress.DXCore.Constants.Str.Language.HTML:
					return (this.Options.LanguagesToFormat & DocumentLanguages.Html) == DocumentLanguages.Html;
				case DevExpress.DXCore.Constants.Str.Language.JavaScript:
					return (this.Options.LanguagesToFormat & DocumentLanguages.JavaScript) == DocumentLanguages.JavaScript;
				case DevExpress.DXCore.Constants.Str.Language.VisualBasic:
					return (this.Options.LanguagesToFormat & DocumentLanguages.VisualBasic) == DocumentLanguages.VisualBasic;
				case DevExpress.DXCore.Constants.Str.Language.XAML:
					return (this.Options.LanguagesToFormat & DocumentLanguages.Xaml) == DocumentLanguages.Xaml;
				case DevExpress.DXCore.Constants.Str.Language.XML:
				case DevExpress.DXCore.Constants.Str.Language.XMLOnly:
					return (this.Options.LanguagesToFormat & DocumentLanguages.Xml) == DocumentLanguages.Xml;
				default:
					return false;
			}
		}

		/// <summary>
		/// Refreshes the set of options being used by this plugin.
		/// </summary>
		public void RefreshOptions()
		{
			this.Options = OptionSet.Load(PluginOptionsPage.Storage);
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
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(content);
			}
		}
	}
}