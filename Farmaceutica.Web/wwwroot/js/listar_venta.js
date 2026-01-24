$(document).ready(function () {
    var tablaSubCategorias = $("#table_data").DataTable({
        aProcessing: true,
        aServerSide: true,
        dom: "Bfrtip",
        buttons: ["copyHtml5", "excelHtml5", "csvHtml5"],
        ajax: {
            url: "/Venta/Listar",
            type: "POST",
            data: function (d) {
                return $.extend({}, d, {
                    numeroFactura: $("#filter_numeroFactura").val(),
                    encargado: $("#filter_encargado").val(),
                    cliente: $("#filter_cliente").val(),
                    metodoPago: $("#filter_metodoPago").val(),
                    fechaInicio: $("#filter_fechaInicio").val(),
                    fechaFin: $("#filter_fechaFin").val()
                });
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

    // Botón filtrar
    $("#btnFiltrar").click(function () {
        tablaSubCategorias.ajax.reload();
    });
});



function editar(numeroFactura) {
    $.post("/Venta/Mostrar", { numeroFactura: numeroFactura }, function (response) {
        if (!response.success) {
            Swal.fire("Error", response.message, "error");
            return;
        }

        // Datos generales
        const v = response.encabezado;
        $("#lblFactura").text(v.numeroFactura);
        $("#lblEncargado").text(v.encargado);
        $("#lblMetodoPago").text(v.metodoPago);
        $("#lblCliente").text(v.cliente);

        // Detalle en tabla DataTable
        if ($.fn.DataTable.isDataTable("#detalleVentaTable")) {
            $("#detalleVentaTable").DataTable().destroy();
        }

        $("#detalleVentaTable").DataTable({
            data: response.detalle,
            columns: [
                { data: "producto", title: "PRODUCTO" },
                { data: "codigoProducto", title: "CÓDIGO" },
                { data: "cantidad", title: "CANTIDAD" },
                {
                    data: "precioUnitaria",
                    title: "PRECIO UNITARIO",
                    render: function (data) {
                        return data ? `$${parseFloat(data).toFixed(2)}` : "$0.00";
                    }
                },
                {
                    data: "total",
                    title: "TOTAL",
                    render: function (data) {
                        return data ? `$${parseFloat(data).toFixed(2)}` : "$0.00";
                    }
                }
            ],
            paging: false,
            searching: false,
            info: false,
            responsive: true,
            language: {
                sEmptyTable: "No hay productos para esta venta",
            },
        });

        $("#modalmantenimiento").modal("show");
    }).fail(() => {
        Swal.fire("Error", "No se pudo obtener los datos de la venta.", "error");
    });
}

