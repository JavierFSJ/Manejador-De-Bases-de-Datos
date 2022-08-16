using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivos_Master.MyClass
{
    class Entidad
    {
        public string nombre { get; set; }
        public long direccion { get; set; }
        public long direccionAtributos { get; set; }
        public long direccionRegistros { get; set; }
        public long direccionHijos { get; set; }
        public long siguiente { get; set; }
        public LinkedList<Atributo> atributos { get; set; }
        public LinkedList<Registro> registros { get; set; }
        public LinkedList<InfoRelacion> hijos { get; set; }
        
        public long direccionEnArchivoRegistro = 0;
        private char[] nombreChar;
        private FileStream archivoDat;

        public Entidad(string nombre, long dir)
        {
            this.nombre = nombre;
            direccion = dir;
            direccionAtributos = -1;
            direccionRegistros = -1;
            direccionHijos = -1;
            siguiente = -1;
            atributos = new LinkedList<Atributo>();
            registros = new LinkedList<Registro>();
            hijos = new LinkedList<InfoRelacion>();
            ConvertToArrayChar();
        }

        public Entidad(char[] nombre, long dir, long dirA, long dirD, long hijosD,long dirS)
        {
            nombreChar = nombre;
            direccion = dir;
            direccionAtributos = dirA;
            direccionRegistros = dirD;
            direccionHijos = hijosD;
            siguiente = dirS;
            atributos = new LinkedList<Atributo>();
            registros = new LinkedList<Registro>();
            hijos = new LinkedList<InfoRelacion>();
            ConverToString();
        }

        public Entidad() { atributos = new LinkedList<Atributo>(); registros = new LinkedList<Registro>();
            hijos = new LinkedList<InfoRelacion>();
        }


        /****************************************
        *                                       * 
        *       Operaciones con atributos       *
        *                                       *
        ****************************************/
        #region
        public int InsertaAtributo(LinkedListNode<Atributo> nuevo)
        {
            int res = 0;
            bool existe = ExisteAtributo(nuevo.Value.nombre);
            if (existe == false)
            {
                atributos.AddLast(nuevo);
                res = 1;
            }
            else
            {
                res = 2;
            }
            return res;
        }

        public int EliminaAtributo(string nombreAtributo)
        {
            int res = 0;
            LinkedListNode<Atributo> itr = atributos.First;
            while (itr != null)
            {
                if (nombreAtributo.Equals(itr.Value.nombre))
                {
                    atributos.Remove(itr);
                    res = 1;
                    break;
                }
                itr = itr.Next;
            }
            return res;
        }

        public bool ExisteAtributo(string nombre)
        {
            bool flag = false;
            LinkedListNode<Atributo> itr = atributos.First;
            while (itr != null)
            {
                if (nombre.Equals(itr.Value.nombre))
                {
                    flag = true;
                    break;
                }
                itr = itr.Next;
            }
            return flag;
        }

        public int ModificaAtributo(string nombre, string nuevonombre, char tipo, int tam, int llave, long padre)
        {
            int res = 0;
            LinkedListNode<Atributo> atributo = BuscaAtributo(nombre);
            bool existe = false;
            if (nombre.Equals(nuevonombre))
            {
                atributo.Value.tipo = tipo;
                atributo.Value.tam = tam;
                atributo.Value.llave = llave;
                atributo.Value.padre = padre;
                atributo.Value.ConvertToArrayChar();
                res = 1;
            }
            else
            {
                existe = ExisteAtributo(nuevonombre);
                if (existe == false)
                {
                    atributo.Value.nombre = nuevonombre;
                    atributo.Value.tipo = tipo;
                    atributo.Value.tam = tam;
                    atributo.Value.llave = llave;
                    atributo.Value.padre = padre;
                    atributo.Value.ConvertToArrayChar();
                    res = 1;
                }
                else
                {
                    res = 2;
                }
            }

            return res;
        }

        public LinkedListNode<Atributo> BuscaAtributo(string nombre)
        {
            LinkedListNode<Atributo> iterador = atributos.First;
            while (iterador != null)
            {
                if (iterador.Value.nombre.Equals(nombre))
                {

                    break;
                }
                iterador = iterador.Next;
            }
            return iterador;
        }

        public void ColocaDireccionAtributoSiguiente()
        {
            for (LinkedListNode<Atributo> tmp = atributos.First; tmp != null; tmp = tmp.Next)
            {
                if (tmp.Next == null)
                {
                    tmp.Value.siguiente = -1;
                }
                else
                {
                    tmp.Value.siguiente = tmp.Next.Value.direccion;
                }
            }
        }

        public void ColocaPrimeraDireccionAtributos()
        {
            if (atributos.First != null)
            {
                direccionAtributos = atributos.First.Value.direccion;
            }
            else
            {
                direccionAtributos = -1;
            }

        }

        #endregion

        /****************************************
        *                                       * 
        *       Auxiliares                      *
        *                                       *
        ****************************************/
        #region 
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

        public void GuardaEntidad(BinaryWriter writer)
        {
            writer.Write(nombreChar);
            writer.Write(direccion);
            writer.Write(direccionAtributos);
            writer.Write(direccionRegistros);
            writer.Write(direccionHijos);
            writer.Write(siguiente);
        }

        public List<long> ObtenPadres()
        {
            List<long> Llaves = new List<long>();
            for (int i = 0; i < atributos.Count; i++)
            {
                if (atributos.ElementAt(i).llave == 2)
                {
                    Llaves.Add(atributos.ElementAt(i).padre);
                }
            }
            return Llaves;
        }

        public List<string> ObtenNombreForanea()
        {
            List<string> Llaves = new List<string>();
            for (int i = 0; i < atributos.Count; i++)
            {
                if (atributos.ElementAt(i).llave == 2)
                {
                    Llaves.Add(atributos.ElementAt(i).nombre);
                }
            }
            return Llaves;

        }

        public List<Atributo> ObtenForena()
        {
            List<Atributo> Llaves = new List<Atributo>();
            for (int i = 0; i < atributos.Count; i++)
            {
                if (atributos.ElementAt(i).llave == 2)
                {
                    Llaves.Add(atributos.ElementAt(i));
                }
            }
            return Llaves;
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
        
        public string Primaria()
        {
            LinkedListNode<Atributo> iterador = atributos.First;
            string tmp = "";
            while (iterador != null)
            {
                if (iterador.Value.llave == 1)
                {
                    tmp = iterador.Value.nombre;
                }
                iterador = iterador.Next;
            }
            return tmp;
        }

        public bool TieneForanea()
        {
            LinkedListNode<Atributo> iterador = atributos.First;
            bool flag = false;
            while (iterador != null)
            {
                if (iterador.Value.llave == 2)
                {
                    flag = true;
                }
                iterador = iterador.Next;
            }
            return flag;
        }
        #endregion

        /****************************************
        *                                       * 
        *               Registros               *
        *                                       *
        ****************************************/
        #region 
        public int AgregaRegistro(LinkedListNode<Registro> nuevo , LinkedListNode<Registro> iterador)
        {
            int res = 0;
            if (iterador == null || nuevo.Value.primaria < iterador.Value.primaria)
            {
                if (iterador == null)
                {
                    registros.AddLast(nuevo);
                    res = 1;
                }
                else
                {
                    registros.AddBefore(iterador, nuevo);
                    res = 1;
                }
            }
            else
            {
                if (iterador.Value.primaria == nuevo.Value.primaria)
                {
                    res = 2;
                }
                else
                {
                    res = AgregaRegistro(nuevo, iterador.Next);
                }
            }
            return res;
        }

        public int EliminarRegistro(int clave)
        {
            int res = 0;
            LinkedListNode<Registro> itr = registros.First;
            while (itr != null)
            {
                if (itr.Value.primaria == clave)
                {
                    registros.Remove(itr);
                    res = 1;
                    break;
                }
                itr = itr.Next;
            }
            return res;
        }

        public int ModificaRegistro(int clavePasada , Registro aux)
        {
            int res = 0;
            if (ExisteRegistroPrimaria(aux.primaria))
            {
                Registro r = BuscaRegistro(clavePasada);
                int claveAux = r.primaria;
                r.SetDatos(atributos.ToList(), aux.getDatosRegistroSinDireccionesObject());
                r.SetPrimaria(claveAux);
                res = 1;
            }
            else
            {
                LinkedListNode<Registro> nuevo = new LinkedListNode<Registro>(aux);
                res = AgregaRegistro(nuevo , registros.First);
                if (res == 1)
                {
                    Registro R = BuscaRegistro(clavePasada);
                    nuevo.Value.direccion = R.direccion;
                    EliminarRegistro(clavePasada);
                    res = 3;
                }
            }
            return res;
        }

        public bool ExisteRegistroPrimaria(int clave)
        {
            bool flag = false;
            LinkedListNode<Registro> itr = registros.First;
            while (itr != null)
            {
                if (itr.Value.primaria == clave)
                {
                    flag = true;
                    break;
                }
                itr = itr.Next;
            }
            return flag;
        }

        public Registro BuscaRegistro(int clave)
        {
            Registro r = new Registro();
            LinkedListNode<Registro> itr = registros.First;
            while (itr != null)
            {
                if (itr.Value.primaria == clave)
                {
                    r = itr.Value;
                   
                    break;
                }
                itr = itr.Next;
            }
            return r;
        }

        public void ColocaDirSuienteRegistro()
        {
            for (LinkedListNode<Registro> tmp = registros.First; tmp != null; tmp = tmp.Next)
            {
                if (tmp.Next == null)
                {
                    tmp.Value.siguiente = -1;
                }
                else
                {
                    tmp.Value.siguiente = tmp.Next.Value.direccion;
                }
            }
        }

        public void ColocaPrimeraDireccionRegistro()
        {
            if (registros.First != null)
            {
                direccionRegistros = registros.First.Value.direccion;
            }
            else
            {
                direccionRegistros = -1;
            }
        }
        #endregion

        /****************************************
        *                                       * 
        *               ArchivoDat              *
        *                                       *
        ****************************************/
        #region 
        private string ObtenRutaDat(string RutaDiccionario)
        {
            string aux = "";
            string[] tmp = RutaDiccionario.Split('\\');
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                aux += tmp[i];
                aux += "\\";
            }
            aux += nombre;
            aux += ".dat";
            return aux;
        }

        public void GuardaRegistros(string Diccionario)
        {
            string ruta = ObtenRutaDat(Diccionario);
            if (File.Exists(ruta))
            {
                archivoDat = new FileStream(ruta, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            else
            {
                archivoDat = new FileStream(ruta, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }

            if (archivoDat != null)
            {
                BinaryWriter writer = new BinaryWriter(archivoDat);
                LinkedListNode<Registro> itr = registros.First;
                while (itr != null)
                {
                    writer.BaseStream.Position = itr.Value.direccion;
                    itr.Value.GuardaRegistro(writer);
                    itr = itr.Next;
                    
                }
                
                writer.Close();
                archivoDat.Close();
            }
        }

        public void LeerRegistro(string Diccionario)
        {
            string ruta = ObtenRutaDat(Diccionario);
            if (File.Exists(ruta))
            {
                archivoDat = new FileStream(ruta, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                if (archivoDat != null)
                {
                    BinaryReader reader = new BinaryReader(archivoDat);
                    long iterador = direccionRegistros;
                    List<object> datos = new List<object>();
                    List<Atributo> atr = atributos.ToList();
                    //MessageBox.Show(ruta);
                    while (iterador != -1)
                    {
                        reader.BaseStream.Position = iterador;
                        for (int i = 0; i < atr.Count; i++)
                        {
                            switch (atr[i].tipo)
                            {
                                case 'E':
                                    int valInt = reader.ReadInt32();
                                    datos.Add(valInt);
                                    break;
                                case 'D':
                                    decimal valDecimal = reader.ReadDecimal();
                                    datos.Add(valDecimal);
                                    break;
                                default:
                                    char[] valChar = reader.ReadChars(atr[i].tam);
                                    string valString = ConvertToString(valChar);
                                    datos.Add(valString);
                                    break;
                            }
                        }
                        long direccion = reader.ReadInt64();
                        long siguiente = reader.ReadInt64();

                        Registro r = new Registro(atr, datos, direccion, siguiente);
                        LinkedListNode<Registro> nuevoRegistro = new LinkedListNode<Registro>(r);
                        registros.AddLast(r);
                        datos.Clear();
                        iterador = siguiente;
                    }
                    direccionEnArchivoRegistro = archivoDat.Length;
                    reader.Close();
                    archivoDat.Close();
                }
            }
            


        }
        #endregion

        /****************************************
        *                                       * 
        *               Hijos                   *
        *                                       *
        ****************************************/
        #region 
        public int insertaNuevaRelacion(InfoRelacion hijo)
        {
            int res = 0;
            if (ExisteHijo(hijo.nombre) == false)
            {
                hijos.AddLast(hijo);
                res = 1;
            }
            return res;
        }

        public bool ExisteHijo(string nombreHijo)
        {
            bool flag = false;
            LinkedListNode<InfoRelacion> tmp = hijos.First;
            while (tmp != null)
            {
                if (tmp.Value.nombre.Equals(nombreHijo))
                {
                    flag = true;
                    break;
                }
                tmp = tmp.Next;
            }
            return flag;
        }

        public int EliminaRelacion(string nombreHijo)
        {
            int res = 0;
            LinkedListNode<InfoRelacion> tmp = hijos.First;
            while (tmp != null)
            {
                if (tmp.Value.atributo.Equals(nombreHijo))
                {
                    hijos.Remove(tmp);
                    break;
                }
                tmp = tmp.Next;
            }
            return res;
        }

        public void ColocaDireccionHijosSiguiente()
        {
            for (LinkedListNode<InfoRelacion> tmp = hijos.First; tmp != null; tmp = tmp.Next)
            {
                if (tmp.Next == null)
                {
                    tmp.Value.siguiente = -1;
                }
                else
                {
                    tmp.Value.siguiente = tmp.Next.Value.direccion;
                }
            }
        }

        public void ColocaPrimeraDireccionHijos()
        {
            if (hijos.First != null)
            {
                direccionHijos= hijos.First.Value.direccion;
            }
            else
            {
                direccionHijos = -1;
            }

        }
        #endregion


    }
}
