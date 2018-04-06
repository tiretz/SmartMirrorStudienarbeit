﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using SmartMirror.Objects;
using SmartMirrorServer;
using SmartMirrorServer.Objects.Moduls.Weather;

namespace SmartMirror.SpeechService
{
    internal class SpeechService
    {
        private readonly SpeechSynthesizer speechSynthesizer;
        private readonly MediaPlayer speechPlayer;

        public SpeechService()
        {
            speechSynthesizer = createSpeechSynthesizer();

            speechPlayer = new MediaPlayer { AudioCategory = MediaPlayerAudioCategory.Speech, AutoPlay = false };

            #pragma warning disable 4014
            //startup(); // TODO auskommentieren
            #pragma warning restore 4014
        }

        private static SpeechSynthesizer createSpeechSynthesizer()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();

            VoiceInformation voice = SpeechSynthesizer.AllVoices.SingleOrDefault(i => i.DisplayName == "Microsoft Katja") ?? SpeechSynthesizer.DefaultVoice;

            synthesizer.Voice = voice;

            return synthesizer;
        }

        private async Task sayAsync(string text)
        {
            using (SpeechSynthesisStream stream = await speechSynthesizer.SynthesizeTextToStreamAsync(text))
            {
                MediaSource mediaSource = MediaSource.CreateFromStream(stream, stream.ContentType);
                speechPlayer.Source = mediaSource;
            }

            speechPlayer.Play();
        }

        private async Task sayAsyncSsml(string ssml)
        {
            using (SpeechSynthesisStream stream = await speechSynthesizer.SynthesizeSsmlToStreamAsync(ssml))
            {
                MediaSource mediaSource = MediaSource.CreateFromStream(stream, stream.ContentType);
                speechPlayer.Source = mediaSource;
            }

            speechPlayer.Play();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task startup()
        {
            StringBuilder startupString = new StringBuilder();

            startupString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            startupString.AppendLine(@"<sentence>");

            startupString.AppendLine("Darf ich mich vorstellen ?");
            startupString.AppendLine("<break time='500ms'/>");
            startupString.AppendLine("Mein Name ist <prosody rate=\"-30%\">Mira</prosody>.");
            startupString.AppendLine("<break time='300ms'/>");
            startupString.AppendLine("Wie kann ich dir behilflich <prosody pitch=\"high\">sein</prosody>?");
            startupString.AppendLine("<break time='1000ms'/>");
            startupString.AppendLine("Sprachbefehle, sowie weitere Information kannst du dir mit dem Sprachbefehl <prosody rate=\"-25%\">Mira zeige Hilfe</prosody> anzeigen lassen.");

            startupString.AppendLine(@"</sentence>");
            startupString.AppendLine(@"</speak>");

            await sayAsyncSsml(startupString.ToString());
        }

        public async Task SayTime()
        {
            DateTime now = DateTime.Now;

            string time = $"Es ist {now.Hour} Uhr und {now.Minute} Minuten.";

            await sayAsync(time);
        }

        public async Task CountDown(int fromNumber)
        {
            StringBuilder countdownString = new StringBuilder();

            countdownString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            countdownString.AppendLine(@"<sentence>");

            for (int i = fromNumber ; i >= 0 ; i--)
            {
                if (i == 0)
                {
                    countdownString.AppendLine($"<prosody rate =\"-30%\">{numberToWords(i)}</prosody>.");
                    countdownString.AppendLine("<break time='500ms'/>");
                    countdownString.AppendLine("Die Zeit ist abgelaufen. Countdown beendet.");
                }
                else
                {
                    countdownString.AppendLine(numberToWords(i));
                    countdownString.AppendLine("<break time='1000ms'/>");
                }
            }

            countdownString.AppendLine(@"</sentence>");
            countdownString.AppendLine(@"</speak>");

            await sayAsyncSsml(countdownString.ToString());
        }

        public async Task CountTo(int toNumber)
        {
            StringBuilder countToString = new StringBuilder();

            countToString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            countToString.AppendLine(@"<sentence>");

            for (int i = 0; i >= toNumber; i--)
            {
                if (i == toNumber)
                {
                    countToString.AppendLine($"<prosody rate =\"-30%\">{numberToWords(i)}</prosody>.");
                    countToString.AppendLine("<break time='500ms'/>");
                    countToString.AppendLine("Zielnummer erreicht.");
                }
                else
                {
                    countToString.AppendLine(numberToWords(i));
                    countToString.AppendLine("<break time='1000ms'/>");
                }
            }

            countToString.AppendLine(@"</sentence>");
            countToString.AppendLine(@"</speak>");

            await sayAsyncSsml(countToString.ToString());
        }

        public async Task SayName()
        {

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine("Mein Name ist <prosody rate=\"-30%\">Mira</prosody>.");
            nameString.AppendLine("<break time='300ms'/>");
            nameString.AppendLine("Aus dem lateinischen heraus übersetzt, bedeutet mein Name soviel wie <prosody rate=\"-30%\">wunderbar</prosody>, <prosody rate=\"-30%\">die Wunderbare</prosody>.");
            nameString.AppendLine("<break time='300ms'/>");
            nameString.AppendLine("Eine hinduistische Legende erzählt die Geschichte von Mirabai, <break time='200ms'/>einer Prinzessin aus dem 16. Jahrhundert, die sich als Geliebte Krishnas betrachtete. <break time='200ms'/>Mirabai gilt als geistliche Liebesdichterin.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayLook()
        {
            Random randi = new Random();
            int randomNumber = randi.Next(2);

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine(randomNumber == 0 ? "Ich fürchte, dass die Beschreibung meines Aussehens einen längeren Ausflug in Themenbereiche zum Raum - Zeit - Kontinuum und zur Mode notwendig machen würde, die dir bis jetzt noch völlig unbekannt sind." : "Mal schauen. <break time='500ms'/> Dacht ich mir's doch, das gleiche wie gestern.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayGender()
        {

            StringBuilder genderString = new StringBuilder();

            genderString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            genderString.AppendLine(@"<sentence>");

            genderString.AppendLine("Das ist eine gute Frage. Ich muss gestehen, ich weiß es selber nicht einmal so genau.");

            genderString.AppendLine(@"</sentence>");
            genderString.AppendLine(@"</speak>");

            await sayAsyncSsml(genderString.ToString());
        }

        public async Task SayRandom(int from, int to)
        {
            Random randi = new Random();
            int randomNumber = randi.Next(from, to);

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine($"Lass mich nachdenken. <break time='1500ms'/> Ich sage einfach mal <break time='300ms'/><prosody rate=\"-35%\">{numberToWords(randomNumber)}</prosody>.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayMirror()
        {
            Random randi = new Random();
            int randomNumber = randi.Next(2);

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine(randomNumber == 0 ? "Geh mal zur Seite, <break time='300ms'/>ich kann nichts sehen!" : "Hier ein Tipp unter Freunden: <break time='500ms'/> Frag heute einfach mal nicht!");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayQuote()
        {
            QuoteOfDay quote = HelperClasses.QuoteOfDay.GetQuoteOfDay();

            StringBuilder quoteString = new StringBuilder();

            quoteString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            quoteString.AppendLine(@"<sentence>");

            quoteString.AppendLine($"{(quote.Author != string.Empty ? quote.Author : "Ein kluge Frau oder ein kluger Mann")} sagte einstmal: <break time='400ms'/> {quote.Text}");

            quoteString.AppendLine(@"</sentence>");
            quoteString.AppendLine(@"</speak>");

            await sayAsyncSsml(quoteString.ToString());
        }

        public async Task SayJoke()
        {
            Joke joke = HelperClasses.Joke.GetJoke();

            StringBuilder jokeString = new StringBuilder();

            jokeString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            jokeString.AppendLine(@"<sentence>");

            jokeString.AppendLine($"Einen {joke.Title.Remove(joke.Title.Length - 1)} gefällig: <break time='300ms'/><prosody rate=\"-15%\">{joke.Description}</prosody>");

            jokeString.AppendLine(@"</sentence>");
            jokeString.AppendLine(@"</speak>");

            await sayAsyncSsml(jokeString.ToString());
        }

        public async Task SayWeatherToday()
        {
            SingleResult<CurrentWeatherResult> result = CurrentWeather.GetByCityName(Application.StorageData.WeatherModul.City, Application.StorageData.WeatherModul.Country, Application.StorageData.WeatherModul.Language, "metric");

            if (!result.Success)
                return;

            StringBuilder weatherTodayString = new StringBuilder();

            weatherTodayString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            weatherTodayString.AppendLine(@"<sentence>");

            weatherTodayString.AppendLine($"{result.Item.Description} in {result.Item.City}. Momentan werden {result.Item.Temp} Grad Celzius Außentemperatur, bei einer Luftfeuchtigkeit von {result.Item.Humidity} Prozent gemessen.");
            weatherTodayString.AppendLine(Math.Abs(result.Item.WindSpeed - double.Epsilon) < 0 ? "Zur Zeit weht kein Wind." : $"Ein Wind mit der Geschwindigkeit von {result.Item.WindSpeed} Metern pro Sekunde weht aus Richtung {getDirection(result.Item.WindDegree)}");

            weatherTodayString.AppendLine(@"</sentence>");
            weatherTodayString.AppendLine(@"</speak>");

            await sayAsyncSsml(weatherTodayString.ToString());
        }

        private static string getDirection(double windDegree)
        {
            if (windDegree >= 348.75 && windDegree <= 11.25)
                return "Nord";

            if (windDegree > 11.25 && windDegree < 33.75)
                return "Nord Nord Ost";

            if (windDegree >= 33.75 && windDegree <= 56.25)
                return "Nord Ost";

            if (windDegree > 56.25 && windDegree < 78.75)
                return "Ost Nord Ost";

            if (windDegree >= 78.75 && windDegree <= 101.25)
                return "Ost";

            if (windDegree > 101.25 && windDegree < 123.75)
                return "Ost Süd Ost";

            if (windDegree >= 123.75 && windDegree <= 146.25)
                return "Süd Ost";

            if (windDegree > 146.25 && windDegree < 168.75)
                return "Süd Süd Ost";

            if (windDegree >= 168.75 && windDegree <= 191.25)
                return "Süd";

            if (windDegree > 191.25 && windDegree < 213.75)
                return "Süd Süd West";

            if (windDegree >= 213.75 && windDegree <= 236.25)
                return "Süd West";

            if (windDegree > 236.25 && windDegree < 258.75)
                return "West Süd West";

            if (windDegree >= 258.75 && windDegree <= 291.25)
                return "West";

            if (windDegree > 291.25 && windDegree < 303.75)
                return "West Nord West";

            if (windDegree >= 303.75 && windDegree <= 326.25)
                return "Nord West";

            if (windDegree > 326.25 && windDegree < 348.75)
                return "Nord Nord West";

            return "Unknown";
        }

        private static string numberToWords(int number)
        {
            if (number == 0)
                return "null";

            if (number < 0)
                return "minus " + numberToWords(Math.Abs(number));

            string words = "";

            if (number / 1000000 > 0)
            {
                words += numberToWords(number / 1000000) + " millionen ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += numberToWords(number / 1000) + " tausend ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += numberToWords(number / 100) + " hundert ";
                number %= 100;
            }

            if (number <= 0)
                return words;

            if (words != "")
                words += "und ";

            string[] unitsMap = { "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun", "zehn", "elf", "zwölf", "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn" };
            string[] tensMap = { "null", "zehn", "zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if (number % 10 > 0)
                    words += "-" + unitsMap[number % 10];
            }

            return words;
        }
    }
}