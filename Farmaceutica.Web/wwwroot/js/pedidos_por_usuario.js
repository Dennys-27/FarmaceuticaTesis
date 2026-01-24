var tablaVentas; // Variable global para la tabla

// Esperar a que jQuery esté disponible
function initDataTable() {
    if (typeof jQuery === 'undefined') {
        console.warn('jQuery no está cargado, reintentando en 100ms...');
        setTimeout(initDataTable, 100);
        return;
    }

    $(document).ready(function () {
        // Inicializar DataTable VACÍO (sin ajax inicial)
        tablaVentas = $('#table_data').DataTable({
            processing: false,
            serverSide: true,
            responsive: true,
            autoWidth: false,
            searching: true,
            ordering: true,
            info: true,
            paging: true,
            ajax: {
                url: "/Pedido/ListarVentasParaAsignarUsuarioCorreo",
                type: "POST",
                data: function (d) {
                    return {
                        fechaInicio: $('#fechaInicio').val(),
                        fechaFin: $('#fechaFin').val(),
                        correo: $('#correo').val(),
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        search: d.search ? d.search.value : ''
                    };
                },
                dataSrc: function (json) {
                    // Si no hay datos o hubo error, mostrar mensaje
                    if (!json || !json.data || json.data.length === 0) {
                        return [];
                    }
                    return json.data;
                },
                error: function (xhr, error, thrown) {
                    console.error("Error al cargar datos:", xhr.responseText);
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudieron cargar los datos. Intente nuevamente.'
                        });
                    }
                    // CORRECCIÓN: Acceder directamente al tbody
                    $('#table_data tbody').html(
                        '<tr><td colspan="14" class="text-center text-muted py-4">Error al cargar los datos</td></tr>'
                    );
                }
            },
            columns: [
                {
                    data: null,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                    orderable: false,
                    className: 'text-center'
                },
                {
                    data: 1,
                    render: function (data, type, row) {
                        var nombre = data || '';
                        var apellido = row[2] || '';
                        return `<div class="fw-medium">${nombre} ${apellido}</div>`;
                    }
                },
                {
                    data: 3,
                    className: 'text-center',
                    render: function (data) {
                        var badgeClass = 'badge ';
                        if (data === 'EFECTIVO') badgeClass += 'bg-success';
                        else if (data === 'Tarjeta') badgeClass += 'bg-info';
                        else if (data === 'TRANSFERENCIA') badgeClass += 'bg-primary';
                        else badgeClass += 'bg-secondary';
                        return `<span class="${badgeClass}">${data}</span>`;
                    }
                },
                { data: 4 },
                { data: 5 },
                { data: 6, className: 'text-center' },
                { data: 7, className: 'text-center' },
                {
                    data: 8,
                    className: 'text-end',
                    render: function (data) {
                        return data ? `S/ ${formatCurrency(data)}` : 'S/ 0.00';
                    }
                },
                {
                    data: 9,
                    className: 'text-end',
                    render: function (data) {
                        return data ? `S/ ${formatCurrency(data)}` : 'S/ 0.00';
                    }
                },
                {
                    data: 10,
                    className: 'text-end fw-bold text-primary',
                    render: function (data) {
                        return data ? `S/ ${formatCurrency(data)}` : 'S/ 0.00';
                    }
                },
                {
                    data: 11,
                    render: function (data, type, row) {
                        var nombre = data || '';
                        var apellido = row[12] || '';
                        return nombre || apellido ?
                            `<div><span class="fw-medium">${nombre} ${apellido}</span></div>` :
                            '<span class="text-muted">Sin asignar</span>';
                    }
                },
                {
                    data: 13,
                    className: 'text-center',
                    render: function (data) {
                        if (!data) return '';
                        var date = new Date(data);
                        return date.toLocaleDateString('es-PE', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });
                    }
                },
                {
                    data: 16,
                    className: 'text-center',
                    render: function (data) {
                        return data || '<span class="badge bg-secondary">Sin estado</span>';
                    }
                },
                {
                    data: 17,
                    orderable: false,
                    className: 'text-center',
                    render: function (data, type, row, meta) {
                        var idVenta = row[0] || (meta.settings.json && meta.settings.json.data && meta.settings.json.data[meta.row] ? meta.settings.json.data[meta.row][0] : '');
                        return data ||
                            `<div class="btn-group btn-group-sm">
                                    <button class="btn btn-sm btn-outline-info btn-detalle" data-id="${idVenta}" title="Ver detalle">
                                        <i class="ri-eye-line"></i>
                                    </button>
                                    <button class="btn btn-sm btn-outline-warning btn-procesar" data-id="${idVenta}" title="Asignar">
                                        <i class="ri-user-add-line"></i>
                                    </button>
                                </div>`;
                    }
                }
            ],
            order: [[12, "desc"]],
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            language: {
                processing: "<div class='spinner-border spinner-border-sm text-primary' role='status'></div> Procesando...",
                lengthMenu: "Mostrar _MENU_ registros",
                zeroRecords: "No se encontraron resultados. Aplique filtros para ver datos.",
                emptyTable: "No hay datos disponibles. Aplique filtros para ver resultados.",
                info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                infoEmpty: "Mostrando 0 de 0 registros",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                search: "Buscar:",
                searchPlaceholder: "Buscar en los resultados...",
                paginate: {
                    first: "Primero",
                    last: "Último",
                    next: '<i class="ri-arrow-right-s-line"></i>',
                    previous: '<i class="ri-arrow-left-s-line"></i>'
                }
            },
            initComplete: function (settings, json) {
                // CORRECCIÓN: Acceder directamente al tbody
                $('#table_data tbody').html(
                    '<tr><td colspan="14" class="text-center text-muted py-5">' +
                    '<i class="ri-filter-line fs-1 mb-3 d-block"></i>' +
                    '<h5 class="text-muted mb-2">Aplicar filtros para ver resultados</h5>' +
                    '<p class="text-muted small">Seleccione fechas y/o correo para comenzar</p>' +
                    '</td></tr>'
                );
            }
        });

        // Evento Filtrar con validación
        $('#btnFiltrar').on('click', function () {
            if (!validarFiltros()) {
                return;
            }

            // Mostrar loading - CORRECCIÓN: Acceder directamente al tbody
            $('#table_data tbody').html(
                '<tr><td colspan="14" class="text-center py-5">' +
                '<div class="spinner-border text-primary" role="status">' +
                '<span class="visually-hidden">Cargando...</span>' +
                '</div>' +
                '<p class="mt-2 text-muted">Buscando resultados...</p>' +
                '</td></tr>'
            );

            // Recargar datos
            tablaVentas.ajax.reload();
        });

        // Evento Limpiar
        $('#btnLimpiar').on('click', function () {
            $('#fechaInicio').val('');
            $('#fechaFin').val('');
            $('#correo').val('');

            // Limpiar tabla y mostrar mensaje inicial - CORRECCIÓN
            tablaVentas.clear().draw();
            $('#table_data tbody').html(
                '<tr><td colspan="14" class="text-center text-muted py-5">' +
                '<i class="ri-filter-line fs-1 mb-3 d-block"></i>' +
                '<h5 class="text-muted mb-2">Aplicar filtros para ver resultados</h5>' +
                '<p class="text-muted small">Seleccione fechas y/o correo para comenzar</p>' +
                '</td></tr>'
            );
        });

        // Permitir Enter en los filtros
        $('#fechaInicio, #fechaFin, #correo').on('keypress', function (e) {
            if (e.which === 13) {
                $('#btnFiltrar').click();
            }
        });

        // Delegación de eventos para botones dinámicos
        $(document).on('click', '.btn-detalle', function () {
            var id = $(this).data('id');
            if (id) {
                verDetalle(id);
            }
        });

        $(document).on('click', '.btn-procesar', function () {
            var id = $(this).data('id');
            if (id) {
                procesar(id);
            }
        });
    });
}

// Función para validar filtros
function validarFiltros() {
    var fechaInicio = $('#fechaInicio').val();
    var fechaFin = $('#fechaFin').val();
    var correo = $('#correo').val();

    // Validar que al menos haya un criterio de búsqueda
    if (!fechaInicio && !fechaFin && !correo) {
        showAlert("warning", "Filtros requeridos",
            "Debe seleccionar al menos un criterio de búsqueda:\n" +
            "• Fecha de inicio\n" +
            "• Fecha de fin\n" +
            "• Correo del cliente");
        return false;
    }

    // Validar que si hay fecha inicio, también haya fecha fin (y viceversa)
    if ((fechaInicio && !fechaFin) || (!fechaInicio && fechaFin)) {
        showAlert("warning", "Fechas incompletas",
            "Debe seleccionar ambas fechas (inicio y fin) o ninguna.");
        return false;
    }

    // Validar que fecha inicio no sea mayor a fecha fin
    if (fechaInicio && fechaFin) {
        var inicio = new Date(fechaInicio);
        var fin = new Date(fechaFin);

        if (inicio > fin) {
            showAlert("warning", "Rango inválido",
                "La fecha de inicio no puede ser mayor a la fecha de fin.");
            return false;
        }
    }

    // Validar formato de correo si se proporciona
    if (correo && correo.trim() !== '') {
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(correo)) {
            showAlert("warning", "Correo inválido",
                "Por favor ingrese un correo electrónico válido.");
            return false;
        }
    }

    return true;
}

// Función para ver detalle
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
    $.post("/Delivery/MostrarVentaFactura", { id: id })
        .done(function (facturaResp) {
            console.log("📦 RESPUESTA FACTURA:", facturaResp);

            if (!facturaResp || !facturaResp.success) {
                showAlert("error", "Error", facturaResp?.message || "Error al cargar la factura");
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
        })
        .fail(function (xhr, status, error) {
            console.error("❌ Error AJAX factura:", error);
            showAlert("error", "Error", "No se pudo cargar la información de la factura");
        });

    // =============================
    // DETALLE DE PRODUCTOS
    // =============================
    $.post("/Delivery/MostrarVentaDetalle", { id: id })
        .done(function (detalleResp) {
            console.log("📦 RESPUESTA DETALLE:", detalleResp);

            if (!detalleResp || !detalleResp.data || detalleResp.data.length === 0) {
                $("#detalle_body").html(`
                        <tr>
                            <td colspan="5" class="text-center py-4">
                                <div class="text-muted">
                                    <i class="ri-shopping-cart-2-line fs-1"></i>
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
                                    ${row[0] || '<div class="avatar-xs mx-auto rounded-circle bg-light d-flex align-items-center justify-content-center"><i class="ri-box-3-line text-muted"></i></div>'}
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
        })
        .fail(function (xhr, status, error) {
            console.error("❌ Error AJAX detalle:", error);
            $("#detalle_body").html(`
                    <tr>
                        <td colspan="5" class="text-center py-4 text-danger">
                            <i class="ri-error-warning-line fs-1"></i>
                            <p class="mt-2">Error al cargar los productos</p>
                            <small class="text-muted">${error}</small>
                        </td>
                    </tr>
                `);
        });

    // Mostrar modal con animación
    const modalElement = document.getElementById('modalDetalleVenta');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } else {
        console.error("Modal #modalDetalleVenta no encontrado");
    }
}

// Función para procesar/editar
function procesar(ventaId) {
    console.log("Procesar venta ID:", ventaId);

    // Setear título
    $("#lbltitulo").text("Asignar Delivery");
    // Setear id de la venta
    $("#venta_id").val(ventaId);

    // Mostrar modal
    const modalElement = document.getElementById('modalmantenimiento');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    } else {
        console.error("Modal #modalmantenimiento no encontrado");
        showAlert("error", "Error", "No se pudo abrir el modal de edición");
    }
}

// Función para mostrar alertas
function showAlert(type, title, message) {
    // Si SweetAlert2 está disponible, usarlo
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: type,
            title: title,
            text: message,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
        });
    } else {
        // Alternativa con Bootstrap
        const alertHtml = `
                <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                    <strong>${title}:</strong> ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </div>
            `;

        // Crear contenedor si no existe
        if (!$('#alertContainer').length) {
            $('body').append('<div id="alertContainer" style="position: fixed; top: 20px; right: 20px; z-index: 9999;"></div>');
        }

        $('#alertContainer').html(alertHtml);

        // Auto-eliminar después de 5 segundos
        setTimeout(() => {
            $('.alert').alert('close');
        }, 5000);
    }
}

// Función para formatear moneda
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

// Inicialización
function init() {
    console.log("Sistema de pedidos inicializado");

    // Manejar submit del formulario de mantenimiento
    $("#mantenimiento_form").on("submit", function (e) {
        e.preventDefault();

        // Capturamos datos del formulario
        let data = {
            ventaId: $("#venta_id").val(),
            estadoDelivery: $("#select_estado").val()
        };

        // Validación básica
        if (!data.estadoDelivery) {
            showAlert("warning", "Advertencia", "Seleccione un estado de delivery");
            return;
        }

        // Mostrar loading
        const submitBtn = $(this).find('button[type="submit"]');
        const originalText = submitBtn.html();
        submitBtn.html('<span class="spinner-border spinner-border-sm me-2"></span> Procesando...');
        submitBtn.prop('disabled', true);

        // Enviar datos al servidor
        $.post("/Delivery/ProcesarDeliveryPorId", data)
            .done(function (resp) {
                if (resp.success) {
                    showAlert("success", "Éxito", resp.message);

                    // Cerrar modal
                    const modal = bootstrap.Modal.getInstance(document.getElementById('modalmantenimiento'));
                    if (modal) modal.hide();

                    // Recargar tabla si hay filtros aplicados
                    if (typeof tablaVentas !== 'undefined' && validarFiltros()) {
                        tablaVentas.ajax.reload(null, false);
                    }
                } else {
                    showAlert("error", "Error", resp.message);
                }
            })
            .fail(function (xhr, status, error) {
                console.error("Error en la solicitud:", error);
                showAlert("error", "Error", "No se pudo procesar la solicitud");
            })
            .always(function () {
                // Restaurar botón
                submitBtn.html(originalText);
                submitBtn.prop('disabled', false);
            });
    });

    // Limpiar formulario cuando se cierra el modal
    $('#modalmantenimiento').on('hidden.bs.modal', function () {
        $("#mantenimiento_form")[0].reset();
        $("#venta_id").val('');
    });
}

// Iniciar cuando el DOM esté listo
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function () {
        initDataTable();
        init();
    });
} else {
    initDataTable();
    init();
}