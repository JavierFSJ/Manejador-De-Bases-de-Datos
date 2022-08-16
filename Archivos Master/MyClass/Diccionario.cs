using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivos_Master.MyClass
{
    class Diccionario
    {
        public string rutaDiccionario { get; set; }
        public string rutaRelaciones { get; set; }
        public string rutaCarpeta { get; set; }
        public long direccionEnArchivo { get; set; }
        public long direccionHijos { get; set; }

        private FileStream archivoDiccionario;
        public LinkedList<Entidad> entidades { get; set; }

        public Diccionario()
        {
            direccionEnArchivo = 8;
            direccionHijos = 0;
            entidades = new LinkedList<Entidad>();
        }

        /****************************************
        *                                       * 
        *         Métodos de diccionario        *
        *                                       *
        ****************************************/
        #region
        public int AgregaNuevaEntidad(string nombre)
        {
            int res = 0;
            Entidad ent = new Entidad(nombre , direccionEnArchivo);
            LinkedListNode<Entidad> nodo = new LinkedListNode<Entidad>(ent);
            res = InsertaEntidadOrdenado(nodo, entidades.First);
            if (res == 1)
            {
                direccionEnArchivo += 70;
                ColocaDireccionSig();
            }
            return res;
        }

        public int EliminaEntidad(string nombre)
        {
            int res = 0;
            long registros = BuscaEntidad(nombre).Value.direccionRegistros;
            if (registros == -1 )
            {
                res = RetiraEntidad(nombre);
                if (res == 1)
                {
                    ColocaDireccionSig();
                }
                
            }
            else
            {
                MessageBox.Show("Esta entidad tiene registros");
            }
            return res;
        }

        public int ActualizaEntidad(string nuevoNombre, string nombreAntiguo)
        {
            int res = 0;
            long registros = BuscaEntidad(nombreAntiguo).Value.direccionRegistros;
            if (registros != -1)
            {
                res = ModificiaEntidad(nombreAntiguo, nuevoNombre);
                if (res == 1)
                {
                    ColocaDireccionSig();
                }
            }
            else
            {
                MessageBox.Show("Esta entidad tiene registros");
            }
            return res;
        }

        public int AgregaAtributo(string nombreEntidad , string nombreAtributo , char tipoDato , int tam , int llave , long padre)
        {
            int res = 0;
            Entidad tmp = BuscaEntidad(nombreEntidad).Value;
            Atributo nuevo = new Atributo(nombreAtributo, direccionEnArchivo, tipoDato, tam, llave, padre);
            LinkedListNode<Atributo> nuevoNodo = new LinkedListNode<Atributo>(nuevo);
            res = tmp.InsertaAtributo(nuevoNodo);
            if (res == 1)
            {
                tmp.ColocaDireccionAtributoSiguiente();
                tmp.ColocaPrimeraDireccionAtributos();
                direccionEnArchivo += 63;
            }
            return res;
        }

        public int EliminaAtributo(string nombreEntidad , string nombreAtributo)
        {
            int res = 0;
            Entidad tmp = BuscaEntidad(nombreEntidad).Value;
            if (tmp.direccionRegistros == -1)
            {
                long padre = tmp.BuscaAtributo(nombreAtributo).Value.padre;
                if (padre != -1)
                {
                    Entidad padreEnt = BuscaEntidadDireccion(padre).Value;
                    int res2 = padreEnt.EliminaRelacion(nombreAtributo);
                    if (res2 == 1)
                    {
                        padreEnt.ColocaDireccionHijosSiguiente();
                        padreEnt.ColocaPrimeraDireccionHijos();
                    }


                }


                res = tmp.EliminaAtributo(nombreAtributo);
                if (res == 1)
                {
                    tmp.ColocaDireccionHijosSiguiente();
                    tmp.ColocaPrimeraDireccionHijos();
                }
                
                
            }
            else
            {
                MessageBox.Show("Esta entidad cuenta con registros");
            }
            return res;
        }

        public int ModificaAtributo(string nombre, string nuevonombre, char tipo, int tam, int llave, long padre , string entidad)
        {
            int res = 0;
            Entidad tmp = BuscaEntidad(entidad).Value;
            if (tmp.direccionRegistros == -1)
            {
                res = tmp.ModificaAtributo(nombre, nuevonombre, tipo, tam ,llave, padre);
            }
            return res;
        }

        public int AgregaRegistro(string entidad , List<object>valores)
        {
            int res = 0;
            Entidad ent = BuscaEntidad(entidad).Value;
            Registro registro = new Registro(ent.atributos.ToList(), valores, ent.direccionEnArchivoRegistro);
            LinkedListNode<Registro> nodo = new LinkedListNode<Registro>(registro);
            res = ent.AgregaRegistro(nodo , ent.registros.First);
            if (res == 1)
            {
                ent.ColocaPrimeraDireccionRegistro();
                ent.ColocaDirSuienteRegistro();
                ent.direccionEnArchivoRegistro += registro.ObtenTam();
            }
            return res;
        }

        public int EliminaRegistro(string entidad, int valor)
        {
            int res = 0;
            Entidad ent = BuscaEntidad(entidad).Value;
            //MessageBox.Show(ent.nombre);
            if (ent.hijos.First != null)
            {
                bool existeValorHijo = false;
                List<InfoRelacion> hijos = ent.hijos.ToList();
                for (int i = 0; i < hijos.Count; i++)
                {
                    Entidad hijoEnt = BuscaEntidad(hijos[i].nombre).Value;
                    //MessageBox.Show(hijoEnt.nombre);
                    //MessageBox.Show(hijos[i].atributo);
                    List<Registro> registroshijos = hijoEnt.registros.ToList();
                    for (int j = 0; j < registroshijos.Count; j++)
                    {
                        if (Convert.ToInt32(registroshijos[j].getDato(hijos[i].atributo)) == valor)
                        {
                            existeValorHijo = true;
                            break;
                        }
                    }

                    if (existeValorHijo == true)
                    {
                        break;
                    }
                    
                }


                //MessageBox.Show(existeValorHijo.ToString());
                if (existeValorHijo == false)
                {
                    res = ent.EliminarRegistro(valor);
                    if (res == 1)
                    {
                        ent.ColocaDirSuienteRegistro();
                        ent.ColocaPrimeraDireccionRegistro();
                    }
                }
                else
                {
                    MessageBox.Show("Otros registro dependen de esta clave, intenta eliminar los registros dependientes");
                }
                
            }
            else
            {
                res = ent.EliminarRegistro(valor);
                if (res == 1)
                {
                    ent.ColocaDirSuienteRegistro();
                    ent.ColocaPrimeraDireccionRegistro();
                }
            }
            return res;


        }

        public int ModificaRegistro(string entidad, int clave , List<object> valores)
        {
            int res = 0;
            Entidad ent = BuscaEntidad(entidad).Value;
            Registro aux = new Registro(ent.atributos.ToList(), valores , -1);
            if (ent.hijos.First != null)
            {
                DialogResult dialogResult = MessageBox.Show("Esta entidad tiene hijos si continua se modificaran los hijos ¿Desea " +
                    "continuar?", "Modificar", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    res = ent.ModificaRegistro(clave , aux);
                    if (res == 3)
                    {
                        List<InfoRelacion> hijos = ent.hijos.ToList();
                        for (int i = 0; i < hijos.Count; i++)
                        {
                            Entidad hijo = BuscaEntidad(hijos[i].nombre).Value;
                            List<Registro> registrosHijos = hijo.registros.ToList();
                            for (int j = 0; j < registrosHijos.Count; j++)
                            {
                                if (Convert.ToInt32(registrosHijos[j].getDato(hijos[i].atributo)) == clave)
                                {
                                    registrosHijos[j].SetDato(aux.primaria , hijos[i].atributo);
                                }
                            }

                        }
                        ent.ColocaDirSuienteRegistro();
                        ent.ColocaPrimeraDireccionHijos();
                    }
                }
                
            }
            else
            {
                res = ent.ModificaRegistro(clave, aux);
                if (res == 1)
                {
                    ent.ColocaDirSuienteRegistro();
                    ent.ColocaPrimeraDireccionRegistro();
                }
            }

            return res;
        }

        public int ModificaRegistroModificandoPadres(string entidad, int clave , List<object> valores)
        {
            int res = 0;
            Entidad ent = BuscaEntidad(entidad).Value;
            Registro aux = new Registro(ent.atributos.ToList(), valores, -1);
            Registro registroPasado = ent.BuscaRegistro(clave);
            List<long> padres = ent.ObtenPadres();
            if (padres.Count > 0 )
            {
                bool flag = false;
                List<Atributo> atributosForaneos = ent.ObtenForena();
                List<object> valoresForaneos = new List<object>();
                List<object> valoresForaneosPasados = new List<object>();
                /*Obtencion de los valores foraneos del nuevo registro y del que se va a modificar*/
                for (int i = 0; i < atributosForaneos.Count; i++)
                {
                    valoresForaneos.Add(aux.getDato(atributosForaneos[i].nombre));
                    //MessageBox.Show(valoresForaneos[i].ToString());
                    valoresForaneosPasados.Add(registroPasado.getDato(atributosForaneos[i].nombre));
                    //MessageBox.Show(valoresForaneosPasados[i].ToString());
                }

                for (int i = 0; i < padres.Count; i++)
                {
                    Entidad padre = BuscaEntidadDireccion(padres[i]).Value;
                    if (padre.ExisteRegistroPrimaria(Convert.ToInt32(valoresForaneos[i])))
                    {
                        MessageBox.Show(atributosForaneos[i].nombre + " En la entidad " + padre.nombre + " Ya existe agregue un valor" +
                            "dirente a los actuales");
                        flag = true;
                        break;
                    }
                }

                int res2 = 0;
                if (flag == false)
                {
                    for (int i = 0; i < padres.Count; i++)
                    {
                        Entidad padre = BuscaEntidadDireccion(padres[i]).Value;
                        Registro registroModificar =padre.BuscaRegistro(Convert.ToInt32(valoresForaneosPasados[i]));
                        int AuxiliarLLavePasa = registroModificar.primaria;
                        //MessageBox.Show(AuxiliarLLavePasa.ToString());
                        registroModificar.SetPrimaria(Convert.ToInt32(valoresForaneos[i]));
                        res2 = padre.ModificaRegistro(AuxiliarLLavePasa, registroModificar);
                        if (res2 != 0)
                        {
                            padre.ColocaPrimeraDireccionRegistro();
                            padre.ColocaDirSuienteRegistro();
                        }
                        else
                        {
                            break;
                        }

                    }
                }

                if (res2 == 1)
                {
                    res = ModificaRegistro(entidad, clave, valores);
                }

            }
            else
            {
                res = ModificaRegistro(entidad, clave, valores);
            }

            return res;

        }

        public void GuardaNuevoDiccionario()
        {
            CrearCarpeta();
            archivoDiccionario = new FileStream(rutaDiccionario, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            if (archivoDiccionario != null)
            {
                BinaryWriter writer = new BinaryWriter(archivoDiccionario);
                EscribeCabecera(writer);
                GuardaEntidades(writer);
                writer.Flush();
                writer.Close();
                MessageBox.Show("Archivo guardado");
            }
            else
            {
                MessageBox.Show("No se pudo crear el diccionario");
            }
            archivoDiccionario.Close();
        }

        public void GuardaCambiosDiccionario()
        {
            bool Existe = File.Exists(rutaDiccionario);
            if (Existe == true)
            {
                archivoDiccionario = new FileStream(rutaDiccionario, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                if (archivoDiccionario != null)
                {
                    BinaryWriter writer = new BinaryWriter(archivoDiccionario);
                    EscribeCabecera(writer);
                    GuardaEntidades(writer);
                    writer.Flush();
                    writer.Close();
                    MessageBox.Show("Archivo guardado");
                }
                else
                {
                    MessageBox.Show("No se pudo crear el diccionario");
                }
                archivoDiccionario.Close();
            }
            else
            {
                MessageBox.Show("No se encuentra cargado ningun archivo");
            }
        }

        public int AbreDiccionario()
        {
            int res = 0;
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() != DialogResult.OK)
                res = -1;
            rutaDiccionario = open.FileName;

            rutaRelaciones = ObtenRutaDat(rutaDiccionario);
            if (string.IsNullOrEmpty(rutaDiccionario) == false)
            {
                archivoDiccionario = new FileStream(rutaDiccionario, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                if (archivoDiccionario != null)
                {
                    BinaryReader reader = new BinaryReader(archivoDiccionario);
                    long Cabecera = ObtenCabecera(reader);
                    //MessageBox.Show(Cabecera.ToString());
                    LeerEntidades(reader, Cabecera);
                    direccionEnArchivo = archivoDiccionario.Length;
                    reader.Close();


                }
                archivoDiccionario.Close();
            }
            return res;
        }

        public int InsertaHijoPadre(string entidadHijo, string padreNombre , string nombreLlaveHijo)
        {
            int res = 0;
            Entidad ent = BuscaEntidad(padreNombre).Value;
            InfoRelacion hijo = new InfoRelacion(entidadHijo, nombreLlaveHijo, direccionHijos);
            res = ent.insertaNuevaRelacion(hijo);
            if (res == 1)
            {
                direccionHijos += 76;
                ent.ColocaDireccionHijosSiguiente();
                ent.ColocaPrimeraDireccionHijos();
            }
            return res;
        }

        public void LimpiarDicionario()
        {
            direccionEnArchivo = 8;
            direccionHijos = 0;
            entidades.Clear();
        }

        #endregion


        /****************************************
        *                                       * 
        * Operaciones con la lista de entidades *
        *                                       *
        ****************************************/
        #region
        private int InsertaEntidadOrdenado(LinkedListNode<Entidad> nuevo , LinkedListNode<Entidad> iterador)
        {
            int res;
            if (iterador == null || string.Compare(iterador.Value.nombre, nuevo.Value.nombre) > 0)
            {
                if (iterador == null)
                {
                    entidades.AddLast(nuevo);
                    res = 1;
                }
                else
                {
                    entidades.AddBefore(iterador, nuevo);
                    res = 1;
                }
            }
            else
            {
                if (string.Compare(iterador.Value.nombre, nuevo.Value.nombre) == 0)
                {
                    res = 2;
                }
                else
                {
                    res = InsertaEntidadOrdenado(nuevo, iterador.Next);
                }
            }
            return res;
        }

        private int RetiraEntidad(string nombre)
        {
            int res = 0;
            LinkedListNode<Entidad> iterador = entidades.First;
            while (iterador != null)
            {
                if (iterador.Value.nombre.Equals(nombre))
                {
                    if (iterador.Value.direccionRegistros == -1)
                    {
                        entidades.Remove(iterador);
                        res = 1;
                        break;
                    }
                    else
                    {
                        res = 2;
                    }
                }
                iterador = iterador.Next;
            }
            return res;
        }

        private int ModificiaEntidad(string nombreAntiguo , string nuevoNombre)
        {
            int res = 0;
            LinkedListNode<Entidad> iterador = entidades.First;
            LinkedListNode<Entidad> tmp = new LinkedListNode<Entidad>(new Entidad());

            while (iterador != null)
            {
                if (iterador.Value.nombre.Equals(nombreAntiguo))
                {
                    tmp.Value.nombre = nuevoNombre;
                    tmp.Value.direccion = iterador.Value.direccion;
                    tmp.Value.direccionAtributos = iterador.Value.direccionAtributos;
                    tmp.Value.direccionRegistros = iterador.Value.direccionRegistros;
                    tmp.Value.atributos = tmp.Value.atributos;
                    tmp.Value.registros = tmp.Value.registros;
                    tmp.Value.hijos = tmp.Value.hijos;
                    tmp.Value.ConvertToArrayChar();
                    res = InsertaEntidadOrdenado(tmp, entidades.First);
                    if (res == 1) { entidades.Remove(iterador); }
                    break;
                }
                iterador = iterador.Next;
            }
            return res;
        }

        private void ColocaDireccionSig()
        {
            for (LinkedListNode<Entidad> tmp = entidades.First; tmp != null; tmp = tmp.Next)
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

        //public string 

        public LinkedListNode<Entidad> BuscaEntidad(string nombre)
        {
            LinkedListNode<Entidad> nodo = null;
            for (LinkedListNode<Entidad> tmp = entidades.First; tmp != null; tmp = tmp.Next)
            {
                if (nombre.Equals(tmp.Value.nombre) == true)
                {
                    nodo = tmp;
                    break;
                }
            }
            return nodo;
        }

        public LinkedListNode<Entidad> BuscaEntidadDireccion(long Dir)
        {
            LinkedListNode<Entidad> nodo = null;
            for (LinkedListNode<Entidad> tmp = entidades.First; tmp != null; tmp = tmp.Next)
            {
                if (Dir == tmp.Value.direccion)
                {
                    nodo = tmp;
                    break;
                }
            }
            return nodo;
        }

        public long getEncabezado()
        {
            long cabeza = -1;
            if (entidades.First != null)
            {
                cabeza = entidades.First.Value.direccion;
            }
            return cabeza;
        }
        #endregion


        /****************************************
        *                                       * 
        *               Archivos                *
        *                                       *
        ****************************************/
        #region 
        private string ObtenerNombreCarpeta(string dir)
        {
            string[] aux = dir.Split('\\');
            int longitud = aux.Length;
            string res = "\\";
            res += aux[longitud - 1];
            return res += ".dd";
        }

        private void CrearCarpeta()
        {
            SaveFileDialog guardar = new SaveFileDialog();
            guardar.ShowDialog();
            rutaCarpeta = guardar.FileName;
            string aux = rutaCarpeta;
            DirectoryInfo di = Directory.CreateDirectory(rutaCarpeta);
            rutaDiccionario = rutaCarpeta += ObtenerNombreCarpeta(rutaCarpeta);
            rutaRelaciones = aux += "\\Relaciones.dd";
        }
        private string ObtenRutaDat(string RutaDiccionario)
        {
            string aux = "";
            string[] tmp = RutaDiccionario.Split('\\');
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                aux += tmp[i];
                aux += "\\";
            }
            aux += "Relaciones.dd";
            
            return aux;
        }
        private void EscribeCabecera(BinaryWriter writer)
        {
            writer.BaseStream.Position = 0;
            writer.Write(getEncabezado());
        }

        private void GuardaEntidades(BinaryWriter writer)
        {
            List<Entidad> tmp = entidades.ToList();
            for (int i = 0; i <tmp.Count; i++)
            {
                writer.BaseStream.Position = tmp[i].direccion;
                tmp[i].GuardaEntidad(writer);

                if (tmp[i].direccionAtributos != -1)
                {
                    GuardaAtributos(tmp[i], writer);
                }

                if (tmp[i].direccionRegistros != -1)
                {
                    tmp[i].GuardaRegistros(rutaDiccionario);
                }

                if (tmp[i].direccionHijos != -1)
                {
                    FileStream archivoRel;
                    if (File.Exists(rutaRelaciones))
                    {
                        archivoRel = new  FileStream(rutaRelaciones, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    else
                    {
                        archivoRel = new FileStream(rutaRelaciones, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                    }

                    if (archivoRel != null)
                    {
                        BinaryWriter binaryWriter = new BinaryWriter(archivoRel);
                        GuardaHijos(tmp[i], binaryWriter);
                        binaryWriter.Flush();
                        direccionHijos = archivoRel.Length;
                        archivoRel.Close();
                    }
                }
            }
        }

        private void GuardaAtributos(Entidad ent , BinaryWriter writer)
        {
            List<Atributo> tmp = ent.atributos.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                writer.BaseStream.Position = tmp[i].direccion;
                tmp[i].GuardarAtributo(writer);
            }
            writer.Flush();
        }

        private void GuardaHijos(Entidad ent, BinaryWriter writer)
        {
            List<InfoRelacion> tmp = ent.hijos.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                writer.BaseStream.Position = tmp[i].direccion;
                tmp[i].GuardarRelacion(writer);
            }
            writer.Flush();
        }

        private long ObtenCabecera(BinaryReader reader)
        {
            long cab = -1;
            reader.BaseStream.Position = 0;
            cab = reader.ReadInt64();
            return cab;
        }

        private void LeerEntidades(BinaryReader reader , long direccionReader)
        {
            while (direccionReader != -1)
            {
                reader.BaseStream.Position = direccionReader;
                char[] nombre = reader.ReadChars(30);
                long dir = reader.ReadInt64();
                long direccionAtributos = reader.ReadInt64();
                long direccionRegis = reader.ReadInt64();
                long dirHijos = reader.ReadInt64();
                long direccionSig = reader.ReadInt64();
                Entidad nuevo = new Entidad(nombre, dir , direccionAtributos , direccionRegis , dirHijos, direccionSig);
                LinkedListNode<Entidad> nodo = new LinkedListNode<Entidad>(nuevo);
                if (direccionAtributos != -1)
                {
                    LeerAtributos(nodo, reader);
                }
                
                if (direccionRegis != -1)
                {
                   nuevo.LeerRegistro(rutaDiccionario);
                }
                
                if (dirHijos != -1)
                {
                    FileStream archivoRel;
                    if (File.Exists(rutaRelaciones))
                    {
                        archivoRel = new FileStream(rutaRelaciones, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    else
                    {
                        archivoRel = new FileStream(rutaRelaciones, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                    }

                    if (archivoRel != null)
                    {
                        BinaryReader reader1 = new BinaryReader(archivoRel);
                        LeerRelaciones(nodo, reader1);
                        archivoRel.Close();
                    }
                }
                entidades.AddLast(nodo);
                direccionReader = direccionSig;
            }
        }

        private void LeerAtributos(LinkedListNode<Entidad> entidad , BinaryReader reader)
        {
            long dir = entidad.Value.direccionAtributos;
            //MessageBox.Show(dir.ToString());
            while (dir != -1)
            {
                reader.BaseStream.Position = dir;
                char[] nombre = reader.ReadChars(30);
                long dirA = reader.ReadInt64();
                char tipoA = reader.ReadChar();
                int tam = reader.ReadInt32();
                int llave = reader.ReadInt32();
                long padre = reader.ReadInt64();
                long dirS = reader.ReadInt64();

                Atributo a = new Atributo(nombre, dirA, tipoA, tam, llave, padre, dirS);
                LinkedListNode<Atributo> nodo = new LinkedListNode<Atributo>(a);
                entidad.Value.atributos.AddLast(nodo);
                dir = dirS;

            }
        }

        private void LeerRelaciones(LinkedListNode<Entidad> entidad, BinaryReader reader)
        {
            long dir = entidad.Value.direccionHijos;
            while (dir != -1)
            {
                reader.BaseStream.Position = dir;
                char[] ent = reader.ReadChars(30);
                char[] atributo = reader.ReadChars(30);
                long direccion = reader.ReadInt64();
                long siguiente = reader.ReadInt64();

                InfoRelacion hijo = new InfoRelacion(ent, atributo, direccion, siguiente);
                LinkedListNode<InfoRelacion> nodo = new LinkedListNode<InfoRelacion>(hijo);
                entidad.Value.hijos.AddLast(nodo);
                dir = siguiente;

            }
        }


        #endregion

        
    }
}
