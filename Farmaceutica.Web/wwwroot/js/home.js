$(document).ready(function () {
    listar();
    listarVentas();
    listarProductos();
});

function listar() {
    $('#table_dataPedidos').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: "/Dashboard/ListarPedidos",
            type: "POST"
        },
        destroy: true,
        responsive: true,
        pageLength: 10,
        order: [[1, "desc"]],
        dom: 'Bfrtip', // 👈 Agregado para los botones
        buttons: [ // 👈 Configuración de botones
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i> Excel',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success btn-sm',
                title: 'Pedidos_' + new Date().toISOString().slice(0, 10),
                exportOptions: {
                    columns: ':visible'
                }
            }
        ],
        initComplete: function () {
            $('#table_dataPedidos').show();
        },
        language: {
            sProcessing: "Procesando...",
            sZeroRecords: "No se encontraron resultados",
            sLengthMenu: "Mostrar _MENU_ registros",
            sSearch: "Buscar:",
            sInfo: "Mostrando _START_ a _END_ de _TOTAL_ registros",
            oPaginate: {
                sFirst: "Primero",
                sLast: "Último",
                sNext: "Siguiente",
                sPrevious: "Anterior"
            }
        }
    });
}

function listarVentas() {
    $('#table_dataVentas').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: "/Dashboard/ListarVentas",
            type: "POST"
        },
        destroy: true,
        responsive: true,
        pageLength: 10,
        order: [[1, "desc"]],
        dom: 'Bfrtip', // 👈 Agregado para los botones
        buttons: [
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i> Excel',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success btn-sm',
                title: 'Ventas_' + new Date().toISOString().slice(0, 10),
                exportOptions: {
                    columns: ':visible'
                }
            }
        ],
        initComplete: function () {
            $('#table_dataVentas').show();
        },
        language: {
            sProcessing: "Procesando...",
            sZeroRecords: "No se encontraron resultados",
            sLengthMenu: "Mostrar _MENU_ registros",
            sSearch: "Buscar:",
            sInfo: "Mostrando _START_ a _END_ de _TOTAL_ registros",
            oPaginate: {
                sFirst: "Primero",
                sLast: "Último",
                sNext: "Siguiente",
                sPrevious: "Anterior"
            }
        }
    });
}

function listarProductos() {
    $('#table_dataProductos').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: "/Dashboard/ListarProducto",
            type: "POST"
        },
        destroy: true,
        responsive: true,
        pageLength: 10,
        order: [[1, "desc"]],
        dom: 'Bfrtip', // 👈 Agregado para los botones
        buttons: [
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i> Excel',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success btn-sm',
                title: 'Productos_' + new Date().toISOString().slice(0, 10),
                exportOptions: {
                    columns: ':visible'
                }
            }
        ],
        initComplete: function () {
            $('#table_dataProductos').show();
        },
        language: {
            sProcessing: "Procesando...",
            sZeroRecords: "No se encontraron resultados",
            sLengthMenu: "Mostrar _MENU_ registros",
            sSearch: "Buscar:",
            sInfo: "Mostrando _START_ a _END_ de _TOTAL_ registros",
            oPaginate: {
                sFirst: "Primero",
                sLast: "Último",
                sNext: "Siguiente",
                sPrevious: "Anterior"
            }
        }
    });
}