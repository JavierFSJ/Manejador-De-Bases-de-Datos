using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivos_Master.MyClass
{
    class Atributo
    {
        public string nombre { get; set; }
        public long direccion { get; set; }
        public char tipo { get; set; } 
        public int tam { get; set; }
        public int llave { get; set; }
        public long padre { get; set; }
        public long siguiente { get; set; }

        private char[] nombreChar;

        public Atributo() { }

        public Atributo(string nombre , long direccion , char tipo , int tam , int llave , long padre) 
        {
            this.nombre = nombre;
            this.direccion = direccion;
            this.tipo = tipo;
            this.tam = tam;
            this.llave = llave;
            this.padre = padre;
            siguiente = -1;
            ConvertToArrayChar();
        }

        public Atributo(char[] nombre, long direccion, char tipo, int tam, int llave, long padre , long sig)
        {
            this.nombreChar = nombre;
            this.direccion = direccion;
            this.tipo = tipo;
            this.tam = tam;
            this.llave = llave;
            this.padre = padre;
            siguiente = sig;
            ConverToString();
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
                if (char.IsLetter(c))
                {
                    nombre += c;
                }
            }
        }

        public void GuardarAtributo(BinaryWriter writer)
        {
            writer.Write(nombreChar);
            writer.Write(direccion);
            writer.Write(tipo);
            writer.Write(tam);
            writer.Write(llave);
            writer.Write(padre);
            writer.Write(siguiente);
        }
    }
}
