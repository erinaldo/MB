﻿using Comercial.Dominio.Entidades;
using Comercial.Dominio.Geral;
using Comercial.Dominio.Interfaces.Repositorio;
using Comercial.Dominio.Interfaces.Servico;
using System;
using System.Collections.Generic;
using System.Linq;
using static Comercial.Dominio.Enumeradores.Enumerador;

namespace Comercial.Dominio.Servicos
{
    public class ServicoCarga : ServicoBase<Carga>, IServicoCarga
    {
        private readonly string _tabela;
        private readonly IRepositorioCarga _repositorioCarga;
        private readonly IServicoPermissao _servicoPermissao;
        private readonly IRepositorioCargaConsulta _repositorioCargaConsulta;
        private readonly IRepositorioPedido _repositorioPedido;
        private readonly IRepositorioCliente _repositorioCliente;
        private readonly IRepositorioFornecedor _repositorioFornecedor;
        private readonly IServicoConta _servicoConta;
        private readonly IServicoCadObs _servicoCadObs;
        private readonly IServicoCargaVencimento _servicoCargaVencimento;

        public ServicoCarga(IRepositorioCarga repositorioCarga,
            IRepositorioCargaConsulta repositorioCargaConsulta, IServicoPermissao servicoPermissao,
            IRepositorioPedido repositorioPedido, IRepositorioCliente repositorioCliente, IRepositorioFornecedor repositorioFornecedor,
            IServicoConta servicoConta, IServicoCadObs servicoCadObs, IServicoCargaVencimento servicoCargaVencimento)
            :base(repositorioCarga)
        {
            _repositorioCarga = repositorioCarga;
            _repositorioCargaConsulta = repositorioCargaConsulta;
            _servicoPermissao = servicoPermissao;
            _repositorioPedido = repositorioPedido;
            _repositorioCliente = repositorioCliente;
            _repositorioFornecedor = repositorioFornecedor;
            _servicoConta = servicoConta;
            _servicoCadObs = servicoCadObs;
            _servicoCargaVencimento = servicoCargaVencimento;
            _tabela = "CARGA";
        }

        public Carga ObterPorId(int id)
        {
            return _repositorioCarga.GetById(id);
        }

        public void Excluir(Carga carga, string nomeUsuario)
        {
            try
            {
                _servicoPermissao.Permitir(AcaoUsuario.Excluir, _tabela, nomeUsuario);
                _repositorioCarga.Delete(carga);
                _servicoConta.ExcluirParcelas(carga.Id_Carga, carga.Cod_Empresa);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int Salvar(Carga carga, string nomeUsuario)
        {
            if (carga.Cod_Cliente == 0)
                throw new Exception("O cliente é obrigatório!");
            if (carga.Cod_For == 0)
                throw new Exception("O fornecedor é obrigatório!");
            if (carga.Num_Pedido == 0)
                throw new Exception("O número do pedido é obrigatório!");
            if (carga.Num_Carga <= 0)
                throw new Exception("O número da carga é obrigatório!");
            if (carga.Valor_Frete <= 0)
                throw new Exception("O valor do frete é obrigatório!");
            if (carga.Qtde <= 0)
                throw new Exception("A quantidade é obrigatório!");
            if (carga.Cod_Contato == 0)
                throw new Exception("O contato é obrigatório!");

            if (_repositorioCarga.GetList(x => x.Cod_Empresa == carga.Cod_Empresa && x.Num_Carga == carga.Num_Carga &&
                x.Letra == carga.Letra && x.Id_Carga != carga.Id_Carga).Any())
                throw new Exception("Número da carga ja existe, escolha outro número");

            bool incluindo = (carga.Id_Carga == 0);

            if (carga.Id_Carga > 0)
            {
                if (ParcelaCargaIguais(carga) == false)
                    throw new Exception("Valor da carga difere do valor das parcelas. Verifique as parcelas!");
            }

            CalcularLucro(carga, true);

            try
            {
                if (carga.Id_Carga == 0)
                {
                    _servicoPermissao.Permitir(AcaoUsuario.Incluir, _tabela, nomeUsuario);
                    carga.Usu_Inc = PermissaoUsuario.GravarUsuarioDataHora(nomeUsuario);
                    _repositorioCarga.Insert(ref carga);
                }
                else
                {
                    _servicoPermissao.Permitir(AcaoUsuario.Alterar, _tabela, nomeUsuario);
                    carga.Usu_Alt = PermissaoUsuario.GravarUsuarioDataHora(nomeUsuario);
                    _repositorioCarga.Update(carga);
                }

                if (carga.Situacao == "C") // se for cancelado
                    _servicoConta.ExcluirParcelas(carga.Id_Carga, carga.Cod_Empresa);
                else
                {
                    if (incluindo)
                    {
                        MontarParcelas(carga);
                    }
                    else
                    {
                        VerificarParcelasCargaClienteDiferente(carga, nomeUsuario);
                    }
                }

                SalvarVencimentos(carga);
                SalvarObservacao(carga);

                // TODO: ver saldo
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return carga.Id_Carga;
        }

        private void SalvarVencimentos(Carga carga)
        {
            var ListaBanco = _servicoCargaVencimento.ListarVencimentosCarga(carga.Id_Carga);
            if (ListaBanco != null)
            {
                var vencimento = new CargaVencimento();
                foreach (var item in ListaBanco)
                {
                    if (carga.CargaVencimentos != null)
                    {
                        vencimento = carga.CargaVencimentos.FirstOrDefault(x => x.Id == item.Id);
                        if (vencimento != null && vencimento.Id > 0)
                        {
                            _servicoCargaVencimento.Alterar(vencimento);
                        }
                        else
                        {
                            _servicoCargaVencimento.Excluir(item);
                        }
                    };
                }

                if (carga.CargaVencimentos != null)
                {
                    foreach (var itemLocal in carga.CargaVencimentos)
                    {
                        if (itemLocal.Id < 0)
                            _servicoCargaVencimento.Inserir(itemLocal);
                    }
                }
            }
        }

        private void SalvarObservacao(Carga carga)
        {
            if (carga.Observacoes != null)
            {
                var listaObs = _servicoCadObs.ObterPorCodigoCarga(carga.Id_Carga, carga.Cod_Empresa);
                if (listaObs != null)
                {
                    var model = new CadObs();
                    foreach (var obs in listaObs)
                    {
                        model = _servicoCadObs.ObterUmRegistroCarga(obs.Codigo, obs.Num_Linha, obs.CodEmpresa);
                        _servicoCadObs.Excluir(model);
                    }
                }
            }

            carga.Observacoes.Clear();
            int i = 0;
            foreach (var item in carga.Observacoes)
            {
                item.Codigo = carga.Id_Carga;
                item.Tipo = "CAR";
                item.Num_Linha = i;
                item.CodEmpresa = carga.Cod_Empresa;
                item.Texto = item.Texto;
                carga.Observacoes.Add(item);
                _servicoCadObs.InserirCarga(item);
                i++;
            }
        }

        private bool ParcelaCargaIguais(Carga carga)
        {
            // tipo C=Cliente P=Pedido
            bool resultado = true;
            if (carga.Situacao != "C") // se nao for cancelado
            {
                var contas = _servicoConta.ObterPorPedido(carga.Id_Carga, carga.Cod_Empresa);
                if (contas != null)
                {
                    decimal valorContas = contas.Sum(x => x.Valor_Pagar);
                    decimal valorCarga = carga.Valor_Carrega;

                    string sValorContas = valorContas.ToString("N2");
                    string sValorCarga = valorCarga.ToString("N2");

                    if (sValorCarga != sValorContas)
                        resultado = false;
                }
            }
            return resultado;
        }

        private void VerificarParcelasCargaClienteDiferente(Carga carga, string nomeUsuario)
        {
            if (carga.Cod_Contato != null && carga.Cod_Contato > 0)
            {
                var contas = _servicoConta.ObterPorPedido(carga.Id_Carga, carga.Cod_Empresa);
                if (contas != null)
                {
                    foreach (var item in contas)
                    {
                        if (item.Cod_Cliente != carga.Cod_Contato)
                        {
                            item.Cod_Cliente = carga.Cod_Contato;
                            _servicoConta.Salvar(item, null, nomeUsuario);
                        }
                    }
                }
            }
        }

        public IEnumerable<CargaConsulta> Filtrar(string campo, string valor, int codEmpresa, CargaFiltro cargaFiltro, int id = 0)
        {
            return _repositorioCargaConsulta.Filtrar(campo, valor, codEmpresa, cargaFiltro, id);
        }

        public void BuscarDadosPedido(Carga carga, int idPedido, bool incluindo)
        {
            var pedido = new Pedido();
            pedido = _repositorioPedido.First(x => x.Cod_Empresa == carga.Cod_Empresa && x.Num_Pedido == idPedido);
            if (pedido == null)
                throw new Exception("Pedido não encontrado!");

            carga.Num_Pedido = pedido.Num_Pedido;
            carga.Qtde_Pedido = pedido.Total_Qtde;
            carga.Valor_Pedido = pedido.Total_Liquido;
            carga.Cod_For = pedido.Cod_For;
            carga.Cod_Contato = pedido.Cod_Contato;

            if (pedido.Cod_For > 0)
                carga.Fornecedor = _repositorioFornecedor.GetById(pedido.Cod_For);

            if (pedido.Cod_Cliente > 0)
                carga.Cliente = _repositorioCliente.GetById(pedido.Cod_Cliente);

            if (pedido.Cod_Contato != null)
                carga.Contato = _repositorioCliente.GetById(pedido.Cod_Contato.Value);

            if (pedido.Cod_Usina != null)
                carga.Usina = _repositorioFornecedor.GetById(pedido.Cod_Usina.Value);

            if (carga.Cod_Cliente != carga.Cod_Contato)
                carga.Visto = "NF";
            else
                carga.Visto = "";

            PesquisarLetra(carga);
            CalcularSaldoCarga(carga, incluindo);
            // calcular lucro (chamar na tela para mostrar a diferenca)
            // montar contas ?? ver se no salvar
        }

        private void PesquisarLetra(Carga carga)
        {
            if (carga.Cod_Motorista != null && carga.Num_Pedido > 0)
            {
                int qtde = 0;
                var CargasMotorista = _repositorioCarga.ObterCargasDoMotorista(carga.Cod_Empresa, carga.Data,
                    carga.Cod_Motorista.Value);

                if (CargasMotorista == null && CargasMotorista.Num_Carga == 0)
                {
                    var ultimaCarga = _repositorioCarga.ObterUltimaCargaDaData(carga.Cod_Empresa, carga.Data);
                    if (ultimaCarga != null)
                    {
                        string dia = carga.Data.Day.ToString();
                        string mes = carga.Data.Month.ToString();
                        string ano = carga.Data.Year.ToString();
                        qtde = ultimaCarga.Num_Carga + 1;
                        string sequencia = dia + mes + ano;

                        if (ultimaCarga.Num_Carga == 0)
                            carga.Num_Carga = Convert.ToInt32(sequencia + qtde.ToString());
                        else
                            carga.Num_Carga = qtde;
                    }
                    else
                    {
                        carga.Num_Carga = 1;
                    }
                    carga.Letra = "";
                }
                else
                {
                    qtde = (int)CargasMotorista.Qtde;
                    carga.Letra = "";
                    switch (qtde)
                    {
                        case 2:
                            carga.Letra = "A";
                            break;
                        case 3:
                            carga.Letra = "B";
                            break;
                        case 4:
                            carga.Letra = "C";
                            break;
                        case 5:
                            carga.Letra = "D";
                            break;
                        case 6:
                            carga.Letra = "E";
                            break;
                        case 7:
                            carga.Letra = "F";
                            break;
                        case 8:
                            carga.Letra = "G";
                            break;
                        case 9:
                            carga.Letra = "H";
                            break;
                        case 10:
                            carga.Letra = "I";
                            break;
                        case 11:
                            carga.Letra = "J";
                            break;
                        case 12:
                            carga.Letra = "K";
                            break;
                        case 13:
                            carga.Letra = "L";
                            break;
                        case 14:
                            carga.Letra = "M";
                            break;
                        case 15:
                            carga.Letra = "N";
                            break;
                    }
                }
            }
        }

        private void CalcularSaldoCarga(Carga carga, bool incluindo)
        {
            decimal saldo = _repositorioCarga.ObterSaldoCarga(carga.Cod_Empresa, carga.Id_Carga, carga.Num_Pedido);
            if (incluindo)
            {
                if (carga.Qtde == 0)
                    carga.Qtde = carga.Qtde_Pedido - saldo;
            }
            carga.Saldo = carga.Qtde_Pedido - saldo - carga.Qtde;
        }

        public decimal CalcularLucro(Carga carga, bool calcularTudo)
        {
            if (calcularTudo)
            {
                carga.Valor_Carrega = (carga.Pedido.Valor_Lucro * carga.Qtde) - carga.Valor_Frete;
            }
            // ValorDif = return mostrar na tela
            return carga.Pedido.Valor_Lucro;
        }

        private void MontarParcelas(Carga carga)
        {
            if (carga.Num_Pedido == 0)
                throw new Exception("Informe o número do pedido.");

            var contas = _servicoConta.ObterPorPedido(carga.Num_Pedido, carga.Cod_Empresa);
            int qtdeParcelas = contas.Count();

            if (qtdeParcelas == 0)
                qtdeParcelas = 1;

            decimal valorBase = carga.Pedido.Total_Liquido * carga.Qtde;
            decimal valorPrimeira = 0;
            decimal valorOutras = 0;
            //TODO: tirar comentarios
            //var financeiro = new Financeiro();
            //financeiro.CalcularParcelas(valorBase, ref valorPrimeira, ref valorOutras, qtdeParcelas);

            int contador = 1;
            foreach (var item in contas)
            {
                var parcela = new CargaVencimento();
                parcela.Carga_Id = carga.Id_Carga;
                parcela.FormaPagto_Id = item.Cod_Pagto;
                parcela.Dias = item.Dias;

                try
                {
                    item.Data_Vencto = carga.Data_NF.Value.AddDays(item.Dias.Value);
                }
                catch
                {
                    item.Data_Vencto = item.Data_Vencto;
                }

                if (contador == 1)
                    parcela.Valor = valorPrimeira;
                else
                    parcela.Valor = valorOutras;

                carga.CargaVencimentos.Add(parcela);
                contador++;
            }
        }

        public IEnumerable<Carga> ObterCargasDoPedido(int numPedido)
        {
            return _repositorioCarga.ObterCargasDoPedido(numPedido);
        }

        public Carga ObterPorPedido(int numPedido)
        {
            return _repositorioCarga.ObterPorPedido(numPedido);
        }
    }
}
