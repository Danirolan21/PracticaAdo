using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using PracticaAdo.Models;

namespace PracticaAdo.Repository
{
    public class RepositoryClientesPedidos
    {
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryClientesPedidos()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=PRACTICA;Persist Security Info=True;User ID=SA;Trust Server Certificate=True";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        public async Task<List<string>> GetClientesAsync()
        {
            string sql =
                    "SP_ALL_CLIENTES";
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            List<string> clientes = new List<string>();
            this.reader = await this.com.ExecuteReaderAsync();
            while ( await this.reader.ReadAsync() )
            {
                string empresa = this.reader["Empresa"].ToString();
                clientes.Add(empresa);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            return clientes;
        }

        public async Task<Cliente> GetDatosClienteAsync(string empresa)
        {
            string sql = "SP_DATOS_CLIENTE";
            this.com.Parameters.AddWithValue("@empresa", empresa);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            Cliente cliente = new Cliente();
            cliente.Pedidos = new List<string>();
            if (await this.reader.ReadAsync())
            {
                cliente.CodigoCliente = this.reader["Empresa"].ToString();
                cliente.Empresa = this.reader["Empresa"].ToString();
                cliente.Contacto = this.reader["Contacto"].ToString();
                cliente.Cargo = this.reader["Cargo"].ToString();
                cliente.Ciudad = this.reader["Ciudad"].ToString();
                cliente.Telefono = int.Parse(this.reader["Telefono"].ToString());
                cliente.Pedidos.Add(this.reader["CodigoPedido"].ToString());
            }

            while (await this.reader.ReadAsync())
            {
                cliente.Pedidos.Add(this.reader["CodigoPedido"].ToString());
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return cliente;
        }

        public async Task<Pedido> GetDatosPedidoAsync(string codigoPedido)
        {
            string sql = "SP_DATOS_PEDIDO";
            this.com.Parameters.AddWithValue("@CodigoPedido", codigoPedido);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            Pedido pedido = new Pedido();
            if (await this.reader.ReadAsync())
            {
                pedido.CodigoPedido = this.reader["CodigoPedido"].ToString();
                pedido.FechaEntrega = this.reader["FechaEntrega"].ToString();
                pedido.FormaEnvio = this.reader["FormaEnvio"].ToString();
                pedido.Importe = int.Parse(this.reader["Importe"].ToString());
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return pedido;
        }

        public async Task<int> InsertPedidoAsync(string codigoPedido, string codigoCliente, string FechaEntrega, string FormaEnvio, string Importe)
        {
            string sql = "SP_INSERT_PEDIDO";
            this.com.Parameters.AddWithValue("@CodigoPedido", codigoPedido);
            this.com.Parameters.AddWithValue("@codigoCliente", codigoCliente);
            this.com.Parameters.AddWithValue("@FechaEntrega", FechaEntrega);
            this.com.Parameters.AddWithValue("@FormaEnvio", FormaEnvio);
            this.com.Parameters.AddWithValue("@Importe", Importe);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int afectados = await this.com.ExecuteNonQueryAsync();
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            return afectados;
        }
        public async Task<int>DeletePedidoAsync(string codigoPedido)
        {
            string sql = "SP_DELETE_PEDIDO";
            this.com.Parameters.AddWithValue("@CodigoPedido", codigoPedido);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int afectados = await this.com.ExecuteNonQueryAsync();
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            return afectados;
        }
    }
}
