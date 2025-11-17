using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.AI
{
    /// <summary>
    /// Request para análisis de consecuencias de un mensaje
    /// </summary>
    public class AnalyzeConsequencesRequest
    {
        /// <summary>
        /// Texto a analizar
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Contexto del mensaje: workplace, academic, personal
        /// </summary>
        public string Context { get; set; } = "workplace";

        /// <summary>
        /// Código del país (CO, PE, EC, VE, BO, PA)
        /// </summary>
        public string Country { get; set; } = "CO";
    }
}
