var usu_id = $('#USU_IDx').val();
console.log("Usuario ID:", usu_id);

$(document).ready(function () {

    // ========================================================
    // Inicializar Select2
    // ========================================================
    $('#cat_id').select2();
    $('#prov_id').select2();
    $('#prod_id').select2();


    $.post("/Compra/Registrar", { usu_id: usu_id }, function (data) {
        if (data.ok) {
            console.log("ID generado:", data.id);
            $('#compr_id').val(data.id);
        } else {
            console.error(data.message);
        }
    });



    // ========================================================
    // 1️⃣ Cargar proveedores al iniciar
    // ========================================================
    $.get("/Proveedor/ComboProveedores", function (data) {
        console.log("✅ Proveedor cargados:", data);
        $('#prov_id').empty().append('<option value="0">Seleccione</option>');
        $.each(data, function (index, provedor) {
            $('#prov_id').append(
                $('<option>', {
                    value: provedor.id,
                    text: provedor.nombre
                })
            );
        });
        $('#prov_id').trigger('change.select2');
    }).fail(function (xhr, status, error) {
        console.error("❌ Error al cargar proveedores:", error);
    });

    // ========================================================
    // 2️⃣ Cargar categorías al iniciar
    // ========================================================
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

    // ========================================================
    // 3️⃣ Cuando se selecciona un proveedor, obtener sus datos
    // ========================================================
    $('#prov_id').change(function () {
        var prov_id = $(this).val();
        console.log("👤 Proveedor seleccionado:", prov_id);

        if (prov_id == 0 || prov_id === null || prov_id === "") {
            $('#prov_ruc').val('');
            $('#prov_direcc').val('');
            $('#prov_correo').val('');
            $('#prov_telf').val('');
            return;
        }

        $.post("/Proveedor/Mostrar", { id: prov_id }, function (data) {
            console.log("📦 Datos del proveedor:", data);
            $('#prov_ruc').val(data.ruc);
            $('#prov_direcc').val(data.direccion);
            $('#prov_correo').val(data.email);
            $('#prov_telf').val(data.telefono);
        }).fail(function (xhr, status, error) {
            console.error("❌ Error al cargar proveedor:", error);
            console.log(xhr.responseText);
        });
    });

    // ========================================================
    // 4️⃣ Cuando se selecciona una categoría, cargar productos
    // ========================================================
    $('#cat_id').change(function () {
        var cat_id = $(this).val();
        console.log("📂 Categoría seleccionada:", cat_id);

        if (cat_id == 0 || cat_id === null || cat_id === "") {
            $('#prod_id').empty().append('<option value="0">Seleccione</option>');
            $('#prod_id').trigger('change.select2');
            return;
        }

        // AJAX GET a /Producto/ComboProductos
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
            // Limpiar campos si no hay selección
            $("#prod_pcompra").val('');
            $("#prod_stock").val('');
            $("#und_nom").val('');
            return;
        }

        // AJAX POST a /Producto/Mostrar
        $.post("/Producto/MostrarCompra", { prod_id: prod_id }, function (data) {
            // Asegúrate de que tu backend devuelve JSON
            // Si es string, parsearlo:
            if (typeof data === "string") {
                data = JSON.parse(data);
            }
            console.log(data);

            $("#prod_pcompra").val(data.precio);
            $("#prod_codigo").val(data.codigo);
            $("#prod_stock").val(data.stockTotal);
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

$(document).on("click", "#btnagregar", function () {
    var compr_id = $("#compr_id").val();
    console.log(compr_id);
    var prod_id = $("#prod_id").val();
    var prod_pcompra = $("#prod_pcompra").val();
    var detc_cant = $("#detc_cant").val();

    // 🔹 Validar campos vacíos
    if (!prod_id || !prod_pcompra || !detc_cant) {
        Swal.fire({
            title: "Compra",
            text: "Tienes campos vacíos",
            icon: "error",
        });
        return;
    }

    // =====================================================
    // 1️⃣ Guardar detalle de la Compra
    // =====================================================
    $.ajax({
        url: "/Compra/GuardarDetalle",
        type: "POST",
        data: {
            ComprId: compr_id,
            ProdId: prod_id,
            ProdPCompra: prod_pcompra,
            DetcCant: detc_cant
        },
        success: function (response) {
            console.log("✅ Detalle guardado:", response);

            // =====================================================
            // 2️⃣ Calcular subtotal, igv y total
            // =====================================================
            $.ajax({
                url: "/Compra/Calculo",
                type: "POST",
                data: { compr_id: compr_id },
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


            // =====================================================
            // 3️⃣ Limpiar inputs y refrescar tabla de detalles
            // =====================================================
            $("#prod_pcompra").val("");
            $("#detc_cant").val("");
            listar(compr_id); // <- función que recarga la tabla de detalle
        },
        error: function (xhr, status, error) {
            console.error("❌ Error al guardar detalle:", error);
            console.log("Response:", xhr.responseText);
        }
    });
});


function listar(compr_id) {
    $('#table_data').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: "/Compra/ListarDetalle",
            type: "POST",
            data: { compr_id: compr_id }
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


function eliminar(detc_id, compr_id) {
    swal.fire({
        title: "Eliminar!",
        text: "Desea Eliminar el Registro?",
        icon: "error",
        confirmButtonText: "Si",
        showCancelButton: true,
        cancelButtonText: "No",
    }).then((result) => {
        if (result.value) {

            console.log("🔹 Eliminando detalle:", detc_id);

            // 1️⃣ Eliminar detalle
            $.post("/Compra/EliminarDetalle", { detc_id: detc_id }, function (resp) {
                console.log("✅ Respuesta EliminarDetalle:", resp);

                if (resp.ok) {

                    // 2️⃣ Recalcular totales
                    $.post("/Compra/Calculo", { compr_id: compr_id }, function (totales) {
                        console.log("💰 Totales recalculados:", totales);

                        $('#txtsubtotal').html(totales.subtotal);
                        $('#txtigv').html(totales.igv);
                        $('#txttotal').html(totales.total);

                        // 3️⃣ Refrescar tabla de detalles
                        listar(compr_id);

                        // 4️⃣ Mensaje final
                        swal.fire({
                            title: 'Compra',
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

$(document).on("click", "#btnguardar", function () {
    var compr_id = $("#compr_id").val();
    var pag_id = $("#pag_id").val();
    var prov_id = $("#prov_id").val();
    var prov_ruc = $("#prov_ruc").val();
    var prov_direcc = $("#prov_direcc").val();
    var prov_correo = $("#prov_correo").val();
    var compr_coment = $("#compr_coment").val(); // ✅ corregido nombre
    var mon_id = $("#mon_id").val();

    console.log("🟢 Datos recogidos:", {
        compr_id, pag_id, prov_id, prov_ruc, prov_direcc, prov_correo, compr_coment, mon_id
    });

    if (pag_id == '0' || prov_id == '0' || mon_id == '0') {
        console.warn("⚠️ Error: Campos vacíos");
        Swal.fire({ title: "Compra", text: "Error: Campos vacíos", icon: "error" });
        return;
    }

    console.log("🔹 Llamando a /Compra/Calculo para compr_id:", compr_id);
    $.post("/Compra/Calculo", { compr_id: compr_id }, function (data) {
        console.log("💰 Totales recibidos:", data);

        if (!data.total || data.total == 0) {
            console.warn("⚠️ Error: No existe detalle");
            Swal.fire({ title: "Compra", text: "Error: No existe detalle", icon: "error" });
        } else {
            console.log("🔹 Llamando a /Compra/GuardarCompra con los datos:", {
                ComprId: compr_id,
                PagId: pag_id,
                ProvId: prov_id,
                ProvRuc: prov_ruc,
                ProvDirecc: prov_direcc,
                ProvCorreo: prov_correo,
                ComprComent: compr_coment,
                MonId: mon_id
            });

            $.post("/Compra/GuardarCompra", {
                ComprId: compr_id,
                PagId: pag_id,
                ProvId: prov_id,
                ProvRuc: prov_ruc,
                ProvDirecc: prov_direcc,
                ProvCorreo: prov_correo,
                ComprComent: compr_coment,
                MonId: mon_id
            }, function (resp) {
                console.log("✅ Respuesta de GuardarCompra:", resp);

                if (resp.ok) {
                    Swal.fire({
                        title: "Compra",
                        text: resp.message,
                        icon: "success",
                        footer: `<a href='/Compra/Ver?v=${compr_id}' target='_blank'>Ver documento</a>`
                    });
                } else {
                    console.error("❌ Error en GuardarCompra:", resp.message);
                    Swal.fire({ title: "Error", text: resp.message, icon: "error" });
                }
            }).fail(function (xhr, status, error) {
                console.error("❌ Fallo en AJAX GuardarCompra:", { xhr, status, error });
                Swal.fire({ title: "Error", text: "Error al guardar la compra", icon: "error" });
            });
        }
    }).fail(function (xhr, status, error) {
        console.error("❌ Fallo en AJAX Calculo:", { xhr, status, error });
        Swal.fire({ title: "Error", text: "Error al calcular totales", icon: "error" });
    });
});



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
                    recargarProveedorConSeleccion(response.id);
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

// Función mejorada para recargar proveedores y seleccionar el nuevo
function recargarProveedorConSeleccion(nuevoProveedorId) {
    console.log("🔄 Recargando proveedores, nuevo ID:", nuevoProveedorId);

    $.ajax({
        url: "/Proveedor/ComboProveedores",
        type: "GET",
        dataType: "json",
        beforeSend: function () {
            // Opcional: Mostrar indicador en el select
            $('#prov_id').prop('disabled', true).next('.select2-container').addClass('select2-container-disabled');
        },
        success: function (data) {
            console.log("✅ Proveedores recargados:", data.length, "registros");

            // Limpiar select
            $('#prov_id').empty().append('<option value="0">Seleccione Proveedor</option>');

            // Agregar opciones
            $.each(data, function (index, proveedor) {
                $('#prov_id').append(
                    $('<option>', {
                        value: proveedor.id,
                        text: proveedor.nombre
                    })
                );
            });

            // Seleccionar el nuevo proveedores si existe
            if (nuevoProveedorId) {
                // Esperar un momento para que Select2 se actualice
                setTimeout(function () {
                    $('#prov_id').val(nuevoProveedorId).trigger('change.select2');
                    console.log("✅ Nuevo Proveedores seleccionado:", nuevoProveedorId);
                }, 300);
            }

            // Cargar datos del proveedores si se seleccionó
            if (nuevoProveedorId && nuevoProveedorId != '0') {
                setTimeout(function () {
                    $('#cli_id').trigger('change');
                }, 500);
            }
        },
        error: function (xhr, status, error) {
            console.error("❌ Error al recargar proveedores:", error);
            Swal.fire({
                title: "Atención",
                text: "El proveedores se guardó, pero no se pudo actualizar la lista. Por favor refresque la página.",
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