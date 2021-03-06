﻿using Comercial.Dominio.Interfaces.Repositorio;
using Comercial.Dominio.Interfaces.Servico;

namespace Comercial.Dominio.Servicos
{
    public class ServicoTabCodigo : IServicoTabCodigo
    {
        private readonly IRepositorioTabCodigo _repositorioTabCodigo;

        public ServicoTabCodigo(IRepositorioTabCodigo repositorioTabCodigo)
        {
            _repositorioTabCodigo = repositorioTabCodigo;
        }

        public void AtualizaCidade(int codigo)
        {
            _repositorioTabCodigo.AtualizaCidade(codigo);
        }

        public void AtualizaCliente(int codigo)
        {
            _repositorioTabCodigo.AtualizaCliente(codigo);
        }

        public void AtualizaCondicaoPagto(int codigo)
        {
            _repositorioTabCodigo.AtualizaCondicaoPagto(codigo);
        }

        public void AtualizaEmpresa(int codigo)
        {
            _repositorioTabCodigo.AtualizaEmpresa(codigo);
        }

        public void AtualizaEstado(int codigo)
        {
            _repositorioTabCodigo.AtualizaEstado(codigo);
        }

        public void AtualizaFormaPagto(int codigo)
        {
            _repositorioTabCodigo.AtualizaFormaPagto(codigo);
        }

        public void AtualizaPermissao(int codigo)
        {
            _repositorioTabCodigo.AtualizaPermissao(codigo);
        }

        public void AtualizaFornecedor(int codigo)
        {
            _repositorioTabCodigo.AtualizaFornecedor(codigo);
        }

        public void AtualizaGrupo(int codigo)
        {
            _repositorioTabCodigo.AtualizaGrupo(codigo);
        }

        public void AtualizaPedido(int codigo)
        {
            _repositorioTabCodigo.AtualizaPedido(codigo);
        }

        public void AtualizaProduto(int codigo)
        {
            _repositorioTabCodigo.AtualizaProduto(codigo);
        }

        public void AtualizaUnidade(int codigo)
        {
            _repositorioTabCodigo.AtualizaUnidade(codigo);
        }

        public void AtualizaUsuario(int codigo)
        {
            _repositorioTabCodigo.AtualizaUsuario(codigo);
        }

        public void AtualizaVendedor(int codigo)
        {
            _repositorioTabCodigo.AtualizaVendedor(codigo);
        }

        public void AtualizaTipoFornecedor(int codigo)
        {
            _repositorioTabCodigo.AtualizaTipoFornecedor(codigo);
        }

        public int RetornarUltimaCidade()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("gen_cidade");
        }

        public int RetornarUltimoCliente()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("gen_cliente");
        }

        public int RetornarUltimaCondicaoPagto()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("gen_condicao");
        }

        public int RetornarUltimaEmpresa()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("gen_empresa");
        }

        public int RetornarUltimoEstado()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("gen_estado");
        }

        public int RetornarUltimaFormaPagto()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_FORMAPAGTO");
        }

        public int RetornarUltimoFornecedor()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_FORNECEDOR");
        }

        public int RetornarUltimoGrupo()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_GRUPO");
        }

        public int RetornarUltimoPedido()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_PEDIDO");
        }

        public int RetornarUltimoPermissao()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_PERMISSAO");
        }

        public int RetornarUltimoProduto()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_PRODUTO");
        }

        public int RetornarUltimoTipoFornecedor()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_FORN_TIPO_EMPRESA");
        }

        public int RetornarUltimaUnidade()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_UNIDADE");
        }

        public int RetornarUltimoUsuario()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_USUARIO");
        }

        public int RetornarUltimoVendedor()
        {
            return _repositorioTabCodigo.RetornarUltimoSequencial("GEN_VENDEDOR");
        }
    }
}
