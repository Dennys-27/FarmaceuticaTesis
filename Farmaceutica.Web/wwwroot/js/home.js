
document.addEventListener("DOMContentLoaded", () => {
        cargarResumen();
});

    async function cargarResumen() {
    try {
        const response = await fetch("/Dashboard/Resumen", {
        method: "POST",
    headers: {
        "Content-Type": "application/json"
            }
        });

    if (!response.ok) {
            throw new Error("No se pudo obtener el resumen del dashboard");
        }

    const data = await response.json();

    animarNumero("totalVentas", data.totalVentas, 1200, true);
    animarNumero("porcentajeCambio", data.porcentajeCrecimiento, 900, true);

    aplicarEstiloCambio(data.porcentajeCrecimiento);

    } catch (error) {
        console.error("Error cargando resumen:", error);
    }
}

    /* =========================
       ANIMACIÓN DE CONTADOR
    ========================= */
    function animarNumero(id, valorFinal, duracion = 1000, decimales = true) {
    const elemento = document.getElementById(id);
    if (!elemento) return;

    const inicio = 0;
    const startTime = performance.now();

    function animacion(tiempoActual) {
        const progreso = Math.min((tiempoActual - startTime) / duracion, 1);
    const easing = easeOutCubic(progreso);
    const valor = inicio + (valorFinal - inicio) * easing;

    elemento.innerText = decimales
    ? valor.toFixed(2)
    : Math.round(valor);

    if (progreso < 1) {
        requestAnimationFrame(animacion);
        }
    }

    requestAnimationFrame(animacion);
}

    /* =========================
       EASING SUAVE
    ========================= */
    function easeOutCubic(t) {
    return 1 - Math.pow(1 - t, 3);
}

    /* =========================
       COLOR + ICONO DINÁMICO
    ========================= */
    function aplicarEstiloCambio(porcentaje) {
    const icono = document.getElementById("iconoCambio");
    const texto = document.getElementById("porcentajeCambio");

    if (!icono || !texto) return;

    if (porcentaje >= 0) {
        icono.className = "ri-arrow-right-up-line text-success";
    texto.classList.add("text-success");
    texto.classList.remove("text-danger");
    } else {
        icono.className = "ri-arrow-right-down-line text-danger";
    texto.classList.add("text-danger");
    texto.classList.remove("text-success");
    }
}
