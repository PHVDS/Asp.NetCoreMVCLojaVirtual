$(document).ready(function () {
    MoverScrollOrdenacao();
    MudarOrdenacao();
    MudarImagemPrincipalProduto();
    MudarQuantidadeProdutoCarrinho();

    MascaraCPF();
    MascaraNascimento();
    MascaraTEL();
    MascaraCEP();
    AJAXBuscarCEP();
    AcaoCalcularFreteBtn();
    AJAXCalcularFrete(false);
});

function AJAXBuscarCEP() {
    $("#CEP").keyup(function () {
        OcultarMensagemDeErro();

        if ($(this).val().length == 10) {

            var cep = RemoverMascara($(this).val());
            var url = "https://viacep.com.br/ws/" + cep + "/json/";

            $.ajax({
                type: "GET",
                url: url,
                dataType: "jsonp",
                error: function (data) {
                    MostrarMensagemDeErro("Opps! Tivemos um erro na busca pelo CEP! Talvez o servidor esteja indisponível!");
                },
                success: function (data) {
                    if (data.erro == undefined) {
                        $("#Estado").val(data.uf);
                        $("#Cidade").val(data.localidade);
                        $("#Endereco").val(data.logradouro);
                        $("#Bairro").val(data.bairro);
                        $("#Complemento").val(data.complemento);
                    }
                    else {
                        MostrarMensagemDeErro("Cep inválido!");
                    }
                }
            });
        }
    });
}

function MascaraNascimento() {
    $("#Nascimento").mask("00/00/0000");
}

function MascaraCPF() {
    $(".cpf").mask("000.000.000-00");
}

function MascaraTEL() {
    $(".tel").mask("(00)00000-0000");
}

function MascaraCEP() {
    $(".cep").mask("00.000-000");
}

function AcaoCalcularFreteBtn() {
    $(".btn-calcular-frete").click(function (e) {
        AJAXCalcularFrete(true);
        e.preventDefault();
    });
}

function AJAXCalcularFrete(chamadaPorBtn) {
    $(".btn-continuar").addClass("disabled");
    if (chamadaPorBtn == false) {
        if ($.cookie('Carrinho.CEP') != undefined) {
            $(".cep").val($.cookie('Carrinho.CEP'));
        }
    }

    var cep = $(".cep").val().replace(".", "").replace("-", "");
    $.removeCookie("Carrinho.TipoFrete");

    if (cep.length == 8) {
        $.cookie('Carrinho.CEP', $(".cep").val());
        $(".container-frete").html("<img src='\\img\\loading.gif' />");
        $(".frete").text("R$ 0,00");
        $(".total").text("R$ 0,00");


        $.ajax({
            type: "GET",
            url: "/CarrinhoCompra/CalcularFrete?cepDestino=" + cep,
            error: function (data) {
                MostrarMensagemDeErro("Opps! Erro ao obter o Frete..." + data.Message);
                console.info(data);
            },
            success: function (data) {

                html = "";

                for (var i = 0; i < data.length; i++) {
                    var tipoFrete = data[i].tipoFrete;
                    var valor = data[i].valor;
                    var prazo = data[i].prazo;

                    html += "<dl class=\"dlist-align\"><dt><input type=\"radio\" name=\"frete\" value=\"" + tipoFrete + "\" /><input type=\"hidden\" name=\"valor\" value=\"" + valor + "\" \/></dt><dd>" + tipoFrete + " - " + numberToReal(valor) + " (" + prazo + " dias úteis)</dd></dl>";
                }

                $(".container-frete").html(html);
                $(".container-frete").find("input[type=radio]").change(function () {

                    $.cookie("Carrinho.TipoFrete", $(this).val());
                    $(".btn-continuar").removeClass("disabled");

                    var valorFrete = parseFloat($(this).parent().find("input[type=hidden]").val());

                    $(".frete").text(numberToReal(valorFrete));

                    var subtotal = parseFloat($(".subtotal").text().replace("R$", "").replace(".", "").replace(",","."));
                    var total = valorFrete + subtotal;

                    $(".total").text(numberToReal(total));

                });
            }
        });
    }
    else {
        if (chamadaPorBtn == true) {
            $(".container-frete").html("");
            MostrarMensagemDeErro("Digite o CEP para calcular o frete!");
        }
    }
}

function numberToReal(numero) {
    var numero = numero.toFixed(2).split('.');
    numero[0] = "R$ " + numero[0].split(/(?=(?:...)*$)/).join('.');
    return numero.join(',');
}

function MudarQuantidadeProdutoCarrinho() {
    $("#order .btn-primary").click(function () {
        var pai = $(this).parent().parent();
        if ($(this).hasClass("diminuir")) {
            OrquestradorDeAcoesProduto("diminuir", $(this));
            var id = pai.find(".inputProdutoId").val();

        }
        if ($(this).hasClass("aumentar")) {
            OrquestradorDeAcoesProduto("aumentar", $(this));
        }
    });
}

function OrquestradorDeAcoesProduto(operacao, botao) {
    OcultarMensagemDeErro();
    /*
     * Carregamento dos valores
     */
    var pai = botao.parent().parent();

    var produtoId = pai.find(".inputProdutoId").val();
    var quantidadeEstoque = parseInt(pai.find(".inputQuantidadeEstoque").val());
    var valorUnitario = parseFloat(pai.find(".inputValorUnitario").val().replace(",", "."));

    var campoQuantidadeProdutoCarrinho = pai.find(".inputQuantidadeProdutoCarrinho");
    var quantidadeProdutoCarrinhoAntiga = parseInt(campoQuantidadeProdutoCarrinho.val());

    var campoValor = botao.parent().parent().parent().parent().parent().find(".price");

    var produto = new ProdutoQuantidadeEValor(produtoId, quantidadeEstoque, valorUnitario, quantidadeProdutoCarrinhoAntiga, 0, campoQuantidadeProdutoCarrinho, campoValor);

    /*
     * Chamada de Métodos
     */
    AlteracoesVisuaisProdutoCarrinho(produto, operacao);

}

function AlteracoesVisuaisProdutoCarrinho(produto, operacao) {
    if (operacao == "aumentar")
    {
        {
            produto.quantidadeProdutoCarrinhoNova = produto.quantidadeProdutoCarrinhoAntiga + 1;

            AtualizarQuantidadeEValor(produto);

            AJAXComunicarAlteracaoQuantidadeProduto(produto);

        }
    }
    else if (operacao == "diminuir")
    {
        {
            produto.quantidadeProdutoCarrinhoNova = produto.quantidadeProdutoCarrinhoAntiga - 1;

            AtualizarQuantidadeEValor(produto);

            AJAXComunicarAlteracaoQuantidadeProduto(produto);
        }
    }
}

function AJAXComunicarAlteracaoQuantidadeProduto(produto) {
    $.ajax({
        type: "GET",
        url: "/CarrinhoCompra/AlterarQuantidade?id=" + produto.produtoId + "&quantidade=" + produto.quantidadeProdutoCarrinhoNova,
        error: function (data) {
            MostrarMensagemDeErro(data.responseJSON.mensagem);

            //Rollback
            produto.quantidadeProdutoCarrinhoNova = produto.quantidadeProdutoCarrinhoAntiga;
            AtualizarQuantidadeEValor(produto);
        },
        success: function () {
            AJAXCalcularFrete();
        }
    });
}

function MostrarMensagemDeErro(mensagem) {
    $(".alert-danger").css("display", "block");
    $(".alert-danger").text(mensagem);
}

function OcultarMensagemDeErro() {
    $(".alert-danger").css("display", "none");
}

function AtualizarQuantidadeEValor(produto) {
    produto.campoQuantidadeProdutoCarrinho.val(produto.quantidadeProdutoCarrinhoNova);

    var resultado = produto.valorUnitario * produto.quantidadeProdutoCarrinhoNova;
    produto.campoValor.text(numberToReal(resultado));

    AtualizarSubtotal();
}

function AtualizarSubtotal() {
    var Subtotal = 0;

    var TagsComPrice = $(".price");

    TagsComPrice.each(function () {
        var ValorReais = parseFloat($(this).text().replace("R$", "").replace(".", "").replace(" ", "").replace(",", "."));

        Subtotal += ValorReais;
    })

    $(".subtotal").text(numberToReal(Subtotal));
}

function MudarImagemPrincipalProduto() {
    $(".img-small-wrap img").click(function () {
        var Caminho = $(this).attr("src");
        $(".img-big-wrap img").attr("src", Caminho);
        $(".img-big-wrap a").attr("href", Caminho);
    });
}

function MoverScrollOrdenacao() {
    if (window.location.hash.length > 0) {
        var hash = window.location.hash;
        if (hash == "#posicao-produto") {
            window.scrollBy(0, 473);
        }
    }
}

function MudarOrdenacao() {
    $("#ordenacao").change(function () {
        var Pagina = 1;
        var Pesquisa = "";
        var Ordenacao = $(this).val();
        var Fragmento = "#posicao-produto";

        var QueryString = new URLSearchParams(window.location.search);
        if (QueryString.has("pagina")) {
            Pagina = QueryString.get("pagina");
        }
        if (QueryString.has("pesquisa")) {
            Pesquisa = QueryString.get("pesquisa");
        }
        if ($("#breadcrumb").length > 0) {
            Fragmento = "";
        }
        var URL = window.location.protocol + "//" + window.location.host + window.location.pathname;

        var URLComParametros = URL + "?pagina=" + Pagina + "&pesquisa=" + Pesquisa + "&ordenacao=" + Ordenacao + Fragmento;
        window.location.href = URLComParametros;

    });
}

/*
 * ------------------ Classes --------------------
 */
class ProdutoQuantidadeEValor {
    constructor(produtoId, quantidadeEstoque, valorUnitario, quantidadeProdutoCarrinhoAntiga, quantidadeProdutoCarrinhoNova, campoQuantidadeProdutoCarrinho, campoValor) {
        this.produtoId = produtoId;
        this.quantidadeEstoque = quantidadeEstoque;
        this.valorUnitario = valorUnitario;

        this.quantidadeProdutoCarrinhoAntiga = quantidadeProdutoCarrinhoAntiga;
        this.quantidadeProdutoCarrinhoNova = quantidadeProdutoCarrinhoNova;

        this.campoQuantidadeProdutoCarrinho = campoQuantidadeProdutoCarrinho;
        this.campoValor = campoValor;
    }
}

function RemoverMascara(valor) {
    return valor.replace(".", "").replace("-", "");
}