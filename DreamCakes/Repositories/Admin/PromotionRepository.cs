using System;
using System.Collections.Generic;
using System.Linq;
using DreamCakes.Dtos.Admin;
using DreamCakes.Repositories.Models;

namespace DreamCakes.Repositories.Admin
{
    // Clase que se encarga del acceso directo a datos para promociones.
    public class PromotionRepository
    {
        // Instancia directa del DbContext.
        private DreamCakesEntities db = new DreamCakesEntities();

        // Método que devuelve todas las promociones desde la base de datos.
        public List<PROMOCION> GetAll()
        {
            // Obtiene todas las promociones y las devuelve en una lista.
            return db.PROMOCIONs.ToList();
        }

        // Método que obtiene una promoción específica por su Id.
        public PROMOCION GetById(int id)
        {
            // Busca y devuelve la promoción según su Id.
            return db.PROMOCIONs.Find(id);
        }

        // Método para agregar una nueva promoción a la base de datos.
        public void Add(PROMOCION promo)
        {
            // Agrega la promoción al contexto.
            db.PROMOCIONs.Add(promo);

            // Guarda los cambios en la base de datos.
            db.SaveChanges();
        }

        // Método para actualizar una promoción existente.
        public void Update(PROMOCION promo)
        {
            // Busca la promoción original.
            var existingPromo = db.PROMOCIONs.Find(promo.ID_Promocion);

            // Si no se encuentra, termina la ejecución.
            if (existingPromo == null) return;

            // Actualiza las propiedades de la promoción.
            existingPromo.Nombre_Prom = promo.Nombre_Prom;
            existingPromo.Porc_Desc = promo.Porc_Desc;
            existingPromo.Fecha_Ini = promo.Fecha_Ini;
            existingPromo.Fecha_Fin = promo.Fecha_Fin;
            existingPromo.Estado = promo.Estado;
            existingPromo.Descrip_Prom = promo.Descrip_Prom;

            // Guarda los cambios en la base de datos.
            db.SaveChanges();
        }

        // Método para eliminar una promoción según su Id.
        public void Delete(int id)
        {
            // Busca la promoción.
            var promo = db.PROMOCIONs.Find(id);

            // Si no existe, termina la ejecución.
            if (promo == null) return;

            // Elimina la promoción del contexto.
            db.PROMOCIONs.Remove(promo);

            // Guarda los cambios en la base de datos.
            db.SaveChanges();
        }


        // Método para activar o desactivar una promoción.
        public void ToggleStatus(int id)
        {
            // Busca la promoción por su Id.
            var promo = db.PROMOCIONs.Find(id);

            // Si no existe, no hace nada.
            if (promo == null) return;

            // Cambia el estado de la promoción (activo/inactivo).
            promo.Estado = !promo.Estado;

            // Guarda los cambios.
            db.SaveChanges();
        }

        internal void Update(PromotionDto promo)
        {
            throw new NotImplementedException();
        }
    }
}