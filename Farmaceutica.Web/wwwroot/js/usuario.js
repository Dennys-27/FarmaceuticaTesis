function init() {
    $("#mantenimiento_form").on("submit", function (e) {
        guardaryeditar(e);
    });
}

function guardaryeditar(e) {
    e.preventDefault();

    var formData = new FormData($("#mantenimiento_form")[0]);

    $.ajax({
        url: "/Usuarios/Guardar",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                tablaUsuarios.ajax.reload(null, false); // <-- usar la instancia
                $("#modalmantenimiento").modal("hide");
                swal.fire({
                    title: "Usuario",
                    text: "Registro Confirmado",
                    icon: "success",
                });
            } else {
                swal.fire({
                    title: "Error",
                    text: response.message,
                    icon: "error",
                });
            }
        },
        error: function (xhr, status, error) {
            swal.fire({
                title: "Error",
                text: "No se pudo guardar el usuario.",
                icon: "error",
            });
            console.error("Error AJAX:", error);
        }
    });
}




var tablaUsuarios; // <-- instancia global

$(document).ready(function () {
    tablaUsuarios = $("#table_data").DataTable({
        aProcessing: true,
        aServerSide: true,
        dom: "Bfrtip",
        buttons: ["copyHtml5", "excelHtml5", "csvHtml5"],
        ajax: {
            url: "/Usuarios/Listar",
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







$(document).on("click", "#btnnuevo", function () {
    console.log("🟢 Evento click detectado en #btnnuevo");

    // Verificar si existen los elementos antes de usarlos
    if (!$("#usu_id").length || !$("#usu_nom").length || !$("#mantenimiento_form").length) {
        console.error("❌ Elementos del formulario no encontrados en el DOM");
        alert("Error: No se encontraron los elementos del formulario.");
        return;
    }

    // Limpiar campos
    $("#usu_id").val("");
    $("#usu_nom").val("");
    console.log("✅ Campos limpiados: usu_id y usu_nom");

    // Cambiar el título del modal
    $("#lbltitulo").html("Nuevo Registro");
    console.log("✅ Título del modal cambiado a 'Nuevo Registro'");

    // Insertar imagen por defecto
    $("#pre_imagen").html(
        '<img src="../images/users/usuario.png" class="rounded-circle avatar-xl img-thumbnail user-profile-image" alt="user-profile-image">' +
        '<input type="hidden" name="hidden_usuario_imagen" value="" />'
    );
    console.log("✅ Imagen de usuario cargada correctamente");

    // Reiniciar el formulario
    $("#mantenimiento_form")[0].reset();
    console.log("✅ Formulario reiniciado");

    // Mostrar el modal
    if ($("#modalmantenimiento").length) {
        $("#modalmantenimiento").modal("show");
        console.log("✅ Modal mostrado correctamente");
    } else {
        console.error("❌ No se encontró el modal con id #modalmantenimiento");
        alert("Error: No se encontró el modal.");
    }

    // Confirmación final visual
    alert("🟢 Botón NUEVO funcionando correctamente.");
});

function editar(usu_id) {
    $.ajax({
        url: "/Usuarios/Mostrar",
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

                $("#lbltitulo").html("Editar Registro");
                $("#modalmantenimiento").modal("show");
            } else {
                Swal.fire("Error", response.message, "error");
            }
        },
        error: function () {
            Swal.fire("Error", "No se pudo obtener el usuario", "error");
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
                url: "/Usuarios/Eliminar",
                type: "POST",
                data: { id: usu_id },
                success: function (response) {
                    if (response.success) {
                        $("#table_data").DataTable().ajax.reload();
                        Swal.fire("Usuario", response.message, "success");
                    } else {
                        Swal.fire("Error", response.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "No se pudo eliminar el usuario", "error");
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

