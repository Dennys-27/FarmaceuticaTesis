function init() {
    $("#mantenimiento_form_producto").on("submit", function (e) {
        guardaryeditar(e);
    });
}



function guardaryeditar(e) {
    e.preventDefault();

    var formData = new FormData($("#mantenimiento_form_producto")[0]);

    $.ajax({
        url: "/Producto/Guardar",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                tablaProductos.ajax.reload(null, false); // <-- usar la instancia
                $("#modalmantenimiento_producto").modal("hide");
                swal.fire({
                    title: "Producto",
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
                text: "No se pudo guardar el Producto.",
                icon: "error",
            });
            console.error("Error AJAX:", error);
        }
    });
}


// Cargar categorías al iniciar
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
        },
        error: function () {
            console.error("Error al cargar las categorías.");
        }
    });
}

// Cargar subcategorías según la categoría seleccionada
function cargarSubCategorias(categoriaId) {
    if (!categoriaId) {
        $("#scat_id").html('<option value="">Seleccione una categoría primero</option>');
        return;
    }

    $.ajax({
        url: `/SubCategoria/ComboSubCategoria?categoria=${categoriaId}`,
        type: "GET",
        success: function (data) {
            let options = '<option value="">Seleccionar</option>';
            data.forEach(s => {
                options += `<option value="${s.id}">${s.nombre}</option>`;
            });
            $("#scat_id").html(options);
        },
        error: function () {
            console.error("Error al cargar las subcategorías.");
        }
    });
}
// Cuando cambie la categoría, cargar las subcategorías
$("#cat_id").on("change", function () {
    const categoriaId = $(this).val();
    cargarSubCategorias(categoriaId);
});







$("#btnProcesar").on("click", function () {
    procesarArchivo();
});

function procesarArchivo() {
    $.ajax({
        url: '/Producto/Procesar',
        type: 'POST',
        beforeSend: function () {
            swal.fire({
                title: "Procesando...",
                text: "Por favor espera mientras se procesa la información.",
                allowOutsideClick: false,
                showConfirmButton: false,
                didOpen: () => {
                    swal.showLoading();
                }
            });
        },
        success: function (response) {
            swal.close();

            if (response.ok) {
                swal.fire({
                    title: "¡Éxito!",
                    text: "Los productos se procesaron correctamente.",
                    icon: "success",
                    timer: 2000,
                    showConfirmButton: false
                });

                console.log(response.data); // Aquí puedes cargar la tabla con los datos procesados
            } else {
                swal.fire({
                    title: "Aviso",
                    text: response.mensaje || "No se encontraron productos para procesar.",
                    icon: "warning"
                });
            }
        },
        error: function () {
            swal.close();
            swal.fire({
                title: "Error",
                text: "Ocurrió un error al procesar los productos.",
                icon: "error"
            });
        }
    });
}







function cargarTabla() {
    // Hacer visible la tabla
    $('#table_dataDetalle').show();

    // Inicializar DataTable
    var table = $('#table_dataDetalle').DataTable({
        destroy: true,  // reinicia la tabla si ya estaba inicializada
        responsive: true,
        processing: true,
        serverSide: true,
        ajax: {
            url: "/Producto/ListarDatosProcesar",
            type: "POST",
            dataType: "json",
            dataSrc: function (json) {
                
                if (json && json.data) {
                    
                    return json.data; // esto es lo que DataTable usará
                } else {
                   
                    return [];
                }
            }
        },
        columns: [
            { data: null, defaultContent: "" }, // columna vacía
            { data: 0 }, // Nombre
            { data: 1 }, // Descripcion
            { data: 2 }, // Precio
            { data: 3 }, // StockTotal
            { data: 4 }, // StockLocal
            { data: 5 }, // StockDelivery
            { data: 6 }, // Codigo
            { data: 7 }, // Categoria
            { data: 8 }, // SubCategoria
            { data: 9 }  // EstadoProducto
        ],
        order: [[1, 'asc']], // ordenar por nombre
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
        drawCallback: function () {
            $('#btnCargar').prop('disabled', true).addClass('disabled');
        }
    });
}







var tablaProductos; // <-- instancia global

$(document).ready(function () {
    cargarCategorias();

    tablaProductos = $("#table_data").DataTable({
        aProcessing: true,
        aServerSide: true,
        dom: "Bfrtip",
        buttons: ["copyHtml5", "excelHtml5", "csvHtml5"],
        ajax: {
            url: "/Producto/Listar",
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






//Abrir modal para procesar registros
$(document).on("click", "#btnCargar", function () {
    console.log("entro a funcion para abrir modal");
    if ($("#modalmantenimiento").length) {
        $("#modalmantenimiento").modal("show");
        console.log("✅ Modal mostrado correctamente");
    } else {
        console.error("❌ No se encontró el modal con id #modalmantenimiento");
        alert("Error: No se encontró el modal.");
    }
});

// =======================================
// Paso 1: Subir archivo
// =======================================
$("#btnLeerArchivo").on("click", function () {
    let archivo = $("#archivoExcel")[0].files[0];

    if (!archivo) {
        alert("Por favor seleccione un archivo Excel.");
        return;
    }

    let formData = new FormData();
    formData.append("archivoExcel", archivo);

    $.ajax({
        url: '/ArchivoProducto/UploadExcel', // Endpoint correcto
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.ok) {
                // 🔹 Guardamos el NOMBRE ÚNICO que genera el backend
                $("#archivoExcel").data("nombreUnico", response.nombreArchivo);

                // Ocultar Step1 y mostrar Step2
                $("#step1").addClass("d-none");
                $("#step2").removeClass("d-none");
            } else {
                alert("Error: " + response.mensaje);
            }
        },
        error: function () {
            alert("Error al subir el archivo.");
        }
    });
});

// =======================================
// Paso 2: Procesar archivo
// =======================================
$("#btnFinalizar").on("click", function () {
    // 🔹 Usamos el nombre único devuelto por el backend
    let archivo = $("#archivoExcel").data("nombreUnico");
    let categoriaId = $("#cat_id").val();
    let subCategoriaId = $("#scat_id").val();

    if (!archivo) {
        alert("No se encontró el archivo para procesar.");
        return;
    }

    if (!categoriaId || !subCategoriaId) {
        alert("Por favor seleccione categoría y subcategoría antes de continuar.");
        return;
    }

    $.ajax({
        url: '/ArchivoProducto/ProcesarExcel',
        type: 'POST',
        data: {
            nombreArchivo: archivo,
            categoriaId: categoriaId,
            subCategoriaId: subCategoriaId
        },
        success: function (response) {
            if (response.ok) {
                // Ocultar Step2 y mostrar Step3
                $("#step2").addClass("d-none");
                $("#step3").removeClass("d-none");

                // Mostrar total de registros insertados
                $("#step3 p").text(`Se insertaron ${response.total} registros en la tabla temporal.`);
            } else {
                alert("Error: " + response.mensaje);
            }

            $("#contenedorTablaDetalle").show();   // muestra la tabla
            cargarTabla();                          // función que carga DataTable
        },
        error: function () {
            alert("Error al procesar el archivo.");
        }
    });
});

function editar(prod_id) {
    $.ajax({
        url: "/Producto/Mostrar",
        type: "POST",
        data: { id: prod_id },
        success: function (response) {
            if (response.success) {
                console.log(response);
                $("#prod_id").val(response.id);
                $("#pre_imagen").html(
                    '<img src="/images/users/' + response.imagen + '" class="rounded-circle avatar-xl img-thumbnail user-profile-image">'
                );
                $("#lbltitulo").html("Editar Registro");
                $("#modalmantenimiento_producto").modal("show");
            } else {
                Swal.fire("Error", response.message, "error");
            }
        },
        error: function () {
            Swal.fire("Error", "No se pudo obtener el Producto", "error");
        }
    });
}

function eliminar(prod_id) {
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
                url: "/Producto/Eliminar",
                type: "POST",
                data: { id: prod_id },
                success: function (response) {
                    if (response.success) {
                        $("#table_data").DataTable().ajax.reload();
                        Swal.fire("Producto", response.message, "success");
                    } else {
                        Swal.fire("Error", response.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "No se pudo eliminar el Producto", "error");
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

$(document).on("change", "#prod_img", function () {
    filePreview(this);
});

$(document).on("click", "#btnremovephoto", function () {
    $("#prod_img").val("");
    $("#pre_imagen").html(
        '<img src="../images/users/producto.png" class="rounded-circle avatar-xl img-thumbnail user-profile-image" alt="user-profile-image"></img><input type="hidden" name="hidden_usuario_imagen" value="" />'
    );
});


$('#btnDescargar').click(function (e) {
    e.preventDefault();

    Swal.fire({
        title: 'Descargando Formato',
        html: `
            <div class="text-center py-4">
                <div class="download-animation">
                    <i class="ri-download-cloud-2-line" style="font-size: 60px; color: #0d6efd;"></i>
                    <div class="pulse-ring"></div>
                </div>
                <p class="mt-3 mb-0">Tu archivo se está preparando</p>
                <p class="text-muted small">Formato_Importacion_Productos.xlsx</p>
            </div>
        `,
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            setTimeout(() => {
                // Descargar archivo
                const link = document.createElement('a');
                link.href = $(this).attr('href');
                link.download = $(this).attr('download');
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);

                // Mostrar éxito con confetti
                Swal.fire({
                    icon: 'success',
                    title: '¡Listo! 🎉',
                    text: 'Formato descargado correctamente',
                    showConfirmButton: true,
                    confirmButtonText: 'Aceptar',
                    confirmButtonColor: '#0d6efd',
                    backdrop: true
                });
            }, 2000);
        }
    });
});

// Agrega este CSS para la animación
const style = document.createElement('style');
style.textContent = `
    .download-animation {
        position: relative;
        display: inline-block;
    }
    .pulse-ring {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        border: 2px solid #0d6efd;
        border-radius: 50%;
        animation: pulse 1.5s infinite;
    }
    @keyframes pulse {
        0% { transform: scale(0.8); opacity: 1; }
        100% { transform: scale(1.5); opacity: 0; }
    }
`;
document.head.appendChild(style);

init();



