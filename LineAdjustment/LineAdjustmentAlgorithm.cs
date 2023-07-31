using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LineAdjustment;

public class LineAdjustmentAlgorithm
{
	private const char Separator = ' ';

	public string Transform(string input, int lineWidth)
	{
		if (string.IsNullOrEmpty(input))
			return string.Empty;

		StringBuilder[] words = GetWords(input);
		int wordsCount = words.Length;
		int currentWordsCount = 0;
		var output = new StringBuilder();

		while (currentWordsCount < wordsCount)
		{
			(bool append, int wordsInString, int stringLength) = GetTransformModel(lineWidth, currentWordsCount, words);
			IEnumerable<StringBuilder> appendedWords = GetAppendedWords(lineWidth, append, stringLength, wordsInString, words, currentWordsCount);
			foreach (StringBuilder appendedWord in appendedWords)
				output.Append(appendedWord);

			currentWordsCount += wordsInString;
			if (wordsCount > 1 && currentWordsCount < wordsCount)
				output.Append('\n');
		}

		return output.ToString();
	}

	private static IEnumerable<StringBuilder> GetAppendedWords(
		int lineWidth,
		bool append,
		int stringLength,
		int wordsInString,
		IList<StringBuilder> words,
		int currentWordsCount)
	{
		switch (append)
		{
			case true:
			{
				int amountOfSpaces = lineWidth - stringLength;
				StringBuilder[] wordsWithSpaces = GetWordsWithSpaces(wordsInString, words, currentWordsCount, amountOfSpaces);
				for (int index = 0; index < wordsInString; index++)
				{
					int i = index + currentWordsCount;
					yield return wordsWithSpaces[i];
				}

				break;
			}

			case false:
			{
				StringBuilder word = words[currentWordsCount];
				string separators = GetSeparators(lineWidth, stringLength);
				word.Append(separators);
				yield return word;

				break;
			}
		}
	}

	private static string GetSeparators(int lineWidth, int stringLength) => new(Separator, lineWidth - stringLength);

	private static StringBuilder[] GetWords(string input)
	{
		return input
			.Split(Separator)
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.Select(value => new StringBuilder(value))
			.ToArray();
	}

	private static StringBuilder[] GetWordsWithSpaces(int wordsInString, IEnumerable<StringBuilder> words, int currentWordsCount, int amountOfSpaces)
	{
		StringBuilder[] wordsWithSpaces = words.ToArray();
		while (true)
			for (int index = 0; index < wordsInString - 1; index++)
			{
				if (amountOfSpaces <= 0)
					return wordsWithSpaces;

				StringBuilder word = wordsWithSpaces[index + currentWordsCount];
				word.Append(Separator);
				amountOfSpaces--;
			}
	}

	private static TransformModel GetTransformModel(int lineWidth, int currentWordsCount, IReadOnlyList<StringBuilder> words)
	{
		bool append = false;
		int wordsInString = currentWordsCount + 1;
		int stringLength = GetWordLength(words, currentWordsCount);
		int wordsCount = words.Count;

		while (wordsInString < wordsCount)
		{
			int length = GetWordLength(words, wordsInString);
			if (stringLength >= lineWidth - length)
				break;

			append = true;
			stringLength += length;
			wordsInString++;
		}

		return new TransformModel(append, wordsInString - currentWordsCount, stringLength);
	}

	private static int GetWordLength(IReadOnlyList<StringBuilder> words, int index)
	{
		StringBuilder word = words[index];

		return word.Length;
	}

	private record TransformModel(bool Append, int WordsInString, int StringLength);
}