using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivos_Master.MyClass
{
    class Registro
    {
        public int primaria { get; set; }
        public long direccion { get; set; }

        public long siguiente { get; set; }

        public List<DatoRegistro> datos { get; set; }

        public Registro() { datos = new List<DatoRegistro>(); }

        public Registro(List<Atributo> atributos, List<object> valores, long direccion)
        {
            primaria = -1;
            datos = new List<DatoRegistro>();
            this.direccion = direccion;
            siguiente = -1;
            AsignaDatos(atributos, valores);
        }

        public Registro(List<Atributo> atributos, List<object> valores, long direccion, long sig)
        {
            primaria = -1;
            datos = new List<DatoRegistro>();
            this.direccion = direccion;
            siguiente = sig;
            AsignaDatos(atributos, valores);
        }

        private void AsignaDatos(List<Atributo> atributos, List<object> valores)
        {
            for (int i = 0; i < atributos.Count; i++)
            {
                DatoRegistro dato = new DatoRegistro(atributos[i].nombre
                    , atributos[i].tipo, atributos[i].tam, valores[i]);

                datos.Add(dato);
                if (atributos[i].llave == 1)
                {
                    primaria = Convert.ToInt32(valores[i]);
                }


            }
        }

        public List<String> getDatosRegistro()
        {
            List<String> tmp = new List<string>();
            for (int i = 0; i < datos.Count; i++)
            {
                tmp.Add(datos[i].valor.ToString());
            }
            tmp.Add(direccion.ToString());
            tmp.Add(siguiente.ToString());
            return tmp;
        }

        public List<String> getDatosRegistroSinDirecciones()
        {
            List<String> tmp = new List<string>();
            for (int i = 0; i < datos.Count; i++)
            {
                tmp.Add(datos[i].valor.ToString());
            }
            //tmp.Add(direccion.ToString());
            //tmp.Add(siguiente.ToString());
            return tmp;
        }

        public List<object> getDatosRegistroSinDireccionesObject()
        {
            List<object> tmp = new List<object>();
            for (int i = 0; i < datos.Count; i++)
            {
                tmp.Add(datos[i].valor.ToString());
            }
            //tmp.Add(direccion.ToString());
            //tmp.Add(siguiente.ToString());
            return tmp;
        }

        public int ObtenTam()
        {
            int count = 0;
            for (int i = 0; i < datos.Count; i++)
            {
                count += datos[i].tam;
            }
            count += 16;
            return count;
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

        public void GuardaRegistro(BinaryWriter writer)
        {
            for (int i = 0; i < datos.Count; i++)
            {
                if (datos[i].tipo == 'E')
                {
                    writer.Write(Convert.ToInt32(datos[i].valor));
                }
                else if (datos[i].tipo == 'D')
                {
                    writer.Write(Convert.ToDecimal(datos[i].valor));
                }
                else
                {
                    writer.Write(ConvertToArrayChar(datos[i].valor.ToString(), datos[i].tam));
                }
            }
            writer.Write(direccion);
            writer.Write(siguiente);
        }

        public object getDato(string nombre)
        {
            object val = -1;
            for (int i = 0; i < datos.Count; i++)
            {
                if (nombre.Equals(datos[i].nombre))
                {
                    val = datos[i].valor;
                    break;
                }
            }
            return val;
        }

        public List<string> getDatos(List<string> nombres)
        {
            List<string> valores = new List<string>();
            for (int i = 0; i < nombres.Count; i++)
            {
                valores.Add(getDato(nombres[i]).ToString());
            }
            return valores;
        }


        public List<object> getDatosObject(List<string> nombres)
        {
            List<object> valores = new List<object>();
            for (int i = 0; i < nombres.Count; i++)
            {
                valores.Add(getDato(nombres[i]));
            }
            return valores;
        }


        public void SetDatos(List<Atributo> atributos, List<object> valores)
        {
            datos.Clear();
            AsignaDatos(atributos, valores);
        }

        public void SetPrimaria(int clave)
        {
            for (int i = 0; i < datos.Count; i++)
            {
                if (primaria == Convert.ToInt32(datos[i].valor))
                {
                    datos[i].valor = clave;
                    primaria = clave;
                    break;
                }
            }
        }

        public void SetDato(int valor , string nombre)
        {
            for (int i = 0; i < datos.Count; i++)
            {
                if (nombre.Equals(datos[i].nombre))
                {
                    datos[i].valor = valor;
                    break;
                }
            }
        }
    }
}
