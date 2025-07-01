using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Unity.InferenceEngine;
using System.Text;
using DefaultNamespace;
using Unity.Collections;
using Newtonsoft.Json;
using States;
using TMPro;
using UnityEditor.Rendering;

public class RunWhisper : MonoBehaviour
{
    //Model Assets : Decoders, Encoders, Spectro
    [Header("Model Assets")]
    public ModelAsset audioDecoder1;
    public ModelAsset audioDecoder2;
    public ModelAsset audioEncoder;
    public ModelAsset logMelSpectro;
    
    //Audio Clip
    [Header("Input/Output")]
    public AudioClip audioClip;
    public MicRecorder micRecorder;
    public TMP_Text textMesh;
    public string[] triggerWords = { "start", "stop", "go", "fire" };
    
    // ------------------------
    // Inference Engine Workers
    // ------------------------

    [HideInInspector] public Worker decoder1;
    [HideInInspector] public Worker decoder2;
    [HideInInspector] public Worker encoder;
    [HideInInspector] public Worker spectrogram;
    [HideInInspector] public Worker argmax;
    
    // ------------------------
    // Transcription Parameters
    // ------------------------

    // Special tokens see added tokens file for details
    public const int END_OF_TEXT = 50257;
    public const int START_OF_TRANSCRIPT = 50258;
    public const int ENGLISH = 50259;
    // public const int GERMAN = 50261;
    // public const int FRENCH = 50265;
    public const int TRANSCRIBE = 50359; //for speech-to-text in specified language
    public const int TRANSLATE = 50358;  //for speech-to-text then translate to English
    public const int NO_TIME_STAMPS = 50363;
    public const int START_TIME = 50364;

    public const int maxTokens = 100;
    public const int maxSamples = 30 * 16000;
    
    // ------------------------
    // Stateful Transcription Data
    // ------------------------
    
    public int numSamples;
    public int tokenCount = 0;
    public string[] tokens;
    public string outputString = "";
    public bool transcribe = false;
    
    //Audio Data
    public Tensor<float> encodedAudio;
    public Tensor<float> audioInput;

    // Character decoding helpers
    public int[] whiteSpaceCharacters = new int[256];

    //Token Buffers
    public NativeArray<int> outputTokens;
    public NativeArray<int> lastToken;
    public Tensor<int> lastTokenTensor;
    public Tensor<int> tokensTensor;
    
    // ------------------------
    // State Machine
    // ------------------------
    
    private IWhisperStates currentState;
    // private Awaitable m_Awaitable;

    private IEnumerator decoder1Schedule;
    private IEnumerator decoder2Schedule;

    private bool runningStep = false;
    private const int k_LayersPerFrame = 16;

    private StringBuilder stringBuilder = new StringBuilder();
    
    void Start()
    {
        TransitionToState(new LoadDecoderState(this));    
    }

    public void TransitionToState(IWhisperStates newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    void Update()
    {
        currentState?.Tick();
    }

    public void SetClipAndBeginTranscription(AudioClip clip)
    {
        audioClip = clip;
        TransitionToState(new StartTranscriptionState(this));
    }
    
    public void LoadAudio()
    {
        numSamples = audioClip.samples;
        var data = new float[maxSamples];
        numSamples = maxSamples;
        audioClip.GetData(data, 0);
        audioInput = new Tensor<float>(new TensorShape(1, numSamples), data);
    }

    public void EncodeAudio()
    {
        spectrogram.Schedule(audioInput);
        var logmel = spectrogram.PeekOutput() as Tensor<float>;
        encoder.Schedule(logmel);
        encodedAudio = encoder.PeekOutput() as Tensor<float>;
    }
    

    public SimpleAwaitable TranscriptionLoop()
    {
        var awaitable = new SimpleAwaitable();
        StartCoroutine(TranscriptionCoroutine(awaitable));
        return awaitable;
    }
    
    private IEnumerator TranscriptionCoroutine(SimpleAwaitable awaitable)
    {
        while (transcribe && tokenCount < outputTokens.Length - 1)
        {
            runningStep = true;
            yield return RunInferenceStepSplit();
            while (runningStep) yield return null;
        }
        
        Debug.Log("Final Transcription " + stringBuilder.ToString());
        textMesh.text = stringBuilder.ToString();

        // if (outputString.ToLower().ContainsAny(triggerWords))
        // {
        //     Debug.Log("Trigger Word Detected");
        // }
        
        awaitable.Complete();
    }

    private IEnumerator RunInferenceStepSplit()
    {
        decoder1.SetInput("input_ids", tokensTensor);
        decoder1.SetInput("encoder_hidden_states", encodedAudio);
        
        decoder1Schedule = decoder1.ScheduleIterable();

        while (decoder1Schedule.MoveNext())
        {
            for (int i = 0; i < k_LayersPerFrame && decoder1Schedule.MoveNext(); ++i) ;
            yield return null;
        }
        
        var past_key_values_0_decoder_key = decoder1.PeekOutput("present.0.decoder.key") as Tensor<float>;
        var past_key_values_0_decoder_value = decoder1.PeekOutput("present.0.decoder.value") as Tensor<float>;
        var past_key_values_1_decoder_key = decoder1.PeekOutput("present.1.decoder.key") as Tensor<float>;
        var past_key_values_1_decoder_value = decoder1.PeekOutput("present.1.decoder.value") as Tensor<float>;
        var past_key_values_2_decoder_key = decoder1.PeekOutput("present.2.decoder.key") as Tensor<float>;
        var past_key_values_2_decoder_value = decoder1.PeekOutput("present.2.decoder.value") as Tensor<float>;
        var past_key_values_3_decoder_key = decoder1.PeekOutput("present.3.decoder.key") as Tensor<float>;
        var past_key_values_3_decoder_value = decoder1.PeekOutput("present.3.decoder.value") as Tensor<float>;

        var past_key_values_0_encoder_key = decoder1.PeekOutput("present.0.encoder.key") as Tensor<float>;
        var past_key_values_0_encoder_value = decoder1.PeekOutput("present.0.encoder.value") as Tensor<float>;
        var past_key_values_1_encoder_key = decoder1.PeekOutput("present.1.encoder.key") as Tensor<float>;
        var past_key_values_1_encoder_value = decoder1.PeekOutput("present.1.encoder.value") as Tensor<float>;
        var past_key_values_2_encoder_key = decoder1.PeekOutput("present.2.encoder.key") as Tensor<float>;
        var past_key_values_2_encoder_value = decoder1.PeekOutput("present.2.encoder.value") as Tensor<float>;
        var past_key_values_3_encoder_key = decoder1.PeekOutput("present.3.encoder.key") as Tensor<float>;
        var past_key_values_3_encoder_value = decoder1.PeekOutput("present.3.encoder.value") as Tensor<float>;

        decoder2.SetInput("input_ids", lastTokenTensor);
        decoder2.SetInput("past_key_values.0.decoder.key", past_key_values_0_decoder_key);
        decoder2.SetInput("past_key_values.0.decoder.value", past_key_values_0_decoder_value);
        decoder2.SetInput("past_key_values.1.decoder.key", past_key_values_1_decoder_key);
        decoder2.SetInput("past_key_values.1.decoder.value", past_key_values_1_decoder_value);
        decoder2.SetInput("past_key_values.2.decoder.key", past_key_values_2_decoder_key);
        decoder2.SetInput("past_key_values.2.decoder.value", past_key_values_2_decoder_value);
        decoder2.SetInput("past_key_values.3.decoder.key", past_key_values_3_decoder_key);
        decoder2.SetInput("past_key_values.3.decoder.value", past_key_values_3_decoder_value);

        decoder2.SetInput("past_key_values.0.encoder.key", past_key_values_0_encoder_key);
        decoder2.SetInput("past_key_values.0.encoder.value", past_key_values_0_encoder_value);
        decoder2.SetInput("past_key_values.1.encoder.key", past_key_values_1_encoder_key);
        decoder2.SetInput("past_key_values.1.encoder.value", past_key_values_1_encoder_value);
        decoder2.SetInput("past_key_values.2.encoder.key", past_key_values_2_encoder_key);
        decoder2.SetInput("past_key_values.2.encoder.value", past_key_values_2_encoder_value);
        decoder2.SetInput("past_key_values.3.encoder.key", past_key_values_3_encoder_key);
        decoder2.SetInput("past_key_values.3.encoder.value", past_key_values_3_encoder_value);

        decoder2Schedule = decoder2.ScheduleIterable();

        while (decoder2Schedule.MoveNext())
        {
            for (int i = 0; i < k_LayersPerFrame && decoder2Schedule.MoveNext(); ++i) ;
            yield return null;
        }

        var logits = decoder2.PeekOutput("logits") as Tensor<float>;
        argmax.Schedule(logits);
        
        using var t_Token = argmax.PeekOutput().ReadbackAndClone() as Tensor<int>;
        int index = t_Token[0];

        outputTokens[tokenCount] = lastToken[0];
        lastToken[0] = index;
        tokenCount++;
        
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
        lastTokenTensor.dataOnBackend.Upload<int>(lastToken, 1);

        if (index == END_OF_TEXT)
        {
            transcribe = false;
        }
        else if (index < tokens.Length)
        {
            //Check Key Words here
            var currentTokenWord = GetUnicodeText(tokens[index]);
            // CheckTargetWords(currentTokenWord);
            
            // outputString += GetUnicodeText(tokens[index]);
            stringBuilder.Append(currentTokenWord);
        }
        
        runningStep = false;
    }

    // private bool CheckTargetWords(string word) 
    // {
    //     if (triggerWords.Contains(word))
    //     {
    //         textMesh.text = word;
    //         return true;
    //     }
    //
    //     return false;
    // }
    
    // Tokenizer
    public TextAsset vocabAsset;
    public void GetTokens()
    {
        var vocab = JsonConvert.DeserializeObject<Dictionary<string, int>>(vocabAsset.text);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }
    }

    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += ((int)letter <= 256) ? letter : (char)whiteSpaceCharacters[(int)(letter - 256)];
        }
        return outText;
    }

    public void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) whiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || ('�' <= c && c <= '�') || ('�' <= c && c <= '�'));
    }

    private void OnDestroy()
    {
        decoder1?.Dispose();
        decoder2?.Dispose();
        encoder?.Dispose();
        spectrogram?.Dispose();
        argmax?.Dispose();
        audioInput?.Dispose();
        lastTokenTensor?.Dispose();
        tokensTensor?.Dispose();
    }
}
