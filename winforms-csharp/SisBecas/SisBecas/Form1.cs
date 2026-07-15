using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; // Librería necesaria para usar expresiones regulares (validar el correo)
using System.Windows.Forms;

namespace SisBecas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        { 
            CargarTiposBeca(); // Llena el ComboBox con las becas 
            RestablecerCampos(); // Limpia los campos

        }
        
        private void CargarTiposBeca()
        {
            try
            {
                // Recupera los tipos de beca desde la clase auxiliar de conexión
                DataTable dt = DbHelper.ObtenerTiposBeca();

                // Creamos una columna temporal para mostrar de forma personalizada el nombre de la beca junto a su monto
                dt.Columns.Add("display_name", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["monto_mensual"]);
                    row["display_name"] = $"{row["nombre"]} — ${monto:0.##}/mes";
                }

                // Inserta una opción por defecto para obligar al usuario a seleccionar una beca
                DataRow newRow = dt.NewRow();
                newRow["id_tipo"] = DBNull.Value;
                newRow["display_name"] = "-- Seleccione --";
                dt.Rows.InsertAt(newRow, 0);

                // Enlaza los datos procesados al ComboBox de la interfaz
                cmbTipoBeca.DataSource = dt;
                cmbTipoBeca.DisplayMember = "display_name"; 
                cmbTipoBeca.ValueMember = "id_tipo";      
                cmbTipoBeca.SelectedIndex = 0;             
            }
            catch (Exception ex)
            {
                // Muestra un mensaje en caso de que ocurra un error al conectar con la base de datos
                MessageBox.Show($"Error al cargar becas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Extrae y limpia los espacios en blanco de todos los campos de texto
            string dui = txtDUI.Text.Trim();
            string nombres = txtNombres.Text.Trim();
            string apellidos = txtApellidos.Text.Trim();
            DateTime fechaNac = dtpFechaNac.Value;
            string sexo = rbFemenino.Checked ? "F" : (rbMasculino.Checked ? "M" : "");
            string telefono = txtTelefono.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string institucion = txtInstitucion.Text.Trim();
            string strPromedio = txtPromedio.Text.Trim();
            string strIngreso = txtIngreso.Text.Trim();

            // 1. Validar que el DUI esté completo según la máscara (00000000-0)
            if (!txtDUI.MaskCompleted)
            {
                MessageBox.Show("DUI incompleto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDUI.Focus();
                return;
            }

            // 2. Validar que el nombre y apellido no estén vacíos
            if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
            {
                MessageBox.Show("Nombre y Apellidos son requeridos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Validar que se haya seleccionado un sexo
            if (string.IsNullOrEmpty(sexo))
            {
                MessageBox.Show("Seleccione el sexo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Validar formato de correo electrónico mediante Expresiones Regulares (Regex)
            if (string.IsNullOrEmpty(correo) || !Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Correo electrónico inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. Validar que el promedio sea un número decimal válido y esté en el rango de 0 a 10
            if (!decimal.TryParse(strPromedio, out decimal promedio) || promedio < 0 || promedio > 10)
            {
                MessageBox.Show("Promedio debe estar entre 0.00 y 10.00", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 6. Validar que el ingreso familiar sea un número decimal coherente
            if (!decimal.TryParse(strIngreso, out decimal ingreso) || ingreso < 0)
            {
                MessageBox.Show("Ingreso familiar inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 7. Validar que se haya seleccionado un tipo de beca real
            if (cmbTipoBeca.SelectedValue == DBNull.Value || cmbTipoBeca.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un tipo de beca.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int idTipo = Convert.ToInt32(cmbTipoBeca.SelectedValue);

            try
            {
                // Comprueba si el DUI ya está registrado en la base de datos
                if (DbHelper.ExisteDUI(dui))
                {
                    MessageBox.Show("Este aspirante ya existe en el sistema.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Intenta insertar el nuevo registro del aspirante
                if (DbHelper.InsertarAspirante(dui, nombres, apellidos, fechaNac, sexo, telefono, correo, institucion, promedio, ingreso, idTipo))
                {
                    MessageBox.Show("Inscripción guardada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RestablecerCampos(); // Limpia los campos para un nuevo ingreso
                }
            }
            catch (Exception ex)
            {
                // Atrapa y muestra cualquier error inesperado del sistema
                MessageBox.Show($"Error: {ex.Message}", "Error de Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            RestablecerCampos(); // Llama al método de limpieza
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Solicita confirmación antes de cerrar la ventana
            if (MessageBox.Show("¿Desea salir?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // Método auxiliar para resetear el estado de todos los controles visuales
        private void RestablecerCampos()
        {
            txtDUI.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            dtpFechaNac.Value = DateTime.Now.AddYears(-18);
            rbFemenino.Checked = false;
            rbMasculino.Checked = false;
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtInstitucion.Clear();
            txtPromedio.Clear();
            txtIngreso.Clear();
            if (cmbTipoBeca.Items.Count > 0) cmbTipoBeca.SelectedIndex = 0;
        }

        // Evento del botón "Nuevo"
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RestablecerCampos();
            txtDUI.Focus();
        }

        // Evento del botón "Guardar"
        private void tsbGuardar_Click(object sender, EventArgs e)
        {
            btnGuardar_Click(sender, e);
        }

        // Evento del botón "Buscar"
        private void tsbBuscar_Click(object sender, EventArgs e)
        {
            string duiABuscar = txtDUI.Text.Trim();

            if (!txtDUI.MaskCompleted)
            {
                MessageBox.Show("Por favor, escribe un DUI completo para buscar.", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDUI.Focus();
                return;
            }

            try
            {
                DataTable dt = DbHelper.ObtenerAspirantePorDUI(duiABuscar);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtNombres.Text = row["nombres"].ToString();
                    txtApellidos.Text = row["apellidos"].ToString();
                    dtpFechaNac.Value = Convert.ToDateTime(row["fecha_nacimiento"]);

                    string sexo = row["sexo"].ToString();
                    rbFemenino.Checked = (sexo == "F");
                    rbMasculino.Checked = (sexo == "M");

                    txtTelefono.Text = row["telefono"].ToString();
                    txtCorreo.Text = row["correo"].ToString();
                    txtInstitucion.Text = row["institucion_estudio"].ToString();
                    txtPromedio.Text = row["promedio"].ToString();
                    txtIngreso.Text = row["ingreso_familiar"].ToString();
                    cmbTipoBeca.SelectedValue = row["id_tipo"];

                    MessageBox.Show("Aspirante encontrado y cargado con éxito.", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontró ningún aspirante con ese DUI.", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón "Imprimir"
        private void tsbImprimir_Click(object sender, EventArgs e)
        {
            System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(this.Width, this.Height);
                this.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, this.Width, this.Height));
                ev.Graphics.DrawImage(bmp, 0, 0);
            };

            PrintDialog pdlg = new PrintDialog();
            pdlg.Document = pd;
            if (pdlg.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        // Evento del botón "Actualizar"
        private void tsbRefrescar_Click(object sender, EventArgs e)
        {
            CargarTiposBeca();
            MessageBox.Show("Tipos de beca actualizados desde la base de datos.", "Refrescar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Evento del botón "Cerrar"
        private void tsbCerrar_Click(object sender, EventArgs e)
        {
            btnCancelar_Click(sender, e);
        }
    }
}
