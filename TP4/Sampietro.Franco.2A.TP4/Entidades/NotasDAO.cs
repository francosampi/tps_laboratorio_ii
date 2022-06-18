﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entidades
{
    public class NotasDAO
    {
        public string cadenaConexion;
        public SqlConnection conexion;
        public SqlCommand comando;
        public SqlDataReader lector;

        public NotasDAO(string cadenaConexion)
        {
            this.cadenaConexion = cadenaConexion;
            this.conexion = new SqlConnection(this.cadenaConexion);
        }

        /// <summary>
        /// Restaura la tabla en la base de datos con los cambios del datagrid
        /// </summary>
        /// <returns></returns>
        public bool GuardarEnBD(DataGridView dg)
        {
            DialogResult dialogResult = MessageBox.Show("Agregar alumno?", "Agregar alumno a la base de datos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    this.comando.CommandText = "INSERT INTO notas VALUES " +
                        "(@alumno, @programaciondevideojuegos, @dibujodecomics, @disenografico, @disenoblender, @programacionweb)";
                    this.comando.Connection = this.conexion;
                    this.conexion.Open();

                    foreach (DataGridViewRow row in dg.Rows)
                    {
                        this.comando.Parameters.Clear();

                        this.comando.Parameters.AddWithValue("@alumno", Convert.ToString(row.Cells["AddAlumno"].Value).Trim());
                        this.comando.Parameters.AddWithValue("@programaciondevideojuegos", Convert.ToString(row.Cells["addProgramacionDeVideojuegos"].Value).Trim());
                        this.comando.Parameters.AddWithValue("@dibujodecomics", Convert.ToString(row.Cells["addDibujoDeComics"].Value).Trim());
                        this.comando.Parameters.AddWithValue("@disenografico", Convert.ToString(row.Cells["addDisenoGrafico"].Value).Trim());
                        this.comando.Parameters.AddWithValue("@disenoblender", Convert.ToString(row.Cells["addDisenoEnBlender"].Value).Trim());
                        this.comando.Parameters.AddWithValue("@programacionweb", Convert.ToString(row.Cells["addProgramacionWeb"].Value).Trim());

                        if (this.comando.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("Sin columnas afectadas", "Ouch!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ocurrio un error: " + e.Message, "Ouch!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                finally
                {
                    if (this.conexion.State == ConnectionState.Open)
                    {
                        this.conexion.Close();
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Trae los datos de la base y los muestra en el datagrid
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        public async Task<List<string[]>> LeerEnBD()
        {
            List<string[]> listaFilas = new List<string[]>();

            await Task.Run(() =>
            {
                try
                {
                    this.comando = new SqlCommand();
                    this.comando.CommandText = "SELECT * FROM notas";
                    this.comando.Connection = this.conexion;
                    this.conexion.Open();

                    this.lector = this.comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int id = lector.GetInt32(0);
                        string alumno = lector.GetString(1);
                        int programaciondevideojuegos = lector.GetInt32(2);
                        int dibujodecomics = lector.GetInt32(3);
                        int disenografico = lector.GetInt32(3);
                        int disenoblender = lector.GetInt32(4);
                        int programacionweb = lector.GetInt32(5);

                        string[] row = {
                            id.ToString(),
                            alumno,
                            programaciondevideojuegos.ToString(),
                            dibujodecomics.ToString(),
                            disenografico.ToString(),
                            disenoblender.ToString(), 
                            programacionweb.ToString()
                        };
                        //(id, alumno, programaciondevideojuegos, dibujodecomics, disenografico, disenoblender, programacionweb);

                        listaFilas.Add(row);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ocurrio un error: " + e.Message, "Ouch!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                finally
                {
                    if (this.conexion.State == ConnectionState.Open)
                    {
                        this.conexion.Close();
                    }
                }
                return true;
            });

            return listaFilas;
        }

        public bool BorrarDeDB(string id)
        {
            DialogResult dialogResult = MessageBox.Show("Eliminar alumno en id "+id, "Borrar alumno de base de datos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    this.comando.CommandText = "DELETE from notas WHERE id in (" + id+")";
                    this.comando.Connection = this.conexion;
                    this.conexion.Open();

                    if (this.comando.ExecuteNonQuery() == 0)
                    {
                        MessageBox.Show("Sin columnas afectadas", "Ouch!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al eliminar alumno en id " + id, "Ouch!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                finally
                {
                    if (this.conexion.State == ConnectionState.Open)
                    {
                        this.conexion.Close();
                    }
                }
            }
            return true;
        }
    }
}
