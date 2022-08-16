using Archivos_Master.MyClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archivos_Master
{
    public partial class Form1 : Form
    {
        Diccionario diccionario;
        CompiladorSQL compilador;
        long DireccionEntidadExterna = -1;
        bool agregarForanea = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelEncabezado.Text = "-1";
            diccionario = new Diccionario();
            compilador = new CompiladorSQL(diccionario.entidades);
            panelLlaveForanea.Enabled = false;
        }


        /****************************************
        *                                       * 
        *         Botones de Entidades          *
        *                                       *
        ****************************************/
        #region
        private void btAgregaEntidad_Click(object sender, EventArgs e)
        {
            int res = 0;
            bool flag = CampoVacio(tbEntidadNombre);
            if (flag == false)
            {
                res = diccionario.AgregaNuevaEntidad(tbEntidadNombre.Text);
                switch (res)
                {
                    case 1:
                        MessageBox.Show("Entidad " + tbEntidadNombre.Text + " Insertada");
                        tbEntidadNombre.Clear();
                        MuestraEntidades();
                        ColocaEntidadesCombox();
                        labelEncabezado.Text = diccionario.getEncabezado().ToString();
                        break;
                    default:
                        MessageBox.Show("No se pudo insertar entidad");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Completa los campos vacios");
            }
        }

        private void btEliminaEntidad_Click(object sender, EventArgs e)
        {
            int res = 0;
            bool flag = CampoVacio(cbEntidadNombre);
            if (flag == false)
            {
                res = diccionario.EliminaEntidad(cbEntidadNombre.Text);
                switch (res)
                {
                    case 1:
                        MessageBox.Show("Entidad eliminada");
                        MuestraEntidades();
                        ColocaEntidadesCombox();
                        cbEntidadNombre.Text = "";
                        labelEncabezado.Text = diccionario.getEncabezado().ToString();
                        break;
                    default:
                        MessageBox.Show("Error al eliminar entidad");
                        break;
                }
            }
        }

        private void btModificaEntidad_Click(object sender, EventArgs e)
        {
            int res = 0;
            bool flag = CampoVacio(tbEntidadNombre) || CampoVacio(cbEntidadNombre);
            if (flag == false)
            {
                res = diccionario.ActualizaEntidad(tbEntidadNombre.Text, cbEntidadNombre.Text);
                switch (res)
                {
                    case 1:
                        MessageBox.Show("Entidad Modificada");
                        MuestraEntidades();
                        ColocaEntidadesCombox();
                        labelEncabezado.Text = diccionario.getEncabezado().ToString();
                        cbEntidadNombre.Text = "";
                        break;
                    default:
                        MessageBox.Show("Error al eliminar entidad");
                        break;
                }
            }
        }
        #endregion

        /****************************************
        *                                       * 
        *         Botones de Atributos          *
        *                                       *
        ****************************************/
        #region
        private void btAgregaAtributo_Click(object sender, EventArgs e)
        {
            bool flag = CampoVacio(cbEntidadAtributo);
            if (flag == false)
            {
                bool campoTipoDatoVacio = CampoVacio(cbTipoDato);
                if (campoTipoDatoVacio == false)
                {
                    char tipoDato = cbTipoDato.Text.First();
                    switch (tipoDato)
                    {
                        case 'E':
                            bool camposRestantes = CampoVacio(tbNombreAtributo) || CampoVacio(cbTipoLlave);
                            if (camposRestantes == false)
                            {
                                int res = 0;
                                if (agregarForanea)
                                {
                                    int insertado = diccionario.InsertaHijoPadre(cbEntidadAtributo.Text, cbEntidadForanea.Text, tbNombreAtributo.Text);
                                    if (insertado == 1)
                                    {
                                        MessageBox.Show("Nueva Relacion creada");
                                        res = diccionario.AgregaAtributo(cbEntidadAtributo.Text, tbNombreAtributo.Text, tipoDato, 4, cbTipoLlave.SelectedIndex, DireccionEntidadExterna);
                                        agregarForanea = false;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ya existe una llave a esa entidad");
                                        agregarForanea = false;
                                        DireccionEntidadExterna = -1;
                                    }
                                }
                                else
                                {
                                    res = diccionario.AgregaAtributo(cbEntidadAtributo.Text, tbNombreAtributo.Text, tipoDato, 4, cbTipoLlave.SelectedIndex, DireccionEntidadExterna);
                                }
                                switch (res)
                                {
                                    case 1:
                                        MessageBox.Show("Atributo agregado");
                                        MuestraAtributos();
                                        MuestraEntidades();
                                        tbNombreAtributo.Clear();
                                        tbTam.Clear();
                                        ColocarAtributosCombox();
                                        panelLlaveForanea.Enabled = false;
                                        DireccionEntidadExterna = -1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case 'D':
                            bool camposRestantes2 = CampoVacio(tbNombreAtributo) || CampoVacio(cbTipoLlave);
                            if (camposRestantes2 == false)
                            {
                                int res = diccionario.AgregaAtributo(cbEntidadAtributo.Text, tbNombreAtributo.Text, tipoDato, 16, cbTipoLlave.SelectedIndex, DireccionEntidadExterna);
                                switch (res)
                                {
                                    case 1:
                                        MessageBox.Show("Atributo agregado");
                                        MuestraAtributos();
                                        MuestraEntidades();
                                        tbNombreAtributo.Clear();
                                        tbTam.Clear();
                                        ColocarAtributosCombox();
                                        panelLlaveForanea.Enabled = false;
                                        DireccionEntidadExterna = -1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        default:
                            bool camposRestantes3 = CampoVacio(tbNombreAtributo) || CampoVacio(cbTipoLlave) || CampoVacio(tbTam);
                            if (camposRestantes3 == false)
                            {
                                int res = diccionario.AgregaAtributo(cbEntidadAtributo.Text, tbNombreAtributo.Text, tipoDato, Convert.ToInt32(tbTam.Text), cbTipoLlave.SelectedIndex, DireccionEntidadExterna);
                                switch (res)
                                {
                                    case 1:
                                        MessageBox.Show("Atributo agregado");
                                        MuestraAtributos();
                                        MuestraEntidades();
                                        tbNombreAtributo.Clear();
                                        tbTam.Clear();
                                        ColocarAtributosCombox();
                                        panelLlaveForanea.Enabled = false;
                                        DireccionEntidadExterna = -1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void btEliminaAtributo_Click(object sender, EventArgs e)
        {
            int res = 0;
            bool flag = CampoVacio(cbEntidadAtributo) || CampoVacio(cbAtributoActual);
            if (flag == false)
            {
                res = diccionario.EliminaAtributo(cbEntidadAtributo.Text, cbAtributoActual.Text);
                switch (res)
                {
                    case 1:
                        MuestraAtributos();
                        MuestraEntidades();
                        ColocarAtributosCombox();
                        MessageBox.Show("Atributo Eliminado");
                        break;
                    default:
                        MessageBox.Show("Operacion fallida");
                        break;
                }
            }
        }

        private void btModificaAtributo_Click(object sender, EventArgs e)
        {
            bool flag = CampoVacio(cbEntidadAtributo);
            if (flag == false)
            {
                bool campoTipoDatoVacio = CampoVacio(cbTipoDato);
                if (campoTipoDatoVacio == false)
                {
                    char tipoDato = cbTipoDato.Text.First();
                    switch (tipoDato)
                    {
                        case 'E':
                            bool camposRestantes = CampoVacio(tbNombreAtributo) || CampoVacio(cbTipoLlave) || CampoVacio(cbAtributoActual);
                            if (camposRestantes == false)
                            {
                                int res = diccionario.ModificaAtributo(cbAtributoActual.Text, tbNombreAtributo.Text, tipoDato, 4, cbTipoLlave.SelectedIndex, DireccionEntidadExterna, cbEntidadAtributo.Text);
                                switch (res)
                                {
                                    case 1:
                                        MessageBox.Show("Atributo Modificado");
                                        MuestraAtributos();
                                        MuestraEntidades();
                                        tbNombreAtributo.Clear();
                                        tbTam.Clear();
                                        ColocarAtributosCombox();
                                        panelLlaveForanea.Enabled = false;
                                        DireccionEntidadExterna = -1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case 'D':
                            bool camposRestantes2 = CampoVacio(tbNombreAtributo) || CampoVacio(cbTipoLlave) || CampoVacio(cbAtributoActual);
                            if (camposRestantes2 == false)
                            {
                                int res = diccionario.ModificaAtributo(cbAtributoActual.Text, tbNombreAtributo.Text, tipoDato, 8, cbTipoLlave.SelectedIndex, DireccionEntidadExterna, cbEntidadAtributo.Text);
                                switch (res)
                                {
                                    case 1:
                                        MessageBox.Show("Atributo Modificado");
                                        MuestraAtributos();
                                        MuestraEntidades();
                                        tbNombreAtributo.Clear();
                                        tbTam.Clear();
                                        ColocarAtributosCombox();
                                        panelLlaveForanea.Enabled = false;
                                        DireccionEntidadExterna = -1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        default:
                            bool camposRestantes3 = CampoVacio(tbNombreAtributo) || CampoVacio(cbTipoLlave) || CampoVacio(tbTam) || CampoVacio(cbAtributoActual);
                            if (camposRestantes3 == false)
                            {
                                int res = diccionario.ModificaAtributo(cbAtributoActual.Text, tbNombreAtributo.Text, tipoDato, Convert.ToInt32(tbTam.Text), cbTipoLlave.SelectedIndex, DireccionEntidadExterna, cbEntidadAtributo.Text);
                                switch (res)
                                {
                                    case 1:
                                        MessageBox.Show("Atributo modificado");
                                        MuestraAtributos();
                                        MuestraEntidades();
                                        tbNombreAtributo.Clear();
                                        tbTam.Clear();
                                        ColocarAtributosCombox();
                                        panelLlaveForanea.Enabled = false;
                                        DireccionEntidadExterna = -1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void btAgregaForanea_Click(object sender, EventArgs e)
        {
            bool flag = CampoVacio(cbEntidadForanea);
            if (flag == false)
            {
                Entidad ent = diccionario.BuscaEntidad(cbEntidadForanea.Text).Value;
                DireccionEntidadExterna = ent.direccion;


                agregarForanea = true;
                MessageBox.Show("Entidad enlazada, ya puedes agregar tu atributo");
            }
        }

        private void btCancelaForanea_Click(object sender, EventArgs e)
        {
            DireccionEntidadExterna = -1;
            panelLlaveForanea.Enabled = false;
            agregarForanea = false;
        }
        #endregion


        /****************************************
        *                                       * 
        *         Datagrids                     *
        *                                       *
        ****************************************/
        #region 
        private void MuestraEntidades()
        {
            dataGridEntidades.Rows.Clear();
            List<Entidad> tmp = diccionario.entidades.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {

                dataGridEntidades.Rows.Add(tmp[i].nombre, tmp[i].direccion, tmp[i].direccionAtributos,
                                           tmp[i].direccionRegistros, tmp[i].direccionHijos ,tmp[i].siguiente);
            }
        }

        private void MuestraAtributos()
        {
            dataGridAtributos.Rows.Clear();
            LinkedListNode<Entidad> entidad = diccionario.BuscaEntidad(cbEntidadAtributo.Text);
            List<Atributo> tmp = entidad.Value.atributos.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                dataGridAtributos.Rows.Add(tmp[i].nombre, tmp[i].direccion, tmp[i].tipo, tmp[i].tam,
                                            tmp[i].llave, tmp[i].padre, tmp[i].siguiente);
            }
        }

        private void MuestraEntidadesPadres()
        {
            dataGridViewPadres.Rows.Clear();
            Entidad tmp = diccionario.BuscaEntidad(cbEntidadRelaciones.Text).Value;
            List<long> direcciones = tmp.ObtenPadres();
            for (int i = 0; i < direcciones.Count; i++)
            {
                dataGridViewPadres.Rows.Add(diccionario.BuscaEntidadDireccion(direcciones[i]).Value.nombre,
                   diccionario.BuscaEntidadDireccion(direcciones[i]).Value.Primaria());
            }
        }

        private void MuestraHijos()
        {
            dataGridHijos.Rows.Clear();
            Entidad tmp = diccionario.BuscaEntidad(cbEntidadRelaciones.Text).Value;
            List<InfoRelacion> hijos = tmp.hijos.ToList();
            for (int i = 0; i < hijos.Count; i++)
            {
                dataGridHijos.Rows.Add(hijos[i].nombre, hijos[i].atributo);
            }
        }

        private void MuestraEntidadForanea()
        {
            dataGridViewEnttidadRelacion.Rows.Clear();
            Entidad tmp = diccionario.BuscaEntidad(cbEntidadRelaciones.Text).Value;
            List<string> llaves = tmp.ObtenNombreForanea();
            for (int i = 0; i < llaves.Count; i++)
            {
                dataGridViewEnttidadRelacion.Rows.Add(tmp.nombre, llaves[i]);
            }
        }

        private void ColocaCapturaRegistro()
        {
            dataGridViewCapturaRegistro.Columns.Clear();
            List<Atributo> tmp = diccionario.BuscaEntidad(cbEntidadRegistros.Text).Value.atributos.ToList();

            for (int i = 0; i < tmp.Count; i++)
            {

                if (tmp[i].llave == 2 && checkBox1.Checked == false)
                {
                    
                    DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                    //column.ReadOnly = true;
                    column.HeaderText = tmp[i].nombre;
                    LlenaComboxForanea(column, tmp[i]);
                    
                    dataGridViewCapturaRegistro.Columns.Add(column);
                }
                else
                {
                    dataGridViewCapturaRegistro.Columns.Add("columna" + i.ToString(), tmp[i].nombre);
                }


            }
            dataGridViewCapturaRegistro.Rows.Add();
        }

        private void ColocaColumnasRegistro()
        {
            dataGridViewRegistros.Columns.Clear();
            List<Atributo> tmp = diccionario.BuscaEntidad(cbEntidadRegistros.Text).Value.atributos.ToList();

            for (int i = 0; i < tmp.Count; i++)
            {

                dataGridViewRegistros.Columns.Add("columna" + i.ToString(), tmp[i].nombre);


            }
            dataGridViewRegistros.Columns.Add("direccion", "Dirección");
            dataGridViewRegistros.Columns.Add("siguiente", "Siguinete");

        }

        private void MuestraRegistros()
        {
            dataGridViewRegistros.Rows.Clear();
            List<Registro> tmp = diccionario.BuscaEntidad(cbEntidadRegistros.Text).Value.registros.ToList();

            for (int i = 0; i < tmp.Count; i++)
            {

                dataGridViewRegistros.Rows.Add(tmp[i].getDatosRegistro().ToArray());
            }
        }
        #endregion

        /****************************************
        *                                       * 
        *              Comboxes                 *
        *                                       *
        ****************************************/
        #region 
        private void ColocaEntidadesCombox()
        {
            cbEntidadNombre.Items.Clear();
            cbEntidadAtributo.Items.Clear();
            cbEntidadRelaciones.Items.Clear();
            cbEntidadRegistros.Items.Clear();
            List<Entidad> tmp = diccionario.entidades.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                cbEntidadNombre.Items.Add(tmp[i].nombre);
                cbEntidadAtributo.Items.Add(tmp[i].nombre);
                cbEntidadRelaciones.Items.Add(tmp[i].nombre);
                cbEntidadRegistros.Items.Add(tmp[i].nombre);
            }
        }

        private void cbTipoDato_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool campoTipoDatoVacio = CampoVacio(cbTipoDato);
            if (campoTipoDatoVacio == false)
            {
                char tipoDato = cbTipoDato.Text.First();
                if (tipoDato == 'C')
                {
                    tbTam.Enabled = true;
                }
                else
                {
                    tbTam.Enabled = false;
                }
            }
        }

        private void ColocarAtributosCombox()
        {
            cbAtributoActual.Items.Clear();
            LinkedListNode<Entidad> entidad = diccionario.BuscaEntidad(cbEntidadAtributo.Text);
            List<Atributo> tmp = entidad.Value.atributos.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                cbAtributoActual.Items.Add(tmp[i].nombre);
            }
        }

        private void cbEntidadAtributo_SelectedIndexChanged(object sender, EventArgs e)
        {
            MuestraAtributos();
            ColocarAtributosCombox();
        }

        private void cbTipoLlave_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTipoLlave.SelectedIndex == 2)
            {
                panelLlaveForanea.Enabled = true;
                ColocaEntidadesExternas();
            }
            else
            {
                panelLlaveForanea.Enabled = false;
            }
        }

        private void ColocaEntidadesExternas()
        {
            cbEntidadForanea.Items.Clear();
            List<Entidad> tmp = diccionario.entidades.ToList();
            for (int i = 0; i < tmp.Count; i++)
            {
                if (cbEntidadAtributo.Text.Equals(tmp[i].nombre) == false)
                {
                    cbEntidadForanea.Items.Add(tmp[i].nombre);
                }
                
            }
        }
        
        private void cbEntidadRelaciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            MuestraEntidadesPadres();
            MuestraEntidadForanea();
            MuestraHijos();
        }

        private void cbEntidadRegistros_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColocaCapturaRegistro();
            ColocaColumnasRegistro();
            MuestraRegistros();
            LLenaComboxLlavesPrimarias();
        }

        private void LlenaComboxForanea(DataGridViewComboBoxColumn column , Atributo atr)
        {
            //column.Items.Clear();
            ///column.ReadOnly = true;
            Entidad ent = diccionario.BuscaEntidadDireccion(atr.padre).Value;
            List<Registro> r = ent.registros.ToList();
            for (int i = 0; i < r.Count; i++)
            {
                column.Items.Add(r[i].primaria.ToString());
            }
            
        }

        private void LLenaComboxLlavesPrimarias()
        {
            cbLLavePrimaria.Items.Clear();
            Entidad ent = diccionario.BuscaEntidad(cbEntidadRegistros.Text).Value;
            List<Registro> r = ent.registros.ToList();
            for (int i = 0; i < r.Count; i++)
            {
                cbLLavePrimaria.Items.Add(r[i].primaria.ToString());
            }
        }
        #endregion

        /****************************************
        *                                       * 
        *         Métodos auxiliares            *
        *                                       *
        ****************************************/
        #region 
        private bool CampoVacio(Control c)
        {
            bool flag = false;
            if (c is TextBox || c is ComboBox)
            {
                if (string.IsNullOrEmpty(c.Text) == true || string.IsNullOrWhiteSpace(c.Text) == true)
                {
                    errorProvider.SetError(c, "Campo obligatorio");
                    flag = true;
                }
                else
                {
                    errorProvider.Clear();
                }

            }
            return flag;
        }

        private void OnlyNumbers(KeyPressEventArgs V)
        {
            if (char.IsNumber(V.KeyChar) || char.IsControl(V.KeyChar))
            {
                V.Handled = false;
            }
            else
            {
                V.Handled = true;
            }

        }

        private void TbTam_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnlyNumbers(e);
        }

        private bool VerificaDatosCapturaRegistro(List<object> valores)
        {
            bool flag = false;
            int a;
            decimal b;
            List<Atributo> atrs = diccionario.BuscaEntidad(cbEntidadRegistros.Text).Value.atributos.ToList();
            for (int i = 0; i < atrs.Count; i++)
            {
                if (atrs[i].tipo == 'E')
                {
                    if (dataGridViewCapturaRegistro.Rows[0].Cells[i].Value != null)
                    {
                        bool esEntero = int.TryParse(dataGridViewCapturaRegistro.Rows[0].Cells[i].Value.ToString(), out a);
                        if (esEntero == false)
                        {
                            MessageBox.Show(dataGridViewCapturaRegistro.Columns[i].HeaderText + "  No es un valor entero");
                            flag = false;
                            break;
                        }
                        else
                        {
                            valores.Add(a);
                            flag = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(dataGridViewCapturaRegistro.Columns[i].HeaderText + "  Vacio");
                        flag = false;
                        break;
                    }
                }
                else if (atrs[i].tipo == 'D')
                {
                    if (dataGridViewCapturaRegistro.Rows[0].Cells[i].Value != null)
                    {
                        bool esEntero = decimal.TryParse(dataGridViewCapturaRegistro.Rows[0].Cells[i].Value.ToString(), out b);
                        if (esEntero == false)
                        {
                            MessageBox.Show(dataGridViewCapturaRegistro.Columns[i].HeaderText + "  No es un valor decimal");
                            flag = false;
                            break;
                        }
                        else
                        {
                            valores.Add(b);
                            flag = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(dataGridViewCapturaRegistro.Columns[i].HeaderText + "  Vacio");
                        flag = false;
                        break;
                    }
                }
                else
                {
                    if (dataGridViewCapturaRegistro.Rows[0].Cells[i].Value != null)
                    {
                        if (dataGridViewCapturaRegistro.Rows[0].Cells[i].Value.ToString().Length <= atrs[i].tam)
                        {
                            valores.Add(dataGridViewCapturaRegistro.Rows[0].Cells[i].Value.ToString());
                            flag = true;
                        }
                        else
                        {
                            MessageBox.Show(dataGridViewCapturaRegistro.Columns[i].HeaderText + "  La cadena sobrepasa el valor especificado");
                            flag = false;
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show(dataGridViewCapturaRegistro.Columns[i].HeaderText + "  Vacio");
                        flag = false;
                        break;
                    }

                }
            }
            return flag;
        }
        #endregion

        /****************************************
        *                                       * 
        *         Ménu Strip                    *
        *                                       *
        ****************************************/
        #region 
        private void guardarNuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diccionario.GuardaNuevoDiccionario();
            ubicacionDiccionario.Text = diccionario.rutaDiccionario;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diccionario.AbreDiccionario();
            MuestraEntidades();
            ColocaEntidadesCombox();
            ubicacionDiccionario.Text = diccionario.rutaDiccionario;
            labelEncabezado.Text = diccionario.getEncabezado().ToString();
            compilador = new CompiladorSQL(diccionario.entidades);
        }

        private void guardarCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diccionario.GuardaCambiosDiccionario();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            diccionario.LimpiarDicionario();
            MuestraEntidades();
            labelEncabezado.Text = "-1";
            cbEntidadAtributo.Items.Clear();
            cbEntidadNombre.Items.Clear();
            cbEntidadRelaciones.Items.Clear();
            cbEntidadRegistros.Items.Clear();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("¿Deseas guardar el diccionario actual antes de salir? ",
                     "Guardar cambios", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                diccionario.GuardaCambiosDiccionario();
            }

            this.Close();
        }
        #endregion


        /****************************************
        *                                       * 
        *         Registros                     *
        *                                       *
        ****************************************/
        #region 
        private void btAgregaRegistro_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                int res = 0;
                bool flag = CampoVacio(cbEntidadRegistros);
                if (flag == false)
                {
                    List<object> val = new List<object>();
                    bool succes = VerificaDatosCapturaRegistro(val);
                    if (succes)
                    {

                        res = diccionario.AgregaRegistro(cbEntidadRegistros.Text, val);
                        switch (res)
                        {
                            case 1:
                                MessageBox.Show("Registro agregado");
                                MuestraEntidades();
                                MuestraRegistros();
                                LimpiaCapturaRegistro();
                                LLenaComboxLlavesPrimarias();
                                break;
                            case 2:
                                MessageBox.Show("Clave ya existente");
                                break;
                            default:
                                MessageBox.Show("Error al agregar el registro");
                                //MessageBox.Show(res.ToString());
                                break;
                        }
                    }
                }
            }
        }

        private void LimpiaCapturaRegistro()
        {
            dataGridViewCapturaRegistro.Rows.Clear();
            dataGridViewCapturaRegistro.Rows.Add();
        }

        private void btEjecutaConsulta_Click(object sender, EventArgs e)
        {
            compilador.RecibeCadena(TextB_Consulta.Text, dataGridConsulta, diccionario.entidades.ToList());
        }

        private void btEliminarRegistros_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                int res = 0;
                bool flag = CampoVacio(cbEntidadRegistros) || CampoVacio(cbLLavePrimaria);
                if (flag == false)
                {
                    res = diccionario.EliminaRegistro(cbEntidadRegistros.Text, Convert.ToInt32(cbLLavePrimaria.Text));
                    switch (res)
                    {
                        case 1:
                            MuestraEntidades();
                            MuestraRegistros();
                            cbLLavePrimaria.Text = "";
                            MessageBox.Show("Registro eliminado");
                            LLenaComboxLlavesPrimarias();
                            break;
                        default:
                            MessageBox.Show("Error: No se pudó eliminar el regisro");
                            break;
                    }
                }
            }
        }

        private void dataGridViewRegistros_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            int FilaSeleccionada = e.RowIndex;
            if (FilaSeleccionada >= 0)
            {
                string valor = dataGridViewRegistros.Rows[FilaSeleccionada].Cells[0].Value.ToString();
                Registro r = diccionario.BuscaEntidad(cbEntidadRegistros.Text).Value.BuscaRegistro(Convert.ToInt32(valor));
                dataGridViewCapturaRegistro.Rows.Clear();
                dataGridViewCapturaRegistro.Rows.Add(r.getDatosRegistroSinDirecciones().ToArray());
                cbLLavePrimaria.Text = valor;
            }

        }

        private void btModificaRegistro_Click(object sender, EventArgs e)
        {
            int res = 0;
            bool flag = CampoVacio(cbEntidadRegistros) || CampoVacio(cbLLavePrimaria);
            if (flag == false)
            {
                List<object> val = new List<object>();
                bool succes = VerificaDatosCapturaRegistro(val);
                if (succes)
                {

                    if (checkBox1.Checked == false)
                    {
                        res = diccionario.ModificaRegistro(cbEntidadRegistros.Text, Convert.ToInt32(cbLLavePrimaria.Text), val);
                    }
                    else
                    {
                        res = diccionario.ModificaRegistroModificandoPadres(cbEntidadRegistros.Text, Convert.ToInt32(cbLLavePrimaria.Text), val);
                    }
                    switch (res)
                    {
                        case 1:
                            MessageBox.Show("Registro modificado");
                            MuestraEntidades();
                            MuestraRegistros();
                            LimpiaCapturaRegistro();
                            cbLLavePrimaria.Text = "";
                            LLenaComboxLlavesPrimarias();
                            break;
                        case 2:
                            MessageBox.Show("Clave ya existente");
                            break;
                        case 3:
                            MessageBox.Show("Registro modificado");
                            MuestraEntidades();
                            MuestraRegistros();
                            LimpiaCapturaRegistro();
                            cbLLavePrimaria.Text = "";
                            LLenaComboxLlavesPrimarias();
                            break;
                        default:
                            MessageBox.Show("Error al modicar registro el registro");
                            //MessageBox.Show(res.ToString());
                            break;
                    }
                }
            }
        }



        #endregion

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ColocaCapturaRegistro();
        }
    }
}
