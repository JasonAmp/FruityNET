#pragma checksum "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c3c2a01b437f6ea1cb5df854f56d852e873a2417"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Groups_GroupDetails), @"mvc.1.0.view", @"/Views/Groups/GroupDetails.cshtml")]
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
#line 1 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
using FruityNET.DTOs;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
using FruityNET.Entities;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c3c2a01b437f6ea1cb5df854f56d852e873a2417", @"/Views/Groups/GroupDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d21de4c8441600b2df7cae93404b93e78c1beda4", @"/Views/_ViewImports.cshtml")]
    public class Views_Groups_GroupDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<GroupDetailsDTO>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Groups", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "AddGroupMember", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-danger btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "RemoveUser", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\n\n");
            WriteLiteral("\n");
            WriteLiteral("\n");
#nullable restore
#line 11 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
  
    ViewData["Title"] = "Group Details";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<h4>Group Details</h4><br>\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c3c2a01b437f6ea1cb5df854f56d852e873a24175583", async() => {
                WriteLiteral("\n                        Add User To Group\n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 21 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
                           WriteLiteral(Model.Id);

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
            WriteLiteral("<br></br>\n<div class=\"card\">\n  <div class=\"card-header\">\n    ");
#nullable restore
#line 26 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
Write(Model.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n  </div>\n  <div class=\"card-body\">\n    <h5 class=\"card-title\">Description: ");
#nullable restore
#line 29 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
                                   Write(Model.Description);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h5>\n    <p class=\"card-text\">Created On: ");
#nullable restore
#line 30 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
                                Write(Model.CreationDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n        <p class=\"card-text\">Created By: ");
#nullable restore
#line 31 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
                                    Write(Model.GroupOwner);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n\n   \n  </div>\n</div>\n<br></br>\n\n\n<h3>Group Members</h3>\n<ul class=\"list-group\">\n");
#nullable restore
#line 41 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
   foreach (var user in Model.GroupMembers)
  {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <li class=\"list-group-item\">");
#nullable restore
#line 43 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
                               Write(user.Username);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 44 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
           if(user.UserId != UserManager.GetUserId(User) )
          {

#line default
#line hidden
#nullable disable
            WriteLiteral("             ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "c3c2a01b437f6ea1cb5df854f56d852e873a241710138", async() => {
                WriteLiteral("\n                        Remove\n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 51 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
                           WriteLiteral(user.Id);

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
#line 54 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
          }

#line default
#line hidden
#nullable disable
            WriteLiteral("         \n         \n        </li>\n");
#nullable restore
#line 58 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"

        

  }

#line default
#line hidden
#nullable disable
#nullable restore
#line 62 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
   if(Model.GroupMembers.Count is 0)
  {

#line default
#line hidden
#nullable disable
            WriteLiteral("    <p>Add Members To Group.</p>\n");
#nullable restore
#line 65 "/Users/jasonampah/Documents/Dev/C#/MVCApps/FruityNET/Views/Groups/GroupDetails.cshtml"
  }

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public UserManager<User> UserManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public SignInManager<User> SignInManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<GroupDetailsDTO> Html { get; private set; }
    }
}
#pragma warning restore 1591
