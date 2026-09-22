namespace TallerMecanicoScrum.Clases
{
    public class Validaciones
    {
        // Permite solamente letras y espacios.
        // Se puede usar en nombres, apellidos y nombres de servicios.
        // Permite: "Juan Perez", "Cambio De Aceite"
        // No permite: números ni símbolos como @, #, !, -, etc.
        public static bool SoloLetras(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                if (!char.IsLetter(caracter) && !char.IsWhiteSpace(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        // Permite solamente números.
        // Se puede usar en teléfono, CI/NIT u otros campos numéricos guardados como texto.
        // Permite: "70707070", "1234567"
        // No permite: letras, espacios ni símbolos.
        public static bool SoloNumeros(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                if (!char.IsDigit(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        // Permite letras, números y espacios.
        // Se puede usar en modelos de vehículos u otros campos que mezclen letras y números.
        // Permite: "Corolla 2020", "Mazda CX5", "Ford F150"
        // No permite: símbolos como @, #, !, %, $, etc.
        public static bool LetrasNumerosEspacios(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char caracter in texto)
            {
                if (!char.IsLetterOrDigit(caracter) &&
                    !char.IsWhiteSpace(caracter))
                {
                    return false;
                }
            }

            return true;
        }

        // Convierte la primera letra de cada palabra a mayúscula
        // y el resto de las letras a minúscula.
        // Se puede usar en nombres, apellidos y nombres de servicios.
        // Ejemplo: "JUAN PEREZ" pasa a "Juan Perez".
        // No valida números ni símbolos, solamente cambia el formato del texto.
        public static string Capitalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return "";
            }

            texto = texto.Trim().ToLower();

            string resultado = "";
            bool nuevaPalabra = true;

            foreach (char caracter in texto)
            {
                if (char.IsWhiteSpace(caracter))
                {
                    resultado += caracter;
                    nuevaPalabra = true;
                }
                else if (nuevaPalabra)
                {
                    resultado += char.ToUpper(caracter);
                    nuevaPalabra = false;
                }
                else
                {
                    resultado += caracter;
                }
            }

            return resultado;
        }

        // Valida el formato de una placa.
        // Permite 3 o 4 números seguidos de 3 letras.
        // Ejemplos válidos: "489BLH", "009SSE", "5214BEX".
        // No permite letras al inicio, menos de 3 números,
        // más de 4 números, menos o más de 3 letras, espacios o símbolos.
        public static bool ValidarPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                return false;
            }

            placa = placa.Trim().ToUpper();

            if (placa.Length != 6 && placa.Length != 7)
            {
                return false;
            }

            int cantidadNumeros = placa.Length - 3;

            for (int i = 0; i < cantidadNumeros; i++)
            {
                if (!char.IsDigit(placa[i]))
                {
                    return false;
                }
            }

            for (int i = cantidadNumeros; i < placa.Length; i++)
            {
                if (!char.IsLetter(placa[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // Quita espacios al inicio y al final de la placa
        // y convierte todas las letras a mayúsculas.
        // Ejemplo: " 5214bex " pasa a "5214BEX".
        // No valida si la placa tiene el formato correcto,
        // solamente la prepara antes de validar o guardar.
        public static string FormatearPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                return "";
            }

            return placa.Trim().ToUpper();
        }
    }
}
