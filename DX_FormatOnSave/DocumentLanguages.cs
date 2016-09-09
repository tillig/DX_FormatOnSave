using System;
using System.Collections.Generic;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Enumeration listing the possible options that the user can choose for which
	/// document languages should be formatted on save.
	/// </summary>
	public static class DocumentLanguages
	{
		/// <summary>
		/// Language option flag corresponding to C++.
		/// </summary>
		public static readonly string CPlusPlus = "C++";

		/// <summary>
		/// Language option flag corresponding to C#.
		/// </summary>
		public static readonly string CSharp = "C#";

		/// <summary>
		/// Language option flag corresponding to CSS.
		/// </summary>
		public static readonly string Css = "CSS";

		/// <summary>
		/// Language option flag corresponding to HTML markup.
		/// </summary>
		public static readonly string Html = "HTML";

		/// <summary>
		/// Language option flag corresponding to JavaScript.
		/// </summary>
		public static readonly string JavaScript = "Script";

		/// <summary>
		/// Language option flag corresponding to Visual Basic.
		/// </summary>
		public static readonly string VisualBasic = "Basic";

		/// <summary>
		/// Language option flag corresponding to XAML markup.
		/// </summary>
		public static readonly string Xaml = "XAML";

		/// <summary>
		/// Language option flag corresponding to XML.
		/// </summary>
		public static readonly string Xml = "XML";

		/// <summary>
		/// Gets display mapping for languages.
		/// </summary>
		/// <value>
		/// An <see cref="IDictionary{TKey, TValue}"/> where the key is the language ID
		/// and the value is a display string.
		/// </value>
		public static IDictionary<string, string> DisplayMapping { get; } = new Dictionary<string, string>()
		{
			{ CPlusPlus, "C++" },
			{ CSharp, "C#" },
			{ Css, "CSS" },
			{ Html, "HTML" },
			{ JavaScript, "JavaScript" },
			{ VisualBasic, "Visual Basic" },
			{ Xaml, "XAML" },
			{ Xml, "XML" }
		};
	}
}
