document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    let debounceTimer;

    if (loginForm) {
        const emailField = document.getElementById('LoginEmail');
        const passwordField = document.getElementById('LoginPassword');
        const emailError = document.getElementById('loginEmailError');
        const passwordError = document.getElementById('loginPasswordError');

        // Función para validar email con debounce
        emailField.addEventListener('input', function () {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                validateEmail(false); // Validación sin mostrar errores
            }, 500); // Espera 500ms después de dejar de escribir
        });

        // Función para validar contraseña con debounce
        passwordField.addEventListener('input', function () {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => {
                validatePassword(false); // Validación sin mostrar errores
            }, 500);
        });

        // Función para validar email
        function validateEmail(showErrors = true) {
            const email = emailField.value.trim();
            if (email === '') {
                if (showErrors) {
                    emailError.textContent = 'El correo electrónico es requerido';
                    emailError.style.display = 'block';
                }
                return false;
            } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
                if (showErrors) {
                    emailError.textContent = 'Ingrese un correo electrónico válido';
                    emailError.style.display = 'block';
                }
                return false;
            }
            emailError.style.display = 'none';
            return true;
        }

        // Función para validar contraseña
        function validatePassword(showErrors = true) {
            const password = passwordField.value.trim();
            if (password === '') {
                if (showErrors) {
                    passwordError.textContent = 'La contraseña es requerida';
                    passwordError.style.display = 'block';
                }
                return false;
            } else if (password.length < 6) {
                if (showErrors) {
                    passwordError.textContent = 'La contraseña debe tener al menos 6 caracteres';
                    passwordError.style.display = 'block';
                }
                return false;
            }
            passwordError.style.display = 'none';
            return true;
        }

        // Evento de envío del formulario
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // Validar ambos campos (mostrando errores)
            const isEmailValid = validateEmail(true);
            const isPasswordValid = validatePassword(true);

            if (isEmailValid && isPasswordValid) {
                this.submit();
            } else {
                // Desplazarse al primer error
                const firstError = document.querySelector('.text-danger[style="display: block;"]');
                if (firstError) {
                    firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }
            }
        });
    }
});