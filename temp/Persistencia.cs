﻿namespace Reflaction.ConsoleApp
{
    public abstract class Persistencia
    {
        public abstract string Inserir(object obj, string tabela, string campoId);

        public abstract string Update(object obj, string tabela, string campoId, int valorId);

        public virtual string Select(object obj, string tabela, string alias = "", bool comFrom = true, bool comSelect = true)
        {
            string InstrucaoSQL = "";

            if (comSelect)
                InstrucaoSQL = "SELECT ";

            string campos = "";

            foreach (var prop in obj.GetType().GetProperties())
            {
                var valor = prop.GetValue(obj);
                if (alias == "")
                    campos += prop.Name + ",";
                else
                    campos += alias + "." + prop.Name + ",";
            }
            campos = campos.Remove(campos.Length - 1, 1);
            InstrucaoSQL += campos;

            if (comFrom)
                InstrucaoSQL += " FROM " + tabela + " " + alias;

            return InstrucaoSQL;
        }

        public virtual string Delete(string tabela, string campoId, int valorId)
        {
            return "DELETE FROM " + tabela + " WHERE " + campoId + " = " + valorId;
        }

    }
}
