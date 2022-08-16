using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivos_Master.MyClass
{
    class CompiladorSQL
    {

        LinkedList<Entidad> entidades;

        public CompiladorSQL(LinkedList<Entidad> entidades)
        {
            this.entidades = entidades;
        }

        public void RecibeCadena(string Instruccion, DataGridView data , List<Entidad> entidades)
        {
            string[] cadena = Instruccion.Split();
            int posicionFrom = ObtenPosicionPalabra("From", cadena);
            int posicionWhere = ObtenPosicionPalabra("Where", cadena);
            int posicionJoin = ObtenPosicionPalabra("Join", cadena);

            /*Obtención de datos de la cadena*/
            List<string> atrs = ObtenAtributosCadena(cadena);
            string ent = ObtenEntidad(posicionFrom, cadena);


            /*Diferenciamos entre Inner Join y consulta con where*/
            if (posicionWhere != -1)
            {
                List<string> condicion = ObtenCondicion(posicionWhere, cadena);
                List<List<string>> registros = ObtenRegistrosCondicionWhere(atrs, ent, condicion);
                ColocaColumnasDataGrid(ent, atrs, data);
                ColocaRegistros(registros, data);

            }
            else if(posicionWhere == -1 && posicionJoin == -1)
            {
                //MessageBox.Show("No tenfo condicion");
                List<List<string>> registros = ObtenRegistrosSinCondicion(atrs, ent);
                ColocaColumnasDataGrid(ent, atrs, data);
                ColocaRegistros(registros ,data);
            }
            else
            {
                MessageBox.Show("Soy Inner Join");
                string segundaEntidad = cadena[posicionJoin + 1];
                int posicionOn = ObtenPosicionPalabra("On", cadena);
                List<string> condicionJoin = ObtenCondicionInnerJoin(posicionOn, cadena);
                List<List<string>> registros = ObtenRegistrosInnerJoin(atrs, ent, segundaEntidad, condicionJoin);
                //MessageBox.Show(registros.Count.ToString());
                ColocaColumnasDataGrid(ent, atrs, data);
                ColocaRegistros(registros, data);
            }
        }


        /****************************************
        *                                       * 
        *         Obtencion de datos            *
        *                                       *
        ****************************************/
        private int ObtenPosicionPalabra(string palabra , string[] cadena)
        {
            int pos = -1;
            for (int i = 0; i < cadena.Length; i++)
            {
                if (palabra.Equals(cadena[i]))
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }
        private List<string> ObtenAtributosCadena(string[] cadena)
        {
            List<string> atrs = new List<string>();
            int i = 0;
            while (i < cadena.Length && cadena[i].Equals("From") == false)
            {
                if (cadena[i].Equals("Select") == false)
                {
                    if (cadena[i].Contains(","))
                    {
                        atrs.Add(cadena[i].Remove(cadena[i].Length - 1));
                    }
                    else
                    {
                        atrs.Add(cadena[i]);
                    }
                   
                }
                i++;
            }
            return atrs;
        }
        private string ObtenEntidad(int indiceFrom, string[] Cadena)
        {
            string ent = "";
            for (int i = indiceFrom; i < Cadena.Length; i++)
            {
                if (Cadena[i].Equals("Where") || Cadena[i].Equals("Inner"))
                {
                    break;
                }
                else
                {
                    if (Cadena[i].Equals("From") == false)
                    {
                        ent = Cadena[i];
                    }
                }
            }
            return ent;
        }
        private List<string> ObtenCondicion(int indiceWhere, string[] Cadena)
        {
            List<string> condicion = new List<string>();
            if (indiceWhere != -1)
            {
                for (int i = indiceWhere; i < Cadena.Length; i++)
                {
                    if (Cadena[i].Equals("Where") == false)
                    {
                        if (Cadena[i].Contains(',') == true)
                        {
                            condicion.Add(Cadena[i].Remove(Cadena[i].Length - 1));
                        }
                        else
                        {
                            condicion.Add(Cadena[i]);
                        }
                    }
                }
            }
            return condicion;
        }
        private List<string> ObtenCondicionInnerJoin(int posicionOn , string[] cadena)
        {
            List<string> condicion = new List<string>();
            if (posicionOn != -1)
            {
                for (int i = posicionOn; i < cadena.Length; i++)
                {
                    if (cadena[i].Equals("On") == false)
                    {
                        if (cadena[i].Contains(',') == true)
                        {
                            condicion.Add(cadena[i].Remove(cadena[i].Length - 1));
                        }
                        else
                        {
                            condicion.Add(cadena[i]);
                        } 
                    }
                }
            }
            return condicion;
        }


        /*DataGrid*/
        private void ColocaColumnasDataGrid(string entidad , List<string> atr , DataGridView data)
        {
            data.Columns.Clear();
            LinkedListNode<Entidad> ent = BuscaEntidad(entidad);
            if (ent != null)
            {
                List<Atributo> tmp = ent.Value.atributos.ToList();
                if (atr.Count == 1 && atr.First().Equals("*"))
                {

                    for (int i = 0; i < tmp.Count; i++)
                    {

                        data.Columns.Add("columna" + i.ToString(), tmp[i].nombre);


                    }

                }
                else
                {
                    for (int i = 0; i < atr.Count; i++)
                    {

                        data.Columns.Add("columna" + i.ToString(), atr[i]);


                    }
                }
            }
        }
        private void ColocaRegistros(List<List<string>> registros , DataGridView data)
        {
            for (int i = 0; i < registros.Count; i++)
            {

                data.Rows.Add(registros[i].ToArray());


            }
        }


        /*Obtencicon de registros*/
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
        private List<List<string>> ObtenRegistrosSinCondicion(List<string> atr , string entidad)
        {
            List<List<string>> registros = new List<List<string>>();
            LinkedListNode<Entidad> ent = BuscaEntidad(entidad);
            if (ent != null)
            {
                List<Registro> r = ent.Value.registros.ToList();
                if (atr.Count == 1 && atr.First().Equals("*"))
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        registros.Add(r[i].getDatosRegistro());
                    }
                }
                else
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        registros.Add(r[i].getDatos(atr));
                    }

                }
            }
            return registros;
        }
        private List<List<string>> ObtenRegistrosCondicionWhere(List<string> atr, string entidad , List<string> condicion)
        {
            List<List<string>> registros = new List<List<string>>();
            LinkedListNode<Entidad> ent = BuscaEntidad(entidad);
            if (ent != null)
            {
                List<Registro> r = ent.Value.registros.ToList();
                if (atr.Count == 1 && atr.First().Equals("*"))
                {
         
                    for (int i = 0; i < r.Count; i++)
                    {
                        if (condicion[1].Equals("="))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato == Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatosRegistro());
                            }

                        }
                        else if (condicion[1].Equals("<>"))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato != Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatosRegistro());
                            }
                        }
                        else if (condicion[1].Equals(">"))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato > Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatosRegistro());
                            }
                        }
                        else if (condicion[1].Equals(">="))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato >= Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatosRegistro());
                            }
                        }
                        else if (condicion[1].Equals("<"))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato < Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatosRegistro());
                            }
                        }
                        else if (condicion[1].Equals("<="))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato <= Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatosRegistro());
                            }
                        }
                        
                    }
                }
                else
                {

                    for (int i = 0; i < r.Count; i++)
                    {
                        if (condicion[1].Equals("="))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato == Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatos(atr));
                            }

                        }
                        else if (condicion[1].Equals("<>"))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato != Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatos(atr));
                            }
                        }
                        else if (condicion[1].Equals(">"))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato > Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatos(atr));
                            }
                        }
                        else if (condicion[1].Equals(">="))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato >= Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatos(atr));
                            }
                        }
                        else if (condicion[1].Equals("<"))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato < Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatos(atr));
                            }
                        }
                        else if (condicion[1].Equals("<="))
                        {
                            int dato = Convert.ToInt32(r[i].getDato(condicion[0]));
                            if (dato <= Convert.ToInt32(condicion[2]))
                            {
                                registros.Add(r[i].getDatos(atr));
                            }
                        }
                        
                    }

                }
            }
            return registros;
        }
        private List<List<string>> ObtenRegistrosInnerJoin(List<string> atr, string entidad, string segundaEntidad , List<string> condicionInnerJoin)
        {
            List<List<string>> registros = new List<List<string>>();
            LinkedListNode<Entidad> ent = BuscaEntidad(entidad);
            LinkedListNode<Entidad> entInner = BuscaEntidad(segundaEntidad);

            if (ent != null && entInner != null)
            {
                List<string> artsEnt = getAtributosEntidad(atr, ent.Value);
                List<string> atrEntInner = getAtributosEntidad(atr, entInner.Value);

                string[] primerMiembro = condicionInnerJoin[0].Split('.');
                string[] segundoMiembro = condicionInnerJoin[0].Split('.');

                List<string> datosInner = datosInnerJoin(entInner.Value, segundoMiembro[1]);

                List<Registro> r = ent.Value.registros.ToList();
                if (atr.Count == 1 && atr.First().Equals("*"))
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        string valor = r[i].getDato(primerMiembro[1]).ToString();
                        if (ExisteValor(valor, datosInner))
                        {
                            Registro tmp = buscaRegistro(segundoMiembro[1], entInner.Value, valor);
                            List<string> datos1 = r[i].getDatosRegistro();
                            List<string> datos2 = tmp.getDatosRegistro();
                            datos1.AddRange(datos2);
                            registros.Add(datos1);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < r.Count; i++)
                    {
                        string valor = r[i].getDato(primerMiembro[1]).ToString();
                        if (ExisteValor(valor, datosInner))
                        {
                            Registro tmp = buscaRegistro(segundoMiembro[1], entInner.Value, valor);
                            List<string> datos1 = r[i].getDatos(artsEnt);
                            List<string> datos2 = tmp.getDatos(atrEntInner);
                            datos1.AddRange(datos2);
                            registros.Add(datos1);
                        }
                    }

                }

            }

            return registros;
        }



        /*Auxiliares*/
        private List<string> datosInnerJoin(Entidad entidad , string atributo)
        {
            List<string> res = new List<string>();
            List <Registro>r = entidad.registros.ToList();
            for (int i = 0; i < r.Count; i++)
            {
                res.Add(r[i].getDato(atributo).ToString());
            }
            return res;

        }
        private Registro buscaRegistro(string atr , Entidad ent , string valor)
        {
            Registro res = null;
            List<Registro> registro = ent.registros.ToList();
            for (int i = 0; i < registro.Count; i++)
            {
                if (valor.Equals(registro[i].getDato(atr).ToString()))
                {
                    res = registro[i];
                }
            }
            return res;
        }
        private bool ExisteValor(string valor , List<string> datosInner)
        {
            bool flag = false;
            for (int i = 0; i < datosInner.Count; i++)
            {
                if (valor.Equals(datosInner[i]))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        private List<string> getAtributosEntidad(List<string> atr , Entidad ent)
        {
            List<string> atributosEnt = new List<string>();
            for (int i = 0; i < atr.Count; i++)
            { 
                if (ent.ExisteAtributo(atr[i]))
                {
                    atributosEnt.Add(atr[i]);
                }
            }
            return atributosEnt;
        }

    }
}
