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
		/// A collection of language <see cref="string"/> ID values indicating the default set of languages to format.
		/// </returns>
		public object CreateDefaultValue(string language)
		{
			var coll = new ObservableCollection<string>();
			foreach (var item in DocumentLanguages.DisplayMapping.Keys)
			{
				coll.Add(item);
			}

			return coll;
		}
	}
}
