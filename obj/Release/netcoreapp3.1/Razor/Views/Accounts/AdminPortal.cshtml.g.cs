#pragma checksum "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "948c6cac17810241133b362066058981d2621e76"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Accounts_AdminPortal), @"mvc.1.0.view", @"/Views/Accounts/AdminPortal.cshtml")]
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
#nullable restore
#line 1 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
using FruityNET.DTOs;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
using FruityNET.Enums;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"948c6cac17810241133b362066058981d2621e76", @"/Views/Accounts/AdminPortal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d21de4c8441600b2df7cae93404b93e78c1beda4", @"/Views/_ViewImports.cshtml")]
    public class Views_Accounts_AdminPortal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AdminPortalViewDTO>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Admin", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Requests", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-info btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "GrantAdminAccess", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-danger btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "RevokeAdminAccess", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Suspend", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Activate", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
            WriteLiteral("\n\n");
#nullable restore
#line 7 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
  
    ViewData["Title"] = "Admin Portal";

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "948c6cac17810241133b362066058981d2621e766571", async() => {
                WriteLiteral("\n");
#nullable restore
#line 13 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                         if(Model.RequestCount != 0)
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("<span class=\"badge badge-light\">");
#nullable restore
#line 14 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                                                    Write(Model.RequestCount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</span>");
#nullable restore
#line 14 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                                                                                   }

#line default
#line hidden
#nullable disable
                WriteLiteral("                         Admin Requests \n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
<h1 style=""text-align: center;"">Manage Users</h1><br>

   <table class=""table table-hover table-light"">
  <thead>
    <tr>
      <th scope=""col"">Username</th>
      <th scope=""col"">First </th>
      <th scope=""col"">Last </th>
      <th scope=""col"">Email </th>
      <th scope=""col"">Last Active </th>
      <th scope=""col""> Role</th>
      <th scope=""col""> </th>
      <th scope=""col""> </th>

    </tr>
  </thead>
  
");
#nullable restore
#line 34 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
         foreach (var account in Model.Accounts)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("      <tbody>\n      <td>");
#nullable restore
#line 37 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
     Write(account.Username);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n      <td>");
#nullable restore
#line 38 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
     Write(account.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n      <td>");
#nullable restore
#line 39 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
     Write(account.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n      <td>");
#nullable restore
#line 40 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
     Write(account.Email);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n      <td>");
#nullable restore
#line 41 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
     Write(account.LastActive);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n      <td>");
#nullable restore
#line 42 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
     Write(account.UserType);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n      <td>\n");
#nullable restore
#line 44 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
           if(account.UserType != UserType.Admin && account.UserType != UserType.SiteOwner && account.AccountStatus.Equals(Status.Active) && 
          Model.CurrentUserPermission.Equals(UserType.SiteOwner))
          {

#line default
#line hidden
#nullable disable
            WriteLiteral("               ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "948c6cac17810241133b362066058981d2621e7611487", async() => {
                WriteLiteral("\n                        Grant Admin \n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 49 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                          WriteLiteral(account.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
#nullable restore
#line 52 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
          }
          else if(account.UserType == UserType.Admin && account.AccountStatus.Equals(Status.Active)&& 
          Model.CurrentUserPermission.Equals(UserType.SiteOwner))
          {

#line default
#line hidden
#nullable disable
            WriteLiteral("              ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "948c6cac17810241133b362066058981d2621e7614132", async() => {
                WriteLiteral("\n                        Revoke Admin \n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_6.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 58 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                          WriteLiteral(account.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
#nullable restore
#line 61 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
          } 

#line default
#line hidden
#nullable disable
            WriteLiteral("          \n         \n      </td>\n      <td>\n");
#nullable restore
#line 66 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
           if((account.AccountStatus == Status.Active && account.UserType == UserType.User) || 
          Model.CurrentUserPermission.Equals(UserType.SiteOwner))
          {

#line default
#line hidden
#nullable disable
            WriteLiteral("               ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "948c6cac17810241133b362066058981d2621e7617013", async() => {
                WriteLiteral("\n                        Suspend Account\n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_7.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_7);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 71 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                          WriteLiteral(account.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
#nullable restore
#line 74 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
          }
          else if(!account.AccountStatus.Equals(Status.Active))
          {

#line default
#line hidden
#nullable disable
            WriteLiteral("              ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "948c6cac17810241133b362066058981d2621e7619556", async() => {
                WriteLiteral("\n                        Activate Account\n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_8.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_8);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 79 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
                          WriteLiteral(account.Id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n");
#nullable restore
#line 82 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"
          } 

#line default
#line hidden
#nullable disable
            WriteLiteral("         \n      </td>\n      \n          </tbody>\n");
#nullable restore
#line 87 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Accounts/AdminPortal.cshtml"

      }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </table>\n   \n     \n\n\n ");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<AdminPortalViewDTO> Html { get; private set; }
    }
}
#pragma warning restore 1591
