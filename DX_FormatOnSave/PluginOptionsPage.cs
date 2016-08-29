using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.CodeRush.Foundation.Options;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Page for setting plugin options.
	/// </summary>
	[UserLevel(UserLevel.NewUser)]
	[RegisterOptionsPage(@"Editor\Code Style", "Format On Save")]
	public class PluginOptionsPage : OptionsPage
	{
		/// <summary>
		/// Gets the options being worked with in this window.
		/// </summary>
		/// <value>
		/// A <see cref="DX_FormatOnSave.OptionSet"/> with the current options.
		/// </value>
		private OptionSet Options
		{
			get
			{
				return this.OptionsContainer as OptionSet;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DX_FormatOnSave.PluginOptionsPage" /> class.
		/// </summary>
		public PluginOptionsPage()
		{
			// Required for Windows.Forms Class Composition Designer support
			InitializeComponent();
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkEnabled = new System.Windows.Forms.CheckBox();
			this.languageFormatSelectors = new System.Windows.Forms.GroupBox();
			this.chkXml = new System.Windows.Forms.CheckBox();
			this.chkXaml = new System.Windows.Forms.CheckBox();
			this.chkVisualBasic = new System.Windows.Forms.CheckBox();
			this.chkJavaScript = new System.Windows.Forms.CheckBox();
			this.chkHtml = new System.Windows.Forms.CheckBox();
			this.chkCss = new System.Windows.Forms.CheckBox();
			this.chkCSharp = new System.Windows.Forms.CheckBox();
			this.chkCPlusPlus = new System.Windows.Forms.CheckBox();
			this.languageFormatSelectors.SuspendLayout();
			//
			// chkEnabled
			//
			this.chkEnabled.AutoSize = true;
			this.chkEnabled.Location = new System.Drawing.Point(4, 4);
			this.chkEnabled.Name = "chkEnabled";
			this.chkEnabled.Size = new System.Drawing.Size(65, 17);
			this.chkEnabled.TabIndex = 0;
			this.chkEnabled.Text = "Enabled";
			this.chkEnabled.UseVisualStyleBackColor = true;
			this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
			//
			// languageFormatSelectors
			//
			this.languageFormatSelectors.Controls.Add(this.chkXml);
			this.languageFormatSelectors.Controls.Add(this.chkXaml);
			this.languageFormatSelectors.Controls.Add(this.chkVisualBasic);
			this.languageFormatSelectors.Controls.Add(this.chkJavaScript);
			this.languageFormatSelectors.Controls.Add(this.chkHtml);
			this.languageFormatSelectors.Controls.Add(this.chkCss);
			this.languageFormatSelectors.Controls.Add(this.chkCSharp);
			this.languageFormatSelectors.Controls.Add(this.chkCPlusPlus);
			this.languageFormatSelectors.Enabled = false;
			this.languageFormatSelectors.Location = new System.Drawing.Point(14, 27);
			this.languageFormatSelectors.Name = "languageFormatSelectors";
			this.languageFormatSelectors.Size = new System.Drawing.Size(263, 212);
			this.languageFormatSelectors.TabIndex = 1;
			this.languageFormatSelectors.TabStop = false;
			this.languageFormatSelectors.Text = "Document types to format on save";
			//
			// chkXml
			//
			this.chkXml.AutoSize = true;
			this.chkXml.Location = new System.Drawing.Point(7, 187);
			this.chkXml.Name = "chkXml";
			this.chkXml.Size = new System.Drawing.Size(48, 17);
			this.chkXml.TabIndex = 7;
			this.chkXml.Text = "XML";
			this.chkXml.UseVisualStyleBackColor = true;
			//
			// chkXaml
			//
			this.chkXaml.AutoSize = true;
			this.chkXaml.Location = new System.Drawing.Point(7, 163);
			this.chkXaml.Name = "chkXaml";
			this.chkXaml.Size = new System.Drawing.Size(55, 17);
			this.chkXaml.TabIndex = 6;
			this.chkXaml.Text = "XAML";
			this.chkXaml.UseVisualStyleBackColor = true;
			//
			// chkVisualBasic
			//
			this.chkVisualBasic.AutoSize = true;
			this.chkVisualBasic.Location = new System.Drawing.Point(7, 139);
			this.chkVisualBasic.Name = "chkVisualBasic";
			this.chkVisualBasic.Size = new System.Drawing.Size(111, 17);
			this.chkVisualBasic.TabIndex = 5;
			this.chkVisualBasic.Text = "Visual Basic .NET";
			this.chkVisualBasic.UseVisualStyleBackColor = true;
			//
			// chkJavaScript
			//
			this.chkJavaScript.AutoSize = true;
			this.chkJavaScript.Location = new System.Drawing.Point(7, 115);
			this.chkJavaScript.Name = "chkJavaScript";
			this.chkJavaScript.Size = new System.Drawing.Size(76, 17);
			this.chkJavaScript.TabIndex = 4;
			this.chkJavaScript.Text = "JavaScript";
			this.chkJavaScript.UseVisualStyleBackColor = true;
			//
			// chkHtml
			//
			this.chkHtml.AutoSize = true;
			this.chkHtml.Location = new System.Drawing.Point(7, 91);
			this.chkHtml.Name = "chkHtml";
			this.chkHtml.Size = new System.Drawing.Size(56, 17);
			this.chkHtml.TabIndex = 3;
			this.chkHtml.Text = "HTML";
			this.chkHtml.UseVisualStyleBackColor = true;
			//
			// chkCss
			//
			this.chkCss.AutoSize = true;
			this.chkCss.Location = new System.Drawing.Point(7, 67);
			this.chkCss.Name = "chkCss";
			this.chkCss.Size = new System.Drawing.Size(47, 17);
			this.chkCss.TabIndex = 2;
			this.chkCss.Text = "CSS";
			this.chkCss.UseVisualStyleBackColor = true;
			//
			// chkCSharp
			//
			this.chkCSharp.AutoSize = true;
			this.chkCSharp.Location = new System.Drawing.Point(7, 43);
			this.chkCSharp.Name = "chkCSharp";
			this.chkCSharp.Size = new System.Drawing.Size(40, 17);
			this.chkCSharp.TabIndex = 1;
			this.chkCSharp.Text = "C#";
			this.chkCSharp.UseVisualStyleBackColor = true;
			//
			// chkCPlusPlus
			//
			this.chkCPlusPlus.AutoSize = true;
			this.chkCPlusPlus.Location = new System.Drawing.Point(7, 19);
			this.chkCPlusPlus.Name = "chkCPlusPlus";
			this.chkCPlusPlus.Size = new System.Drawing.Size(57, 17);
			this.chkCPlusPlus.TabIndex = 0;
			this.chkCPlusPlus.Text = "C/C++";
			this.chkCPlusPlus.UseVisualStyleBackColor = true;
			//
			// PlugInOptionsPage
			//
			this.Name = "PluginOptionsPage";
			this.languageFormatSelectors.ResumeLayout(false);
			this.languageFormatSelectors.PerformLayout();

		}

		private System.Windows.Forms.CheckBox chkEnabled;
		private System.Windows.Forms.GroupBox languageFormatSelectors;
		private System.Windows.Forms.CheckBox chkCPlusPlus;
		private System.Windows.Forms.CheckBox chkXml;
		private System.Windows.Forms.CheckBox chkXaml;
		private System.Windows.Forms.CheckBox chkVisualBasic;
		private System.Windows.Forms.CheckBox chkJavaScript;
		private System.Windows.Forms.CheckBox chkHtml;
		private System.Windows.Forms.CheckBox chkCss;
		private System.Windows.Forms.CheckBox chkCSharp;

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
				options.LanguagesToFormat |= DocumentLanguages.CPlusPlus;
			if (this.chkCSharp.Checked)
				options.LanguagesToFormat |= DocumentLanguages.CSharp;
			if (this.chkCss.Checked)
				options.LanguagesToFormat |= DocumentLanguages.Css;
			if (this.chkHtml.Checked)
				options.LanguagesToFormat |= DocumentLanguages.Html;
			if (this.chkJavaScript.Checked)
				options.LanguagesToFormat |= DocumentLanguages.JavaScript;
			if (this.chkVisualBasic.Checked)
				options.LanguagesToFormat |= DocumentLanguages.VisualBasic;
			if (this.chkXaml.Checked)
				options.LanguagesToFormat |= DocumentLanguages.Xaml;
			if (this.chkXml.Checked)
				options.LanguagesToFormat |= DocumentLanguages.Xml;
			return options;
		}

		/// <summary>
		/// Synchronizes the options into the form. The form will display what the options contain.
		/// </summary>
		/// <param name="optionSet">The options to display.</param>
		public void SyncOptionsToForm(OptionSet optionSet)
		{
			if (optionSet == null)
				throw new ArgumentNullException("optionSet");
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