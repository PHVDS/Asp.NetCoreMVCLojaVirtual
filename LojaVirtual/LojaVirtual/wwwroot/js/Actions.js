$(document).ready(function () {
    $(".btn-danger").click(function (e) {
        var resultado = confirm("Tem certeza que quer realizar está operação?");

        if (resultado == false) {
            e.preventDefault();
        }
    });
    $('.dinheiro').mask('000.000.000.000.000,00', { reverse: true });
});

$(document).ready(function () {
    $(".btn-outline-danger").click(function (e) {
        var resultado = confirm("Tem certeza que quer realizar está operação?");

        if (resultado == false) {
            e.preventDefault();
        }
    });
});