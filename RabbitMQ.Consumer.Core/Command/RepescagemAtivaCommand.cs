using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace RabbitMQ.Consumer.Core.Command
{
    public static class RepescagemAtivaCommand
    {
        public static bool EnviarCPFsForRepescagemAtivaImportacao(ImportacaoRepescagemAtivaViewModel model, string uri)
        {
            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var result = client.PostAsync($"{uri}/importacao-repescagem-ativa", content).Result;

            bool ocorreuTudoCerto;
            if (!result.IsSuccessStatusCode)
            {
                ocorreuTudoCerto = false;
                throw new ApplicationException($"Problema ao adicionar o atendimento. ReferenciaId: {model.CpfDeRepescagem}");
            }
            else
            {
                ocorreuTudoCerto = true;
            }

            return ocorreuTudoCerto;
        }

        public class ImportacaoRepescagemAtivaViewModel
        {
            public int Id { get; set; }
            public string CpfDeRepescagem { get; set; }
            public DateTime DataFimVigencia { get; set; }
        }
    }
}