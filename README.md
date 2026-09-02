# 🎙️ VoskDesktop — Real-Time Meeting Transcriber

A high-performance Windows desktop application built with **C# (.NET Framework 4.7.2)** featuring an authentic **Windows XP collapsible panel aesthetic**. 

Originally prototyped with Vosk and evolved into an offline **Whisper.net** inference engine, this tool captures live audio via **NAudio** and streams transcribed speech with minimal latency. It is designed to assist remote developers and freelancers during technical client interviews and standup meetings.

---

## 🚀 Key Features

* **Local Machine Learning Inference**: Uses `Whisper.net` running GGUF/ggml models locally on CPU without relying on paid cloud transcription services.
* **Low-Latency Streaming**: Ingests 16kHz, 16-bit Mono audio using `NAudio` with buffered chunking to maintain transcription context and accuracy.
* **Custom Windows XP Styling**: Custom-painted `XPPanel` and `XPPanelGroup` container components utilizing GDI+ gradients, custom collapse glyphs, and dynamic layout reflow.
* **Thread-Safe Architecture**: Asynchronous background workers decoupled from the WinForms UI thread via safe invocation dispatchers.
* **Noise & Silence Filtering**: Built-in heuristic filtering to discard blank audio frames, breathing artifacts, and repetitive bracket outputs.

---

## 🛠️ Tech Stack

* **Language**: C# (Language Version: 8.0+)
* **Framework**: .NET Framework 4.7.2 (Platform Target: `x64`)
* **Core Libraries**:
  * [Whisper.net](https://github.com/sandrohanea/whisper.net) + `Whisper.net.Runtime`
  * [NAudio 2.2.1](https://github.com/naudio/NAudio)
  * Newtonsoft.Json

---

## 🔮 Future Roadmap & API Extensibility

The application architecture is designed with modular endpoints ready for future expansion:

* **Real-Time Translation via API**: Each transcription panel already includes a `Translate` trigger button. Contributors can wire this event directly to LLM endpoints (such as the **Google Gemini API**, OpenAI, or DeepL) to achieve instant bidirectional translation (English ⇄ Indonesian / other languages).
* **Configurable LLM Prompts**: Future updates will support sending custom system prompts to summarize technical decisions, extract action items, or explain domain-specific jargon during meetings.
* **Direct System Audio Loopback**: Expanding beyond microphone capture by utilizing NAudio WASAPI loopback to record system speaker output directly from Zoom, Google Meet, or Microsoft Teams.

> **💡 Contribution Welcome**: Developers interested in implementing the translation provider interface or adding new LLM adapters are welcome to open a Pull Request!

---

## 📦 Setup & Installation

### 1. Clone the Repository
```bash
git clone [https://github.com/fjrmaulana/VoskDesktop.git](https://github.com/fjrmaulana/VoskDesktop.git)
cd VoskDesktop
