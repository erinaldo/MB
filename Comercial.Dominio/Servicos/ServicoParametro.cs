﻿using Comercial.Dominio.Entidades;
using Comercial.Dominio.Interfaces.Repositorio;
using Comercial.Dominio.Interfaces.Servico;
using System;
using System.Collections.Generic;
using System.Data;

namespace Comercial.Dominio.Servicos
{
    public class ServicoParametro : ServicoBase<Parametro>, IServicoParametro
    {
        private readonly IRepositorioParametro _repositorioParametro;
        private readonly IServicoPermissao _servicoPermissao;
        private readonly IRepositorioParametroConsulta _repositorioParametroConsulta;

        public ServicoParametro(IRepositorioParametro repositorioParametro,
            IRepositorioParametroConsulta repositorioParametroConsulta, IServicoPermissao servicoPermissao)
            :base(repositorioParametro)
        {
            _repositorioParametro = repositorioParametro;
            _repositorioParametroConsulta = repositorioParametroConsulta;
            _servicoPermissao = servicoPermissao;
        }

        public Parametro ObterPorId(int id)
        {
            return _repositorioParametro.GetById(id);
        }

        public void Excluir(Parametro parametro, string nomeUsuario)
        {
            try
            {
                _repositorioParametro.Delete(parametro);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public int Salvar(Parametro parametro, string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(parametro.Nome))
                throw new Exception("O nome é obrigatório!");
            if (string.IsNullOrWhiteSpace(parametro.Valor))
                throw new Exception("O valor é obrigatório!");

            try
            {
                if (parametro.Par_Id == 0)
                {
                    _repositorioParametro.Insert(ref parametro);
                }
                else
                {
                    _repositorioParametro.Update(parametro);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return parametro.Par_Id;
        }

        public IEnumerable<ParametroConsulta> Filtrar(string campo, string valor, int id = 0)
        {
            return _repositorioParametroConsulta.Filtrar(campo, valor, id);
        }
    }
}
