namespace Comentsys.Toolkit.Blazor.Demo.Pages;

public partial class GettingStarted
{
    private const string PackageReferenceSnippet =
@"<PackageReference Include=""Comentsys.Toolkit.Blazor"" Version=""1.1.0"" />";

    private const string InstallSnippet =
@"dotnet add package Comentsys.Toolkit.Blazor";

    private const string ImportsSnippet =
@"@using System.Drawing
@using Comentsys.Toolkit.Blazor";

    private const string UsageSnippet =
@"<Gauge Minimum=""0""
       Maximum=""100""
       Value=""72""
       Fill=""Color.FromArgb(132, 0, 132)""
       Foreground=""Color.White"" />";

}