using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace ASChurchManager.Infra.Data.Repository
{
    public static class DataMapper
    {
        private static readonly Type[] FrameworkTypes = GetFrameworkTypes();

        /// <summary>
        /// Cria uma nova instância do objeto passado como tipo para o método genérico.
        /// O objeto é criado com os valores retornados pelo banco de dados.
        /// </summary>
        /// <typeparam name="TOutput">Tipo do objeto que deve ser instanciado</typeparam>
        /// <param name="dataReader">DataReader com os valores retornados pelo banco de dados</param>
        /// <returns>Retorna um objeto com as propriedades preenchidas</returns>
        public static TOutput ExecuteMapping<TOutput>(DbDataReader dataReader)
            where TOutput : class, new()
        {
            var returnObj = new TOutput();
            CreateWithValues(returnObj, dataReader);
            return returnObj;
        }

        /// <summary>
        /// Define o valor das propriedades de um objeto genérico a partir dos dados retornados pelo SqlDataReader
        /// </summary>
        /// <typeparam name="T">Tipo genérico</typeparam>
        /// <param name="instance">Instancia do objeto</param>
        /// <param name="dataReader">DataReader com os valores retornados pelo banco de dados</param>
        private static void CreateWithValues<T>(T instance, DbDataReader dataReader)
            where T : class
        {
            if (!dataReader.HasRows)
            {
                return;
            }

            var instanceFields = instance.GetType().GetProperties();

            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                var property = instanceFields.FirstOrDefault(t => t.Name == dataReader.GetName(i));

                if (property != null)
                {
                    var value = dataReader.GetValue(i) == DBNull.Value
                                                    ? null
                                                    : dataReader.GetValue(i);

                    if (value != null)
                    {
                        // Verifica se o tipo da propriedade está entre os Built-in types do .Net Framework
                        if (FrameworkTypes
                                .ToList()
                                .FirstOrDefault(a => a.Name == property.PropertyType.Name) != null)
                        {
                            if (property.PropertyType.Name.ToLower().Contains("string"))
                            {
                                property.SetValue(instance, Convert.ChangeType(value.ToString().RemoveCaracteres(), property.PropertyType));
                            }
                            else
                            {
                                property.SetValue(instance, Convert.ChangeType(value, property.PropertyType));
                            }
                        }
                        else
                        {
                            if (property.PropertyType.Name.ToLower().Contains("string"))
                            {
                                property.SetValue(instance, Convert.ChangeType(value.ToString().RemoveCaracteres(), property.PropertyType));
                            }
                            else
                            {
                                property.SetValue(instance, property.PropertyType.IsEnum
                                                    ? Enum.Parse(property.PropertyType, value.ToString())
                                                    : value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifica o nome correto do atríbuto ou propriedade da classe
        /// </summary>
        /// <param name="field">Atributo que deve ser verificado</param>
        /// <returns>Retorna o nome correto do atributo</returns>
        private static string CorrectFieldName(FieldInfo field)
        {
            var fieldName = string.Empty;

            // Verifica se é uma propriedade automática
            if (field.Name.Contains("k__BackingField"))
            {
                fieldName = field.Name.Substring(1, field.Name.IndexOf('>') - 1);
            }
            else
            {
                // Remove underline do atributo
                if (string.CompareOrdinal(field.Name.Substring(0, 1), "_") == 0)
                {
                    fieldName = field.Name.Remove(0, 1);
                }
            }

            return fieldName;
        }

        /// <summary>
        /// Obtém os Built-in Types suportados pelo .Net Framework
        /// </summary>
        /// <returns></returns>
        private static Type[] GetFrameworkTypes()
        {
            var valueTypes = Assembly.Load("Mscorlib")
                                    .GetTypes()
                                    .Where(t => t.IsPrimitive == true
                                                || t.Name.Equals("String")
                                                || t.Name.Equals("DateTimeOffset")
                                                || t.Name.Equals("DateTimeOffsetOffset")
                                                || t.Name.Equals("Decimal"));

            return valueTypes
                        .ToArray()
                        .OrderBy(t => t.Name)
                        .ToArray();
        }
    }
}
