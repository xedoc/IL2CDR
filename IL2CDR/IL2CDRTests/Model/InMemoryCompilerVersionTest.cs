using System;
using CSScriptLibrary;
using IL2CDR.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IL2CDRTests.Model
{

	public interface ITestIface
	{
		string FirstName { get; }
		string LastName { get; set; }
		string FullName { get; }

		void ChangeName(string newName, object param);
		string GetNamePrefix();
	}


	[TestClass]
	public class InMemoryCompilerVersionTest
	{
		[TestMethod]
		public void TestCSharp_7_0_LanguageFeatures()
		{

			CSScript.AssemblyResolvingEnabled = true;
			CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Roslyn;


			ITestIface script = (ITestIface)CSScript.Evaluator.LoadCode(
@"using System; 
using IL2CDRTests.Model;

public class Student : ITestIface
{
	public const int ThirtyTwo = 0b0010_0000;

	public string FirstName { get; }
	public string LastName { get; set; }
	public string haluz;

	public string FullName => $""{FirstName} {LastName}"";

	public Student() 
	{
		this.FirstName = ""John"";
		this.LastName = ""Doedoe""; 
	}


	public void ChangeName(string newLastName, object param)
	{
		this.LastName = newLastName;
		if (param is int count) {
			this.LastName += ""_"" + count;
		} else if (param is string str) {
			if (int.TryParse(str, out int result)) {
				this.LastName += ""_successfullyParsed-"" + result;
			}
		}
		
	}

	public string GetNamePrefix() 
	{
		return this.LastName?.Substring(0, 3) ?? ""_x_"";
	}
}
");

			Assert.AreEqual("John Doedoe", script.FullName);
			Assert.AreEqual("Doe", script.GetNamePrefix());

			script.ChangeName("Pumpkin", 8);

			Assert.AreEqual("John Pumpkin_8", script.FullName);
			Assert.AreEqual("Pum", script.GetNamePrefix());

			script.ChangeName("Integer", "13");

			Assert.AreEqual("John Integer_successfullyParsed-13", script.FullName);
			Assert.AreEqual("Int",script.GetNamePrefix());

			script.ChangeName(null, null);

			Assert.AreEqual("John ", script.FullName);
			Assert.AreEqual("_x_", script.GetNamePrefix());


		}
	}
}
