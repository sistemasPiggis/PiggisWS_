using System.ComponentModel.DataAnnotations;

namespace PIGGISWS.Models.DTOs;

public class Fcm_Token
{
    [Key]
    public decimal FCM_CODIGO {  get; set; }
    public string? FCM_TOKEN {  get; set; } = string.Empty;
    public string? FCM_EMP_CODIGO { get; set; } = "0";
    public string? FCM_OS { get; set; } = string.Empty;
    public string? APP { get; set; } = string.Empty;
    public string? CREA_USR { get; set; } = string.Empty;
    public DateTime? CREA_FECHA { get; set; } = DateTime.Now;

    public string? MOD_USR { get; set; } = string.Empty;
    public DateTime? MOD_FECHA { get; set; } = DateTime.Now;
}
