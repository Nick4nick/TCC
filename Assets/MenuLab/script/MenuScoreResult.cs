using UnityEngine;

/// <summary>
/// Nota de um cardápio (ou de um dos 3 cardápios da Fase 5), nos 3 critérios.
/// Cada critério começa em 10 e vai sendo descontado a cada feedback disparado.
/// </summary>
public class MenuScoreResult
{
    public float Nutricional { get; private set; } = 10f;
    public float Publico { get; private set; } = 10f;
    public float Variedade { get; private set; } = 10f;

    public float NotaFinal => (Nutricional + Publico + Variedade) / 3f;

    public void DescontarNutricional(float valor) => Nutricional = Mathf.Clamp(Nutricional - valor, 0f, 10f);
    public void DescontarPublico(float valor) => Publico = Mathf.Clamp(Publico - valor, 0f, 10f);
    public void DescontarVariedade(float valor) => Variedade = Mathf.Clamp(Variedade - valor, 0f, 10f);

    public void ZerarPublico() => Publico = 0f;

    public void ZerarTudo()
    {
        Nutricional = 0f;
        Publico = 0f;
        Variedade = 0f;
    }
}
