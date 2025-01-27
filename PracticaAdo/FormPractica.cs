using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PracticaAdo.Models;
using PracticaAdo.Repository;

#region STORED PROCEDURES

    //create procedure SP_ALL_CLIENTES
    //as
    // select Empresa from clientes
    //go

    //create procedure SP_DATOS_CLIENTE
    //(@empresa nvarchar(50))
    //as
    //	declare @codEmpresa nvarchar(50)
    //	SELECT @codEmpresa = CodigoCliente FROM clientes WHERE Empresa = @empresa
    //	SELECT cli.Empresa, cli.Contacto, cli.Cargo, cli.Ciudad, cli.Telefono, ped.CodigoPedido
    //	FROM clientes cli
    //	INNER JOIN pedidos ped
    //	ON cli.CodigoCliente = ped.CodigoCliente
    //	WHERE cli.CodigoCliente = @codEmpresa;
    //go

#endregion

namespace Test
{
    public partial class FormPractica : Form
    {
        RepositoryClientesPedidos repo;
        public FormPractica()
        {
            InitializeComponent();
            this.repo = new RepositoryClientesPedidos();
            this.LoadClientes();
        }

        private async void LoadClientes()
        {
            List<string> clientes = await this.repo.GetClientesAsync();
            foreach (string cliente in clientes)
            {
                this.cmbclientes.Items.Add(cliente);
            }
        }

        private async void lstpedidos_SelectedIndexChanged(object sender, EventArgs e)
        {
            string codigoPedido = this.lstpedidos.SelectedItem.ToString();
            Pedido datos = await this.repo.GetDatosPedidoAsync(codigoPedido);
            this.txtcodigopedido.Text = datos.CodigoPedido;
            this.txtfechaentrega.Text = datos.FechaEntrega;
            this.txtformaenvio.Text = datos.FormaEnvio;
            this.txtimporte.Text = datos.Importe.ToString();
        }

        private async void cmbclientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string empresa = this.cmbclientes.SelectedItem.ToString();
            this.lstpedidos.Items.Clear();
            Cliente datos = await this.repo.GetDatosClienteAsync(empresa);
            this.txtempresa.Text = datos.Empresa;
            this.txtcontacto.Text = datos.Contacto;
            this.txtcargo.Text = datos.Cargo;
            this.txtciudad.Text = datos.Ciudad;
            this.txttelefono.Text = datos.Telefono.ToString();
            foreach (string pedido in datos.Pedidos)
            {
                this.lstpedidos.Items.Add(pedido);
            }
        }

        private async void btnnuevopedido_Click(object sender, EventArgs e)
        {
            int afectados = await this.repo.InsertPedidoAsync(this.txtcodigopedido.Text, this.cmbclientes.SelectedItem.ToString(), this.txtfechaentrega.Text, this.txtformaenvio.Text, this.txtimporte.Text);
            MessageBox.Show("afectados" + afectados);
        }

        private async void btneliminarpedido_Click(object sender, EventArgs e)
        {
            int afectados = await this.repo.DeletePedidoAsync(this.txtcodigopedido.Text);
            MessageBox.Show("afectados" + afectados);
        }
    }
}
