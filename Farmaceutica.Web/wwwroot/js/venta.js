var usu_id = $('#USU_IDx').val();
console.log("Usuario ID:", usu_id);

$(document).ready(function () {
    // Inicializar Select2
    $('#cat_id').select2();
    $('#cli_id').select2();
    $('#prod_id').select2();

    // Registrar venta inicial
    $.post("/Venta/Registrar", { usu_id: usu_id }, function (data) {
        if (data.ok) {
            console.log("ID generado:", data.id);
            $('#vent_id').val(data.id);
        } else {
            console.error(data.message);
        }
    });

    // Cargar clientes al iniciar
    $.get("/Clientes/ComboClientes", function (data) {
        console.log("✅ Clientes cargados:", data);
        $('#cli_id').empty().append('<option value="0">Seleccione</option>');
        $.each(data, function (index, cliente) {
            $('#cli_id').append(
                $('<option>', {
                    value: cliente.id,
                    text: cliente.nombre
                })
            );
        });
        $('#cli_id').trigger('change.select2');
    }).fail(function (xhr, status, error) {
        console.error("❌ Error al cargar clientes:", error);
    });

    // Cargar categorías al iniciar
    $.get("/Categoria/ComboCategoria", function (data) {
        console.log("✅ Categorías cargadas:", data);
        $('#cat_id').empty().append('<option value="0">Seleccione</option>');
        $.each(data, function (index, categoria) {
            $('#cat_id').append(
                $('<option>', {
                    value: categoria.id,
                    text: categoria.nombre
                })
            );
        });
        $('#cat_id').trigger('change.select2');
    }).fail(function (xhr, status, error) {
        console.error("❌ Error al cargar categorías:", error);
    });

    // Cuando se selecciona un cliente, obtener sus datos
    $('#cli_id').change(function () {
        var cli_id = $(this).val();
        console.log("👤 Cliente seleccionado:", cli_id);

        if (cli_id == 0 || cli_id === null || cli_id === "") {
            $('#cli_ruc').val('');
            $('#cli_direcc').val('');
            $('#cli_correo').val('');
            $('#cli_telf').val('');
            return;
        }

        $.post("/Clientes/Mostrar", { id: cli_id }, function (data) {
            console.log("📦 Datos del cliente:", data);
            $('#cli_ruc').val(data.ruc);
            $('#cli_direcc').val(data.direccion);
            $('#cli_correo').val(data.email);
            $('#cli_telf').val(data.telefono);
        }).fail(function (xhr, status, error) {
            console.error("❌ Error al cargar cliente:", error);
            console.log(xhr.responseText);
        });
    });

    // Cuando se selecciona una categoría, cargar productos
    $('#cat_id').change(function () {
        var cat_id = $(this).val();
        console.log("📂 Categoría seleccionada:", cat_id);

        if (cat_id == 0 || cat_id === null || cat_id === "") {
            $('#prod_id').empty().append('<option value="0">Seleccione</option>');
            $('#prod_id').trigger('change.select2');
            return;
        }

        $.ajax({
            url: "/Producto/ComboProductos",
            type: "GET",
            data: { cat_id: cat_id },
            cache: false,
            dataType: "json",
            success: function (data) {
                console.log("✅ Productos recibidos:", data);
                $('#prod_id').empty().append('<option value="0">Seleccione</option>');
                $.each(data, function (index, producto) {
                    $('#prod_id').append(
                        $('<option>', {
                            value: producto.id,
                            text: producto.nombre
                        })
                    );
                });
                $('#prod_id').trigger('change.select2');
            },
            error: function (xhr, status, error) {
                console.error("❌ Error al cargar productos:", error);
                console.log("Response:", xhr.responseText);
            },
            complete: function () {
                console.log("🔄 Petición a ComboProductos completada.");
            }
        });
    });

    $('#prod_id').change(function () {
        var prod_id = $(this).val();
        if (prod_id == 0 || prod_id === null || prod_id === "") {
            $("#prod_pventa").val('');
            $("#prod_stock").val('');
            $("#und_nom").val('');
            return;
        }

        $.post("/Producto/MostrarVenta", { prod_id: prod_id }, function (data) {
            if (typeof data === "string") {
                data = JSON.parse(data);
            }
            console.log(data);
            $("#prod_pventa").val(data.precio);
            $("#prod_codigo").val(data.codigo);
            $("#prod_stock").val(data.stockLocal);
            $("#und_nom").val(data.UND_NOM);
        }).fail(function (xhr, status, error) {
            console.error("❌ Error al mostrar producto:", error);
            console.log("Response:", xhr.responseText);
        });
    });
});

// ============================================================
// FUNCIONES PARA EL MODAL DE CLIENTES (CORREGIDAS)
// ============================================================

$(document).on("click", "#btnNuevoCliente", function (e) {
    e.preventDefault(); // Evita comportamiento por defecto
    e.stopPropagation(); // Evita propagación del evento

    console.log("🟢 Botón NUEVO CLIENTE clickeado");

    // Cambiar título del modal (si es para cliente, no para usuario)
    // Si el modal es para clientes, deberías cambiar esto:
    if ($("#lbltitulo").length) {
        $("#lbltitulo").text("Nuevo Cliente");
    }

    // Si el modal es para usuarios pero el botón dice "Nuevo Cliente",
    // probablemente necesites un modal diferente para clientes.
    // Por ahora, dejamos el modal de usuarios como está.

    // IMPORTANTE: Si el modal es para USUARIOS, pero el botón dice "NUEVO CLIENTE"
    // hay una inconsistencia. ¿Dónde está el modal para clientes?

    // Mostrar el modal
    if ($("#modalmantenimiento").length) {
        $("#modalmantenimiento").modal("show");
        console.log("✅ Modal de mantenimiento mostrado");
    } else {
        console.error("❌ Modal #modalmantenimiento no encontrado");
        swal.fire({
            title: "Error",
            text: "No se encontró el formulario para agregar cliente/usuario",
            icon: "error"
        });
    }
});

// OPCIÓN ALTERNATIVA: Si necesitas un modal específico para clientes
// Deberías tener un modal con id="modalCliente" con campos para cliente

$(document).on("click", "#btnagregar", function () {
    var vent_id = $("#vent_id").val();
    var prod_id = $("#prod_id").val();
    var prod_pventa = $("#prod_pventa").val();
    var detv_cant = $("#detv_cant").val();

    console.log("📝 Datos del formulario:", {
        vent_id,
        prod_id,
        prod_pventa,
        detv_cant
    });

    if (!prod_id || !prod_pventa || !detv_cant) {
        Swal.fire({
            title: "Venta",
            text: "Tienes campos vacíos",
            icon: "error",
        });
        console.warn("❗ Campos vacíos, no se envía info");
        return;
    }

    $.ajax({
        url: "/Venta/GuardarDetalle",
        type: "POST",
        data: {
            VentId: vent_id,
            ProdId: prod_id,
            ProdPVenta: prod_pventa,
            DetvCant: detv_cant
        },
        beforeSend: function () {
            console.log("🚀 Enviando datos a /Venta/GuardarDetalle...");
        },
        success: function (response) {
            console.log("✅ Detalle guardado:", response);

            $.ajax({
                url: "/Venta/Calculo",
                type: "POST",
                data: { vent_id: vent_id },
                beforeSend: function () {
                    console.log("🔄 Solicitando cálculo de totales para VentId:", vent_id);
                },
                success: function (data) {
                    console.log("💰 Totales calculados:", data);
                    $("#txtsubtotal").html(data.subtotal);
                    $("#txtigv").html(data.igv);
                    $("#txttotal").html(data.total);
                },
                error: function (xhr, status, error) {
                    console.error("❌ Error al calcular totales:", error);
                    console.log("Response:", xhr.responseText);
                }
            });

            $("#prod_pventa").val("");
            $("#detv_cant").val("");
            console.log("🔹 Inputs limpiados, recargando tabla...");
            listar(vent_id);
        },
        error: function (xhr, status, error) {
            console.error("❌ Error al guardar detalle:", error);
            console.log("Response:", xhr.responseText);
        }
    });
});

function listar(vent_id) {
    $('#table_data').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: "/Venta/ListarDetalle",
            type: "POST",
            data: { vent_id: vent_id }
        },
        destroy: true,
        responsive: true,
        info: true,
        pageLength: 10,
        order: [[0, "desc"]],
        dom: 'Bfrtip',
        buttons: [
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
        ],
        language: {
            sProcessing: "Procesando...",
            sLengthMenu: "Mostrar _MENU_ registros",
            sZeroRecords: "No se encontraron resultados",
            sEmptyTable: "Ningún dato disponible en esta tabla",
            sInfo: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            sInfoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
            sInfoFiltered: "(filtrado de un total de _MAX_ registros)",
            sSearch: "Buscar:",
            oPaginate: {
                sFirst: "Primero",
                sLast: "Último",
                sNext: "Siguiente",
                sPrevious: "Anterior"
            }
        }
    });
}

$(document).on("click", "#btnguardar", function () {
    var vent_id = $("#vent_id").val();
    var pag_id = $("#pag_id").val();
    var cli_id = $("#cli_id").val();
    var cli_ruc = $("#cli_ruc").val();
    var cli_direcc = $("#cli_direcc").val();
    var cli_correo = $("#cli_correo").val();
    var vent_coment = $("#vent_coment").val();
    var mon_id = $("#mon_id").val();

    console.log("🟢 Datos recogidos:", {
        vent_id, pag_id, cli_id, cli_ruc, cli_direcc, cli_correo, vent_coment, mon_id
    });

    if (pag_id == '0' || cli_id == '0' || mon_id == '0') {
        console.warn("⚠️ Error: Campos vacíos");
        swal.fire({ title: "Venta", text: "Error: Campos vacíos", icon: "error" });
        return;
    }

    console.log("🔹 Llamando a /Venta/Calculo para vent_id:", vent_id);
    $.post("/Venta/Calculo", { vent_id: vent_id }, function (data) {
        console.log("💰 Totales recibidos:", data);

        if (!data.total || data.total == 0) {
            console.warn("⚠️ Error: No existe detalle");
            swal.fire({ title: "Venta", text: "Error: No existe detalle", icon: "error" });
        } else {
            console.log("🔹 Llamando a /Venta/GuardarVenta con los datos:", {
                VentId: vent_id,
                PagId: pag_id,
                CliId: cli_id,
                CliRuc: cli_ruc,
                CliDirecc: cli_direcc,
                CliCorreo: cli_correo,
                VentComent: vent_coment,
                MonId: mon_id
            });

            $.post("/Venta/GuardarVenta", {
                VentId: vent_id,
                PagId: pag_id,
                CliId: cli_id,
                CliRuc: cli_ruc,
                CliDirecc: cli_direcc,
                CliCorreo: cli_correo,
                VentComent: vent_coment,
                MonId: mon_id
            }, function (resp) {
                console.log("✅ Respuesta de GuardarVenta:", resp);

                if (resp.ok) {
                    swal.fire({
                        title: "Venta",
                        text: resp.message,
                        icon: "success",
                        footer: `<a href='/Venta/Ver?v=${vent_id}' target='_blank'>Ver documento</a>`
                    });
                } else {
                    console.error("❌ Error en GuardarVenta:", resp.message);
                    swal.fire({ title: "Error", text: resp.message, icon: "error" });
                }
            }).fail(function (xhr, status, error) {
                console.error("❌ Fallo en AJAX GuardarVenta:", { xhr, status, error });
                swal.fire({ title: "Error", text: "Error al guardar la venta", icon: "error" });
            });
        }
    }).fail(function (xhr, status, error) {
        console.error("❌ Fallo en AJAX Calculo:", { xhr, status, error });
        swal.fire({ title: "Error", text: "Error al calcular totales", icon: "error" });
    });
});

function eliminar(detv_id, vent_id) {
    swal.fire({
        title: "Eliminar!",
        text: "Desea Eliminar el Registro?",
        icon: "error",
        confirmButtonText: "Si",
        showCancelButton: true,
        cancelButtonText: "No",
    }).then((result) => {
        if (result.value) {
            console.log("🔹 Eliminando detalle:", detv_id);

            $.post("/Venta/EliminarDetalle", { detv_id: detv_id }, function (resp) {
                console.log("✅ Respuesta EliminarDetalle:", resp);

                if (resp.ok) {
                    $.post("/Venta/Calculo", { vent_id: vent_id }, function (totales) {
                        console.log("💰 Totales recalculados:", totales);

                        $('#txtsubtotal').html(totales.subtotal);
                        $('#txtigv').html(totales.igv);
                        $('#txttotal').html(totales.total);

                        listar(vent_id);

                        swal.fire({
                            title: 'Venta',
                            text: 'Registro Eliminado',
                            icon: 'success'
                        });
                    }).fail(function (xhr, status, error) {
                        console.error("❌ Error recalculando totales:", { xhr, status, error });
                        swal.fire({ title: "Error", text: "No se pudo recalcular totales", icon: "error" });
                    });
                } else {
                    swal.fire({ title: "Error", text: resp.message, icon: "error" });
                }
            }).fail(function (xhr, status, error) {
                console.error("❌ Error eliminando detalle:", { xhr, status, error });
                swal.fire({ title: "Error", text: "No se pudo eliminar el detalle", icon: "error" });
            });
        } 
    });
}

$(document).on("click", "#btnlimpiar", function () {
    location.reload();
});

// ============================================================
// FUNCIONES PARA EL MODAL DE USUARIOS (MANTENIMIENTO)
// ============================================================

// Inicializar el formulario de mantenimiento (usuarios)
function init() {
    $("#mantenimiento_form").on("submit", function (e) {
        guardaryeditar(e);
    });
}

function guardaryeditar(e) {
    e.preventDefault();

    var formData = new FormData($("#mantenimiento_form")[0]);

    // Mostrar indicador de carga
    Swal.fire({
        title: 'Guardando...',
        text: 'Por favor espere',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: "/Usuarios/Guardar",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            Swal.close();

            if (response.success) {
                // Cerrar modal
                $("#modalmantenimiento").modal("hide");

                // Mostrar mensaje de éxito
                Swal.fire({
                    title: "¡Éxito!",
                    text: response.message || "Cliente guardado correctamente",
                    icon: "success",
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => {
                    // Recargar la lista de clientes
                    recargarClientesConSeleccion(response.id);
                });

            } else {
                Swal.fire({
                    title: "Error",
                    text: response.message || "No se pudo guardar el cliente",
                    icon: "error",
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.close();
            Swal.fire({
                title: "Error del sistema",
                text: "Ocurrió un error al intentar guardar. Por favor intente nuevamente.",
                icon: "error",
            });
            console.error("Error AJAX:", error, xhr.responseText);
        }
    });
}

// Función mejorada para recargar clientes y seleccionar el nuevo
function recargarClientesConSeleccion(nuevoClienteId) {
    console.log("🔄 Recargando clientes, nuevo ID:", nuevoClienteId);

    $.ajax({
        url: "/Clientes/ComboClientes",
        type: "GET",
        dataType: "json",
        beforeSend: function () {
            // Opcional: Mostrar indicador en el select
            $('#cli_id').prop('disabled', true).next('.select2-container').addClass('select2-container-disabled');
        },
        success: function (data) {
            console.log("✅ Clientes recargados:", data.length, "registros");

            // Limpiar select
            $('#cli_id').empty().append('<option value="0">Seleccione cliente</option>');

            // Agregar opciones
            $.each(data, function (index, cliente) {
                $('#cli_id').append(
                    $('<option>', {
                        value: cliente.id,
                        text: cliente.nombre
                    })
                );
            });

            // Seleccionar el nuevo cliente si existe
            if (nuevoClienteId) {
                // Esperar un momento para que Select2 se actualice
                setTimeout(function () {
                    $('#cli_id').val(nuevoClienteId).trigger('change.select2');
                    console.log("✅ Nuevo cliente seleccionado:", nuevoClienteId);
                }, 300);
            }

            // Cargar datos del cliente si se seleccionó
            if (nuevoClienteId && nuevoClienteId != '0') {
                setTimeout(function () {
                    $('#cli_id').trigger('change');
                }, 500);
            }
        },
        error: function (xhr, status, error) {
            console.error("❌ Error al recargar clientes:", error);
            Swal.fire({
                title: "Atención",
                text: "El cliente se guardó, pero no se pudo actualizar la lista. Por favor refresque la página.",
                icon: "warning",
                timer: 3000
            });
        },
        complete: function () {
            // Reactivar el select
            $('#cli_id').prop('disabled', false).next('.select2-container').removeClass('select2-container-disabled');
        }
    });
}

// Función para previsualizar imagen en el modal de usuarios
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

// Inicializar
init();

// ============================================================
// OBSERVACIÓN IMPORTANTE:
// ============================================================
// Hay una confusión en tu código:
// 1. El botón #btnNuevoCliente parece estar en el formulario de VENTAS
// 2. Pero abre el modal #modalmantenimiento que es para USUARIOS
// 3. Si quieres agregar un NUEVO CLIENTE desde el formulario de ventas,
//    probablemente necesites un modal diferente con campos de cliente
//    (ruc, dirección, teléfono, email) en lugar de campos de usuario