var tablaUsuarios;

$(document).ready(function () {

    // Inicializar DataTable
    tablaUsuarios = $("#table_data").DataTable({
        aProcessing: true,
        aServerSide: true,
        dom: "Bfrtip",
        buttons: ["copyHtml5", "excelHtml5", "csvHtml5"],
        ajax: {
            url: "/Delivery/ListarVentasParaAsignarDelivery",
            type: "POST",
            data: function (d) {
                // 🔹 Enviar filtros de fecha al controller
                d.fechaInicio = $("#fechaInicio").val();
                d.fechaFin = $("#fechaFin").val();
            },
            error: function (xhr, error, thrown) {
                console.error("Error al cargar datos:", error);
                alert("Error al cargar los datos. Por favor, intente nuevamente.");
            }
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
                sPrevious: "Anterior"
            }
        }
    });

    // 🔹 Evento para aplicar filtro por fechas
    $("#btnFiltrar").on("click", function () {
        tablaUsuarios.ajax.reload();
    });

    // 🔹 Evento para limpiar filtros
    $("#btnLimpiar").on("click", function () {
        $("#fechaInicio").val("");
        $("#fechaFin").val("");
        tablaUsuarios.ajax.reload();
    });
});

function verDetalle(id) {
    console.log("📌 verDetalle() llamado con ID:", id);

    // Mostrar loading en productos
    $("#detalle_body").html(`
        <tr id="loadingRow">
            <td colspan="5" class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Cargando...</span>
                </div>
                <p class="mt-2 text-muted">Cargando productos...</p>
            </td>
        </tr>
    `);

    // Limpiar datos previos
    $("#d_cliente, #d_correo, #d_telefono, #d_direccion, #d_ruc").text("-");
    $("#d_subtotal, #d_igv, #d_total").text("0.00");
    $("#cantidadProductos").text("0");

    // =============================
    // CABECERA (FACTURA)
    // =============================
    $.post("/Delivery/MostrarVentaFactura", { id: id }, function (facturaResp) {
        console.log("📦 RESPUESTA FACTURA:", facturaResp);

        if (!facturaResp.success) {
            showAlert("error", "Error", facturaResp.message || "Error al cargar la factura");
            return;
        }

        // Construir nombre completo
        let nombreCompleto = facturaResp.cliente || "";
        if (facturaResp.apellido) {
            nombreCompleto += " " + facturaResp.apellido;
        }

        // Actualizar UI
        $("#d_cliente").text(nombreCompleto.trim() || "-");
        $("#d_correo").html(facturaResp.correo ? `<a href="mailto:${facturaResp.correo}">${facturaResp.correo}</a>` : "-");
        $("#d_telefono").html(facturaResp.telefono ? `<a href="tel:${facturaResp.telefono}">${facturaResp.telefono}</a>` : "-");
        $("#d_direccion").text(facturaResp.direccion || "-");
        $("#d_ruc").text(facturaResp.ruc || "-");

        // Formatear y mostrar montos
        $("#d_subtotal").text(`S/ ${formatCurrency(facturaResp.subtotal)}`);
        $("#d_igv").text(`S/ ${formatCurrency(facturaResp.igv)}`);
        $("#d_total").text(`S/ ${formatCurrency(facturaResp.total)}`);

    }).fail(function (xhr, status, error) {
        console.error("❌ Error AJAX factura:", error);
        showAlert("error", "Error", "No se pudo cargar la información de la factura");
    });

    // =============================
    // DETALLE DE PRODUCTOS
    // =============================
    $.post("/Delivery/MostrarVentaDetalle", { id: id }, function (detalleResp) {
        console.log("📦 RESPUESTA DETALLE:", detalleResp);

        if (!detalleResp || !detalleResp.data || detalleResp.data.length === 0) {
            $("#detalle_body").html(`
                <tr>
                    <td colspan="5" class="text-center py-4">
                        <div class="text-muted">
                            <i class="bi bi-cart-x fs-1"></i>
                            <p class="mt-2">No hay productos en esta venta</p>
                        </div>
                    </td>
                </tr>
            `);
            return;
        }

        console.log("🛒 Número de productos:", detalleResp.data.length);
        $("#cantidadProductos").text(detalleResp.data.length);

        let html = "";
        let subtotalCalculado = 0;

        detalleResp.data.forEach(function (row, index) {
            if (row && row.length >= 5) {
                const cantidad = parseFloat(row[2]) || 0;
                const precio = parseFloat(row[3]) || 0;
                const total = parseFloat(row[4]) || 0;
                subtotalCalculado += total;

                html += `
                <tr class="product-row">
                    <td class="text-center">
                        ${row[0] || '<div class="avatar-xs mx-auto rounded-circle bg-light d-flex align-items-center justify-content-center"><i class="bi bi-box text-muted"></i></div>'}
                    </td>
                    <td class="ps-3 fw-semibold">
                        ${row[1] || 'Producto no especificado'}
                    </td>
                    <td class="text-center">
                        <span class="badge bg-primary rounded-pill">${cantidad}</span>
                    </td>
                    <td class="text-end fw-semibold text-success">
                        S/ ${formatCurrency(precio)}
                    </td>
                    <td class="text-end fw-bold">
                        <span class="text-primary">S/ ${formatCurrency(total)}</span>
                    </td>
                </tr>`;
            }
        });

        // Agregar fila de total si hay productos
        if (detalleResp.data.length > 0) {
            html += `
            <tr class="table-active">
                <td colspan="4" class="text-end fw-bold">Total Productos:</td>
                <td class="text-end fw-bold fs-5 text-success">
                    S/ ${formatCurrency(subtotalCalculado)}
                </td>
            </tr>`;
        }

        $("#detalle_body").html(html);

    }).fail(function (xhr, status, error) {
        console.error("❌ Error AJAX detalle:", error);
        $("#detalle_body").html(`
            <tr>
                <td colspan="5" class="text-center py-4 text-danger">
                    <i class="bi bi-exclamation-triangle fs-1"></i>
                    <p class="mt-2">Error al cargar los productos</p>
                    <small class="text-muted">${error}</small>
                </td>
            </tr>
        `);
    });

    // Mostrar modal con animación
    const modal = new bootstrap.Modal(document.getElementById('modalDetalleVenta'));
    modal.show();
}

// Función para mostrar alertas
function showAlert(type, title, message) {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <strong>${title}:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    // Puedes agregar esto en un contenedor específico
    $("#alertContainer").html(alertHtml);

    // O usar toast si prefieres
    showToast(type, title, message);
}

// Función para toast
function showToast(type, title, message) {
    const toastHtml = `
        <div class="toast align-items-center text-bg-${type} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <strong>${title}:</strong> ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    $("#toastContainer").html(toastHtml);
    const toast = new bootstrap.Toast($("#toastContainer .toast")[0]);
    toast.show();
}

// Función para imprimir
function imprimirDetalle() {
    window.print();
}

// Función para exportar PDF (mockup)
function exportarPDF() {
    showAlert("info", "Exportar", "La funcionalidad de exportar PDF estará disponible pronto");
}

// Función auxiliar para formatear moneda
function formatCurrency(value) {
    if (value === null || value === undefined || value === "") {
        return "0.00";
    }

    try {
        var num = parseFloat(value);
        if (isNaN(num)) {
            return "0.00";
        }
        return num.toFixed(2);
    } catch (e) {
        console.error("Error formateando moneda:", e, "Valor:", value);
        return "0.00";
    }
}

// Función para formatear número con separadores de miles
function formatNumberWithCommas(value) {
    if (value === null || value === undefined || value === "") {
        return "0.00";
    }

    try {
        var num = parseFloat(value);
        if (isNaN(num)) {
            return "0.00";
        }
        return num.toLocaleString('es-PE', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    } catch (e) {
        console.error("Error formateando número:", e);
        return "0.00";
    }
}

function init() {
    // reservado si usas inicialización adicional
    console.log("Sistema de delivery inicializado");
}

function procesar(ventaId) {

    // Setear título
    $("#lbltitulo").text("Asignar Delivery");

    // Setear id de la venta
    $("#venta_id").val(ventaId);

    // Limpiar formulario
    $("#repartidor_id").val("");
    $("#fecha_asignacion").val("");
    $("#comentario").val("");

    // Cargar repartidores (si aplica)
    cargarRepartidores();

    // Mostrar modal
    $("#modalmantenimiento").modal("show");
}


function cargarRepartidores() {

    $.get("/Delivery/ListarRepartidores", function (data) {

        let html = "<option value=''>Seleccionar repartidor</option>";

        data.forEach(r => {
            html += `<option value="${r.id}">${r.nombre}</option>`;
        });

        $("#repartidor_id").html(html);
    });
}



$("#mantenimiento_form").on("submit", function (e) {
    e.preventDefault();

    let data = {
        ventaId: $("#venta_id").val(),
        repartidorId: $("#repartidor_id").val(),
        fechaAsignacion: $("#fecha_asignacion").val(),
        comentario: $("#comentario").val(),
    };

    if (!data.repartidorId) {
        alert("Seleccione un repartidor");
        return;
    }

    $.post("/Delivery/ProcesarDelivery", data, function (resp) {

        if (resp.success) {
            Swal.fire("Éxito", resp.message, "success");

            $("#modalmantenimiento").modal("hide");
            tablaUsuarios.ajax.reload(null, false);

        } else {
            Swal.fire("Error", resp.message, "error");
        }
    });
});
