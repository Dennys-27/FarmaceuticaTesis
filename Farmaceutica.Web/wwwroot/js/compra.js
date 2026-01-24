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