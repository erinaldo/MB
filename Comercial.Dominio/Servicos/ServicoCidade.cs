﻿using Comercial.Dominio.Entidades;
using Comercial.Dominio.Interfaces.Repositorio;
using Comercial.Dominio.Interfaces.Servico;
using System;
using System.Collections.Generic;
using static Comercial.Dominio.Enumeradores.Enumerador;

namespace Comercial.Dominio.Servicos
{
    public class ServicoCidade : ServicoBase<Cidade>, IServicoCidade
    {
        private readonly string _tabela;
        private readonly IRepositorioCidade _repositorioCidade;
        private readonly IServicoPermissao _servicoPermissao;
        private readonly IRepositorioCidadeConsulta _repositorioCidadeConsulta;

        public ServicoCidade(IRepositorioCidade repositorioCidade,
            IRepositorioCidadeConsulta repositorioCidadeConsulta, IServicoPermissao servicoPermissao)
            :base(repositorioCidade)
        {
            _repositorioCidade = repositorioCidade;
            _repositorioCidadeConsulta = repositorioCidadeConsulta;
            _servicoPermissao = servicoPermissao;
            _tabela = "CIDADE";
        }

        public Cidade ObterPorId(int id)
        {
            return _repositorioCidade.GetById(id);
        }

        public void Excluir(Cidade cidade, string nomeUsuario)
        {
            try
            {
                _servicoPermissao.Permitir(AcaoUsuario.Excluir, _tabela, nomeUsuario);
                _repositorioCidade.Delete(cidade);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public int Salvar(Cidade cidade, string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(cidade.Desc_Cidade))
                throw new Exception("A Descrição é obrigatória!");
            if (cidade.Id_Estado == 0)
                throw new Exception("Informe o Estado!");

            try
            {
                if (cidade.Cod_Cidade == 0)
                {
                    cidade.Cod_Empresa = DadosStaticos.IdEmpresa;
                    _servicoPermissao.Permitir(AcaoUsuario.Incluir, _tabela, nomeUsuario);
                    cidade.Usu_Inc = Geral.PermissaoUsuario.GravarUsuarioDataHora(nomeUsuario);
                    _repositorioCidade.Insert(ref cidade);
                }
                else
                {
                    _servicoPermissao.Permitir(AcaoUsuario.Alterar, _tabela, nomeUsuario);
                    cidade.Usu_Alt = Geral.PermissaoUsuario.GravarUsuarioDataHora(nomeUsuario);
                    _repositorioCidade.Update(cidade);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return cidade.Cod_Cidade;
        }

        public IEnumerable<CidadeConsulta> Filtrar(string campo, string valor, int codEmpresa, int id = 0)
        {
            return _repositorioCidadeConsulta.Filtrar(campo, valor, codEmpresa, id);
        }
    }
}
