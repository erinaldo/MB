﻿using Comercial.Dominio.Entidades;
using Comercial.Dominio.Enumeradores;
using Comercial.Dominio.Geral;
using Comercial.Dominio.Interfaces.Repositorio;
using Comercial.Dominio.Interfaces.Servico;
using System;
using System.Collections.Generic;
using System.Data;
using static Comercial.Dominio.Enumeradores.Enumerador;

namespace Comercial.Dominio.Servicos
{
    public class ServicoContato : ServicoBase<Contato>, IServicoContato
    {
        private readonly string _tabela;
        private readonly IRepositorioContato _repositorioContato;
        private readonly IServicoPermissao _servicoPermissao;
        private readonly IRepositorioContatoConsulta _repositorioContatoConsulta;

        public ServicoContato(IRepositorioContato repositorioContato,
            IRepositorioContatoConsulta repositorioContatoConsulta, IServicoPermissao servicoPermissao)
            :base(repositorioContato)
        {
            _repositorioContato = repositorioContato;
            _repositorioContatoConsulta = repositorioContatoConsulta;
            _servicoPermissao = servicoPermissao;
            _tabela = "CONTATO";
        }

        public Contato ObterPorId(int id)
        {
            return _repositorioContato.GetById(id);
        }

        public void Excluir(Contato contato, string nomeUsuario)
        {
            try
            {
                _servicoPermissao.Permitir(AcaoUsuario.Excluir, _tabela, nomeUsuario);
                _repositorioContato.Excluir(contato.Cod_Empresa, contato.Codigo, contato.Seq, contato.Tipo);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private int SalvarContato(Contato contato, string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(contato.ContatoTexto))
                throw new Exception("A Descrição é obrigatória!");

            try
            {
                if (contato.Seq == 0)
                {
                    contato.Cod_Empresa = DadosStaticos.IdEmpresa;
                    _servicoPermissao.Permitir(AcaoUsuario.Incluir, _tabela, nomeUsuario);
                    contato.Usu_Inc = PermissaoUsuario.GravarUsuarioDataHora(nomeUsuario);
                    _repositorioContato.Insert(ref contato);
                }
                else
                {
                    _servicoPermissao.Permitir(AcaoUsuario.Alterar, _tabela, nomeUsuario);
                    contato.Usu_Alt = PermissaoUsuario.GravarUsuarioDataHora(nomeUsuario);
                    _repositorioContato.Update(contato);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return contato.Codigo;
        }

        public IEnumerable<ContatoConsulta> Filtrar(string campo, string valor, int codEmpresa)
        {
            return _repositorioContatoConsulta.Filtrar(campo, valor, codEmpresa);
        }

        public Contato ObterPorId(int codEmpresa, int codigo, int seq, Enumerador.EnContato enContato)
        {
            if (enContato == EnContato.Cliente)
                return _repositorioContato.First(x => x.Cod_Empresa == codEmpresa && x.Codigo == codigo && x.Seq == seq && x.Tipo == "C");
            else
                return _repositorioContato.First(x => x.Cod_Empresa == codEmpresa && x.Codigo == codigo && x.Seq == seq && x.Tipo == "F");
        }

        public IEnumerable<ContatoConsulta> ObterPorCodigo(int codEmpresa, int codigo, EnContato enContato)
        {
            if (enContato == EnContato.Cliente)
                return _repositorioContatoConsulta.BuscarDados(codigo, codEmpresa, Enumerador.EnContato.Cliente);
            else
                return _repositorioContatoConsulta.BuscarDados(codigo, codEmpresa, Enumerador.EnContato.Fornecedor);
        }

        public int Salvar(Contato contato, string nomeUsuario, EnContato enContato)
        {
            if (string.IsNullOrWhiteSpace(contato.ContatoTexto))
                throw new Exception("A Descrição é obrigatória!");

            if (enContato == EnContato.Cliente)
                contato.Tipo = "C";
            else
                contato.Tipo = "F";

            return SalvarContato(contato, nomeUsuario);
        }
    }
}
