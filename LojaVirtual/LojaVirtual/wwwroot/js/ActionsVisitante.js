$(document).ready(function () {
    MoverScrollOrdenacao();
    MudarOrdenacao();
    MudarImagemPrincipalProduto();
    MudarQuantidadeProdutoCarrinho();
});

function numberToReal(numero) {
    var numero = numero.toFixed(2).split('.');
    numero[0] = "R$ " + numero[0].split(/(?=(?:...)*$)/).join('.');
    return numero.join(',');
}

function MudarQuantidadeProdutoCarrinho() {
    $("#order .btn-primary").click(function () {
        var pai = $(this).parent().parent();
        if ($(this).hasClass("diminuir")) {
            LogicaMudarQuantidadeProdutoUnitarioCarrinho("diminuir", $(this));
            var id = pai.find(".inputProdutoId").val();

        }
        if ($(this).hasClass("aumentar")) {
            LogicaMudarQuantidadeProdutoUnitarioCarrinho("aumentar", $(this));
         
        }
    });
}

function LogicaMudarQuantidadeProdutoUnitarioCarrinho(operacao, botao) {
    var pai = botao.parent().parent();

    var produtoId = pai.find(".inputProdutoId").val();
    var quantidadeEstoque = parseInt(pai.find(".inputQuantidadeEstoque").val());
    var valorUnitario = parseFloat(pai.find(".inputValorUnitario").val().replace("," , "."));

    var campoQuantidadeProdutoCarrinho = pai.find(".inputQuantidadeProdutoCarrinho");
    var quantidadeProdutoCarrinho = parseInt(campoQuantidadeProdutoCarrinho.val());

    var campoValor = botao.parent().parent().parent().parent().parent().find(".price");

    if (operacao == "aumentar")
    {
        if (quantidadeProdutoCarrinho == quantidadeEstoque) {
            alert("Opps! Voçê atingiu o valor total do estoque disponivel!");
        }
        else
        {
            quantidadeProdutoCarrinho++;
            campoQuantidadeProdutoCarrinho.val(quantidadeProdutoCarrinho);

            var resultado = valorUnitario * quantidadeProdutoCarrinho;
            campoValor.text(numberToReal(resultado));
        }
    }
    else if (operacao == "diminuir")
    {
        if (quantidadeProdutoCarrinho == 1) {
            alert("Opps! Valor Insuficiente!");
        }
        else
        {
            quantidadeProdutoCarrinho--;
            campoQuantidadeProdutoCarrinho.val(quantidadeProdutoCarrinho);

            var resultado = valorUnitario * quantidadeProdutoCarrinho;
            campoValor.text(numberToReal(resultado));
        }
    }
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