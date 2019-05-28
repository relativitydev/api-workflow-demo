using System;

namespace E2EEDRM.Helpers
{
	public static class Console2
	{
		private static ConsoleColor DefaultConsoleColor = ConsoleColor.Green;

		public static void Initialize()
		{
			DefaultConsoleColor = Console.ForegroundColor;
		}

		public static void WriteTapiStartHeader(string message)
		{
			if (Constants.DebugMode)
			{
				WriteStartHeader(message);
			}
		}

		public static void WriteTapiEndHeader()
		{
			if (Constants.DebugMode)
			{
				WriteEndHeader();
			}
		}

		public static void WriteStartHeader(string message)
		{
			int bannerLength = Console.WindowWidth - 6;

			string asterisks = new string('*', (bannerLength - message.Length - 1) / 2);
			message = $"{asterisks} {message} {asterisks}".Substring(0, bannerLength);
			WriteDebugLine(string.Empty);
			WriteDebugLine(DefaultConsoleColor, message);
		}

		public static void WriteEndHeader()
		{
			int bannerLength = Console.WindowWidth - 6;
			string asterisks = new string('*', bannerLength);
			string message = $"{asterisks}{asterisks}".Substring(0, bannerLength);
			WriteDebugLine(DefaultConsoleColor, message);
		}

		public static void WriteDisplayStartLine(string message)
		{
			WriteDebugLine(ConsoleColor.White, $"{message}");
		}

		public static void WriteDisplayEndLine(string message)
		{
			WriteDebugLine(ConsoleColor.White, $"{message}");
			WriteDisplayEmptyLine();
		}

		public static void WriteDisplayEmptyLine()
		{
			Console.WriteLine();
		}

		public static void WriteDebugLine()
		{
			WriteDebugLine(string.Empty);
		}

		public static void WriteDebugLine(string message)
		{
			if (Constants.DebugMode)
			{
				WriteDebugLine(ConsoleColor.Cyan, message);
			}
		}

		public static void WriteDebugLine(string format, params object[] args)
		{
			if (Constants.DebugMode)
			{
				WriteDebugLine(ConsoleColor.White, format, args);
			}
		}

		public static void WriteDebugLine(ConsoleColor color, string format, params object[] args)
		{
			if (Constants.DebugMode)
			{
				WriteDebugLine(color, string.Format(format, args));
			}
		}

		public static void WriteDebugLine(ConsoleColor color, string message)
		{
			ConsoleColor existingColor = Console.ForegroundColor;

			try
			{
				Console.ForegroundColor = color;
				Console.WriteLine(message);
			}
			finally
			{
				Console.ForegroundColor = existingColor;
			}
		}

		public static void WriteErrorLine(string message)
		{
			if (Constants.DebugMode)
			{
				WriteDebugLine(ConsoleColor.Red, message);
			}
		}

		public static void WriteTerminateLine(int exitCode)
		{
			if (Constants.DebugMode)
			{
				WriteDebugLine();
				Console2.WriteDebugLine(
					exitCode == 0
						? "The sample successfully completed. Exit code: {0}"
						: "The sample failed to complete. Exit code: {0}",
					exitCode);
				WriteDebugLine("Press any key to terminate.");
				Console.ReadLine();
			}
		}
	}
}
