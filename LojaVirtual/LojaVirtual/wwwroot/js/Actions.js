$(document).ready(function () {
    $(".btn-danger").click(function (e) {
        var resultado = confirm("Tem certeza que deseja excluir?");

        if (resultado == false) {
            e.preventDefault();
        }
    });
});