﻿using Comercial.Dominio.Constantes;
using Comercial.Dominio.Entidades;
using Comercial.Dominio.Enumeradores;
using System;
using System.Linq;
using static Comercial.Dominio.Enumeradores.Enumerador;

namespace Comercial.Dominio.Geral
{
    public class PermissaoUsuario
    {
        public Enumerador Incluir { get; private set; }

        public static string GravarUsuarioDataHora(string nomeUsuario)
        {
            return nomeUsuario + "-" + DateTime.Now;
        }

        public void Permitir(DadosFixos dadosFixos, AcaoUsuario enumerador, string tabela)
        {
            if (enumerador == AcaoUsuario.Incluir)
            {
                if (dadosFixos.Permissoes.Any(x => x.Tabela == tabela && x.Inc == "N"))
                    throw new Exception(MensagensPadrao.UsuarioSemPermissao);
            }

            if (enumerador == AcaoUsuario.Alterar)
            {
                if (dadosFixos.Permissoes.Any(x => x.Tabela == tabela && x.Alt == "N"))
                    throw new Exception(MensagensPadrao.UsuarioSemPermissao);
            }

            if (enumerador == AcaoUsuario.Excluir)
            {
                if (dadosFixos.Permissoes.Any(x => x.Tabela == tabela && x.Exc == "N"))
                    throw new Exception(MensagensPadrao.UsuarioSemPermissao);
            }

            if (enumerador == AcaoUsuario.Consultar)
            {
                if (dadosFixos.Permissoes.Any(x => x.Tabela == tabela && x.Con == "N"))
                    throw new Exception(MensagensPadrao.UsuarioSemPermissao);
            }
        }
    }
}
