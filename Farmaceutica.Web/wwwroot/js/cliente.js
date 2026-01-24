




var tablaUsuarios; // <-- instancia global

$(document).ready(function () {
    tablaUsuarios = $("#table_data").DataTable({
        aProcessing: true,
        aServerSide: true,
        dom: "Bfrtip",
        buttons: ["copyHtml5", "excelHtml5", "csvHtml5"],
        ajax: {
            url: "/Clientes/Listar",
            type: "POST"
        },
        bDestroy: true,
        responsive: true,
        bInfo: true,
        iDisplayLength: 10,
        order: [[0, "desc"]],
        language: {
            sProcessing: "Procesando...",
            sLengthMenu: "Mostrar _MENU_ registros",
            sZeroRecords: "No se encontraron resultados",
            sEmptyTable: "Ningún dato disponible en esta tabla",
            sInfo: "Mostrando _START_ a _END_ de _TOTAL_ registros",
            sInfoEmpty: "Mostrando 0 de 0 registros",
            sInfoFiltered: "(filtrado de _MAX_ registros totales)",
            sSearch: "Buscar:",
            oPaginate: {
                sFirst: "Primero",
                sLast: "Último",
                sNext: "Siguiente",
                sPrevious: "Anterior",
            },
        },
    });
});

function editar(usu_id) {
    $.ajax({
        url: "/Clientes/Mostrar",
        type: "POST",
        data: { id: usu_id },
        success: function (response) {
            if (response.success) {
                console.log(response);
                $("#usu_id").val(response.id);
                $("#usu_correo").val(response.email);
                $("#usu_nom").val(response.nombre);
                $("#usu_ape").val(response.apellido);
                $("#usu_usu").val(response.usuarioNombre);
                $("#usu_telf").val(response.telefono);
                $("#rol_id").val(response.rol).trigger("change");

                $("#pre_imagen").html(
                    '<img src="/images/users/' + response.imagen + '" class="rounded-circle avatar-xl img-thumbnail user-profile-image">'
                );

                $("#lbltitulo").html("Registro del Cliente" + response.nombre);
                $("#modalmantenimiento").modal("show");
            } else {
                Swal.fire("Error", response.message, "error");
            }
        },
        error: function () {
            Swal.fire("Error", "No se pudo obtener el cliente", "error");
        }
    });
}

function eliminar(usu_id) {
    Swal.fire({
        title: "Eliminar!",
        text: "Desea eliminar el registro?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí",
        cancelButtonText: "No"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Clientes/Eliminar",
                type: "POST",
                data: { id: usu_id },
                success: function (response) {
                    if (response.success) {
                        $("#table_data").DataTable().ajax.reload();
                        Swal.fire("Cliente", response.message, "success");
                    } else {
                        Swal.fire("Error", response.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "No se pudo eliminar el cliente", "error");
                }
            });
        }
    });
}


function filePreview(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#pre_imagen").html(
                "<img src=" +
                e.target.result +
                ' class="rounded-circle avatar-xl img-thumbnail user-profile-image" alt="user-profile-image"></img>'
            );
        };
        reader.readAsDataURL(input.files[0]);
    }
}

$(document).on("change", "#usu_img", function () {
    filePreview(this);
});

$(document).on("click", "#btnremovephoto", function () {
    $("#usu_img").val("");
    $("#pre_imagen").html(
        '<img src="../images/users/usuario.png" class="rounded-circle avatar-xl img-thumbnail user-profile-image" alt="user-profile-image"></img><input type="hidden" name="hidden_usuario_imagen" value="" />'
    );
});

init();

