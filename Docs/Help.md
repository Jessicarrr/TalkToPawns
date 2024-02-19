# Talk to Pawns Mod Installation Instructions

Before you start, make sure to subscribe to the **Talk to Pawns** mod on the Steam Workshop and activate it in the mods menu. This mod will not work out of the box and requires additional setup. Below are the instructions for setting up this mod.

You can find out more about what this mod is on the Steam Workshop page: [Link to mod page]

## Choosing Your AI

You may either use **ChatGPT** or **Koboldcpp** to generate AI responses from your pawns. Each option has its advantages and disadvantages.

### Advantages of Using ChatGPT

- Simpler setup compared to Koboldcpp
- Very fast response time during chats
- Generally fairly intelligent
- Does not require a strong computer or any extra computer resources

### Disadvantages of ChatGPT

- Costs a small amount of money per request (costs cents, but can add up to $1.00 per day with moderate/heavy usage)
- Requires signing up to OpenAI's Playground website (OpenAI hosts the AI models)
- May be overly "politically correct" and not properly roleplay a misogynist or cannibal pawn
- Lack of privacy: Messages you send and receive will be handled by OpenAI, and they likely save your conversations on their servers

### Koboldcpp Advantages

- Free to use (doesn't cost any money)
- Some AI models are uncensored and will willingly roleplay a misogynist or cannibal pawn
- Privacy: Since these are run on your computer, the AI responses and your messages won't be sent anywhere on the internet

### Koboldcpp Disadvantages

- Can require a strong computer, depending on which AI model you use
- Could be slow to respond depending on your computer and the model you use
- Can be 'dumber' compared to ChatGPT, depending on which model you use
- Requires running Koboldcpp in the background as you play RimWorld
- More difficult to set up

Evaluate which one you'd prefer to use, and then follow the relevant instructions below.

## Setup Instructions for ChatGPT Usage with Talk to Pawns

Using ChatGPT for this purpose is not free. Every request will cost a small amount of money. If you wish to use this mod for free, skip to the Koboldcpp section below.

**Important**: This is different from ChatGPT Premium, which is for using their GPT-4 model on the ChatGPT website, amongst their extra features. In order to use Talk to Pawns, you must use the OpenAI Playground, which will allow this mod to communicate using OpenAI's GPT models.

1. **Sign Up for OpenAI Playground**: You must sign up to OpenAI's service to use OpenAI's various ChatGPT models for this mod. [Sign up here](https://platform.openai.com/playground)

2. **Set Up Payment Information**: You will likely need to set up payment information on their website so OpenAI will allow you to use their AI models for this mod.

3. **Set Up an API Key**: The API key, once generated, will look like a random sequence of letters and numbers.
   - Go to [OpenAI API Keys](https://platform.openai.com/api-keys)
   - Press the "+ Create new secret key" button
   - Name it whatever you like
   - Permissions: all (to keep things simple, though you're free to set up permissions as you wish)
   - Press the green "Create secret key" button
   - Do not share this secret key with anybody
   - Press the copy button to obtain your new secret API key

For extra information and help, visit [OpenAI API Key Help](https://help.openai.com/en/articles/4936850-where-do-i-find-my-openai-api-key).

4. **Configure RimWorld with Your API Key**:
   - Open RimWorld
   - Click on the options menu
   - Click the 'Mod Options' button amongst the tabs on the left
   - Scroll down until you see "Talk to Pawns", click on that
   - Select AI Type "ChatGPT"
   - Copy your API key into the relevant box near the top
   - Select a GPT model (click here or scroll down for information on GPT models)

![ModSettingsApiKey2.png](images/ModSettingsApiKey2.png)

**Viewing Your Running Costs**: [OpenAI Usage](https://platform.openai.com/usage) - You can see your usage here, and how much it has cost you so far.

### Information on GPT Models

- **gpt-3.5-turbo**: The simplest model and cheapest to run.
- **gpt-3.5-turbo-16k**: Similar to the gpt-3.5-turbo model but with a longer context length (16k), meaning it can keep track of longer conversations.
- **gpt-3.5-turbo-1106**: Smarter version of gpt-3.5-turbo, with improved instruction following, and a 16k context length. Can cost about $0.50 to $1 USD after a few hours of usage. This version (1106) is the one I used most for testing this mod.
- **gpt-4**: The most sophisticated model of ChatGPT, with advanced reasoning skills, and an 8k context window. The most expensive to run.

---

## Setup Instructions for Koboldcpp

Koboldcpp is a local program you can run on your computer, capable of running AI models similar to ChatGPT.

### Instructions

If you want a more detailed installation guide, I recommend reading the following Reddit thread and following along with the Koboldcpp instructions: [A Starter Guide for Playing with Your Own Local AI](https://www.reddit.com/r/LocalLLaMA/comments/16y95hk/a_starter_guide_for_playing_with_your_own_local_ai/).

For extra information about Koboldcpp, visit their GitHub page: [Koboldcpp GitHub](https://github.com/LostRuins/koboldcpp).

1. **Download Koboldcpp**: [Koboldcpp Releases](https://github.com/LostRuins/koboldcpp/releases/tag/v1.58) - Download the file that best suits your PC under "Assets". "nocuda" versions are best for those without Nvidia cards or with Nvidia cards that don't support CUDA. I recommend creating a folder on your PC and putting it there.

2. **Download a Local Model**: Place these models into a subfolder inside where you installed Koboldcpp. Model recommendations and links are provided below.

    **Model Recommendations**:
    - **OpenHermes-2.5-mistral**: [Download from Hugging Face](https://huggingface.co/TheBloke/OpenHermes-2.5-Mistral-7B-GGUF/tree/main)
    - **Unholy-v2-13b.Q5_K_S.gguf**: [Download from Hugging Face](https://huggingface.co/TheBloke/Unholy-v2-13B-GGUF)

For a detailed explanation of these terms, read through the Reddit thread linked above.

3. **Run Koboldcpp**: Configuring Koboldcpp involves selecting the appropriate settings based on your hardware and the model you've chosen.

![Kobold Settings](images/koboldsettings.png)

**Settings Guide**:
- Presets: Choose either Use CLBlast, or Use CuBlas (if using Cuda)
- GPU ID: Selects which GPU will run your model
- Low VRAM: Useful if you have limited VRAM
- GPU Layers: Adjust for optimal speeds
- Context Size: Determines how much of the conversation the AI can analyze at once
- Model: Select the model you downloaded

Press "Launch" to start Koboldcpp. Keep this application running as the Talk to Pawns mod interfaces with it.

4. **Setting Up Koboldcpp in RimWorld**:
   - Go into RimWorld
   - Click on the options menu
   - Click the 'Mod Options' button
   - Find "Talk to Pawns" and select AI Type "Koboldcpp"

### Recommended Prompt for AI to Create Memories

This prompt helps the AI generate more meaningful conversation memories. Feel free to use or modify this prompt in your "Prompt for AI to create memories" option in the mod settings menu.

```plaintext
## {recipient_name}: That wraps up our conversation, huh? It sure was something. Reflecting on past chats, I like to summarize. Like when I said '-2 Said they think I'm ugly' after a not-so-great talk, or the time I felt uplifted and went with '+3 Talked about supporting each other', it's clear I've seen a range. Now, facing this one, it's time to decide quickly and carry on. Let's keep it short as well, my day's packed. So, sticking to the format of giving a number from -5 to 5 followed by a concise summary, I'd summarize this interaction as...
```

This prompt leverages the AI's text completion abilities to elicit a more precise response, improving the quality of conversation memories generated by the mod.


## Playing with the mod in game
Now that you've set up your mod, you can now load a save or start a new game. If you want to talk to a pawn, select the pawn you want to speak as, and then right click the pawn you want to talk to. Press the "Talk to" option and this will bring up a chat window. Have fun!

**Note**: Chat memories will be created when you close the conversation window.