﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Threading;

namespace ElGranPollo
{
    public partial class Control_acceso : Form
    {
        public Control_acceso(string ds,string ds2)
        {
            //---------
            Thread t = new Thread(new ThreadStart(splashtart));
            t.Start();
            Thread.Sleep(5000);
            InitializeComponent();
            t.Abort();
            //---------
            this.ds = ds;
            this.ds2 = ds2;
        }       

        private void splashtart()
        {
            Application.Run(new Form2(ds));
        }

        private void LIMPIAR()
        {
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.Text = "SELECCIONAR";
        }
        
        string ds,ds2,operador;
        int band;              

        int veces = 0;
        private const int intentos = 2;


        //para ingresar
        private void CONECTAR()
        {
            //checamos la encriptacion
            string var1;
            var1 = Encriptado.Encriptar(textBox2.Text);
            //--------               

            OleDbConnection conexion = new OleDbConnection(ds2);
            conexion.Open();
            string select = "SELECT * FROM USUARIOS where USUARIOS.nombre='" + textBox1.Text + "'and USUARIOS.clave='" + var1 + "'and USUARIOS.tipo_usuario='" + comboBox1.Text + "'";
            OleDbCommand cmd6 = new OleDbCommand(select, conexion);
            try
            {
                OleDbDataReader reader = cmd6.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {                        
                        CHECAR2();

                    }
                }
                else
                {
                    if (veces == 2)
                    {
                        MessageBox.Show("Has excedido el limite permitido ", "conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Su Usuario o Contraseña o Tipo NO Coinciden o son Erroneas \n \n                        Le Quedan " + (intentos - veces) + " Intento(s)", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LIMPIAR();
                        veces = veces + 1;
                        textBox1.Focus();
                    }
                }               
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error orden" + ex, "MENSAJE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CHECAR();
        }

        private void CHECAR2()
        {
            if ((comboBox1.Text == "ROOT") || (comboBox1.Text == "ADMINISTRADOR") || (comboBox1.Text == "OPERADOR"))
            {
                textBox1.Focus();
                TIPO();                
            }
            else if ((comboBox1.Text != "ROOT") || (comboBox1.Text != "ADMINISTRADOR") || (comboBox1.Text != "OPERADOR"))
            {
                if (veces == 3)
                {
                    MessageBox.Show("Has excedido el limite permitido ", "conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Su Usuario o Contraseña o Tipo NO Coinciden o son Erroneas \n \n                        Le Quedan " + (intentos - veces) + " Intento(s)", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LIMPIAR();
                    veces = veces + 1;
                    textBox1.Focus();
                }
            }
        }

        private void TIPO()
        {
            if (comboBox1.Text == "ROOT")
            {
                band = 0;
                Users corre = new Users(ds,ds2, band,operador);
                corre.Show();                
                LIMPIAR();
                this.Hide();
            }
            else if (comboBox1.Text == "ADMINISTRADOR")
            {
                band = 1;
                Users corre = new Users(ds,ds2, band,operador);
                corre.usuariosToolStripMenuItem.Visible = false;                
                corre.Show();                
                LIMPIAR();
                this.Hide();
            }
            else if(comboBox1.Text=="OPERADOR")
            {
                operador = textBox1.Text;
                band = 3;                
                              
                LIMPIAR();                

                DateTime fechahoy = DateTime.Now;
                string fecha = fechahoy.ToString("d");

                string var1 = fecha;
                var1 = var1.Substring(0, 2);

                string var2 = fecha;
                var2 = var2.Substring(3, 2);

                string var3 = fecha;
                var3 = var3.Substring(6, 4);

                //juntando las cadenas
                string fechacompleta = string.Concat(var3, var2, var1);
                int fechanum = Convert.ToInt32(fechacompleta);
                try
                {
                    OleDbConnection conexion = new OleDbConnection(ds);

                    conexion.Open();
                    string insertar = "INSERT INTO FECHA (fecha, id) VALUES (@fecha, @id)";
                    OleDbCommand cmd = new OleDbCommand(insertar, conexion);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@id", fechanum);

                    cmd.ExecuteNonQuery();

                    conexion.Close();
                    Pricipal form = new Pricipal(fecha, ds, band);
                    form.Show();
                    this.Close();
                }

                catch (Exception)
                {
                    DialogResult resultado = MessageBox.Show("Ya existe un historial del dia de hoy\n\n      Desea continuar el dia de hoy?", "ADVERTENCIA", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resultado == DialogResult.Yes)
                    {
                        Pricipal form = new Pricipal(fecha, ds, band);
                        form.Show();
                        this.Close();
                    }
                }
            }
        }

        private void CHECAR()
        {
            do
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Ingrese su Nombre para Continuar", "conexion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Focus();
                    return;
                }
                if (textBox2.Text == "")
                {
                    MessageBox.Show("Ingrese su Clave para Continuar", "conexion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox2.Focus();
                    return;
                }
                if (comboBox1.Text == "SELECCIONAR")
                {
                    MessageBox.Show("Seleccione Tipo para Continuar", "conexion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBox1.Focus();
                    return;
                }
                if ((textBox1.Text != "") && (textBox2.Text != "") && (comboBox1.Text != "SELECCIONAR"))
                {
                    CONECTAR();
                    break;
                }                
            } while (true);

        }

        private void Control_acceso_Load(object sender, EventArgs e)
        {           
        }      

        private void button3_Click(object sender, EventArgs e)
        {
            CHECAR();
        }

    }
}
