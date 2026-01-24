function init() {
    $("#mantenimiento_form").on("submit", function (e) {
        guardaryeditar(e);
    });
}

function guardaryeditar(e) {
    e.preventDefault();

    var formData = new FormData($("#mantenimiento_form")[0]);

    $.ajax({
        url: "/SubCategoria/Guardar",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                tablaSubCategorias.ajax.reload(null, false); // <-- usar la instancia
                $("#modalmantenimiento").modal("hide");
                swal.fire({
                    title: "SubCategoria",
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
                text: "No se pudo guardar el SubCategoria.",
                icon: "error",
            });
            console.error("Error AJAX:", error);
        }
    });
}


function cargarCategorias() {
    $.ajax({
        url: "/Categoria/ComboCategoria",
        type: "GET",
        success: function (data) {
            let options = '<option value="">Seleccionar</option>';
            data.forEach(c => {
                options += `<option value="${c.id}">${c.nombre}</option>`;
            });
            $("#cat_id").html(options);
        }
    });
}


var tablaSubCategorias; // <-- instancia global

$(document).ready(function () {
    cargarCategorias();

    tablaSubCategorias = $("#table_data").DataTable({
        aProcessing: true,
        aServerSide: true,
        dom: "Bfrtip",
        buttons: ["copyHtml5", "excelHtml5", "csvHtml5"],
        ajax: {
            url: "/SubCategoria/Listar",
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
    if (!$("#scat_id").length || !$("#scat_nom").length || !$("#mantenimiento_form").length) {
        console.error("❌ Elementos del formulario no encontrados en el DOM");
        alert("Error: No se encontraron los elementos del formulario.");
        return;
    }

    // Limpiar campos
    $("#scat_id").val("");
    $("#scat_nom").val("");
    $("#scat_descrip").val("");
    $("#scat_fil").val("");
    $("#cat_id").val("");
    console.log("✅ Campos limpiados: usu_id y usu_nom");

    // Cambiar el título del modal
    $("#lbltitulo").html("Nuevo Registro");
    console.log("✅ Título del modal cambiado a 'Nuevo Registro'");



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

function editar(scat_id) {
    $.ajax({
        url: "/SubCategoria/Mostrar",
        type: "POST",
        data: { id: scat_id },
        success: function (response) {
            if (response.success) {
                console.log(response);
                $("#scat_id").val(response.id);
                $("#scat_nom").val(response.nombre);
                $("#scat_descrip").val(response.descripcion);
                $("#scat_fil").val(response.filter);
                $("#cat_id").val(response.categoriaId);
                $("#lbltitulo").html("Editar Registro");
                $("#modalmantenimiento").modal("show");
            } else {
                Swal.fire("Error", response.message, "error");
            }
        },
        error: function () {
            Swal.fire("Error", "No se pudo obtener el Categoria", "error");
        }
    });
}

function eliminar(scat_id) {
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
                url: "/SubCategoria/Eliminar",
                type: "POST",
                data: { id: scat_id },
                success: function (response) {
                    if (response.success) {
                        $("#table_data").DataTable().ajax.reload();
                        Swal.fire("SubCategoria", response.message, "success");
                    } else {
                        Swal.fire("Error", response.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "No se pudo eliminar el SubCategoria", "error");
                }
            });
        }
    });
}



init();

