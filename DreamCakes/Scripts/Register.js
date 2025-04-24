document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        let isValid = true;

        // Ocultar todos los mensajes de error previos
        document.querySelectorAll('.text-danger').forEach(el => {
            el.style.display = 'none';
        });
        document.getElementById('generalError').style.display = 'none';

        // Validar Nombres
        const nombres = document.getElementById('Nombres').value.trim();
        if (nombres.length < 2 || !/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(nombres)) {
            document.getElementById('nombresError').textContent = 'Debe contener solo letras y espacios, con mínimo 2 caracteres.';
            document.getElementById('nombresError').style.display = 'block';
            isValid = false;
        }

        // Validar Apellidos
        const apellidos = document.getElementById('Apellidos').value.trim();
        if (apellidos.length < 2 || !/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/.test(apellidos)) {
            document.getElementById('apellidosError').textContent = 'Debe contener solo letras y espacios, con mínimo 2 caracteres.';
            document.getElementById('apellidosError').style.display = 'block';
            isValid = false;
        }

        // Validar Teléfono
        const telefono = document.getElementById('Telefono').value.trim();
        if (!/^(\+?\d{1,3}?[- .]?)?\(?\d{3}\)?[- .]?\d{3}[- .]?\d{4}$/.test(telefono)) {
            document.getElementById('telefonoError').textContent = 'Ingrese un número de teléfono válido.';
            document.getElementById('telefonoError').style.display = 'block';
            isValid = false;
        }

        // Validar Dirección
        const direccion = document.getElementById('Direccion').value.trim();
        if (direccion.length < 5) {
            document.getElementById('direccionError').textContent = 'La dirección debe tener al menos 5 caracteres.';
            document.getElementById('direccionError').style.display = 'block';
            isValid = false;
        }

        // Validar Email
        const email = document.getElementById('Email').value.trim();
        if (!/^[^\s]+[^\s]+\.[^\s]+$/.test(email)) {
            document.getElementById('emailError').textContent = 'Ingrese un correo electrónico válido.';
            document.getElementById('emailError').style.display = 'block';
            isValid = false;
        }

        // Validar Contraseña
        const contrasena = document.getElementById('Contrasena').value;
        if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(contrasena)) {
            document.getElementById('contrasenaError').textContent = 'Debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.';
            document.getElementById('contrasenaError').style.display = 'block';
            isValid = false;
        }

        if (!isValid) {
            document.getElementById('generalError').style.display = 'block';

            // Desplazarse al primer error
            const firstError = document.querySelector('.text-danger[style="display: block;"]');
            if (firstError) {
                firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        } else {
            form.submit();
        }
    });

    // Opcional: Validación en tiempo real mientras el usuario escribe
    document.querySelectorAll('.floating-input').forEach(input => {
        input.addEventListener('input', function () {
            const errorElement = document.getElementById(this.id + 'Error');
            if (errorElement) {
                errorElement.style.display = 'none';
            }
        });
    });
});