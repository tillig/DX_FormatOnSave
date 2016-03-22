using System;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.Diagnostics.Internal;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Provides a structure where plugin options can be saved.
	/// </summary>
	public class OptionSet
	{
		private const string SectionGeneral = "General";
		private const string KeyEnabled = "Enabled";
		private const string KeyLanguagesToFormat = "LanguagesToFormat";

		/// <summary>
		/// Initializes a new instance of the <see cref="DX_FormatOnSave.OptionSet" /> class
		/// with default options enabled.
		/// </summary>
		public OptionSet()
		{
			this.Enabled = true;
			this.LanguagesToFormat = DocumentLanguages.CPlusPlus | DocumentLanguages.CSharp | DocumentLanguages.JavaScript | DocumentLanguages.VisualBasic | DocumentLanguages.Xml;
		}

		/// <summary>
		/// Gets or sets a value indicating if the plugin is enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the plugin is enabled; <see langword="false" /> if not.
		/// </value>
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the flags indicating the language types to format.
		/// </summary>
		/// <value>
		/// A <see cref="DocumentLanguages"/> flag combination indicating which
		/// types of documents should be formatted on save.
		/// </value>
		public DocumentLanguages LanguagesToFormat { get; set; }

		/// <summary>
		/// Loads the options from storage.
		/// </summary>
		/// <param name="storage">A storage object from which options should be loaded.</param>
		/// <returns>
		/// A populated <see cref="DX_FormatOnSave.OptionSet"/> from the storage.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="storage" /> is <see langword="null" />.
		/// </exception>
		public static OptionSet Load(DecoupledStorage storage)
		{
			if (storage == null)
			{
				throw new ArgumentNullException("storage");
			}
			Log.SendMsg("Loading 'format on save' options.");
			OptionSet options = new OptionSet();
			OptionSet defaults = new OptionSet();
			options.Enabled = storage.ReadBoolean(SectionGeneral, KeyEnabled, defaults.Enabled);
			Log.SendBool("Enabled", options.Enabled);
			options.LanguagesToFormat = storage.ReadEnum<DocumentLanguages>(SectionGeneral, KeyLanguagesToFormat, defaults.LanguagesToFormat);
			Log.SendEnum("Languages to format", options.LanguagesToFormat);
			return options;
		}

		/// <summary>
		/// Saves the options to storage.
		/// </summary>
		/// <param name="storage">A storage object to which the options should be saved.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="storage" /> is <see langword="null" />.
		/// </exception>
		public void Save(DecoupledStorage storage)
		{
			if (storage == null)
			{
				throw new ArgumentNullException("storage");
			}
			Log.SendMsg("Saving 'format on save' options.");
			storage.LanguageID = "";
			storage.WriteBoolean(SectionGeneral, KeyEnabled, this.Enabled);
			Log.SendBool("Enabled", this.Enabled);
			storage.WriteEnum(SectionGeneral, KeyLanguagesToFormat, this.LanguagesToFormat);
			Log.SendEnum("Languages to format", this.LanguagesToFormat);
			storage.UpdateStorage();
		}
	}
}
