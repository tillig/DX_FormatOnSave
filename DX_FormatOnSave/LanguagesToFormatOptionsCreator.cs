using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.CodeRush.Platform.Options;

namespace DX_FormatOnSave
{
	/// <summary>
	/// Creator for default options value for languages to format.
	/// </summary>
	public class LanguagesToFormatOptionsCreator : IOptionDefaultValueProcessor
	{
		/// <summary>
		/// Creates the default value for languages to format.
		/// </summary>
		/// <param name="language">
		/// The options language being used.
		/// </param>
		/// <returns>
		/// A collection of <see cref="DocumentLanguages"/> indicating the default set of languages to format.
		/// </returns>
		public object CreateDefaultValue(string language)
		{
			var coll = new ObservableCollection<DocumentLanguages>();
			foreach (var item in Enum.GetValues(typeof(DocumentLanguages)).Cast<DocumentLanguages>())
			{
				coll.Add(item);
			}

			return coll;
		}
	}
}
