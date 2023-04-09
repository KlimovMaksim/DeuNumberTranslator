using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace homework1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.label1.MaximumSize = new Size(300, 50);
        }

        List<string> sentence = new List<string>();

        static Dictionary<string, int> digits = new Dictionary<string, int>()
            {
                {"ein", 1},
                {"zwei", 2},
                {"drei", 3},
                {"vier", 4},
                {"fünf", 5},
                {"sechs", 6},
                {"sieben", 7},
                {"acht", 8},
                {"neun",  9}
            };
        static Dictionary<string, int> numerals = new Dictionary<string, int>()
            {
                {"zehn", 10},
                {"elf", 11},
                {"zwölf", 12},
                {"dreizehn", 13},
                {"vierzehn", 14},
                {"fünfzehn", 15},
                {"sechzehn", 16},
                {"siebzehn", 17},
                {"achtzehn", 18},
                {"neunzehn",  19}
            };
        static Dictionary<string, int> decades = new Dictionary<string, int>()
            {
                {"zwanzig", 20},
                {"dreißig", 30},
                {"vierzig", 40},
                {"fünfzig", 50},
                {"sechzig", 60},
                {"siebzig", 70},
                {"achtzig", 80},
                {"neunzig",  90}
            };
        static Dictionary<string, int> hundred = new Dictionary<string, int>()
            {
                {"hundert", 100}
            };

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string input = textBox1.Text.Trim().ToLower();
            int hundert_index = input.IndexOf("hundert");
            // нужно, чтобы подстроку und не находилo в строке hundert
            int und_index = (hundert_index > -1 && input.LastIndexOf("und") >= hundert_index && input.LastIndexOf("und") <= hundert_index + 6) ? -1: input.LastIndexOf("und");

            if (und_index > -1 || hundert_index > -1)
            {
                if (und_index > -1 && hundert_index > -1)
                {
                    sentence.AddRange(new string[] { input.Substring(0, hundert_index),
                        "hundert",
                        input.Substring(hundert_index + 7, und_index - hundert_index - 7),
                        "und",
                        input.Substring(und_index + 3, input.Length - und_index - 3) });
                    sentence.RemoveAll((string s) => s == "");

                }
                else if (!(und_index > -1) && hundert_index > -1)
                {
                    sentence.AddRange(new string[] { input.Substring(0, hundert_index),
                        "hundert",
                        input.Substring(hundert_index + 7, input.Length - hundert_index - 7) });
                    sentence.RemoveAll((string s) => s == "");
                }
                else if (und_index > -1 && !(hundert_index > -1))
                {
                    sentence.AddRange(new string[] { input.Substring(0, und_index),
                        "und",
                        input.Substring(und_index + 3, input.Length - und_index - 3) });
                    sentence.RemoveAll((string s) => s == "");
                }
            }
            else { sentence.Add(input); }

            if (textBox1.Text != "")
            {
                wordToNumber();
            }
            else label1.Text = "Введите число";
            sentence.Clear();
        }

        private void PrintNumber(string format, Dictionary<string, int> dict)
        {
            if (sentence.Count == 1) { label1.Text = dict[sentence[0]].ToString(); }
            else if (sentence[1] == "hundert") { label1.Text = $"Ошибка: Ключевое слово hundert не может идти после чисел {format}."; }
            else if (sentence[1] == "und") { label1.Text = $"Ошибка: Ключевое слово und не может идти после чисел {format}."; }
            else if (digits.ContainsKey(sentence[1]) || decades.ContainsKey(sentence[1]) || numerals.ContainsKey(sentence[1])) { label1.Text = $"Ошибка: После чисел {format} не могут идти числа других форматов."; }
            else { label1.Text = $"Ошибка: Грамматическая ошибка во 2-ом слове -- {sentence[1]}."; }
        }

        private void wordToNumber()
        {
            // числа 10-19
            if (numerals.ContainsKey(sentence[0])) { PrintNumber("10-19", numerals); return; }
            // десятки
            if (decades.ContainsKey(sentence[0])) { PrintNumber("20, 30, ... 90", decades); return; }
            // число начинается с единиц
            if (digits.ContainsKey(sentence[0]))
            {
                // если число единстенное, то пишем его, иначе проверяем дальше
                if (sentence.Count == 1) { label1.Text = (digits[sentence[0]]).ToString(); return; }
                // если второе слово hundert
                else if (hundred.ContainsKey(sentence[1]))
                {
                    // если слов в списке 2, то выводим сотни, иначе проверяем дальше
                    if (sentence.Count == 2) { label1.Text = (digits[sentence[0]] * hundred[sentence[1]]).ToString(); return; }
                    else if (digits.ContainsKey(sentence[2]))
                    {
                        if (sentence.Count == 3) { label1.Text = (digits[sentence[0]] * hundred[sentence[1]] + digits[sentence[2]]).ToString(); return; }
                        else if (digits.ContainsKey(sentence[3])) { label1.Text = $"Ошибка: Повторение чисел единичного формата -- {sentence[3]} И {sentence[2]}."; return; }
                        else if (numerals.ContainsKey(sentence[3])) { label1.Text = $"Ошибка: Числа 10-19 не могут идти после чисел единичного формата  -- {sentence[3]} И {sentence[2]}."; return; }
                        else if (decades.ContainsKey(sentence[3])) { label1.Text = $"Ошибка: Десятки не могут следовать за числами единичного формата без ключевого слова und -- {sentence[3]} И {sentence[2]}."; return; }
                        else if (hundred.ContainsKey(sentence[3])) { label1.Text = $"Ошибка: Ключевое слово hundert не может повторяться -- {sentence[1]} И {sentence[3]}."; return; }
                        else if (sentence[3] == "und")
                        {
                            if (sentence.Count == 4) { label1.Text = $"Ошибка: Число оканчивается ключевым словом und."; return; }
                            else if (decades.ContainsKey(sentence[4])) { label1.Text = (digits[sentence[0]] * hundred[sentence[1]] + digits[sentence[2]] + decades[sentence[4]]).ToString(); return; }
                            else if (digits.ContainsKey(sentence[4])) { label1.Text = $"Ошибка: Числа единичного формата идут после ключевого слова und -- {sentence[4]} И {sentence[3]}."; return; }
                            else if (numerals.ContainsKey(sentence[4])) { label1.Text = $"Ошибка: Числа 10-19 идут после ключевого слова und -- {sentence[4]} И {sentence[3]}."; return; }
                            else if (hundred.ContainsKey(sentence[4])) { label1.Text = $"Ошибка: Ключевое слово hundert идет после ключевого слова und -- {sentence[4]} И {sentence[3]}."; return; }
                            else if (sentence[4] == "und") { label1.Text = $"Ошибка: Ключевое слово und идет после ключевого слова und -- {sentence[4]} и{sentence[3]}."; return; }
                            else { label1.Text = $"Ошибка: Грамматическая ошибка в 5-oм слове -- {sentence[4]}."; return; }
                        }
                        else { label1.Text = $"Ошибка: Грамматическая ошибка в 4-ом слове -- {sentence[3]}."; return; }

                    }
                    else if (numerals.ContainsKey(sentence[2])) 
                    {
                        if (sentence.Count == 3) { label1.Text = (digits[sentence[0]] * hundred[sentence[1]] + numerals[sentence[2]]).ToString(); return; }
                        else if (hundred.ContainsKey(sentence[3])) { label1.Text = $"Ошибка: Ключевое слово hundert идет за числами 10-19."; return; }
                        else if (sentence[3] == "und") { label1.Text = $"Ошибка: Ключевое слово und идет за числами 10-19."; return; }
                        else { label1.Text = $"Ошибка: Грамматическая ошибка в 4-ом слове -- {sentence[3]}"; return; }
                         
                    }
                    else if (decades.ContainsKey(sentence[2])) 
                    { 
                        if (sentence.Count == 3) { label1.Text = (digits[sentence[0]] * hundred[sentence[1]] + decades[sentence[2]]).ToString(); return; }
                        else if (hundred.ContainsKey(sentence[3])) { label1.Text = $"Ошибка: Ключевое слово hundert идет за числами десятками."; return; }
                        else if (sentence[3] == "und") { label1.Text = $"Ошибка: Ключевое слово und идет за числами десятками."; return; }
                        else { label1.Text = $"Ошибка: Грамматическая ошибка в 4-ом слове -- {sentence[3]}"; return; }
                    }
                    else if (hundred.ContainsKey(sentence[2])) { label1.Text = "Ошибка: Ключевое слово hundert не может следовать за ключевым словом hundert."; return; }
                    else if (sentence[2] == "und") { label1.Text = "Ошибка: Ключевое слово und не может следовать за ключевым словом hundert."; return; }
                    else { label1.Text = $"Ошибка: Грамматическая ошибка в 3-eм слове -- {sentence[2]}."; return; }
                }
                // если второе слово und
                else if (sentence[1] == "und")
                {
                    // если слов в списке 2, то выводим сообщение об ошибке, иначе проверяем дальше
                    if (sentence.Count == 2) { label1.Text = "Ошибка: Число не может заканчивать ключевым словом und."; return; }
                    else if (decades.ContainsKey(sentence[2])) { label1.Text = (decades[sentence[2]] + digits[sentence[0]]).ToString(); return; }
                    else if (sentence[2] == "hundert") { label1.Text = "Ошибка: Ключевое слово hundert не может идти после ключевого слова und."; return; }
                    else if (digits.ContainsKey(sentence[2])) { label1.Text = $"Ошибка: Повторение чисел единичного формата -- {sentence[2]} И {sentence[0]}."; return; }
                    else if (numerals.ContainsKey(sentence[2])) { label1.Text = $"Ошибка: Числа формата (10-19) идут после ключевого слова und -- {sentence[2]} И {sentence[1]}."; return; }
                    else { label1.Text = $"Ошибка: Грамматическая ошибка во 3-ем слове -- {sentence[2]}."; return; }
                }
                else if (digits.ContainsKey(sentence[1])) { label1.Text = $"Ошибка: Повторение чисел единичного формата -- {sentence[1]} И {sentence[0]}."; return; }
                else if (numerals.ContainsKey(sentence[1])) { label1.Text = $"Ошибка: Числа формата (10-19) идут после чисел единичного формата -- {sentence[1]} и {sentence[0]}."; return; }
                else if (decades.ContainsKey(sentence[1])) { label1.Text = $"Ошибка: Числа десятичного формата идут после чисел единичного формата -- {sentence[1]} и {sentence[0]}."; return; }
                else { label1.Text = $"Ошибка: Грамматическая ошибка во 2-ом слове -- {sentence[1]}."; return; }
            }

            // ввели ни цифру, ни числительное
            if (hundred.ContainsKey(sentence[0])) { label1.Text = "Логическая ошибка: Число не может начинаться с ключевого слова hundert."; return; }
            else if (sentence[0] == "und") { label1.Text = "Логическая ошибка: Число не может начинаться с ключевого слова und."; return; }
            else { label1.Text = $"Ошибка: Грамматическая ошибка в 1-ом слове -- {sentence[0]}"; return; }
        }
    }
}