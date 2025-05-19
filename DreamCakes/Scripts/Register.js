document.addEventListener('DOMContentLoaded', function () {
    const registerForm = document.getElementById('registerForm');
    let debounceTimer;

    // Elementos del formulario
    const fields = {
        nombres: document.getElementById('Nombres'),
        apellidos: document.getElementById('Apellidos'),
        telefono: document.getElementById('Telefono'),
        direccion: document.getElementById('Direccion'),
        email: document.getElementById('Email'),
        contrasena: document.getElementById('Contrasena')
    };

    // Elementos de error
    const errors = {
        nombres: document.getElementById('nombresError'),
        apellidos: document.getElementById('apellidosError'),
        telefono: document.getElementById('telefonoError'),
        direccion: document.getElementById('direccionError'),
        email: document.getElementById('emailError'),
        contrasena: document.getElementById('contrasenaError'),
        general: document.getElementById('generalError')
    };

    // Patrones de validación
    const patterns = {
        soloTexto: /^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]{2,}$/,
        telefono: /^(\+?\d{1,3}?[- .]?)?\(?\d{3}\)?[- .]?\d{3}[- .]?\d{4}$/,
        email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
        contrasena: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/
    };

    // Validación en tiempo real con debounce (500ms)
    Object.keys(fields).forEach(fieldName => {
        if (fields[fieldName]) {
            fields[fieldName].addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(() => {
                    validateField(fieldName, false);
                }, 500);
            });
        }
    });

    // Validación al enviar el formulario
    registerForm.addEventListener('submit', function (e) {
        e.preventDefault();
        let isValid = true;

        // Ocultar errores generales previos
        if (errors.general) errors.general.style.display = 'none';

        // Validar todos los campos
        Object.keys(fields).forEach(fieldName => {
            if (!validateField(fieldName, true)) {
                isValid = false;
            }
        });

        if (isValid) {
            this.submit();
        } else {
           

            // Desplazarse al primer error
            const firstError = document.querySelector('.text-danger[style="display: block;"]');
            if (firstError) {
                firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    });

    // Función para validar un campo específico
    function validateField(fieldName, showErrors = true) {
        const value = fields[fieldName] ? fields[fieldName].value.trim() : '';
        let isValid = true;
        let errorMessage = '';

        switch (fieldName) {
            case 'nombres':
                if (value.length < 5) {
                    errorMessage = 'El nombre debe tener al menos 5 caracteres.';
                    isValid = false;
                } else if (!patterns.soloTexto.test(value)) {
                    errorMessage = 'Solo se permiten letras y espacios.';
                    isValid = false;
                }
                break;

            case 'apellidos':
                if (value.length < 5) {
                    errorMessage = 'Los apellidos deben tener al menos 5 caracteres.';
                    isValid = false;
                } else if (!patterns.soloTexto.test(value)) {
                    errorMessage = 'Solo se permiten letras y espacios.';
                    isValid = false;
                }
                break;

            case 'telefono':
                if (value.length < 10) {
                    errorMessage = 'El Teléfono debe tener al menos 10 caracteres';
                    isValid = false;
                } else if (value && !patterns.telefono.test(value)) {
                    errorMessage = 'Ingrese un teléfono válido (ej: +51 123 456 789).';
                    isValid = false;
                }

                break;

            case 'direccion':
                if (value.length < 8) {
                    errorMessage = 'La dirección debe tener al menos 8 caracteres.';
                    isValid = false;
                }
                break;

            case 'email':
                if (!patterns.email.test(value)) {
                    errorMessage = 'Ingrese un correo electrónico válido.';
                    isValid = false;
                }
                break;

            case 'contrasena':
                if (!patterns.contrasena.test(value)) {
                    errorMessage = 'Mínimo 8 caracteres, 1 mayúscula, 1 minúscula y 1 número.';
                    isValid = false;
                }
                break;
        }

        // Mostrar u ocultar errores según parámetro
        if (errors[fieldName]) {
            if (!isValid && showErrors) {
                errors[fieldName].textContent = errorMessage;
                errors[fieldName].style.display = 'block';
            } else {
                errors[fieldName].style.display = 'none';
            }
        }

        return isValid;
    }
});