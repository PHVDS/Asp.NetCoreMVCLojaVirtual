#pragma checksum "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "03fad7e9ad59839ed13a7372a4bb918aee0730b8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Pagamento_Index), @"mvc.1.0.view", @"/Views/Pagamento/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Pagamento/Index.cshtml", typeof(AspNetCore.Views_Pagamento_Index))]
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
#line 3 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\_ViewImports.cshtml"
using X.PagedList.Mvc.Core;

#line default
#line hidden
#line 4 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\_ViewImports.cshtml"
using X.PagedList;

#line default
#line hidden
#line 6 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\_ViewImports.cshtml"
using LojaVirtual.Models.ViewModels;

#line default
#line hidden
#line 7 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\_ViewImports.cshtml"
using LojaVirtual.Models.ProdutoAgregador;

#line default
#line hidden
#line 8 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\_ViewImports.cshtml"
using LojaVirtual.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"03fad7e9ad59839ed13a7372a4bb918aee0730b8", @"/Views/Pagamento/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"57c8cffc9397c11418479049ac7a5ddc1acbe5b0", @"/Views/_ViewImports.cshtml")]
    public class Views_Pagamento_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<ProdutoItem>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/imagem-produto.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("img-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("role", new global::Microsoft.AspNetCore.Html.HtmlString("form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
  
    ViewData["Title"] = "Index";
    decimal Subtotal = 0;

#line default
#line hidden
            BeginContext(94, 219, true);
            WriteLiteral("\r\n<div class=\"container\">\r\n    <br />\r\n    <br />\r\n    <div class=\"row\">\r\n        <div class=\"col-md-6\">\r\n            <div id=\"code_itemside_img2\">\r\n                <div class=\"box table-bordered items-bordered-wrap\">\r\n");
            EndContext();
#line 14 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                     foreach (var item in Model)
                    {

#line default
#line hidden
            BeginContext(386, 100, true);
            WriteLiteral("                        <figure class=\"itemside\">\r\n                            <div class=\"aside\">\r\n");
            EndContext();
#line 18 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                 if (item.Imagens.Count() > 0)
                                {

#line default
#line hidden
            BeginContext(585, 40, true);
            WriteLiteral("                                    <img");
            EndContext();
            BeginWriteAttribute("src", " src=\"", 625, "\"", 660, 1);
#line 20 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
WriteAttributeValue("", 631, item.Imagens.First().Caminho, 631, 29, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(661, 18, true);
            WriteLiteral(" class=\"img-sm\">\r\n");
            EndContext();
#line 21 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                }
                                else
                                {

#line default
#line hidden
            BeginContext(787, 36, true);
            WriteLiteral("                                    ");
            EndContext();
            BeginContext(823, 51, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "03fad7e9ad59839ed13a7372a4bb918aee0730b87297", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(874, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 25 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                }

#line default
#line hidden
            BeginContext(911, 116, true);
            WriteLiteral("                            </div>\r\n                            <figcaption class=\"text-wrap align-self-center\">\r\n\r\n");
            EndContext();
#line 29 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                  
                                    var ResultadoSubtotalItem = item.Valor * item.QuantidadeProdutoCarrinho;
                                    Subtotal = Subtotal + ResultadoSubtotalItem;
                                

#line default
#line hidden
            BeginContext(1290, 60, true);
            WriteLiteral("\r\n                                <h6 class=\"title\"><strong>");
            EndContext();
            BeginContext(1351, 19, false);
#line 34 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                     Write(item.Nome.ToUpper());

#line default
#line hidden
            EndContext();
            BeginContext(1370, 134, true);
            WriteLiteral("</strong></h6>\r\n                                <div class=\"price-wrap\">\r\n                                    <span class=\"price-new\">");
            EndContext();
            BeginContext(1505, 24, false);
#line 36 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                       Write(item.Valor.ToString("C"));

#line default
#line hidden
            EndContext();
            BeginContext(1529, 3, true);
            WriteLiteral(" x ");
            EndContext();
            BeginContext(1533, 30, false);
#line 36 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                                                   Write(item.QuantidadeProdutoCarrinho);

#line default
#line hidden
            EndContext();
            BeginContext(1563, 11, true);
            WriteLiteral(" = <strong>");
            EndContext();
            BeginContext(1575, 35, false);
#line 36 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                                                                                             Write(ResultadoSubtotalItem.ToString("C"));

#line default
#line hidden
            EndContext();
            BeginContext(1610, 136, true);
            WriteLiteral("</strong></span>\r\n                                </div>\r\n                            </figcaption>\r\n                        </figure>\r\n");
            EndContext();
#line 40 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                    }

#line default
#line hidden
            BeginContext(1769, 351, true);
            WriteLiteral(@"                </div>
                <div class=""box table-bordered items-bordered-wrap"">
                    <h4 class=""subtitle-doc"">
                        Resumo
                    </h4>
                    <dl class=""dlist-align box"">
                        <dt>Subtotal: </dt>
                        <dd class=""text-right subtotal"">");
            EndContext();
            BeginContext(2121, 22, false);
#line 48 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                   Write(Subtotal.ToString("C"));

#line default
#line hidden
            EndContext();
            BeginContext(2143, 178, true);
            WriteLiteral("</dd>\r\n                    </dl>\r\n                    <dl class=\"dlist-align box\">\r\n                        <dt>Frete:</dt>\r\n                        <dd class=\"text-right frete\">");
            EndContext();
            BeginContext(2322, 23, false);
#line 52 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                Write(ViewBag.Frete.TipoFrete);

#line default
#line hidden
            EndContext();
            BeginContext(2345, 3, true);
            WriteLiteral(" - ");
            EndContext();
            BeginContext(2349, 33, false);
#line 52 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                                           Write(ViewBag.Frete.Valor.ToString("C"));

#line default
#line hidden
            EndContext();
            BeginContext(2382, 125, true);
            WriteLiteral("</dd>\r\n                    </dl>\r\n                    <dl class=\"dlist-align box\">\r\n                        <dt>TOTAL:</dt>\r\n");
            EndContext();
#line 56 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                           
                            decimal valorFrete = (decimal)ViewBag.Frete.Valor;
                        

#line default
#line hidden
            BeginContext(2643, 53, true);
            WriteLiteral("                        <dd class=\"text-right total\">");
            EndContext();
            BeginContext(2698, 37, false);
#line 59 "C:\Users\PAULO HENRIQUE\Documents\Visual Studio 2019\LojaVirtual\src\Views\Pagamento\Index.cshtml"
                                                 Write((Subtotal + valorFrete).ToString("C"));

#line default
#line hidden
            EndContext();
            BeginContext(2736, 1273, true);
            WriteLiteral(@"</dd>
                    </dl>
                </div>
            </div>
            </div>
        <div class=""col-md-6"">
            <div id=""code_payment2"">

                <article class=""card"">
                    <div class=""card-body p-5"">

                        <ul class=""nav bg radius nav-pills nav-fill mb-3"" role=""tablist"">
                            <li class=""nav-item"">
                                <a class=""nav-link active show"" data-toggle=""pill"" href=""#nav-tab-card"">
                                    <i class=""fa fa-credit-card""></i> Cartão de Crédito
                                </a>
                            </li>
                            <li class=""nav-item"">
                                <a class=""nav-link"" data-toggle=""pill"" href=""#nav-tab-paypal"">
                                    <i class=""fas fa-barcode""></i>  Boleto Bancário
                                </a>
                            </li>
                        </ul>

               ");
            WriteLiteral("         <div class=\"tab-content\">\r\n                            <div class=\"tab-pane fade active show\" id=\"nav-tab-card\">\r\n                                <p class=\"alert alert-danger\">Some text success or error</p>\r\n                                ");
            EndContext();
            BeginContext(4009, 2801, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "03fad7e9ad59839ed13a7372a4bb918aee0730b815908", async() => {
                BeginContext(4027, 252, true);
                WriteLiteral("\r\n                                    <div class=\"form-group\">\r\n                                        <label for=\"username\">Nome (como no cartão)</label>\r\n                                        <input type=\"text\" class=\"form-control\" name=\"username\"");
                EndContext();
                BeginWriteAttribute("placeholder", " placeholder=\"", 4279, "\"", 4293, 0);
                EndWriteAttribute();
                BeginWriteAttribute("required", " required=\"", 4294, "\"", 4305, 0);
                EndWriteAttribute();
                BeginContext(4306, 367, true);
                WriteLiteral(@">
                                    </div> 

                                    <div class=""form-group"">
                                        <label for=""cardNumber"">Número cartão</label>
                                        <div class=""input-group"">
                                            <input type=""text"" class=""form-control"" name=""cardNumber""");
                EndContext();
                BeginWriteAttribute("placeholder", " placeholder=\"", 4673, "\"", 4687, 0);
                EndWriteAttribute();
                BeginContext(4688, 1077, true);
                WriteLiteral(@">
                                            <div class=""input-group-append"">
                                                <span class=""input-group-text text-muted"">
                                                    <i class=""fab fa-cc-visa""></i> &nbsp; <i class=""fab fa-cc-amex""></i> &nbsp;
                                                    <i class=""fab fa-cc-mastercard""></i>
                                                </span>
                                            </div>
                                        </div>
                                    </div> 

                                    <div class=""row"">
                                        <div class=""col-sm-8"">
                                            <div class=""form-group"">
                                                <label><span class=""hidden-xs"">Vencimento</span> </label>
                                                <div class=""input-group"">
                                                    <inpu");
                WriteLiteral("t type=\"number\" class=\"form-control\" placeholder=\"MM\"");
                EndContext();
                BeginWriteAttribute("name", " name=\"", 5765, "\"", 5772, 0);
                EndWriteAttribute();
                BeginContext(5773, 113, true);
                WriteLiteral(">\r\n                                                    <input type=\"number\" class=\"form-control\" placeholder=\"YY\"");
                EndContext();
                BeginWriteAttribute("name", " name=\"", 5886, "\"", 5893, 0);
                EndWriteAttribute();
                BeginContext(5894, 369, true);
                WriteLiteral(@">
                                                </div>
                                            </div>
                                        </div>
                                        <div class=""col-sm-4"">
                                            <div class=""form-group"">
                                                <label data-toggle=""tooltip""");
                EndContext();
                BeginWriteAttribute("title", " title=\"", 6263, "\"", 6271, 0);
                EndWriteAttribute();
                BeginContext(6272, 213, true);
                WriteLiteral(" data-original-title=\"3 digits code on back side of the card\">Cód. segurança <i class=\"fa fa-question-circle\"></i></label>\r\n                                                <input type=\"number\" class=\"form-control\"");
                EndContext();
                BeginWriteAttribute("required", " required=\"", 6485, "\"", 6496, 0);
                EndWriteAttribute();
                BeginContext(6497, 306, true);
                WriteLiteral(@">
                                            </div> 
                                        </div>
                                    </div> 
                                    <button class=""subscribe btn btn-primary btn-block"" type=""button""> Confirmar  </button>
                                ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(6810, 609, true);
            WriteLiteral(@"
                            </div> 
                            <div class=""tab-pane fade"" id=""nav-tab-paypal"">
                                <p>Clique no botão para imprimir o boleto bancário.</p>
                                <p>
                                    <button type=""button"" class=""btn btn-primary""> <i class=""fas fa-barcode""></i> Imprimir boleto </button>
                                </p>
                            </div> 
                        </div> 

                    </div> 
                </article> 

            </div>
        </div>
    </div>
</div>
");
            EndContext();
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<ProdutoItem>> Html { get; private set; }
    }
}
#pragma warning restore 1591
