﻿ $(document).ready(function () {
    $(".btn-danger").click(function (e) {
        var resultado = confirm("Tem certeza que quer realizar está operação?");

        if (resultado == false) {
            e.preventDefault();
        }
    });
    $('.dinheiro').mask('000.000.000.000.000,00', { reverse: true });

     AJAXUploadImagemProduto();
     CategoriaSlug();
 });

function CategoriaSlug() {
    if ($("#form-categoria").length > 0) {
        $("input[name=Nome]").keyup(function myfunction() {
            $("input[name=Slug]").val(convertToSlug($(this).val()));
        });
        
    }
}

function AJAXUploadImagemProduto() {
    $(".img-upload").click(function () {
        $(this).parent().parent().find(".input-file").click();
    });

    $(".btn-imagem-excluir").click(function () {
        var CampoHidden = $(this).parent().find("input[name=imagem]");
        var Imagem = $(this).parent().find(".img-upload");
        var BtnExcluir = $(this).parent().find(".btn-imagem-excluir");
        var InputFile = $(this).parent().find(".input-file");

        $.ajax({
            type: "GET",
            url: "/Colaborador/Imagem/Deletar?caminho=" + CampoHidden.val(),
            error: function () {

            },
            success: function () {
                Imagem.attr("src", "/img/imagem-padrao.png");
                BtnExcluir.addClass("btn-ocultar");
                CampoHidden.val("");
                InputFile.val("");
            }
        });
    });

    $(".input-file").change(function () {

        var ArquivoEnviado = $(this)[0].files[0].name;
        var EhPermitidoUpload = true;

        $("input[name=imagem]").each(function () {
            if ( $(this).val().length > 0 ) {
                var NomeDaImagem = $(this).val().split("/")[3];

                if (NomeDaImagem == ArquivoEnviado) {
                    alert(`Não é permitido enviar imagens com o mesmo nome, renomeia a imagem e envie novamente! (${NomeDaImagem})`);
                    EhPermitidoUpload = false;
                }
            }
        });

        if (!EhPermitidoUpload) return;

        //Formulário de dados via JavaScript
        var Binario = $(this)[0].files[0];
        var Formulario = new FormData();
        Formulario.append("file", Binario);

        var CampoHidden = $(this).parent().find("input[name=imagem]");
        var Imagem = $(this).parent().find(".img-upload");
        var BtnExcluir = $(this).parent().find(".btn-imagem-excluir");

        //Apresenta Imagem Loading
        Imagem.attr("src", "/img/loading.gif");
        Imagem.addClass("thumb");
        
        //Requisição Ajax enviado a Formulario
        $.ajax({
            type: "POST",
            url: "/Colaborador/Imagem/Armazenar",
            data: Formulario,
            contentType: false,
            processData: false,
            error: function () {
                alert("Erro no envio do arquivo!");
                Imagem.attr("src", "/img/imagem-padrao.png");
                Imagem.removeClass("thumb");
            },
            success: function (data) {
                var caminho = data.caminho;
                Imagem.attr("src", caminho);
                Imagem.removeClass("thumb");
                CampoHidden.val(caminho);
                BtnExcluir.removeClass("btn-ocultar");
            }
        });
    });
}

$(document).ready(function () {
    $(".btn-outline-danger").click(function (e) {
        var resultado = confirm("Tem certeza que quer realizar está operação?");

        if (resultado == false) {
            e.preventDefault();
        }
    });
});

function convertToSlug(Text) {
    return Text
        .toLowerCase()
        .replace(/ /g, '-')
        .replace(/[^\w-]+/g, '');
}
