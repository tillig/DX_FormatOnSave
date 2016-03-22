using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.CodeRush.Core;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Page for setting plugin options.
	/// </summary>
	[UserLevel(UserLevel.NewUser)]
	public partial class PluginOptionsPage : OptionsPage
	{
		/// <summary>
		/// Gets the options being worked with in this window.
		/// </summary>
		/// <value>
		/// A <see cref="DX_FormatOnSave.OptionSet"/> with the current options.
		/// </value>
		public OptionSet Options { get; private set; }

		/// <summary>
		/// Gets the category in which the options page should appear.
		/// </summary>
		/// <returns>
		/// The identifier for the "Code Style" category.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetCategory()
		{
			return @"Editor\Code Style";
		}

		/// <summary>
		/// Gets the name of the options page.
		/// </summary>
		/// <returns>
		/// <c>Format On Save</c>
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static string GetPageName()
		{
			return @"Format On Save";
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			this.CommitChanges += new CommitChangesEventHandler(PluginOptionsPage_CommitChanges);
			this.RestoreDefaults += new RestoreDefaultsEventHandler(PluginOptionsPage_RestoreDefaults);
			this.Options = OptionSet.Load(Storage);
			this.SyncOptionsToForm(this.Options);
		}

		void PluginOptionsPage_RestoreDefaults(object sender, OptionsPageEventArgs ea)
		{
			OptionSet defaults = new OptionSet();
			this.SyncOptionsToForm(defaults);
			this.Options = defaults;
		}

		void PluginOptionsPage_CommitChanges(object sender, CommitChangesEventArgs ea)
		{
			this.Options = this.SyncOptionsFromForm();
			this.Options.Save(Storage);
		}

		/// <summary>
		/// Gets the options out of the form and turns them into an option set.
		/// </summary>
		/// <returns>
		/// An <see cref="DX_FormatOnSave.OptionSet"/> with the options as seen
		/// in the form.
		/// </returns>
		public OptionSet SyncOptionsFromForm()
		{
			OptionSet options = new OptionSet();
			options.Enabled = this.chkEnabled.Checked;
			options.LanguagesToFormat = DocumentLanguages.None;
			if (this.chkCPlusPlus.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.CPlusPlus;
			}
			if (this.chkCSharp.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.CSharp;
			}
			if (this.chkCss.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.Css;
			}
			if (this.chkHtml.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.Html;
			}
			if (this.chkJavaScript.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.JavaScript;
			}
			if (this.chkVisualBasic.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.VisualBasic;
			}
			if (this.chkXaml.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.Xaml;
			}
			if (this.chkXml.Checked)
			{
				options.LanguagesToFormat |= DocumentLanguages.Xml;
			}
			return options;
		}

		/// <summary>
		/// Synchronizes the options into the form. The form will display what the options contain.
		/// </summary>
		/// <param name="optionSet">The options to display.</param>
		public void SyncOptionsToForm(OptionSet optionSet)
		{
			if (optionSet == null)
			{
				throw new ArgumentNullException("optionSet");
			}
			this.chkEnabled.Checked = optionSet.Enabled;
			this.chkCPlusPlus.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.CPlusPlus) == DocumentLanguages.CPlusPlus;
			this.chkCSharp.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.CSharp) == DocumentLanguages.CSharp;
			this.chkCss.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.Css) == DocumentLanguages.Css;
			this.chkHtml.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.Html) == DocumentLanguages.Html;
			this.chkJavaScript.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.JavaScript) == DocumentLanguages.JavaScript;
			this.chkVisualBasic.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.VisualBasic) == DocumentLanguages.VisualBasic;
			this.chkXaml.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.Xaml) == DocumentLanguages.Xaml;
			this.chkXml.Checked = (optionSet.LanguagesToFormat & DocumentLanguages.Xml) == DocumentLanguages.Xml;
		}

		private void chkEnabled_CheckedChanged(object sender, EventArgs e)
		{
			this.languageFormatSelectors.Enabled = this.chkEnabled.Checked;
		}
	}
}