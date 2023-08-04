﻿using System.Web;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RestSharp;
using TriviaBot.App.Models;

namespace TriviaBot.App.Commands;

public class TriviaCommand : CommandBase
{
    [Command("video games")]
    public async Task TriviaCommandAsync()
    {
        var restClient = new RestClient();
        var request = new RestRequest("https://opentdb.com/api.php?amount=1&category=15&type=multiple");
        var response = await restClient.ExecuteAsync<TriviaModel>(request);

        if (response.Data != null && response.Data.Results.Count > 0)
        {
            var triviaQuestion = response.Data.Results[0];
            if (triviaQuestion.Category == "Entertainment: Video Games")
            {
                var decodedQuestion = DecodeHtml(triviaQuestion.Question);

                var decodedCorrectAnswer = DecodeHtml(triviaQuestion.Correct_Answer);
                var decodedAllAnswers = triviaQuestion.AllAnswers.Select(DecodeHtml).ToList();
                
                var shuffledChoices = ShuffleAnswers(decodedAllAnswers);
                var multipleChoice = MultipleChoice(shuffledChoices);

                var questionEmbed = new EmbedBuilder()
                    .WithColor(Color.Gold)
                    .WithTitle($"Category: {triviaQuestion.Category}")
                    .WithDescription($"**Difficulty: **{triviaQuestion.Difficulty.ToUpper()}")
                    .AddField("Question:", decodedQuestion, inline: false)
                    .AddField("Choices:", multipleChoice, inline: false)
                    .WithImageUrl(
                        "https://princewilliamlivingweb.s3-accelerate.amazonaws.com/2022/01/BBaFnKbM-Trivia-Day--702x336.gif")
                    .Build();

                await ReplyAsync(embed: questionEmbed);

                var timeFive = new EmbedBuilder()
                    .WithColor(Color.LighterGrey)
                    .WithTitle("5")
                    .Build();

                var timeFour = new EmbedBuilder()
                    .WithColor(Color.LighterGrey)
                    .WithTitle("4")
                    .Build();

                var timeThree = new EmbedBuilder()
                    .WithColor(Color.LighterGrey)
                    .WithTitle("3")
                    .Build();

                var timeTwo = new EmbedBuilder()
                    .WithColor(Color.LighterGrey)
                    .WithTitle("2")
                    .Build();

                var timeOne = new EmbedBuilder()
                    .WithColor(Color.LighterGrey)
                    .WithTitle("1")
                    .Build();

                // Change back to 15
                await Task.Delay(TimeSpan.FromSeconds(3));
                await ReplyAsync(embed: timeFive);

                await Task.Delay(TimeSpan.FromSeconds(1));
                await ReplyAsync(embed: timeFour);

                await Task.Delay(TimeSpan.FromSeconds(1));
                await ReplyAsync(embed: timeThree);

                await Task.Delay(TimeSpan.FromSeconds(1));
                await ReplyAsync(embed: timeTwo);

                await Task.Delay(TimeSpan.FromSeconds(1));
                await ReplyAsync(embed: timeOne);

                var answerEmbed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithTitle("🎉 Correct Answer 🥳")
                    .WithDescription($"**{decodedCorrectAnswer}**")
                    .Build();

                await Task.Delay(TimeSpan.FromSeconds(1));
                await ReplyAsync(embed: answerEmbed);
            }
            else
            {
                await ReplyAsync("No trivia questions available for this category.");
            }
        }
        else
        {
            await ReplyAsync("No trivia questions available.");
        }
    }

    private string DecodeHtml(string rawHtml)
    {
        string decodedQuestion = HttpUtility.HtmlDecode(rawHtml);
        string cleanedQuestion = decodedQuestion.Replace("\"", "");

        return cleanedQuestion;
    }

    private static List<T> ShuffleAnswers<T>(List<T> list)
    {
        var random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    private string MultipleChoice(List<string> choices)
    {
        char[] letters = {'A', 'B', 'C', 'D'};

        List<string> multipleChoiceFormat = new List<string>();

        for (int i = 0; i < choices.Count; i++)
        {
            if (i < letters.Length)
            {
                multipleChoiceFormat.Add($"**{letters[i]}.** {choices[i]}");
            }
            else
            {
                multipleChoiceFormat.Add($"**{i + 1}.** {choices[i]}");
            }
        }

        return string.Join(Environment.NewLine, multipleChoiceFormat);
    }
}