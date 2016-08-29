using System;
using System.Collections.ObjectModel;
using DevExpress.CodeRush.Platform.Options;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Provides a structure where plugin options can be saved.
	/// </summary>
	public class OptionSet : OptionsContainer
	{
		/// <summary>
		/// Backing store for <see cref="LanguagesToFormat"/>.
		/// </summary>
		private ObservableCollection<DocumentLanguages> _languagesToFormat;

		/// <summary>
		/// Initializes a new instance of the <see cref="DX_FormatOnSave.OptionSet" /> class
		/// with default options enabled.
		/// </summary>
		public OptionSet()
		{
			this.Enabled = true;
			this.LanguagesToFormat = new ObservableCollection<DocumentLanguages>();
		}

		/// <summary>
		/// Gets or sets a value indicating if the plugin is enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the plugin is enabled; <see langword="false" /> if not.
		/// </value>
		[Option]
		[OptionDefaultValue(true, "Neutral")]
		public bool Enabled
		{
			get
			{
				return this.GetProperty(() => this.Enabled);
			}
			set
			{
				this.SetProperty<bool>(() => this.Enabled, value);
			}
		}

		/// <summary>
		/// Gets or sets the flags indicating the language types to format.
		/// </summary>
		/// <value>
		/// A <see cref="DocumentLanguages"/> flag combination indicating which
		/// types of documents should be formatted on save.
		/// </value>
		[Option]
		[OptionDefaultValue(typeof(LanguagesToFormatOptionsCreator), "Neutral")]
		public ObservableCollection<DocumentLanguages> LanguagesToFormat
		{
			get
			{
				return this._languagesToFormat;
			}
			set
			{
				this.SetProperty(ref this._languagesToFormat, value, () => this.LanguagesToFormat);
			}
		}
	}
}
