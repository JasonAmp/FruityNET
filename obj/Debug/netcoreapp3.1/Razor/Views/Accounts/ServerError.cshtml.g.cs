#pragma checksum "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/ServerError.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "75bd743570492bf514b50d4d918ac7fbd09798b6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Accounts_ServerError), @"mvc.1.0.view", @"/Views/Accounts/ServerError.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/_ViewImports.cshtml"
using FruityNET;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/_ViewImports.cshtml"
using FruityNET.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"75bd743570492bf514b50d4d918ac7fbd09798b6", @"/Views/Accounts/ServerError.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d21de4c8441600b2df7cae93404b93e78c1beda4", @"/Views/_ViewImports.cshtml")]
    public class Views_Accounts_ServerError : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 2 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/ServerError.cshtml"
  
    ViewData["Title"] = "500";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    <p>HTTP ");
#nullable restore
#line 6 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/ServerError.cshtml"
       Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n\n        <h1>Internal Server Error</h1>\n        <p>There is an error within our site server. Please check back later.</p>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
