using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivos_Master.MyClass
{
    class DatoRegistro
    {
        public string nombre { get; set; }
        public char tipo { get; set; }
        public int tam { get; set; }
        public object valor { get; set; }

        public DatoRegistro() { }

        public DatoRegistro (string nombre , char tipo, int tam , object valor)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.tam = tam;
            this.valor = valor;
        }
    }
}
