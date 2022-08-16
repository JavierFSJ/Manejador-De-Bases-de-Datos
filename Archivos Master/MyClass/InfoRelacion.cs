using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivos_Master.MyClass
{
    class InfoRelacion
    {
        public string nombre { get; set; }
        public string atributo { get; set; }
        public long direccion { get; set; }
        public long siguiente { get; set;}

        private char[] nombreChar;

        public InfoRelacion(string nombre , string atributo , long direccion)
        {
            this.nombre = nombre;
            this.atributo = atributo;
            this.direccion = direccion;
            ConvertToArrayChar();
            siguiente = -1;
        }

        public InfoRelacion(char[] nombre, char[] atributo, long direccion , long siguiente)
        {
            nombreChar = nombre;
            this.atributo = ConvertToString(atributo);
            this.direccion = direccion;
            this.siguiente = siguiente;
            ConverToString();
        }

        private string ConvertToString(char[] cadena)
        {
            string cad = "";
            foreach (char c in cadena)
            {
                if (char.IsLetterOrDigit(c) || c == ' ' ||
                    char.IsPunctuation(c))
                {
                    cad += c;
                }
            }
            return cad;
        }

        public void ConvertToArrayChar()
        {
            nombreChar = new char[30];
            int i = 0;
            foreach (char c in nombre)
            {
                nombreChar[i] = c;
                i++;
            }
        }

        private void ConverToString()
        {
            nombre = "";
            foreach (char c in nombreChar)
            {
                if (char.IsLetter(c) || c == ' ')
                {
                    nombre += c;
                }
            }
        }

        private char[] ConvertToArrayChar(string val, int size)
        {
            char[] aux = new char[size];
            int i = 0;
            foreach (char c in val)
            {
                aux[i] = c;
                i++;
            }
            return aux;
        }

        public void GuardarRelacion(BinaryWriter writer)
        {
            writer.Write(nombreChar);
            writer.Write(ConvertToArrayChar(atributo , 30));
            writer.Write(direccion);
            writer.Write(siguiente);
        }
    }
}
