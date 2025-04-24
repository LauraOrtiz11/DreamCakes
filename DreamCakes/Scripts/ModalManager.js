// Scripts/auth/modalManager.js
(function ($) {
    'use strict';

    // Configuración global
    var config = {
        cleanupUrl: '/Cleanup/ClearTempMessages',
        modals: {
            register: '#registerModal',
            login: '#loginModal'
        }
    };

    // Limpiar modales y mensajes
    function cleanModals() {
        // Limpiar formularios
        $(config.modals.register + ' form')[0].reset();
        $(config.modals.login + ' form')[0].reset();

        // Ocultar mensajes locales
        $('.text-danger').hide();
        $('.alert').hide();

        // Limpiar mensajes del servidor
        clearServerMessages();
    }

    // Limpiar mensajes del servidor via AJAX
    function clearServerMessages() {
        $.ajax({
            url: config.cleanupUrl,
            type: 'POST',
            success: function () {
                console.log('TempData limpiado exitosamente');
            },
            error: function (xhr, status, error) {
                console.error('Error limpiando TempData:', error);
            }
        });
    }

    // Inicialización
    function init() {
        // Eventos para limpieza al cerrar modales
        $(config.modals.register).on('hidden.bs.modal', cleanModals);
        $(config.modals.login).on('hidden.bs.modal', cleanModals);

        // Eventos para el botón de cerrar
        $(config.modals.register + ' .close').click(cleanModals);
        $(config.modals.login + ' .close').click(cleanModals);

        // Mostrar modal correspondiente al cargar
        showInitialModal();
    }

    // Mostrar modal basado en TempData
    function showInitialModal() {
        // Esta función se implementaría en el layout como script inline
        // porque necesita acceder a TempData de Razor
    }

    // Exponer funciones públicas si es necesario
    window.ModalManager = {
        cleanModals: cleanModals,
        clearServerMessages: clearServerMessages
    };

    // Inicializar cuando el DOM esté listo
    $(document).ready(init);

})(jQuery);