document.addEventListener('DOMContentLoaded', function () {
    // 1. Funcionalidad para mostrar/ocultar contraseña
    const passwordInput = document.getElementById('password-input');
    const passwordAddon = document.getElementById('password-addon');
    const passwordIcon = passwordAddon.querySelector('i');

    passwordAddon.addEventListener('click', function () {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        // Cambiar ícono
        if (type === 'text') {
            passwordIcon.className = 'ri-eye-off-fill align-middle';
        } else {
            passwordIcon.className = 'ri-eye-fill align-middle';
        }
    });

    // 2. Elementos de validación
    const uppercaseCheck = document.getElementById('uppercase-check');
    const numberCheck = document.getElementById('number-check');
    const specialCheck = document.getElementById('special-check');
    const submitBtn = document.getElementById('submitBtn');
    const registerForm = document.getElementById('registerForm');

    // 3. Función para validar contraseña
    function validatePassword(password) {
        const hasUppercase = /[A-Z]/.test(password);
        const hasNumber = /\d/.test(password);
        const hasSpecial = /[@#$%&*]/.test(password);

        return {
            hasUppercase: hasUppercase,
            hasNumber: hasNumber,
            hasSpecial: hasSpecial,
            isValid: hasUppercase && hasNumber && hasSpecial
        };
    }

    // 4. Función para actualizar íconos de validación
    function updateCheckIcon(element, isValid) {
        const icon = element.querySelector('i');
        if (isValid) {
            icon.className = 'ri-checkbox-circle-fill align-middle text-success';
            element.classList.remove('text-muted');
            element.classList.add('text-success');
        } else {
            icon.className = 'ri-checkbox-blank-circle-line align-middle';
            element.classList.remove('text-success');
            element.classList.add('text-muted');
        }
    }

    // 5. Función para actualizar toda la validación
    function updateValidation(password) {
        const validation = validatePassword(password);

        updateCheckIcon(uppercaseCheck, validation.hasUppercase);
        updateCheckIcon(numberCheck, validation.hasNumber);
        updateCheckIcon(specialCheck, validation.hasSpecial);

        // Cambiar estado del botón
        submitBtn.disabled = !validation.isValid;
        if (validation.isValid) {
            submitBtn.classList.remove('btn-secondary');
            submitBtn.classList.add('btn-success');
        } else {
            submitBtn.classList.remove('btn-success');
            submitBtn.classList.add('btn-secondary');
        }
    }

    // 6. Validar en tiempo real
    passwordInput.addEventListener('input', function () {
        updateValidation(this.value);
    });

    // 7. Validar al enviar el formulario
    registerForm.addEventListener('submit', function (event) {
        const password = passwordInput.value;
        const validation = validatePassword(password);

        if (!validation.isValid) {
            event.preventDefault();
            alert('La contraseña debe contener al menos:\n• Una letra mayúscula\n• Un número\n• Un signo especial (@ # $ % & *)');
            passwordInput.focus();
        }
    });

    // 8. Validación inicial
    updateValidation(passwordInput.value);
});