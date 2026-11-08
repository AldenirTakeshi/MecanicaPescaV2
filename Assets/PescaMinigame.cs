using UnityEngine;
using UnityEngine.InputSystem;

public class PescaMinigame : MonoBehaviour
{
    [Header("Componentes de UI")]
    public RectTransform trilhaFundo;
    public RectTransform barraJogador;
    public RectTransform iconePeixe;
    public RectTransform barraProgresso;

    [Header("Configurações do Peixe")]
    public string nomePeixe = "Tambaqui";
    public float resistenciaPeixe = 15f;
    
    [Header("Física da Barra")]
    public float barraTamanho = 0.30f;
    public float forcaSubida = 1.8f;   
    public float gravidade = 1.4f;     
    public float resistenciaAr = 0.95f; 

    private float posicaoPeixe = 0.5f;
    private float posicaoBarra = 0.0f;
    private float velocidadeBarra = 0f;
    private float progressoCaptura = 0.3f; 
    private bool emPesca = true;

    void Update()
    {
        if (!emPesca) return;

        float destinoPeixe = Mathf.PerlinNoise(Time.time * (resistenciaPeixe / 10f), 0f);
        posicaoPeixe = Mathf.MoveTowards(posicaoPeixe, destinoPeixe, Time.deltaTime * 0.7f);
        posicaoPeixe = Mathf.Clamp01(posicaoPeixe);

        bool estaPressionando = (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) || 
                                (Mouse.current != null && Mouse.current.leftButton.isPressed);

        if (estaPressionando)
        {
            velocidadeBarra += forcaSubida * Time.deltaTime;
        }
        else
        {
            velocidadeBarra -= gravidade * Time.deltaTime;
        }

        velocidadeBarra *= Mathf.Pow(resistenciaAr, Time.deltaTime * 60f);
        posicaoBarra += velocidadeBarra * Time.deltaTime;

        if (posicaoBarra <= 0f)
        {
            posicaoBarra = 0f;
            if (velocidadeBarra < 0) velocidadeBarra = 0f;
        }
        else if (posicaoBarra >= 1f - barraTamanho)
        {
            posicaoBarra = 1f - barraTamanho;
            if (velocidadeBarra > 0) velocidadeBarra = 0f;
        }

        bool peixeNaBarra = posicaoPeixe >= posicaoBarra && posicaoPeixe <= (posicaoBarra + barraTamanho);

        if (peixeNaBarra)
        {
            progressoCaptura += 0.20f * Time.deltaTime;
        }
        else
        {
            progressoCaptura -= 0.12f * Time.deltaTime;
        }

        progressoCaptura = Mathf.Clamp01(progressoCaptura);

        AtualizarInterfaceVisual();

        if (progressoCaptura >= 1f)
        {
            Debug.Log($"<color=green>SUCESSO!</color> Você pegou um <b>{nomePeixe}</b>!");
            emPesca = false;
        }
        else if (progressoCaptura <= 0f)
        {
            Debug.Log("<color=red>FALHA!</color> O peixe escapou!");
            emPesca = false;
        }
    }

    void AtualizarInterfaceVisual()
    {
        if (trilhaFundo == null) return;

        float alturaTotal = trilhaFundo.rect.height;

        if (barraJogador != null)
        {
            float alturaBarra = alturaTotal * barraTamanho;
            barraJogador.sizeDelta = new Vector2(barraJogador.sizeDelta.x, alturaBarra);
            barraJogador.anchoredPosition = new Vector2(0, posicaoBarra * alturaTotal);
        }

        if (iconePeixe != null)
        {
            float alturaPeixe = iconePeixe.rect.height;
            float posicaoYPeixe = posicaoPeixe * (alturaTotal - alturaPeixe);
            iconePeixe.anchoredPosition = new Vector2(0, posicaoYPeixe);
        }

        if (barraProgresso != null)
        {
            barraProgresso.sizeDelta = new Vector2(barraProgresso.sizeDelta.x, progressoCaptura * alturaTotal);
        }
    }
}