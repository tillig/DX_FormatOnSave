using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace DX_FormatOnSave
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
	[Guid("02b990d9-96c3-42ba-b2f8-2ad6dcb079f3")]
	public class FormatOnSavePluginPackage : Package
	{
		public FormatOnSavePluginPackage()
		{
		}
	}
}