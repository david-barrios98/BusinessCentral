using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Application.Constants
{
    public static class MessageKeys
    {
        public const string QUERY = "QUERY";
        public const string QUERY_EMPTY = "QUERY_EMPTY";
        public const string SAVE = "SAVE";
        public const string ERROR_SAVE = "ERROR_SAVE";
        public const string UPDATE = "UPDATE";
        public const string ERROR_UPDATE = "ERROR_UPDATE";
        public const string DELETE = "DELETE";
        public const string ERROR_DELETE = "ERROR_DELETE";
        public const string ACTIVATE = "ACTIVATE";
        public const string ERROR_ACTIVATE = "ERROR_ACTIVATE";
        public const string UPDATE_STATE = "UPDATE_STATE";
        public const string ERROR_UPDATE_STATE = "ERROR_UPDATE_STATE";
        public const string EXISTS = "EXISTS";
        public const string NOT_EXISTS = "NOT_EXISTS";
        public const string TOKEN = "TOKEN";
        public const string ERROR_TOKEN = "ERROR_TOKEN";
        public const string LOGIN = "LOGIN";
        public const string ERROR_LOGIN = "ERROR_LOGIN";
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        public const string FORBIDDEN = "FORBIDDEN";
        public const string VALIDATE = "VALIDATE";
        public const string FAILED = "FAILED";
        public const string EXCEPTION = "EXCEPTION";
        public const string NULL_PARAMETER = "NULL_PARAMETER";
        public const string INVALID_PARAMETER = "INVALID_PARAMETER";
        public const string SUCCESS = "SUCCESS";
        public const string ERROR = "ERROR";
        public const string LOADING = "LOADING";
        public const string PROCESSING = "PROCESSING";
        public const string COMPLETED = "COMPLETED";
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string USER_NOT_PASSWORD = "USER_NOT_PASSWORD";

    }

    public static class Messages
    {
        public static readonly Dictionary<string, string> GENERAL = new()
        {
            [MessageKeys.QUERY] = "Consulta exitosa.",
            [MessageKeys.QUERY_EMPTY] = "No se encontraron registros.",
            [MessageKeys.SAVE] = "Se registró correctamente.",
            [MessageKeys.ERROR_SAVE] = "No se pudo registrar.",
            [MessageKeys.UPDATE] = "Se actualizó correctamente.",
            [MessageKeys.ERROR_UPDATE] = "No se pudo actualizar.",
            [MessageKeys.DELETE] = "Se eliminó correctamente.",
            [MessageKeys.ERROR_DELETE] = "No se pudo eliminar.",
            [MessageKeys.ACTIVATE] = "El registro fue activado.",
            [MessageKeys.ERROR_ACTIVATE] = "No se pudo activar el registro.",
            [MessageKeys.UPDATE_STATE] = "Se actualizó el estado correctamente.",
            [MessageKeys.ERROR_UPDATE_STATE] = "No se pudo actualizar el estado.",
            [MessageKeys.EXISTS] = "El registro ya existe.",
            [MessageKeys.NOT_EXISTS] = "El registro no existe.",
            [MessageKeys.TOKEN] = "Token generado correctamente.",
            [MessageKeys.ERROR_TOKEN] = "Error al generar el token.",
            [MessageKeys.LOGIN] = "Inicio de sesión exitoso.",
            [MessageKeys.ERROR_LOGIN] = "Credenciales incorrectas.",
            [MessageKeys.UNAUTHORIZED] = "No autorizado.",
            [MessageKeys.FORBIDDEN] = "Acceso prohibido.",
            [MessageKeys.VALIDATE] = "Errores de validación.",
            [MessageKeys.FAILED] = "La operación falló.",
            [MessageKeys.EXCEPTION] = "Ocurrió un error inesperado.",
            [MessageKeys.NULL_PARAMETER] = "Parámetro requerido no puede ser nulo.",
            [MessageKeys.INVALID_PARAMETER] = "Parámetro inválido.",
            [MessageKeys.SUCCESS] = "Operación exitosa.",
            [MessageKeys.ERROR] = "Error en la operación.",
            [MessageKeys.LOADING] = "Cargando...",
            [MessageKeys.PROCESSING] = "Procesando solicitud...",
            [MessageKeys.COMPLETED] = "Proceso completado.",
            [MessageKeys.USER_NOT_FOUND] = "El usuario no existe o no está registrado.",
            [MessageKeys.USER_NOT_PASSWORD] = "Credenciales invalidas",

        };
    }


}
